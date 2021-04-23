using Jfx.App.UI.Operations;
using Jfx.App.UI.Inputs;
using Jfx.Mathematic;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Jfx.App.UI
{
    public abstract class Window : IWindow
    {
        protected JfxSize bufferSize;
        protected readonly IntPtr HostHandle;
        protected readonly Fps Fps = new Fps(TimeSpan.FromSeconds(1));
        protected DateTime FrameStarted { get; private set; }
        private IEnumerable<Operation> Operations { get; }
        public IInput Input { get; set; }
        public JfxPerspectiveCamera Camera { get; }


        public Window(IntPtr hostHandle, IInput input)
        {
            Input = input;
            HostHandle = hostHandle;
            bufferSize = new JfxSize(input.Width, input.Height);

            var viewport = new JfxViewport(0, 0, bufferSize, 0, 1);
            var nearPlane = 0.001f;
            var farPlane = 1000;
            var fieldOfViewY = MathF.PI * 0.5f;
            var projection = new JfxPerspectiveProjection(fieldOfViewY, viewport.AspectRatio, nearPlane, farPlane);
            Camera = new JfxPerspectiveCamera(
                position: new JfxVector3F(1, 1, 1),
                target: new JfxVector3F(0, 0, 0),
                upVector: new JfxVector3F(0, 0, 1),
                viewport: viewport,
                projection: projection
            );

            Operations = new List<Operation>
            {
                new Resize(this),
                new CameraZoom(this, 0.15f),
                new CameraPan(this),
                new CameraOrbit(this)
            };
        }

        public virtual void Dispose()
        {
            foreach (var o in Operations)
                o.Dispose();

            Fps.Dispose();
            Input.Dispose();
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

        public void Resize(in JfxSize size)
        {
            ResizeSurface(size);
            if (bufferSize != size)
            {
                bufferSize = size;
                ResizeBuffers(bufferSize);
            }
        }
    }
}
