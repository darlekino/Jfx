using Jfx.Mathematic;
using System;
using System.Threading.Tasks;

namespace Jfx
{
    public interface IInterpolate<TValue> :
        IInterpolateMultiply<TValue>,
        IInterpolateLinear<TValue>,
        IInterpolateBarycentric<TValue>
    {
    }

    public interface IInterpolateMultiply<out TValue>
    {
        TValue InterpolateMultiply(float multiplier);
    }

    public interface IInterpolateLinear<TValue>
    {
        TValue InterpolateLinear(in TValue other, float alpha);
    }

    public interface IInterpolateBarycentric<TValue>
    {
        TValue InterpolateBarycentric(in TValue other0, in TValue other1, Vector3F barycentric);
    }

    public enum PrimitiveTopology
    {
        PointList,
        LineList,
        LineStrip,
        TriangleList,
        TriangleStrip,
    }

    public enum Processing
    {
        Parallel,
        Sequential
    }

    public enum Interpolation
    {
        Undefined,
        Linear
    }

    public class Pipeline
    {
        private IFrameBuffer FrameBuffer { get; }
        public Viewport Viewport { get; set; }

        public Pipeline(in Viewport viewport, IFrameBuffer frameBuffer)
        {
            Viewport = viewport;
            FrameBuffer = frameBuffer!;
        }

        public void Render<TVSIn, TFSIn>(IShaders<TVSIn, TFSIn> shaders, IVertexBuffer<TVSIn> buffer)
            where TVSIn : unmanaged
             where TFSIn : unmanaged, IInterpolate<TFSIn>
        {
            switch (shaders.PrimitiveTopology)
            {
                case PrimitiveTopology.PointList:
                    {
                        switch (shaders.Processing)
                        {
                            case Processing.Parallel: PointListTopology.ParallerRender(this, shaders, buffer); break;
                            case Processing.Sequential: PointListTopology.SequentialRender(this, shaders, buffer); break;
                        }
                    }
                    break;
                case PrimitiveTopology.LineList:
                    {
                        switch (shaders.Processing)
                        {
                            case Processing.Parallel: Line.ListTopology.ParallerRender(this, shaders, buffer); break;
                            case Processing.Sequential: Line.ListTopology.SequentialRender(this, shaders, buffer); break;
                        }
                    }
                    break;
                case PrimitiveTopology.LineStrip:
                    {
                        switch (shaders.Processing)
                        {
                            case Processing.Parallel: Line.StripTopology.ParallerRender(this, shaders, buffer); break;
                            case Processing.Sequential: Line.StripTopology.SequentialRender(this, shaders, buffer); break;
                        }
                    }
                    break;
                case PrimitiveTopology.TriangleList:
                    {
                        switch (shaders.Processing)
                        {
                            case Processing.Parallel: Triangle.ListTopology.ParallerRender(this, shaders, buffer); break;
                            case Processing.Sequential: Triangle.ListTopology.SequentialRender(this, shaders, buffer); break;
                        }
                    }
                    break;
                case PrimitiveTopology.TriangleStrip:
                    {
                        switch (shaders.Processing)
                        {
                            case Processing.Parallel: Triangle.StripTopology.ParallerRender(this, shaders, buffer); break;
                            case Processing.Sequential: Triangle.StripTopology.SequentialRender(this, shaders, buffer); break;
                        }
                    }
                    break;
            }
        }

        private Vector4F VertexPostProcessing(in Vector4F position)
        {
            float wInv = 1 / position.W;
            return Vector4F.Transform(position * wInv, Viewport.Matrix);
        }

        private bool IsOnScreen(int x, int y)
        {
            return x >= 0 && y >= 0 && x < FrameBuffer.Width && y < FrameBuffer.Height;
        }

        private static class Clipping
        {
            public static bool IsOutside<TFSIn>(in Vector4F position)
                 where TFSIn : unmanaged, IInterpolate<TFSIn>
            {
                return false;
            }
        }

        private static class PointListTopology
        {
            public static unsafe void ParallerRender<TVSIn, TFSIn>(Pipeline pipeline, IShaders<TVSIn, TFSIn> shaders, IVertexBuffer<TVSIn> buffer)
                where TVSIn : unmanaged
                 where TFSIn : unmanaged, IInterpolate<TFSIn>
            {
                Parallel.For(0, buffer.Count, i => Render(pipeline, shaders, buffer[i]));
            }

            public static unsafe void SequentialRender<TVSIn, TFSIn>(Pipeline pipeline, IShaders<TVSIn, TFSIn> shaders, IVertexBuffer<TVSIn> buffer)
                where TVSIn : unmanaged
                 where TFSIn : unmanaged, IInterpolate<TFSIn>
            {
                for (int i = 0; i < buffer.Count; i++)
                    Render(pipeline, shaders, buffer[i]);
            }

            private static void Render<TVSIn, TFSIn>(Pipeline pipeline, IShaders<TVSIn, TFSIn> shader, in TVSIn vsin)
                where TVSIn : unmanaged
                 where TFSIn : unmanaged, IInterpolate<TFSIn>
            {
                // Vertex shader stage
                Vector4F position = shader.VertexShader.ExecuteStage(vsin, out TFSIn fsin);
                position = pipeline.VertexPostProcessing(position);

                int x = (int)position.X;
                int y = (int)position.Y;

                if (pipeline.IsOnScreen(x, y))
                {
                    float z = position.Z;

                    // Fragment shader stage
                    if (shader.FragmentShader.ExecuteStage(position, fsin, out Vector4F color))
                    {
                        pipeline.FrameBuffer.PutPixel(x, y, color);
                    }
                }
            }
        }

        private static class Line
        {
            private struct BresenhamLine
            {
                public Vector4F P0 { get; }
                public Vector4F P1 { get; }

                public BresenhamLine(in Vector4F p0, in Vector4F p1)
                {
                    P0 = p0;
                    P1 = p1;
                }

                public Enumerator GetEnumerator() => new Enumerator(P0, P1);

                public struct Enumerator
                {
                    private int dx0;
                    private int dy0;
                    private int dx1;
                    private int dy1;
                    private int longest;
                    private int shortest;
                    private int numerator;

                    private int index;
                    private int x;
                    private int y;

                    internal Enumerator(in Vector4F p0, in Vector4F p1)
                    {
                        int x0 = (int)p0.X;
                        int y0 = (int)p0.Y;

                        int x1 = (int)p1.X;
                        int y1 = (int)p1.Y;

                        int w = x1 - x0;
                        int h = y1 - y0;

                        dx0 = 0;
                        dy0 = 0;
                        dx1 = 0;
                        dy1 = 0;
                        if (w < 0) dx0 = -1; else if (w > 0) dx0 = 1;
                        if (h < 0) dy0 = -1; else if (h > 0) dy0 = 1;
                        if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;

                        longest = Math.Abs(w);
                        shortest = Math.Abs(h);

                        if (longest < shortest)
                        {
                            Swap(ref longest, ref shortest);
                            if (h < 0)
                            {
                                dy1 = -1;
                            }
                            else if (h > 0)
                            {
                                dy1 = 1;
                            }
                            dx1 = 0;
                        }

                        numerator = longest >> 1;
                        index = -1;
                        x = x0;
                        y = y0;
                    }
                    
                    public bool MoveNext()
                    {
                        numerator += shortest;
                        if (!(numerator < longest))
                        {
                            numerator -= longest;
                            x += dx0;
                            y += dy0;
                        }
                        else
                        {
                            x += dx1;
                            y += dy1;
                        }

                        return ++index <= longest;
                    }

                    public (int, int) Current => (x, y);

                    public void Reset() => index = -1;
                    public void Dispose() { }

                    private static void Swap(ref int x0, ref int x1)
                    {
                        int tmp = x0;
                        x0 = x1;
                        x1 = tmp;
                    }
                }
            }

            private static void Render<TVSIn, TFSIn>(Pipeline pipeline, IShaders<TVSIn, TFSIn> shaders, in TVSIn vsin0, in TVSIn vsin1)
                where TVSIn : unmanaged
                 where TFSIn : unmanaged, IInterpolate<TFSIn>
            {
                Vector4F position1 = shaders.VertexShader.ExecuteStage(vsin0, out TFSIn fsin0);
                Vector4F position2 = shaders.VertexShader.ExecuteStage(vsin1, out TFSIn fsin1);

                Vector4F screenPosition1 = pipeline.VertexPostProcessing(position1);
                Vector4F screenPosition2 = pipeline.VertexPostProcessing(position2);

                Rasterize(pipeline, shaders.FragmentShader, screenPosition1, screenPosition2);
            }

            private static void Rasterize<TFSIn>(Pipeline pipeline, IFragmentShader<TFSIn> fragmentShader, in Vector4F p0, in Vector4F p1)
                 where TFSIn : unmanaged, IInterpolate<TFSIn>

            {
                foreach (var (x, y) in new BresenhamLine(p0, p1))
                {
                    if (pipeline.IsOnScreen(x, y))
                    {
                        //throw new NotImplementedException();
                        if (fragmentShader.ExecuteStage(default, default, out Vector4F color))
                        {
                            pipeline.FrameBuffer.PutPixel(x, y, color);
                        }
                    }
                }
            }

            public static class ListTopology
            {
                private static void Check<TVSIn>(IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                {
                    if (buffer.Count % 2 != 0)
                        throw new ArgumentException($"line list topology cannot be used to render odd count of vertices");
                }

                public static unsafe void ParallerRender<TVSIn, TFSIn>(Pipeline pipeline, IShaders<TVSIn, TFSIn> shaders, IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                     where TFSIn : unmanaged, IInterpolate<TFSIn>
                {
                    Check(buffer);
                    Parallel.For(0, buffer.Count / 2, i =>
                    {
                        int index = i * 2;
                        Render(pipeline, shaders, buffer[index], buffer[index + 1]);
                    });
                }

                public static unsafe void SequentialRender<TVSIn, TFSIn>(Pipeline pipeline, IShaders<TVSIn, TFSIn> shaders, IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                     where TFSIn : unmanaged, IInterpolate<TFSIn>
                {
                    Check(buffer);
                    for (int i = 0; i < buffer.Count; i += 2)
                        Render(pipeline, shaders, buffer[i], buffer[i + 1]);
                }

            }

            public static class StripTopology
            {
                private static void Check<TVSIn>(IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                {
                    if (buffer.Count < 2)
                        throw new ArgumentException($"line strip topology cannot be used to render less than 2 vertices");
                }

                public static unsafe void ParallerRender<TVSIn, TFSIn>(Pipeline pipeline, IShaders<TVSIn, TFSIn> shaders, IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                     where TFSIn : unmanaged, IInterpolate<TFSIn>
                {
                    Check(buffer);
                    Parallel.For(0, buffer.Count - 1, i => Render(pipeline, shaders, buffer[i], buffer[i + 1]));
                }

                public static unsafe void SequentialRender<TVSIn, TFSIn>(Pipeline pipeline, IShaders<TVSIn, TFSIn> shaders, IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                     where TFSIn : unmanaged, IInterpolate<TFSIn>
                {
                    Check(buffer);
                    for (int i = 0; i < buffer.Count - 1; i++)
                        Render(pipeline, shaders, buffer[i], buffer[i + 1]);
                }
            }
        }

        private static class Triangle
        {
            private static void Swap(ref Vector4F t0, ref Vector4F t1)
            {
                Vector4F tmp = t0;
                t0 = t1;
                t1 = tmp;
            }

            private static int Clamp(int value, int min, int max)
            {
                if (value < min) return min;
                else if (value > max) return max;
                else return value;
            }

            private static int TriangleClampX(int value, in Viewport viewport) => Clamp(value, viewport.X, viewport.X + viewport.Size.Width);
            private static int TriangleClampY(int value, in Viewport viewport) => Clamp(value, viewport.Y, viewport.Y + viewport.Size.Height);

            private readonly struct PrimitiveTriangle<TFSIn> 
                 where TFSIn : unmanaged, IInterpolate<TFSIn>
            {
                public readonly Vector4F V0;
                public readonly Vector4F V1;
                public readonly Vector4F V2;

                public readonly TFSIn FSIn0;
                public readonly TFSIn FSIn1;
                public readonly TFSIn FSIn2;

                public PrimitiveTriangle(Vector4F v0, Vector4F v1, Vector4F v2, TFSIn fsin0, TFSIn fsin1, TFSIn fsin2)
                {
                    V0 = v0;
                    V1 = v1;
                    V2 = v2;
                    FSIn0 = fsin0;
                    FSIn1 = fsin1;
                    FSIn2 = fsin2;
                }
            }

            private static void Render<TVSIn, TFSIn>(Pipeline pipeline, IShaders<TVSIn, TFSIn> shaders, in TVSIn vsin0, in TVSIn vsin1, in TVSIn vsin2)
                where TVSIn : unmanaged
                 where TFSIn : unmanaged, IInterpolate<TFSIn>
            {
                // Vertex shader stage
                Vector4F position0 = shaders.VertexShader.ExecuteStage(vsin0, out TFSIn fsin0);
                Vector4F position1 = shaders.VertexShader.ExecuteStage(vsin1, out TFSIn fsin1);
                Vector4F position2 = shaders.VertexShader.ExecuteStage(vsin2, out TFSIn fsin2);

                // Vertex post processing
                position0 = pipeline.VertexPostProcessing(position0);
                position1 = pipeline.VertexPostProcessing(position1);
                position2 = pipeline.VertexPostProcessing(position2);

                // Rasterization
                if (position1.Y < position0.Y) Swap(ref position0, ref position1);
                if (position2.Y < position1.Y) Swap(ref position1, ref position2);
                if (position1.Y < position0.Y) Swap(ref position0, ref position1);

                PrimitiveTriangle<TFSIn> triangle = new PrimitiveTriangle<TFSIn>(
                    position0,
                    position1,
                    position2,
                    fsin0,
                    fsin1,
                    fsin2
                );

                const float error = 0.0001f;
                if (MathF.Abs(position0.Y - position1.Y) < error)
                {
                    // natural flat top
                    if (position1.X < position0.X) Swap(ref position0, ref position1);

                    /*
                        (v0)--(v1)
                           \  /
                           (v2)
                    */

                    RasterizeTriangleFlatTop(pipeline, shaders.FragmentShader, triangle, position0, position1, position2);
                }
                else if (MathF.Abs(position1.Y - position2.Y) < error)
                {
                    // natural flat bottom
                    if (position2.X < position1.X) Swap(ref position1, ref position2);

                    /*
                           (v0)
                           /  \
                        (v1)--(v2)
                    */

                    RasterizeTriangleFlatBottom(pipeline, shaders.FragmentShader, triangle, position1, position2, position0);
                }
                else
                {
                    // regular triangle

                    // find splitting vertex (and interpolate)
                    float alpha = (position1.Y - position0.Y) / (position2.Y - position0.Y);
                    Vector4F interpolant = Interpolation.Linear(position0, position2, alpha);

                    if (position1.X < interpolant.X)
                    {
                        /*
                              (v0)
                              / |
                          (v1)-(i)
                              \ |
                              (v2)
                        */
                        RasterizeTriangleFlatBottom(pipeline, shaders.FragmentShader, triangle, position1, interpolant, position0);
                        RasterizeTriangleFlatTop(pipeline, shaders.FragmentShader, triangle, position1, interpolant, position2);
                    }
                    else
                    {
                        /*
                            (v0)
                             | \
                            (i)-(v1)
                             | /
                            (v2)
                        */
                        RasterizeTriangleFlatBottom(pipeline, shaders.FragmentShader, triangle, interpolant, position1, position0);
                        RasterizeTriangleFlatTop(pipeline, shaders.FragmentShader, triangle, interpolant, position1, position2);
                    }
                }
            }

            private static void RasterizeTriangleFlatTop<TFSIn>(Pipeline pipeline, IFragmentShader<TFSIn> fragmentShader, in PrimitiveTriangle<TFSIn> primitive, in Vector4F vertexLeft, in Vector4F vertexRight, in Vector4F vertexBottom)
                 where TFSIn : unmanaged, IInterpolate<TFSIn>
            {
                float height = vertexBottom.Y - vertexLeft.Y;
                float invH = 1 / height;
                Vector4F deltaLeft = (vertexBottom - vertexLeft) * invH;
                Vector4F deltaRight = (vertexBottom - vertexRight) * invH;
                RasterizeTriangleFlat(pipeline, fragmentShader, primitive, vertexLeft, vertexRight, deltaLeft, deltaRight, height);
            }

            private static void RasterizeTriangleFlatBottom<TFSIn>(Pipeline pipeline, IFragmentShader<TFSIn> fragmentShader, in PrimitiveTriangle<TFSIn> primitive, in Vector4F vertexLeft, in Vector4F vertexRight, in Vector4F vertexTop)
                 where TFSIn : unmanaged, IInterpolate<TFSIn>
            {
                float height = vertexLeft.Y - vertexTop.Y;
                float invH = 1 / height;
                Vector4F deltaLeft = (vertexLeft - vertexTop) * invH;
                Vector4F deltaRight = (vertexRight - vertexTop) * invH;
                RasterizeTriangleFlat(pipeline, fragmentShader, primitive, vertexTop, vertexTop, deltaLeft, deltaRight, height);
            }

            private static void RasterizeTriangleFlat<TFSIn>(Pipeline pipeline, IFragmentShader<TFSIn> fragmentShader, in PrimitiveTriangle<TFSIn> primitive, Vector4F edgeLeft, Vector4F edgeRight, Vector4F deltaLeft, Vector4F deltaRight, float height)
                 where TFSIn : unmanaged, IInterpolate<TFSIn>
            {
                // get where we start and end vertically
                int yStart = TriangleClampY((int)MathF.Round(edgeLeft.Y), pipeline.Viewport);
                int yEnd = TriangleClampY((int)MathF.Round(edgeLeft.Y + height), pipeline.Viewport);

                // prestep (compensate for clamping + move to middle of the pixel)
                edgeLeft += deltaLeft * (yStart - edgeLeft.Y + 0.5f);
                edgeRight += deltaRight * (yStart - edgeRight.Y + 0.5f);

                // go vertically down
#if !USE_PARALLEL
            Parallel.For(yStart, yEnd, y =>
#else
                for (int y = yStart; y < yEnd; y++)
#endif
                {
                    int k = y - yStart;
                    Vector4F eLeft = edgeLeft + deltaLeft * k;
                    Vector4F eRight = edgeRight + deltaRight * k;

                    int xStart = TriangleClampX((int)MathF.Round(eLeft.X), pipeline.Viewport);
                    int xEnd = TriangleClampX((int)MathF.Round(eRight.X), pipeline.Viewport);

                    // go horizontally (execute scanline)
                    for (int x = xStart; x < xEnd; x++)
                    {
                        if (pipeline.IsOnScreen(x, y))
                        {
                            //throw new NotImplementedException();
                            if (fragmentShader.ExecuteStage(default, default, out Vector4F color))
                            {
                                pipeline.FrameBuffer.PutPixel(x, y, color);
                            }
                        }
                    }
                }
#if !USE_PARALLEL
            ); // end of Parallel.For
#endif
            }

            public static class ListTopology
            {
                private static void Check<TVSIn>(IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                {
                    if (buffer.Count % 3 != 0)
                        throw new ArgumentException($"triangle list topology cannot be used to render {buffer.Count} vertices. It has to be able to be divided by 3");
                }

                public static unsafe void ParallerRender<TVSIn, TFSIn>(Pipeline pipeline, IShaders<TVSIn, TFSIn> shaders, IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                     where TFSIn : unmanaged, IInterpolate<TFSIn>
                {
                    Check(buffer);
                    Parallel.For(0, buffer.Count / 3, i =>
                    {
                        int index = i * 3;
                        Render(pipeline, shaders, buffer[index], buffer[index + 1], buffer[index + 2]);
                    });
                }

                public static unsafe void SequentialRender<TVSIn, TFSIn>(Pipeline pipeline, IShaders<TVSIn, TFSIn> shaders, IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                     where TFSIn : unmanaged, IInterpolate<TFSIn>
                {
                    Check(buffer);
                    for (int i = 0; i < buffer.Count; i += 3)
                        Render(pipeline, shaders, buffer[i], buffer[i + 1], buffer[i + 2]);
                }

            }

            public static class StripTopology
            {
                private static void Check<TVSIn>(IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                {
                    if (buffer.Count < 3)
                        throw new ArgumentException($"line strip topology cannot be used to render less than 3 vertices");
                }

                public static unsafe void ParallerRender<TVSIn, TFSIn>(Pipeline pipeline, IShaders<TVSIn, TFSIn> shaders, IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                     where TFSIn : unmanaged, IInterpolate<TFSIn>
                {
                    Check(buffer);
                    Parallel.For(0, buffer.Count - 2, i => Render(pipeline, shaders, buffer[i], buffer[i + 1], buffer[i + 2]));
                }

                public static unsafe void SequentialRender<TVSIn, TFSIn>(Pipeline pipeline, IShaders<TVSIn, TFSIn> shaders, IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                     where TFSIn : unmanaged, IInterpolate<TFSIn>
                {
                    Check(buffer);
                    for (int i = 0; i < buffer.Count - 2; i++)
                        Render(pipeline, shaders, buffer[i], buffer[i + 1], buffer[i + 2]);
                }


            }
        }

        public static class Interpolation
        {
            private static float Linear(float left, float right, float alpha) => left + (right - left) * alpha;

            public static Vector4F Linear(in Vector4F v0, in Vector4F v1, float alpha)
            {
                return new Vector4F(
                    Linear(v0.X, v1.X, alpha),
                    Linear(v0.Y, v1.Y, alpha),
                    Linear(v0.Z, v1.Z, alpha),
                    Linear(v0.W, v1.W, alpha)
                );
            }
        }
    }
}
