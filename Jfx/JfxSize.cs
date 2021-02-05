using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx
{
    public readonly struct JfxSize
    {
        public readonly int Width;
        public readonly int Height;

        public JfxSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override bool Equals(object obj)
        {
            if (obj is JfxSize size)
                return size == this;

            return false;
        }

        public override int GetHashCode() => Width ^ Height;
        public static bool operator ==(in JfxSize left, in JfxSize right) => left.Width == right.Width && left.Height == right.Height;
        public static bool operator !=(in JfxSize left, in JfxSize right) => !(left == right);
    }
}
