using Jfx.Mathematic;
using System;

namespace Jfx.App.UI
{
    public class Visual : IDisposable
    {
        private readonly IModel model;
        private readonly VertexBuffer vertexBuffer;

        public Shader Shader { get; }

        public Visual(IModel model, Shader shader)
        {
            this.model = model;
            Shader = shader;
            this.vertexBuffer = new VertexBuffer(model.Positions);
        }

        public IVertexBuffer<Vector3F> GetVertexBuffer() => vertexBuffer;

        public void Dispose() => vertexBuffer.Dispose();
    }

    public struct FSIn : IFSIn
    {
        public Vector4F Position { get; set; }
    }

    public class Shader : IShader<Vector3F, FSIn>
    {
        private Matrix4F matrixToClip;
        private static Vector4F white = new Vector4F(1, 1, 1, 1);

        public void Update(in Matrix4F matrixToClip) => this.matrixToClip = matrixToClip;

        public void VertexShader(in Vector3F vsin, out FSIn fsin)
        {
            fsin = new FSIn { Position = Vector4F.Transform(new Vector4F(vsin, 1), matrixToClip) };
        }

        public void FragmentShader(in FSIn fsin, out Vector4F color)
        {
            color = white;
        }
    }
}
