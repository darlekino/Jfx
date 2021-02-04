using Jfx.Mathematic;
using System;
using System.Drawing;

namespace Jfx.Test.UI.Gdi.Extensions
{
    internal static class RasterizationExtensions
    {
        private static void Swap<T>(ref T x0, ref T y0, ref T x1, ref T y1)
        {
            var tmp = x0;
            x0 = x1;
            x1 = tmp;

            tmp = y0;
            y0 = y1;
            y1 = tmp;
        }

        private static JfxList<int> Interpolate(int i0, int d0, int i1, int d1)
        {
            if (i0 == i1)
            {
                return new JfxList<int>(elem: d0);
            }

            var values = new JfxList<int>(i1 - i0 + 1);
            float a = (d1 - d0) / (float)(i1 - i0);
            float d = d0;
            for (int i = i0; i <= i1; i++)
            {
                values.Add((int)d);
                d += a;
            }

            return values;
        }

        public static void DrawLine(this DirectBitmap bitmap, in JfxPoint2D p0, in JfxPoint2D p1, in Color color) 
        {
            bitmap.DrawLine(p0.X, p0.Y, p1.X, p1.Y, color);
        }

        public static void DrawLine(this DirectBitmap bitmap, int x0, int y0, int x1, int y1, in Color color)
        {

            if (Math.Abs(x1 - x0) > Math.Abs(y1 - y0))
            {
                if (x0 > x1)
                {
                    Swap(ref x0, ref y0, ref x1, ref y1);
                }


                var ys = Interpolate(x0, y0, x1, y1);
                try
                {
                    for (int x = x0; x <= x1; x++)
                    {
                        bitmap.SetPixel(x, ys[x - x0], color);
                    }
                }
                finally
                {
                    ys.Dispose();
                }
            }
            else
            {
                if (y0 > y1)
                {
                    Swap(ref x0, ref y0, ref x1, ref y1);
                }

                var xs = Interpolate(y0, x0, y1, x1);
                try
                {
                    for (int y = y0; y <= y1; y++)
                    {
                        bitmap.SetPixel(xs[y - y0], y, color);
                    }
                }
                finally
                {
                    xs.Dispose();
                }
            }
        }

        public static void DrawWireframeTriangle(
                this DirectBitmap bitmap, 
                in JfxPoint2D p0, 
                in JfxPoint2D p1, 
                in JfxPoint2D p2, 
                in Color color) 
            
        {
            bitmap.DrawLine(p0, p1, color);
            bitmap.DrawLine(p1, p2, color);
            bitmap.DrawLine(p2, p0, color);
        }

        public static void DrawFilledTriangle(
                this DirectBitmap bitmap,
                in JfxPoint2D p0,
                in JfxPoint2D p1,
                in JfxPoint2D p2,
                in Color color) 
            
        {
            bitmap.DrawFilledTriangle(p0.X, p0.Y, p1.X, p1.Y, p2.X, p2.Y, color);
        }

        private static void DrawFilledTriangle(this DirectBitmap bitmap, int x0, int y0, int x1, int y1, int x2, int y2, in Color color)
        {
            if (y1 < y0) { Swap(ref x1, ref y1, ref x0, ref y0); }
            if (y2 < y0) { Swap(ref x2, ref y2, ref x0, ref y0); }
            if (y2 < y1) { Swap(ref x2, ref y2, ref x1, ref y1); }

            using var x01 = Interpolate(y0, x0, y1, x1);
            using var x12 = Interpolate(y1, x1, y2, x2);
            using var x02 = Interpolate(y0, x0, y2, x2);

            x01.RemoveLast();
            var x012 = new JfxList2<int>(x01, x12);

            IJfxList<int> x_left = default;
            IJfxList<int> x_right = default;

            var m = x012.Length / 2;
            if (x02[m] < x012[m])
            {
                x_left = x02;
                x_right = x012;
            }
            else
            {
                x_left = x012;
                x_right = x02;
            }

            for (int y = y0; y < y2; y++)
            {
                for (int x = x_left[y - y0]; x < x_right[y - y0]; x++)
                {
                    bitmap.SetPixel(x, y, color);
                }
            }
        }

        public static void DrawShadedTriangle(
                this DirectBitmap bitmap,
                in JfxPoint2D p0,
                in JfxPoint2D p1,
                in JfxPoint2D p2,
                in Color color)
            
        {
            bitmap.DrawShadedTriangle(p0.X, p0.Y, p1.X, p1.Y, p2.X, p2.Y, color);
        }

        private static JfxList<float> Interpolate(int i0, float d0, int i1, float d1)
        {
            if (i0 == i1)
            {
                return new JfxList<float>(d0);
            }

            var values = new JfxList<float>(i1 - i0 + 1);
            float a = (d1 - d0) / (i1 - i0);
            float d = d0;
            for (int i = i0; i <= i1; i++)
            {
                values.Add(d);
                d += a;
            }

            return values;
        }

        private static void DrawShadedTriangle(this DirectBitmap bitmap, int x0, int y0, int x1, int y1, int x2, int y2, in Color color)
        {
            if (y1 < y0) { Swap(ref x1, ref y1, ref x0, ref y0); }
            if (y2 < y0) { Swap(ref x2, ref y2, ref x0, ref y0); }
            if (y2 < y1) { Swap(ref x2, ref y2, ref x1, ref y1); }

            const float h0 = 1;
            const float h1 = 0.5f;
            const float h2 = 0;

            using var x01 = Interpolate(y0, x0, y1, x1);
            using var h01 = Interpolate(y0, h0, y1, h1);

            using var x12 = Interpolate(y1, x1, y2, x2);
            using var h12 = Interpolate(y1, h1, y2, h2);

            using var x02 = Interpolate(y0, x0, y2, x2);
            using var h02 = Interpolate(y0, h0, y2, h2);

            x01.RemoveLast();
            h01.RemoveLast();
            var x012 = new JfxList2<int>(x01, x12);
            var h012 = new JfxList2<float>(h01, h12);

            IJfxList<int> x_left = default;
            IJfxList<int> x_right = default;

            IJfxList<float> h_left = default;
            IJfxList<float> h_right = default;

            var m = x012.Length / 2;
            if (x02[m] < x012[m])
            {
                x_left = x02;
                x_right = x012;

                h_left = h02;
                h_right = h012;
            }
            else
            {
                x_left = x012;
                x_right = x02;

                h_left = h012;
                h_right = h02;
            }

            for (int y = y0; y < y2; y++)
            {
                var x_l = x_left[y - y0];
                var x_r = x_right[y - y0];

                using var h_segment = Interpolate(x_l, h_left[y - y0], x_r, h_right[y - y0]);
                for (int x = x_l; x < x_r; x++)
                {
                    var h = h_segment[x - x_l];
                    float r = color.R * h;
                    float g = color.G * h;
                    float b = color.B * h;
                    bitmap.SetPixel(x, y, Color.FromArgb((int)r, (int)g, (int)b));
                }
            }
        }
    }
}
