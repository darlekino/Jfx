using Jfx.Mathematic;

namespace Jfx
{
    public class Model : IModel
    {
        public Vector3F[] Positions { get; }

        public Model(params Vector3F[] positions)
        {
            Positions = positions;
        }
    }

    public interface IModel
    {
        Vector3F[] Positions { get; }
    }
}
