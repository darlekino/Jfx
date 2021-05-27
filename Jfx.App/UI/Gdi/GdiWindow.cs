using Jfx.App.UI.Inputs;
using Jfx.Mathematic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using GDIColor = System.Drawing.Color;

namespace Jfx.App.UI.Gdi
{
    public class GdiWindow : Window
    {
        enum Space
        {
            World,
            View,
            Screen
        }

        class DirectBitmap : IDisposable, IFrameBuffer
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
            public void SetPixel(int x, int y, in GDIColor color) => SetArgb(x, y, color.ToArgb());
            public GDIColor GetPixel(int x, int y) => GDIColor.FromArgb(GetArgb(x, y));

            public void PutPixel(int x, int y, in Vector4F color)
            {
                var r = (byte)(color.X * byte.MaxValue);
                var g = (byte)(color.Y * byte.MaxValue);
                var b = (byte)(color.Z * byte.MaxValue);
                var a = (byte)(color.W * byte.MaxValue);
                var argb = ((((a << 8) + r) << 8) + g << 8) + b;
                SetArgb(x, y, argb);
            }
        }

        private Graphics graphicsHost;
        private IntPtr graphicsHostDeviceContext;
        private BufferedGraphics bufferedGraphics;
        private DirectBitmap backBuffer;
        private Font consolas12;
        private Shader shader;
        private Pipeline pipeline;

        public GdiWindow(IntPtr hostHandle, IInput input) : base(hostHandle, input)
        {
            graphicsHost = Graphics.FromHwnd(HostHandle);
            graphicsHostDeviceContext = graphicsHost.GetHdc();
            CreateSurface(bufferSize);
            CreateBuffers(bufferSize);
            consolas12 = new Font("Consolas", 12);
            shader = new Shader();
            pipeline = new Pipeline(Camera.Viewport, backBuffer);
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

        private void CreateBuffers(in Size size)
        {
            backBuffer = new DirectBitmap(size.Width, size.Height);
        }

        protected override void ResizeBuffers(in Size size)
        {
            DisposeBuffers();
            CreateBuffers(size);
        }

        private void DisposeBuffers()
        {
            backBuffer?.Dispose();
        }

        private void CreateSurface(in Size size)
        {
            var rectangle = new Rectangle(0, 0, size.Width, size.Height);
            bufferedGraphics = BufferedGraphicsManager.Current.Allocate(graphicsHostDeviceContext, rectangle);
            bufferedGraphics.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        }

        protected override void ResizeSurface(in Size size)
        {
            DisposeSurface();
            CreateSurface(size);
        }

        private void DisposeSurface()
        {
            bufferedGraphics?.Dispose();
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

        private static IReadOnlyList<IReadOnlyList<Vector3F>> Transform(in Matrix4F transformation, IReadOnlyList<IReadOnlyList<Vector3F>> points)
        {
            var list = new List<IReadOnlyList<Vector3F>>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                list.Add(Transform(transformation, points[i]));
            }

            return list;
        }

        private static IReadOnlyList<Vector3F> Transform(in Matrix4F transformation, IReadOnlyList<Vector3F> points)
        {
            var list = new List<Vector3F>(points.Count);
            for (int i = 0; i < points.Count; i++)
            {
                list.Add(Vector3F.Transform(points[i], transformation));
            }

            return list;
        }

        private void DrawPolyline(Pen pen, Space space, params Vector3F[] points)
        {
            DrawPolyline(pen, space, points.ToList());
        }

        private void DrawPolyline(Pen pen, Space space, IReadOnlyList<Vector3F> points)
        {
            switch (space)
            {
                case Space.World:
                    DrawPolylineScreenSpace(pen, Transform(Camera.TransformMatrix, points));
                    break;
                case Space.View:
                    DrawPolylineScreenSpace(pen, Transform(Camera.Viewport.Matrix, points));
                    break;
                case Space.Screen:
                    DrawPolylineScreenSpace(pen, points);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(space), space, null);
            }
        }

        private void DrawPolylineScreenSpace(Pen pen, IReadOnlyList<Vector3F> points)
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
            DrawPolyline(Pens.Red, Space.World, new Vector3F(0, 0, 0), new Vector3F(1, 0, 0));
            DrawPolyline(Pens.LawnGreen, Space.World, new Vector3F(0, 0, 0), new Vector3F(0, 1, 0));
            DrawPolyline(Pens.Blue, Space.World, new Vector3F(0, 0, 0), new Vector3F(0, 0, 1));
        }


        private static readonly IReadOnlyList<IReadOnlyList<Vector3F>> CubePolylines;

        static GdiWindow()
        {
            var points = new[]
            {
                new[]
                {
                    new Vector3F(0, 0, 0),
                    new Vector3F(1, 0, 0),
                    new Vector3F(1, 1, 0),
                    new Vector3F(0, 1, 0),
                    new Vector3F(0, 0, 0),
                },
                new[]
                {
                    new Vector3F(0, 0, 1),
                    new Vector3F(1, 0, 1),
                    new Vector3F(1, 1, 1),
                    new Vector3F(0, 1, 1),
                    new Vector3F(0, 0, 1),
                },
                new[] { new Vector3F(0, 0, 0), new Vector3F(0, 0, 1), },
                new[] { new Vector3F(1, 0, 0), new Vector3F(1, 0, 1), },
                new[] { new Vector3F(1, 1, 0), new Vector3F(1, 1, 1), },
                new[] { new Vector3F(0, 1, 0), new Vector3F(0, 1, 1), },
            };

            CubePolylines = new List<IReadOnlyList<Vector3F>>(); //Transform(Matrix4F.Translate(-0.5f, -0.5f, -0.5f), points);
        }

        private void DrawGeometry()
        {
            float angle = GetDeltaTime(new TimeSpan(0, 0, 0, 5)) * MathF.PI * 2;
            var matrixModel = 
                Matrix4F.Scale(0.5f) *
                Matrix4F.Translate(1, 0, 0) *
                Matrix4F.Rotate(new Vector3F(1, 0, 0), angle);

            foreach (var cubePolyline in CubePolylines)
            {
                DrawPolyline(Pens.White, Space.World, Transform(matrixModel, cubePolyline));
            }

            //smaller cube
            angle = GetDeltaTime(new TimeSpan(0, 0, 0, 1)) * MathF.PI * 2;
            matrixModel =
                Matrix4F.Scale(0.5f) *
                Matrix4F.Translate(0, 1, 0) *
                Matrix4F.Rotate(new Vector3F(0, 1, 0), angle) *
                matrixModel;

            foreach (var cubePolyline in CubePolylines)
            {
                DrawPolyline(Pens.Yellow, Space.World, Transform(matrixModel, cubePolyline));
            }
        }

        protected override void RenderInternal(IEnumerable<Visual> models)
        {
            backBuffer.Graphics.Clear(GDIColor.Black);
            backBuffer.Graphics.DrawString(Fps.ToString(), consolas12, Brushes.Red, 0, 0);

            DrawAxis();
            //DrawGeometry();

            shader.Update(Camera.MatrixToClip);

            foreach (var m in models)
            {
                pipeline.Render(shader, m.GetVertexBuffer(), PrimitiveTopology.PointList, Processing.Parallel);
            }

            // flush and swap buffers
            bufferedGraphics.Graphics.DrawImage(
                backBuffer.Bitmap, 
                new RectangleF(0, 0, Camera.Viewport.Size.Width, Camera.Viewport.Size.Height), 
                new RectangleF(-0.5f, -0.5f, bufferSize.Width, bufferSize.Height), 
                GraphicsUnit.Pixel
            );

            bufferedGraphics.Render(graphicsHostDeviceContext);
        }
    }
}
