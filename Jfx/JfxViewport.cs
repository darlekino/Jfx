using Jfx.Mathematic;
using System;

namespace Jfx
{
    public class JfxViewport
    {
        public readonly int X;
        public readonly int Y;
        public readonly float MinZ;
        public readonly float MaxZ;
        private JfxMatrix4F transformation;
        private JfxSize size;

        public float AspectRatio { get; private set; }
        public JfxSize Size
        {
            get => size;
            set
            {
                size = value;
                Update();
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
        public ref readonly JfxMatrix4F Transformation => ref transformation;
        public event EventHandler Changed;

        public JfxViewport(int x, int y, in JfxSize size, float minZ, float maxZ)
        {
            X = x;
            Y = y;
            MinZ = minZ;
            MaxZ = maxZ;
            Size = size;
            Update();
        }

        private void Update()
        {
            AspectRatio = (float)size.Width / size.Height;

            float halfOfWidth = 0.5f * size.Width;
            float halfOfHeight = 0.5f * size.Height;

            transformation = new JfxMatrix4F(
                halfOfWidth, 0, 0, 0,
                0, -halfOfHeight, 0, 0,
                0, 0, MaxZ - MinZ, 0,
                X + halfOfWidth, Y + halfOfHeight, MinZ, 1
            );
        }
    }
}
