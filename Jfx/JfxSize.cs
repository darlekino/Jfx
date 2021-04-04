﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx
{
    public class JfxSizeEventArgs : EventArgs
    {
        private readonly JfxSize size;
        public ref readonly JfxSize Size => ref size;
        public JfxSizeEventArgs(in JfxSize size)
        {
            static JfxSize Sanitize(int width, int height)
            {
                if (width < 1 || height < 1)
                {
                    return new JfxSize(1, 1);
                }

                return new JfxSize(width, height);
            }

            this.size = Sanitize(size.Width, size.Height);
        }
    }

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
