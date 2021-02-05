using Jfx.App.UI.Inputs;
using Jfx.Mathematic;
using System;

namespace Jfx.App.UI
{
    internal abstract class Window : IWindow
    {
        private JfxSize bufferSize;

        internal protected readonly IInput Input;
        internal protected readonly IntPtr HostHandle;
        internal protected readonly JfxCamera Camera;
        internal protected readonly JfxViewport Viewport;
        internal protected readonly JfxProjection Projection;
        internal protected readonly Fps Fps = new Fps(new TimeSpan(0, 0, 0, 0, 1000));

        internal protected DateTime FrameStarted { get; private set; }
        internal protected ref readonly JfxSize BufferSize => ref bufferSize;

        public Window(IntPtr hostHandle, IInput input)
        {
            var size = new JfxSize(input.Width, input.Height);

            Input = input;
            HostHandle = hostHandle;
            bufferSize = size;
            Viewport = new JfxViewport(0, 0, size, 0, 1);

            var cameraPosition = new JfxVector3F(2, 2, 2);
            var cameraTarget = new JfxVector3F(0, 0, 0);
            var cameraUpVector = new JfxVector3F(0, 0, 1);
            Camera = new JfxCamera(cameraPosition, cameraTarget, cameraUpVector);

            var aspectRatio = Viewport.AspectRatio;
            var nearPlane = 0.001f;
            var farPlane = 1000;
            var fieldOfViewY = MathF.PI * 0.5f;
            //Projection = new JfxPerspectiveProjection(nearPlane, farPlane, fieldOfViewY, Viewport.AspectRatio);

            var filedHeight = 3f;
            Projection = new JfxOrthographicProjection(nearPlane, farPlane, filedHeight, aspectRatio);

            Input.SizeChanged += OnSizeChanged;
        }

        public virtual void Dispose()
        {
            Input.SizeChanged -= OnSizeChanged;

            Fps.Dispose();
            Input.Dispose();
        }

        private void OnSizeChanged(object _, SizeEventArgs e)
        {
            static JfxSize Sanitize(int width, int height)
            {
                if (width < 1 || height < 1)
                {
                    return new JfxSize(1, 1);
                }

                return new JfxSize(width, height);
            }

            var size = Sanitize(e.Width, e.Height);
            Viewport.UpdateSize(size);
            Projection.AspectRatio = Viewport.AspectRatio;
            ResizeSurface(size);

            if ( bufferSize != size)
            {
                bufferSize = size;
                ResizeBuffers(bufferSize);
            }
        }

        protected abstract void ResizeBuffers(in JfxSize size);
        protected abstract void ResizeSurface(in JfxSize size);

        protected abstract void RenderInternal();

        public void Render()
        {
            FrameStarted = DateTime.UtcNow;
            Fps.StartFrame();
            RenderInternal();
            Fps.EndFrame();
        }
    }
}
