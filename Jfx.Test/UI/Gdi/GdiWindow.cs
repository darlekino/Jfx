using Jfx.Test.UI.Inputs;
using Jfx.Mathematic;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Buffers;
using Jfx.Test.UI.Gdi.Extensions;

namespace Jfx.Test.UI.Gdi
{
    interface IJfxList<T>
    {
        ref T this[int index] { get; }
    }

    internal readonly ref struct SmallList<T>
    {
        private readonly Span<T> span;
        public SmallList(Span<T> span)
        {
            this.span = span;
        }

        public ref T this[int index]
        {
            get
            {
#if DEBUG
                if (index >= Length || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
#endif

                return ref span[index];
            }
        }

        public int Length => span.Length;
    }

    internal class JfxList<T> : IJfxList<T>, IDisposable
    {
        private T[] buff;
        public int length;

        public JfxList(int minCapacity)
        {
            length = 0;
            buff = minCapacity == 0 ? default : ArrayPool<T>.Shared.Rent(minCapacity);
        }

        public JfxList(T elem) : this(1)
        {
            Add(elem);
        }

        public ref T this[int index]
        {
            get
            {
#if DEBUG
                if (index >= Length || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
#endif

                return ref buff[index];
            }
        }

        public int Length => length;

        public void Add(T item)
        {
            buff[Length] = item;
            length++;
        }

        public void RemoveLast()
        {
            if (length > 0)
            {
                length--;
            }
        }

        public void Dispose()
        {
            if (buff != default)
            {
                ArrayPool<T>.Shared.Return(buff);
            }
        }
    }

    internal class JfxList2<T> : IJfxList<T>
    {
        JfxList<T> vec0;
        JfxList<T> vec1;

        public JfxList2(JfxList<T> vec0, JfxList<T> vec1)
        {
            this.vec0 = vec0;
            this.vec1 = vec1;
        }

        public ref T this[int index]
        {
            get
            {
#if DEBUG
                if (index >= Length || index < 0)
                {
                    throw new IndexOutOfRangeException();
                }
#endif


                if (index < vec0.Length)
                {
                    return ref vec0[index];
                }

                return ref vec1[index - vec0.Length];
            }
        }

        public int Length => vec0.Length + vec1.Length;
    }

    internal class GdiWindow : Window
    {
        private readonly Graphics GraphicsHost;
        private readonly IntPtr GraphicsHostDeviceContext;
        private readonly Font Consolas12;
        private BufferedGraphics bufferedGraphics;
        private DirectBitmap backBuffer;

        public GdiWindow(IntPtr hostHandle, IInput input) : base(hostHandle, input)
        {
            GraphicsHost = Graphics.FromHwnd(HostHandle);
            GraphicsHostDeviceContext = GraphicsHost.GetHdc();
            Consolas12 = new Font("Consolas", 12);

            CreateSurface(SurfaceWidth, SurfaceHeight);
            CreateBuffers(BufferWidth, BufferHeight);
        }

        public override void Dispose()
        {
            Consolas12.Dispose();
            Consolas12.Dispose();

            DisposeSurface();
            DisposeBuffers();

            GraphicsHost.ReleaseHdc(GraphicsHostDeviceContext);

            GraphicsHost.Dispose();

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
            bufferedGraphics = BufferedGraphicsManager.Current.Allocate(GraphicsHostDeviceContext, new Rectangle(0, 0, width, height));
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

        private JfxPoint2D ViewPortToScreen(float x, float y)
        {
            int Vw = SurfaceWidth;
            int Vh = SurfaceHeight;

            x = (x + 1) / 0.5f * Vw;
            x = (-y + 1) / 0.5f * Vh;
            return new JfxPoint2D((int)x, (int)y);
        }

        private JfxPoint2D ProjectVertex(in JfxVector4F point)
        {
            int d = 3;
            float x = point.X * d / point.Z;
            float y = point.Y * d / point.Z;
            return ViewPortToScreen(x, y);
        }

        protected override void RenderInternal()
        {
            var gfx = backBuffer.Graphics;

            gfx.Clear(Color.Black);
            gfx.DrawString(Fps.ToString(), Consolas12, Brushes.Red, 0, 0);
            gfx.DrawString($"Buffer   = {BufferWidth}, {BufferHeight}", Consolas12, Brushes.Cyan, 0, 16);
            gfx.DrawString($"Surface = {SurfaceWidth}, {SurfaceHeight}", Consolas12, Brushes.Cyan, 0, 32);




            var vertices = new SmallList<JfxVector4F>(stackalloc JfxVector4F[8]);

            vertices[0] = new JfxVector4F(-1, 1, 1); // 0 вершина
            vertices[1] = new JfxVector4F(-1, 1, -1); // 1 вершина
            vertices[2] = new JfxVector4F(1, 1, -1); // 2 вершина
            vertices[3] = new JfxVector4F(1, 1, 1); // 3 вершина
            vertices[4] = new JfxVector4F(-1, -1, 1); // 4 вершина
            vertices[5] = new JfxVector4F(-1, -1, -1); // 5 вершина
            vertices[6] = new JfxVector4F(1, -1, -1); // 6 вершина
            vertices[7] = new JfxVector4F(1, -1, 1); // 7 вершина

            var edges = new SmallList<ValueTuple<int, int>>(stackalloc ValueTuple<int, int>[12]);
            edges[0] = (0, 1);
            edges[1] = (1, 2);
            edges[2] = (2, 3);
            edges[3] = (3, 0);

            edges[4] = (0, 4);
            edges[5] = (1, 5);
            edges[6] = (2, 6);
            edges[7] = (3, 7);

            edges[8] = (4, 5);
            edges[9] = (5, 6);
            edges[10] = (6, 7);
            edges[11] = (7, 4);

            var sceneVertices = new SmallList<JfxVector4F>(stackalloc JfxVector4F[vertices.Length]);

            //var scale = JfxMatrix4F.Scale(100, 100, 100);
            //var translation = JfxMatrix4F.Translate(400, 300, 0);
            //var rotationX = JfxMatrix4F.RotateX(MathF.PI / 9);
            //var rotationY = JfxMatrix4F.RotateY(MathF.PI / 9);

            var transformation = JfxMatrix4F.Translate(400, 300, 0) *  JfxMatrix4F.Scale(100, 100, 100) * JfxMatrix4F.RotateY(MathF.PI / 9) * JfxMatrix4F.RotateX(MathF.PI / 9);

            for (int i = 0; i < sceneVertices.Length; i++)
            {
                //sceneVertices[i] = rotationX * vertices[i];
                //sceneVertices[i] = rotationY * sceneVertices[i];
                //sceneVertices[i] = scale * sceneVertices[i];
                //sceneVertices[i] = translation * sceneVertices[i];
                sceneVertices[i] = transformation * vertices[i];
            }


            for (var i = 0; i < edges.Length; i++)
            {
                var e = edges[i];
                backBuffer.DrawLine(
                    (int)sceneVertices[e.Item1].X,
                    (int)sceneVertices[e.Item1].Y,
                    (int)sceneVertices[e.Item2].X,
                    (int)sceneVertices[e.Item2].Y,
                    Color.Cyan
                );
            }       

            FlushBuffers();
        }

        private void FlushBuffers()
        {
            bufferedGraphics.Graphics.DrawImage(
                backBuffer.Bitmap,
                new RectangleF(0, 0, SurfaceWidth, SurfaceHeight),
                new RectangleF(-0.5f, -0.5f, BufferWidth, BufferHeight),
                GraphicsUnit.Pixel
            );

            bufferedGraphics.Render(GraphicsHostDeviceContext);
        }
    }
}