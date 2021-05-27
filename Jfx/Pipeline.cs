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
                            case Processing.Parallel: LineListToPology.ParallerRender(this, shader, buffer); break;
                            case Processing.Sequential: LineListToPology.SequentialRender(this, shader, buffer); break;
                        }
                    }
                    break;
                case PrimitiveTopology.LineStrip:
                    break;
                case PrimitiveTopology.TriangleList:
                    break;
                case PrimitiveTopology.TriangleStrip:
                    break;
            }
        }

        private void VertexPostProcessing<TFSIn>(Pipeline pipeline, ref TFSIn fsin)
            where TFSIn : unmanaged, IFSIn
        {
            float wInv = 1 / fsin.Position.W;
            fsin.Position = Vector4F.Transform(fsin.Position, pipeline.Viewport.Matrix);
            fsin.Position = new Vector4F(
                fsin.Position.X * wInv,
                fsin.Position.Y * wInv,
                fsin.Position.Z * wInv
            );
        }

        private bool IsOnScreen(int x, int y)
        {
            return x >= 0 && y >= 0 && x < FrameBuffer.Width && y < FrameBuffer.Height;
        }

        private static class Clipping
        {
            public static bool IsOutside<TFSIn>(in TFSIn fsin)
                where TFSIn : unmanaged, IFSIn
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
                shader.VertexShader(vsin, out TFSIn fsin);
                pipeline.VertexPostProcessing(pipeline, ref fsin);

                int x = (int)fsin.Position.X;
                int y = (int)fsin.Position.Y;

                if (pipeline.IsOnScreen(x, y))
                {
                    float z = fsin.Position.Z;

                    // Fragment shader stage
                    shader.FragmentShader(fsin, out Vector4F color);

                    pipeline.FrameBuffer.PutPixel(x, y, color);
                }
            }
        }

        private static class LineListToPology
        {
            private static void Check<TVSIn>(IVertexBuffer<TVSIn> buffer)
                where TVSIn : unmanaged
            {
                if (buffer.Count % 2 != 0)
                    throw new ArgumentException($"line list topology cannot be used to render odd count of vertices");
            }

            public static unsafe void ParallerRender<TVSIn, TFSIn>(Pipeline pipeline, IShader<TVSIn, TFSIn> shader, IVertexBuffer<TVSIn> buffer)
                where TVSIn : unmanaged
                where TFSIn : unmanaged, IFSIn
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
                where TFSIn : unmanaged, IFSIn
            {
                Check(buffer);
                TVSIn* vsin = buffer.UnsafeVertexPtr();
                for (int i = 0; i < buffer.Count / 2; i++)
                {
                    var index = i * 2;
                    Render(pipeline, shader, *(vsin + index), *(vsin + index + 1));
                }
            }

            public static void Render<TVSIn, TFSIn>(Pipeline pipeline, IShader<TVSIn, TFSIn> shader, in TVSIn vsin0, in TVSIn vsin1)
                where TVSIn : unmanaged
                where TFSIn : unmanaged, IFSIn
            {
                // Vertex shader stage
                shader.VertexShader(vsin0, out TFSIn fsin0);
                shader.VertexShader(vsin1, out TFSIn fsin1);
                pipeline.VertexPostProcessing(pipeline, ref fsin0);
                pipeline.VertexPostProcessing(pipeline, ref fsin1);

                int x1 = (int)fsin0.Position.X;
                int y1 = (int)fsin0.Position.Y;
                float z1 = fsin0.Position.Z;

                int x2 = (int)fsin1.Position.X;
                int y2 = (int)fsin1.Position.Y;
                float z2 = fsin1.Position.Z;

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
                    (longest,shortest) = (shortest, longest);
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
                        shader.FragmentShader(default, out Vector4F color);
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

            //    void line(int x0, int y0, int x1, int y1, TGAImage &image, TGAColor color)
            //{
            //    bool steep = false;
            //    if (std::abs(x0 - x1) < std::abs(y0 - y1))
            //    {
            //        std::swap(x0, y0);
            //        std::swap(x1, y1);
            //        steep = true;
            //    }
            //    if (x0 > x1)
            //    {
            //        std::swap(x0, x1);
            //        std::swap(y0, y1);
            //    }
            //    int dx = x1 - x0;
            //    int dy = y1 - y0;
            //    int derror2 = std::abs(dy) * 2;
            //    int error2 = 0;
            //    int y = y0;
            //    for (int x = x0; x <= x1; x++)
            //    {
            //        if (steep)
            //        {
            //            image.set(y, x, color);
            //        }
            //        else
            //        {
            //            image.set(x, y, color);
            //        }
            //        error2 += derror2;

            //        if (error2 > dx)
            //        {
            //            y += (y1 > y0 ? 1 : -1);
            //            error2 -= dx * 2;
            //        }
            //    }
            //}

        }
    }
}
