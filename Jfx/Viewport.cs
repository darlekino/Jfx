using Jfx.Mathematic;

namespace Jfx
{
    public readonly struct Viewport
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Width;
        public readonly int Height;
        public readonly float MinZ;
        public readonly float MaxZ;
        public readonly float AspectRatio;
        public readonly JfxMatrix4F Transformation;

        public Viewport(int x, int y, int width, int height, float minZ, float maxZ)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            MinZ = minZ;
            MaxZ = maxZ;
            AspectRatio = (float)Width / Height;

            float halfOfWidth = 0.5f * Width;
            float halfOfHeight = 0.5f * Height;

            Transformation = new JfxMatrix4F(
                halfOfWidth, 0, 0, 0,
                0, -halfOfHeight, 0, 0,
                0, 0, MaxZ - MinZ, 0,
                X + halfOfWidth, Y + halfOfHeight, MinZ, 1
            );
        }
    }
}
