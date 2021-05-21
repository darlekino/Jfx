using Jfx.Mathematic;
using System;
using System.Threading.Tasks;

namespace Jfx
{
    public class Pipeline<TShader, TVSIn, TFSIn>
        where TShader : IShader<TVSIn, TFSIn>
        where TVSIn : unmanaged
        where TFSIn : unmanaged, IFSIn
    {
        private TShader shader;
        private Viewport viewport;
        private IFrameBuffer frameBuffer;

        public TShader Shader
        {
            get => shader;
            set => shader = value!;
        }

        public Pipeline(TShader shader, in Viewport viewport, IFrameBuffer frameBuffer)
        {
            this.shader = shader!;
            this.viewport = viewport;
            this.frameBuffer = frameBuffer!;
        }

        public void Render(IVertexBuffer<TVSIn> buffer, PrimitiveTopology primitiveTopology)
        {
            switch (primitiveTopology)
            {
                case PrimitiveTopology.PointList:
                    PointListTopology.Render(this, buffer);
                    break;
                case PrimitiveTopology.LineList:
                    break;
                case PrimitiveTopology.LineStrip:
                    break;
                case PrimitiveTopology.TriangleList:
                    break;
                case PrimitiveTopology.TriangleStrip:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(primitiveTopology));
            }
        }

        private static class Clipping
        {
            public static bool IsOutside(in TFSIn fsin)
            {
                return false;
            }
        }

        private class Utils
        {
            public static void VertexPostProcessing(Pipeline<TShader, TVSIn, TFSIn> pipeline, ref TFSIn fsin)
            {
                fsin.Position = Vector4F.Transform(fsin.Position, pipeline.viewport.Matrix);
                fsin.Position = new Vector4F(
                    fsin.Position.X / fsin.Position.W,
                    fsin.Position.Y / fsin.Position.W,
                    fsin.Position.Z / fsin.Position.W
                );
            }
        }

        private static class PointListTopology
        {
            public unsafe static void Render(Pipeline<TShader, TVSIn, TFSIn> pipeline, IVertexBuffer<TVSIn> buffer)
            {
                Parallel.For(0, buffer.Count, i => { 
                //for (int i = 0; i < buffer.Count; i++)
                //{
                    // Vertex shader stage
                    pipeline.shader.VertexShader(*buffer[i], out TFSIn fsin);
                    Utils.VertexPostProcessing(pipeline, ref fsin);

                    int x = (int)fsin.Position.X;
                    int y = (int)fsin.Position.Y;
                    float z = fsin.Position.Z;

                    // Fragment shader stage
                    pipeline.shader.FragmentShader(fsin, out Vector4F color);

                    if (x >= 0 && y >= 0 && x < pipeline.frameBuffer.Width && y < pipeline.frameBuffer.Height)
                    {
                        pipeline.frameBuffer.PutPixel(x, y, color);
                    }
                //}
                });
            }
        }

        private static class LineListToPology
        {
            public unsafe static void Render(Pipeline<TShader, TVSIn, TFSIn> pipeline, IVertexBuffer<TVSIn> buffer)
            {
                //Parallel.For(0, buffer.Count, i => {
                for (int i = 0; i < buffer.Count; i+=2)
                {
                    int index0 = i;
                    int index1 = i + 1;
                    // Vertex shader stage
                    pipeline.shader.VertexShader(*buffer[index0], out TFSIn fsin0);
                    pipeline.shader.VertexShader(*buffer[index1], out TFSIn fsin1);
                    Utils.VertexPostProcessing(pipeline, ref fsin0);
                    Utils.VertexPostProcessing(pipeline, ref fsin1);

                    int x0 = (int)fsin0.Position.X;
                    int y0 = (int)fsin0.Position.Y;
                    float z0 = fsin0.Position.Z;

                    int x1 = (int)fsin1.Position.X;
                    int y1 = (int)fsin1.Position.Y;
                    float z1 = fsin1.Position.Z;

                    // Fragment shader stage
                    pipeline.shader.FragmentShader(fsin0, out Vector4F color0);
                    pipeline.shader.FragmentShader(fsin1, out Vector4F color1);

                    //if (x >= 0 && y >= 0 && x < pipeline.frameBuffer.Width && y < pipeline.frameBuffer.Height)
                    //{
                    //    pipeline.frameBuffer.PutPixel(x, y, color);
                    //}
                }
                //});
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
