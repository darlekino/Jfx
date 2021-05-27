using Jfx.Mathematic;
using System;
using System.Threading.Tasks;

namespace Jfx
{
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


    public class Pipeline
    {
        private IFrameBuffer FrameBuffer { get; }
        public Viewport Viewport { get; set; }

        public Pipeline(in Viewport viewport, IFrameBuffer frameBuffer)
        {
            Viewport = viewport;
            FrameBuffer = frameBuffer!;
        }

        public void Render<TVSIn, TFSIn>(IShader<TVSIn, TFSIn> shader, IVertexBuffer<TVSIn> buffer, PrimitiveTopology primitiveTopology, Processing processing)
            where TVSIn : unmanaged
            where TFSIn : unmanaged
        {
            switch (primitiveTopology)
            {
                case PrimitiveTopology.PointList:
                    {
                        switch (processing)
                        {
                            case Processing.Parallel: PointListTopology.ParallerRender(this, shader, buffer); break;
                            case Processing.Sequential: PointListTopology.SequentialRender(this, shader, buffer); break;
                        }
                    }
                    break;
                case PrimitiveTopology.LineList:
                    {
                        switch (processing)
                        {
                            case Processing.Parallel: Line.ListTopology.ParallerRender(this, shader, buffer); break;
                            case Processing.Sequential: Line.ListTopology.SequentialRender(this, shader, buffer); break;
                        }
                    }
                    break;
                case PrimitiveTopology.LineStrip:
                    {
                        switch (processing)
                        {
                            case Processing.Parallel: Line.StripTopology.ParallerRender(this, shader, buffer); break;
                            case Processing.Sequential: Line.StripTopology.SequentialRender(this, shader, buffer); break;
                        }
                    }
                    break;
                case PrimitiveTopology.TriangleList:
                    break;
                case PrimitiveTopology.TriangleStrip:
                    break;
            }
        }

        private Vector4F VertexPostProcessing(Pipeline pipeline, in Vector4F position)
        {
            float wInv = 1 / position.W;
            return Vector4F.Transform(position * wInv, pipeline.Viewport.Matrix);
        }

        private bool IsOnScreen(int x, int y)
        {
            return x >= 0 && y >= 0 && x < FrameBuffer.Width && y < FrameBuffer.Height;
        }

        private static class Clipping
        {
            public static bool IsOutside<TFSIn>(in Vector4F position)
                where TFSIn : unmanaged
            {
                return false;
            }
        }

        private static class PointListTopology
        {
            public static unsafe void ParallerRender<TVSIn, TFSIn>(Pipeline pipeline, IShader<TVSIn, TFSIn> shader, IVertexBuffer<TVSIn> buffer)
                where TVSIn : unmanaged
                where TFSIn : unmanaged
            {
                TVSIn* vsin = buffer.UnsafeVertexPtr();
                Parallel.For(0, buffer.Count, i => Render(pipeline, shader, *(vsin + i)));
            }

            public static unsafe void SequentialRender<TVSIn, TFSIn>(Pipeline pipeline, IShader<TVSIn, TFSIn> shader, IVertexBuffer<TVSIn> buffer)
                where TVSIn : unmanaged
                where TFSIn : unmanaged
            {
                TVSIn* vsin = buffer.UnsafeVertexPtr();
                for (int i = 0; i < buffer.Count; i++)
                    Render(pipeline, shader, *(vsin + i));
            }

            public static void Render<TVSIn, TFSIn>(Pipeline pipeline, IShader<TVSIn, TFSIn> shader, in TVSIn vsin)
                where TVSIn : unmanaged
                where TFSIn : unmanaged
            {
                // Vertex shader stage
                Vector4F position = shader.VertexShader(vsin, out TFSIn fsin);
                Vector4F screenPosition = pipeline.VertexPostProcessing(pipeline, position);

                int x = (int)screenPosition.X;
                int y = (int)screenPosition.Y;

                if (pipeline.IsOnScreen(x, y))
                {
                    float z = screenPosition.Z;

                    // Fragment shader stage
                    var color = shader.FragmentShader(screenPosition, fsin);

                    pipeline.FrameBuffer.PutPixel(x, y, color);
                }
            }
        }

        private static class Line
        {
            private static void Render<TVSIn, TFSIn>(Pipeline pipeline, IShader<TVSIn, TFSIn> shader, in TVSIn vsin0, in TVSIn vsin1)
                where TVSIn : unmanaged
                where TFSIn : unmanaged
            {
                // Vertex shader stage
                Vector4F position1 = shader.VertexShader(vsin0, out TFSIn fsin0);
                Vector4F position2 = shader.VertexShader(vsin1, out TFSIn fsin1);
                Vector4F screenPosition1 = pipeline.VertexPostProcessing(pipeline, position1);
                Vector4F screenPosition2 = pipeline.VertexPostProcessing(pipeline, position2);

                int x1 = (int)screenPosition1.X;
                int y1 = (int)screenPosition1.Y;
                float z1 = screenPosition1.Z;

                int x2 = (int)screenPosition2.X;
                int y2 = (int)screenPosition2.Y;
                float z2 = screenPosition2.Z;

                int w = x2 - x1;
                int h = y2 - y1;
                int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
                if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
                if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
                if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
                int longest = Math.Abs(w);
                int shortest = Math.Abs(h);
                if (longest < shortest)
                {
                    (longest, shortest) = (shortest, longest);
                    if (h < 0)
                        dy2 = -1;
                    else if (h > 0)
                        dy2 = 1;
                    dx2 = 0;
                }
                int numerator = longest >> 1;
                for (int i = 0; i <= longest; i++)
                {
                    if (pipeline.IsOnScreen(x1, y1))
                    {
                        //throw new NotImplementedException();
                        var color = shader.FragmentShader(default, default);
                        pipeline.FrameBuffer.PutPixel(x1, y1, color);
                    }

                    numerator += shortest;
                    if (!(numerator < longest))
                    {
                        numerator -= longest;
                        x1 += dx1;
                        y1 += dy1;
                    }
                    else
                    {
                        x1 += dx2;
                        y1 += dy2;
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

                public static unsafe void ParallerRender<TVSIn, TFSIn>(Pipeline pipeline, IShader<TVSIn, TFSIn> shader, IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                    where TFSIn : unmanaged
                {
                    Check(buffer);
                    TVSIn* vsin = buffer.UnsafeVertexPtr();
                    Parallel.For(0, buffer.Count / 2, i => {
                        var index = i * 2;
                        Render(pipeline, shader, *(vsin + index), *(vsin + index + 1));
                    });
                }

                public static unsafe void SequentialRender<TVSIn, TFSIn>(Pipeline pipeline, IShader<TVSIn, TFSIn> shader, IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                    where TFSIn : unmanaged
                {
                    Check(buffer);
                    TVSIn* vsin = buffer.UnsafeVertexPtr();
                    for (int i = 0; i < buffer.Count / 2; i++)
                    {
                        var index = i * 2;
                        Render(pipeline, shader, *(vsin + index), *(vsin + index + 1));
                    }
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

                public static unsafe void ParallerRender<TVSIn, TFSIn>(Pipeline pipeline, IShader<TVSIn, TFSIn> shader, IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                    where TFSIn : unmanaged
                {
                    Check(buffer);
                    TVSIn* vsin = buffer.UnsafeVertexPtr();
                    Parallel.For(0, buffer.Count - 1, i => Render(pipeline, shader, *(vsin + i), *(vsin + i + 1)));
                }

                public static unsafe void SequentialRender<TVSIn, TFSIn>(Pipeline pipeline, IShader<TVSIn, TFSIn> shader, IVertexBuffer<TVSIn> buffer)
                    where TVSIn : unmanaged
                    where TFSIn : unmanaged
                {
                    Check(buffer);
                    TVSIn* vsin = buffer.UnsafeVertexPtr();
                    for (int i = 0; i < buffer.Count - 1; i++)
                        Render(pipeline, shader, *(vsin + i), *(vsin + i + 1));
                }


            }
        }
    }
}
