using Jfx.Mathematic;
using System;

namespace Jfx
{
    public readonly struct JfxViewport
    {
        public readonly int X;
        public readonly int Y;
        public readonly float MinZ;
        public readonly float MaxZ;
        public readonly JfxMatrix4F Matrix;
        public readonly JfxMatrix4F MatrixInverse;
        public readonly JfxSize Size;
        public readonly float AspectRatio;

        public JfxViewport(int x, int y, in JfxSize size, float minZ, float maxZ)
        {
            X = x;
            Y = y;
            MinZ = minZ;
            MaxZ = maxZ;
            Size = size;
            AspectRatio = (float)Size.Width / Size.Height;
            float halfOfWidth = 0.5f * size.Width;
            float halfOfHeight = 0.5f * size.Height;

            Matrix = new JfxMatrix4F(
                halfOfWidth, 0, 0, 0,
                0, -halfOfHeight, 0, 0,
                0, 0, MaxZ - MinZ, 0,
                X + halfOfWidth, Y + halfOfHeight, MinZ, 1
            );

            MatrixInverse = Matrix.Inverse();
        }

        public JfxViewport(in JfxViewport viewport, in JfxSize size) : this(viewport.X, viewport.Y, size, viewport.MinZ, viewport.MaxZ)
        {
        }
    }
}
