using Jfx.Mathematic;
using System;

namespace Jfx
{
    public readonly struct Viewport
    {
        public readonly int X;
        public readonly int Y;
        public readonly float MinZ;
        public readonly float MaxZ;
        public readonly Matrix4F Matrix;
        public readonly Matrix4F MatrixInverse;
        public readonly Size Size;
        public readonly float AspectRatio;

        public Viewport(int x, int y, in Size size, float minZ, float maxZ)
        {
            X = x;
            Y = y;
            MinZ = minZ;
            MaxZ = maxZ;
            Size = size;
            AspectRatio = (float)Size.Width / Size.Height;
            float halfOfWidth = 0.5f * size.Width;
            float halfOfHeight = 0.5f * size.Height;

            Matrix = new Matrix4F(
                halfOfWidth, 0, 0, 0,
                0, -halfOfHeight, 0, 0,
                0, 0, MaxZ - MinZ, 0,
                X + halfOfWidth, Y + halfOfHeight, MinZ, 1
            );

            MatrixInverse = Matrix.Inverse();
        }

        public Viewport(in Viewport viewport, in Size size) : this(viewport.X, viewport.Y, size, viewport.MinZ, viewport.MaxZ)
        {
        }
    }
}
