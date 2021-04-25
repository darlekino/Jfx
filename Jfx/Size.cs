using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx
{
    public class SizeEventArgs : EventArgs
    {
        private readonly Size size;
        public ref readonly Size Size => ref size;
        public SizeEventArgs(in Size size)
        {
            static Size Sanitize(int width, int height)
            {
                if (width < 1 || height < 1)
                {
                    return new Size(1, 1);
                }

                return new Size(width, height);
            }

            this.size = Sanitize(size.Width, size.Height);
        }
    }

    public readonly struct Size
    {
        public readonly int Width;
        public readonly int Height;

        public Size(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public override bool Equals(object obj)
        {
            if (obj is Size size)
                return size == this;

            return false;
        }

        public override int GetHashCode() => Width ^ Height;
        public static bool operator ==(in Size left, in Size right) => left.Width == right.Width && left.Height == right.Height;
        public static bool operator !=(in Size left, in Size right) => !(left == right);
    }
}
