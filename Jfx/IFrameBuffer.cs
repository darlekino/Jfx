using Jfx.Mathematic;

namespace Jfx
{
    public interface IFrameBuffer
    {
        public int Width { get; }
        public int Height { get; }
        void PutPixel(int x, int y, in Vector4F color);
    }
}
