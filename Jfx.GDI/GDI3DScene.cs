using Jfx.Mathematic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Jfx.GDI
{
    public class GDI3dScene : IJfx3DScene
    {
        enum Space
        {
            World,
            View,
            Screen
        }

        private readonly Graphics graphics;
        private readonly IntPtr graphicsDeviceContext;
        private BufferedGraphics bufferedGraphics;
        private DirectBitmap backBuffer;
        private Font consolas12;
        private JfxMatrix4F transformation;

        public JfxCamera Camera { get; }
        public JfxProjection Projection { get; }
        public JfxViewport Viewport { get; }
        public event EventHandler<JfxSizeEventArgs> WindowResized;

        public GDI3dScene(Graphics graphics, JfxCamera camera, JfxProjection projection, JfxViewport viewport, in JfxSize windowSize)
        {
            Camera = camera;
            Projection = projection;
            Viewport = viewport;
            UpdateTransformation();

            Camera.Changed += OnTransformationChanged;
            Viewport.Changed += OnTransformationChanged;
            WindowResized += OnWindowResized;
            CreateSurface(Viewport.Size);
            CreateBuffers(windowSize);

            this.graphics = graphics;
            graphicsDeviceContext = graphics.GetHdc();
            consolas12 = new Font("Consolas", 12);
        }

        public void Dispose()
        {
            consolas12.Dispose();
            consolas12.Dispose();

            bufferedGraphics.Dispose();
            backBuffer.Dispose();

            graphics.ReleaseHdc(graphicsDeviceContext);
            graphics.Dispose();
        }

        private void UpdateTransformation() => transformation = Camera.Transformation * Projection.Transformation * Viewport.Transformation;

        private void OnTransformationChanged(object _, EventArgs __)
        {
            UpdateTransformation();
        }

        private void OnWindowResized(object _, JfxSizeEventArgs e)
        {
            Viewport.Size = e.Size;
            ResizeBuffers(e.Size);
            ResizeSurface(e.Size);
        }

        private void CreateBuffers(in JfxSize size)
        {
            backBuffer = new DirectBitmap(size.Width, size.Height);
        }

        protected void ResizeBuffers(in JfxSize size)
        {
            backBuffer.Dispose();
            CreateBuffers(size);
        }

        private void CreateSurface(in JfxSize size)
        {
            var rectangle = new Rectangle(0, 0, size.Width, size.Height);
            bufferedGraphics = BufferedGraphicsManager.Current.Allocate(graphicsDeviceContext, rectangle);
            bufferedGraphics.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
        }

        protected void ResizeSurface(in JfxSize size)
        {
            bufferedGraphics.Dispose();
            CreateSurface(size);
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
            switch (space)
            {
                case Space.World:
                    DrawPolylineScreenSpace(pen, Transform(Transformation, points));
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
        static GDI3dScene()
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

         public void RenderInternal()
        {
            backBuffer.Graphics.Clear(Color.Black);
            backBuffer.Graphics.DrawString(Fps.ToString(), consolas12, Brushes.Red, 0, 0);

            DrawAxis();
            DrawGeometry();

            // flush and swap buffers
            bufferedGraphics.Graphics.DrawImage(
                backBuffer.Bitmap,
                new RectangleF(0, 0, Viewport.Size.Width, Viewport.Size.Height),
                new RectangleF(-0.5f, -0.5f, BufferSize.Width, BufferSize.Height),
                GraphicsUnit.Pixel
            );

            bufferedGraphics.Render(graphicsHostDeviceContext);
        }
    }
}
}
