using Jfx.App.UI.Inputs;
using Jfx.Mathematic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;

namespace Jfx.App.UI.Gdi
{
    internal class GdiWindow : Window
    {
        enum Space
        {
            World,
            View,
            Screen
        }

        class DirectBitmap : IDisposable
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

        private Graphics graphicsHost;
        private IntPtr graphicsHostDeviceContext;
        private BufferedGraphics bufferedGraphics;
        private DirectBitmap backBuffer;
        private Font consolas12;

        public GdiWindow(IntPtr hostHandle, IInput input) : base(hostHandle, input)
        {
            graphicsHost = Graphics.FromHwnd(HostHandle);
            graphicsHostDeviceContext = graphicsHost.GetHdc();
            consolas12 = new Font("Consolas", 12);

            CreateSurface(SurfaceWidth, SurfaceHeight);
            CreateBuffers(BufferWidth, BufferHeight);
        }

        public override void Dispose()
        {
            consolas12.Dispose();
            consolas12.Dispose();

            DisposeSurface();
            DisposeBuffers();

            graphicsHost.ReleaseHdc(graphicsHostDeviceContext);

            graphicsHost.Dispose();

            base.Dispose();
        }
        private void CreateBuffers(int width, int height)
        {
            backBuffer = new DirectBitmap(width, height);
        }

        protected override void ResizeBuffers(int width, int height)
        {
            DisposeBuffers();
            CreateBuffers(width, height);
        }

        private void DisposeBuffers()
        {
            backBuffer.Dispose();
            backBuffer = default;
        }

        private void CreateSurface(int width, int height)
        {
            bufferedGraphics = BufferedGraphicsManager.Current.Allocate(graphicsHostDeviceContext, new Rectangle(0, 0, width, height));
            bufferedGraphics.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        }

        protected override void ResizeSurface(int width, int height)
        {
            DisposeSurface();
            CreateSurface(width, height);
        }

        private void DisposeSurface()
        {
            bufferedGraphics.Dispose();
            bufferedGraphics = default;
        }

        private float GetDeltaTime(TimeSpan periodDuration)
        {
            return GetDeltaTime(FrameStarted, periodDuration);
        }

        private static float GetDeltaTime(DateTime timestamp, TimeSpan periodDuration)
        {
            var result = (timestamp.Second * 1000 + timestamp.Millisecond) % periodDuration.TotalMilliseconds / periodDuration.TotalMilliseconds;
            return (float)result;
        }

        private static IReadOnlyList<IReadOnlyList<JfxVector3F>> Transform(in JfxMatrix4F transformation, IReadOnlyList<IReadOnlyList<JfxVector3F>> points)
        {
            var list = new List<IReadOnlyList<JfxVector3F>>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                list.Add(Transform(transformation, points[i]));
            }

            return list;
        }

        private static IReadOnlyList<JfxVector3F> Transform(in JfxMatrix4F transformation, IReadOnlyList<JfxVector3F> points)
        {
            var list = new List<JfxVector3F>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                list.Add(JfxVector3F.Transform(points[i], transformation));
            }

            return list;
        }

        private void DrawPolyline(Pen pen, Space space, params JfxVector3F[] points)
        {
            DrawPolyline(pen, space, points.ToList());
        }

        private void DrawPolyline(Pen pen, Space space, IReadOnlyList<JfxVector3F> points)
        {
            static JfxMatrix4F LookAtRH(in JfxVector3F cameraPosition, in JfxVector3F cameraTarget, in JfxVector3F cameraUpVector)
            {
                var zaxis = (cameraPosition - cameraTarget).Normilize();
                var xaxis = cameraUpVector.CrossProduct(zaxis).Normilize();
                var yaxis = zaxis.CrossProduct(xaxis);

                //var transformation = new JfxMatrix4F(
                //    xaxis.X, yaxis.X, zaxis.X, 0,
                //    xaxis.Y, yaxis.Y, zaxis.Y, 0,
                //    xaxis.Z, yaxis.Z, zaxis.Z, 0,
                //    0, 0, 0, 1
                //);

                //var translation = new JfxMatrix4F(
                //    1, 0, 0, 0,
                //    0, 1, 0, 0,
                //    0, 0, 1, 0,
                //    -cameraPosition.X, -cameraPosition.Y, -cameraPosition.Z, 1
                //);

                //  translation * transformation == new JfxMatrix4F(
                //    xaxis.X, yaxis.X, zaxis.X, 0,
                //    xaxis.Y, yaxis.Y, zaxis.Y, 0,
                //    xaxis.Z, yaxis.Z, zaxis.Z, 0,
                //    -xaxis.DotProduct(cameraPosition), -yaxis.DotProduct(cameraPosition), -zaxis.DotProduct(cameraPosition), 1
                //);
                // that's why we have dot product for translations (just try to multiply by yourself)

                /*
                 * x = x' + a;  ==> x' = x - a;
                 * y = y' + b;  ==> y' = y - b;
                 * that's why we have minus for translations
                 */

                return new JfxMatrix4F(
                    xaxis.X, yaxis.X, zaxis.X, 0,
                    xaxis.Y, yaxis.Y, zaxis.Y, 0,
                    xaxis.Z, yaxis.Z, zaxis.Z, 0,
                    -xaxis.DotProduct(cameraPosition), -yaxis.DotProduct(cameraPosition), -zaxis.DotProduct(cameraPosition), 1
                );
            }

            static JfxMatrix4F PerspectiveFovRH(float fieldOfViewY, float aspectRatio, float znearPlane, float zfarPlane)
            {
                float h = 1 / MathF.Tan(fieldOfViewY * 0.5f);
                float w = h / aspectRatio;
                return new JfxMatrix4F(
                    w, 0, 0, 0,
                    0, h, 0, 0,
                    0, 0, (znearPlane + zfarPlane) / (zfarPlane - znearPlane), -1,
                    0, 0, 2 * znearPlane * zfarPlane / (zfarPlane - znearPlane), 0
                );
            }

            static JfxMatrix4F OrthographicRH(float width, float height, float znearPlane, float zfarPlane)
            {
                return new JfxMatrix4F(
                    2 / width, 0, 0, 0,
                    0, 2 / height, 0, 0,
                    0, 0, 1 / (znearPlane - zfarPlane), 0,
                    0, 0, znearPlane / (znearPlane - zfarPlane), 1
                );
            }

            switch (space)
            {
                case Space.World:
                    var t = GetDeltaTime(new TimeSpan(0, 0, 0, 10));
                    var angle = t * MathF.PI * 2;
                    var radius = 2;

                    // view matrix
                    var cameraPosition = new JfxVector3F(MathF.Sin(angle) * radius, MathF.Cos(angle) * radius, 1);
                    var cameraTarget = new JfxVector3F(0, 0, 0);
                    var cameraUpVector = new JfxVector3F(0, 0, 1);
                    var viewMatrix = LookAtRH(cameraPosition, cameraTarget, cameraUpVector);


                    var fovY = MathF.PI * 0.5f;
                    var aspectRatio = Viewport.AspectRatio;
                    var nearPlane = 0.001f;
                    var farPlane = 1000;
                    var perspectiveMatrix = PerspectiveFovRH(fovY, aspectRatio, nearPlane, farPlane);


                    var filedHeight = 3f;
                    var filedWidth = filedHeight * aspectRatio;
                    var orthographicMatrix = OrthographicRH(filedWidth, filedHeight, nearPlane, farPlane);

                    var projectionMatrix = perspectiveMatrix;

                    var tramsformation = viewMatrix * projectionMatrix * Viewport.Transformation;
                    DrawPolylineScreenSpace(pen, Transform(tramsformation, points));
                    break;
                case Space.View:
                    DrawPolylineScreenSpace(pen, Transform(Viewport.Transformation, points));
                    break;
                case Space.Screen:
                    DrawPolylineScreenSpace(pen, points);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(space), space, null);
            }
        }

        private void DrawPolylineScreenSpace(Pen pen, IReadOnlyList<JfxVector3F> points)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                var from = points[i];
                var to = points[i + 1];
                backBuffer.Graphics.DrawLine(pen, from.X, from.Y, to.X, to.Y);
            }
        }

        private void DrawAxis()
        {
            DrawPolyline(Pens.Red, Space.World, new JfxVector3F(0, 0, 0), new JfxVector3F(1, 0, 0));
            DrawPolyline(Pens.LawnGreen, Space.World, new JfxVector3F(0, 0, 0), new JfxVector3F(0, 1, 0));
            DrawPolyline(Pens.Blue, Space.World, new JfxVector3F(0, 0, 0), new JfxVector3F(0, 0, 1));
        }


        private static readonly IReadOnlyList<IReadOnlyList<JfxVector3F>> CubePolylines;
        static GdiWindow()
        {
            var points = new[]
            {
                new[]
                {
                    new JfxVector3F(0, 0, 0),
                    new JfxVector3F(1, 0, 0),
                    new JfxVector3F(1, 1, 0),
                    new JfxVector3F(0, 1, 0),
                    new JfxVector3F(0, 0, 0),
                },
                new[]
                {
                    new JfxVector3F(0, 0, 1),
                    new JfxVector3F(1, 0, 1),
                    new JfxVector3F(1, 1, 1),
                    new JfxVector3F(0, 1, 1),
                    new JfxVector3F(0, 0, 1),
                },
                new[] { new JfxVector3F(0, 0, 0), new JfxVector3F(0, 0, 1), },
                new[] { new JfxVector3F(1, 0, 0), new JfxVector3F(1, 0, 1), },
                new[] { new JfxVector3F(1, 1, 0), new JfxVector3F(1, 1, 1), },
                new[] { new JfxVector3F(0, 1, 0), new JfxVector3F(0, 1, 1), },
            };

            CubePolylines = Transform(JfxMatrix4F.Translate(-0.5f, -0.5f, -0.5f), points);
        }

        private void DrawGeometry()
        {
            float angle = GetDeltaTime(new TimeSpan(0, 0, 0, 5)) * MathF.PI * 2;
            var matrixModel =
                JfxMatrix4F.Scale(0.5f) *
                JfxMatrix4F.Translate(1, 0, 0) *
                JfxMatrix4F.Rotate(new JfxVector3F(1, 0, 0).Normilize(), angle);

            foreach (var cubePolyline in CubePolylines)
            {
                DrawPolyline(Pens.White, Space.World, Transform(matrixModel, cubePolyline));
            }

            //smaller cube
            angle = GetDeltaTime(new TimeSpan(0, 0, 0, 1)) * MathF.PI * 2;
            matrixModel =
                JfxMatrix4F.Scale(0.5f) *
                JfxMatrix4F.Translate(0, 1, 0) *
                JfxMatrix4F.Rotate(new JfxVector3F(0, 1, 0).Normilize(), angle) *
                matrixModel;

            foreach (var cubePolyline in CubePolylines)
            {
                DrawPolyline(Pens.Yellow, Space.World, Transform(matrixModel, cubePolyline));
            }
        }

        protected override void RenderInternal()
        {
            backBuffer.Graphics.Clear(Color.Black);
            backBuffer.Graphics.DrawString(Fps.ToString(), consolas12, Brushes.Red, 0, 0);

            DrawAxis();
            DrawGeometry();

            // flush and swap buffers
            bufferedGraphics.Graphics.DrawImage(backBuffer.Bitmap, new RectangleF(0, 0, Viewport.Width, Viewport.Height), new RectangleF(-0.5f, -0.5f, BufferWidth, BufferHeight), GraphicsUnit.Pixel);
            bufferedGraphics.Render(graphicsHostDeviceContext);
        }
    }
}
