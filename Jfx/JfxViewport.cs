using Jfx.Mathematic;
using System;

namespace Jfx
{
    public class JfxViewport : JfxTransformable
    {
        private int x;
        private int y;
        private JfxSize size;
        private float minZ;
        private float maxZ;
        private float aspectRatio;

        public JfxViewport(int x, int y, in JfxSize size, float minZ, float maxZ)
        {
            Update(x, y, size, minZ, maxZ);
        }

        public int X
        {
            set 
            {
                x = value;
                UpdateTransformation();
            }
            get => x;
        }

        public int Y
        {
            set
            {
                y = value;
                UpdateTransformation();
            }
            get => y;
        }

        public float MinZ
        {
            set
            {
                minZ = value;
                UpdateTransformation();
            }
            get => minZ;
        }

        public float MaxZ
        {
            set
            {
                maxZ = value;
                UpdateTransformation();
            }
            get => maxZ;
        }

        public float AspectRatio => aspectRatio;
        public ref readonly JfxSize Size => ref size;

        public event EventHandler SizeChanged;

        internal protected override void UpdateTransformation()
        {
            float halfOfWidth = 0.5f * size.Width;
            float halfOfHeight = 0.5f * size.Height;

            transformation = new JfxMatrix4F(
                halfOfWidth, 0, 0, 0,
                0, -halfOfHeight, 0, 0,
                0, 0, maxZ - minZ, 0,
                x + halfOfWidth, y + halfOfHeight, minZ, 1
            );

            base.UpdateTransformation();
        }

        public void UpdateSize(in JfxSize size)
        {
            this.size = size;
            aspectRatio = (float)size.Width / size.Height;
            UpdateTransformation();

            SizeChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Update(int x, int y, in JfxSize size, float minZ, float maxZ)
        {
            this.x = x;
            this.y = y;
            this.size = size;
            this.minZ = minZ;
            this.maxZ = maxZ;
            aspectRatio = (float)size.Width / size.Height;
            UpdateTransformation();

            SizeChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
