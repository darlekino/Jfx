using Jfx.App.UI.Operations;
using Jfx.App.UI.Inputs;
using Jfx.Mathematic;
using System;
using System.Collections;
using System.Collections.Generic;
using Jfx.ThreeDEngine;
using Jfx.App.UI.Gdi;

namespace Jfx.App.UI
{
    public abstract class Window : IWindow
    {
        protected Size bufferSize;
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
            bufferSize = new Size(input.Width, input.Height);

            var viewport = new Viewport(0, 0, bufferSize, 0, 1);
            var nearPlane = 0.001f;
            var farPlane = 1000;
            var fieldOfViewY = MathF.PI * 0.5f;
            var projection = new PerspectiveProjection(fieldOfViewY, viewport.AspectRatio, nearPlane, farPlane);
            Camera = new JfxPerspectiveCamera(
                position: new Vector3F(1, 1, 1),
                target: new Vector3F(0, 0, 0),
                upVector: new Vector3F(0, 0, 1),
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
        

        protected abstract void ResizeBuffers(in Size size);
        protected abstract void ResizeSurface(in Size size);

        protected abstract void RenderInternal(IEnumerable<IModel> models);

        public void Render(IEnumerable<Visual> models)
        {
            FrameStarted = DateTime.UtcNow;
            Fps.StartFrame();
            RenderInternal(models);
            Fps.EndFrame();
        }

        public void Resize(in Size size)
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
