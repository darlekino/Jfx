using Jfx.Mathematic;
using System;

namespace Jfx.App.UI
{
    public class Visual : IDisposable
    {
        private readonly IModel model;
        private readonly VertexBuffer vertexBuffer;

        public Shaders Shaders { get; }

        public Visual(IModel model, Shaders shader)
        {
            this.model = model;
            Shaders = shader;
            this.vertexBuffer = new VertexBuffer(model.Positions);
        }

        public IVertexBuffer<Vector3F> GetVertexBuffer() => vertexBuffer;

        public void Dispose() => vertexBuffer.Dispose();
    }

    public class VertexShader : IVertexShader<Vector3F, Nothing>
    {
        private Matrix4F matrixToClip;

        public void Update(in Matrix4F matrixToClip) => this.matrixToClip = matrixToClip;

        public Vector4F ExecuteStage(in Vector3F vsin, out Nothing fsin)
        {
            fsin = default;
            return Vector4F.Transform(new Vector4F(vsin, 1), matrixToClip);
        }
    }

    public class FragmentShader : IFragmentShader<Nothing>
    {
        private static Vector4F white = new Vector4F(1, 1, 1, 1);
        private readonly Vector4F color;

        public FragmentShader(Vector4F? color = null)
        {
            this.color = color ?? white;
        }

        public bool ExecuteStage(in Vector4F fragCoord, in Nothing _, out Vector4F color)
        {
            color = this.color;
            return true;
        }
    }

    public class Shaders : IShaders<Vector3F, Nothing>
    {
        private readonly VertexShader vertexShader;
        private readonly FragmentShader fragmentShader;

        public IVertexShader<Vector3F, Nothing> VertexShader => vertexShader;
        public IFragmentShader<Nothing> FragmentShader => fragmentShader;

        public PrimitiveTopology PrimitiveTopology { get; }
        public Processing Processing { get; }
        public Interpolation Interpolation { get; }

        public Shaders(PrimitiveTopology primitiveTopology, Processing processing, Interpolation interpolation, Vector4F? color = null)
        {
            vertexShader = new VertexShader();
            fragmentShader = new FragmentShader(color);
            PrimitiveTopology = primitiveTopology;
            Processing = processing;
            Interpolation = interpolation;
        }

        public void Update(in Matrix4F matrixToClip) => vertexShader.Update(matrixToClip);
    }
}
