using Jfx.Mathematic;
using System;

namespace Jfx
{
    public class Pipeline<TShader, TVSIn, TFSIn>
        where TShader : IShader<TVSIn, TFSIn>
        where TVSIn : unmanaged
        where TFSIn : unmanaged, IFSIn
    {
        private IShader<TVSIn, TFSIn> shader;
        private Viewport viewport;
        private IFrameBuffer frameBuffer;

        public IShader<TVSIn, TFSIn> Shader
        {
            get => shader;
            set => shader = value!;
        }

        public Pipeline(IShader<TVSIn, TFSIn> shader, in Viewport viewport, IFrameBuffer frameBuffer)
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

        private static class PointListTopology
        {
            public static unsafe void Render(Pipeline<TShader, TVSIn, TFSIn> pipeline, IVertexBuffer<TVSIn> buffer)
            {
                for (int i = 0; i < buffer.Count; i++)
                {
                    // Vertex shader stage
                    pipeline.shader.VertexShader(*buffer[i], out TFSIn fsin);
                    VertexPostProcessing(pipeline, ref fsin);

                    int x = (int)fsin.Position.X;
                    int y = (int)fsin.Position.Y;
                    float z = fsin.Position.Z;

                    // Fragment shader stage
                    pipeline.shader.FragmentShader(fsin, out Vector4F color);

                    if (x < 0 || y < 0 || x >= pipeline.frameBuffer.Width || y >= pipeline.frameBuffer.Height)
                    {
                        return;
                    }

                    pipeline.frameBuffer.PutPixel(x, y, color);
                }
            }

            private static void VertexPostProcessing(Pipeline<TShader, TVSIn, TFSIn> pipeline, ref TFSIn fsin)
            {
                var wInv = 1 / fsin.Position.W;
                fsin.Position = Vector4F.Transform(new Vector4F(fsin.Position.X * wInv, fsin.Position.Y * wInv, fsin.Position.Z * wInv), pipeline.viewport.Matrix);
            }
        }
    }
}
