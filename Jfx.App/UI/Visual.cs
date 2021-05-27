using Jfx.Mathematic;
using System;

namespace Jfx.App.UI
{
    public class Visual : IDisposable
    {
        private readonly IModel model;
        private readonly VertexBuffer vertexBuffer;

        public Shader Shader { get; }
        public PrimitiveTopology PrimitiveTopology { get; }
        public Processing Processing { get; }

        public Visual(IModel model, Shader shader, PrimitiveTopology primitiveTopology, Processing processing)
        {
            this.model = model;
            Shader = shader;
            PrimitiveTopology = primitiveTopology;
            Processing = processing;
            this.vertexBuffer = new VertexBuffer(model.Positions);
        }

        public IVertexBuffer<Vector3F> GetVertexBuffer() => vertexBuffer;

        public void Dispose() => vertexBuffer.Dispose();
    }

    public class Shader : IShader<Vector3F, Nothing>
    {
        private Matrix4F matrixToClip;
        private static Vector4F white = new Vector4F(1, 1, 1, 1);
        private readonly Vector4F color;

        public Shader(Vector4F? color = null)
        {
            this.color = color ?? white;
        }

        public void Update(in Matrix4F matrixToClip) => this.matrixToClip = matrixToClip;

        public Vector4F VertexShader(in Vector3F vsin, out Nothing fsin)
        {
            fsin = default;
            return Vector4F.Transform(new Vector4F(vsin, 1), matrixToClip);
        }

        public Vector4F FragmentShader(in Vector4F fragCoord, in Nothing _) => color;
    }
}
