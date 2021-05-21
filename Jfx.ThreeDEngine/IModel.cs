using Jfx.Mathematic;

namespace Jfx.ThreeDEngine
{
    public class Model : IModel
    {
        private readonly VertexBuffer vertexBuffer;
        public Vector3F[] Positions { get; }

        public Model(Vector3F[] positions)
        {
            Positions = positions;
            vertexBuffer = new VertexBuffer(positions);
        }

        public IVertexBuffer<Vector3F> GetVertexBuffer()
        {
            return vertexBuffer;
        }
    }

    public interface IModel
    {
        public Vector3F[] Positions { get; }
        public IVertexBuffer<Vector3F> GetVertexBuffer();
    }
}
