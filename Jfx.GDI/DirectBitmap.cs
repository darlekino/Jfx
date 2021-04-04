using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.GDI
{
    internal class DirectBitmap : IDisposable
    {
        public int Width { get; }
        public int Height { get; }
        public int[] Buffer { get; private set; }
        private GCHandle BufferHandle { get; set; }
        public Bitmap Bitmap { get; private set; }
        public Graphics Graphics { get; private set; }

        public DirectBitmap(int width, int height)
        {
            Width = width;
            Height = height;
            Buffer = new int[width * height];
            BufferHandle = GCHandle.Alloc(Buffer, GCHandleType.Pinned);
            Bitmap = new Bitmap(width, height, width * 4, PixelFormat.Format32bppPArgb, BufferHandle.AddrOfPinnedObject());
            Graphics = Graphics.FromImage(Bitmap);
        }

        public void Dispose()
        {
            Graphics.Dispose();
            Graphics = default;

            Bitmap.Dispose();
            Bitmap = default;

            BufferHandle.Free();
            BufferHandle = default;

            Buffer = default;
        }

        public void GetXY(int index, out int x, out int y)
        {
            y = index / Width;
            x = index - y * Width;
        }

        public int GetIndex(int x, int y) => x + y * Width;
        public void SetArgb(int x, int y, int argb) => Buffer[GetIndex(x, y)] = argb;
        public int GetArgb(int x, int y) => Buffer[GetIndex(x, y)];
        public void SetPixel(int x, int y, in Color color) => SetArgb(x, y, color.ToArgb());
        public Color GetPixel(int x, int y) => Color.FromArgb(GetArgb(x, y));
    }
}
