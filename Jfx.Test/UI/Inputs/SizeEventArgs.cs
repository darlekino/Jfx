﻿using System;
using System.Drawing;

namespace Jfx.Test.UI.Inputs
{
    internal class SizeEventArgs : EventArgs
    {
        public int Height { get; }
        public int Width { get; }

        public SizeEventArgs(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}