﻿using Jfx.App.UI.Inputs;
using System;

namespace Jfx.App.UI
{
    internal abstract class Window : IWindow
    {
        protected readonly IInput Input;
        protected readonly IntPtr HostHandle;
        protected readonly Fps Fps = new Fps(new TimeSpan(0, 0, 0, 0, 1000));

        protected DateTime FrameStarted { get; private set; }
        protected Viewport Viewport { get; private set; }
        protected int BufferWidth { get; private set; }
        protected int BufferHeight { get; private set; }
        protected int SurfaceWidth { get; private set; }
        protected int SurfaceHeight { get; private set; }

        public Window(IntPtr hostHandle, IInput input)
        {
            Input = input;
            HostHandle = hostHandle;

            BufferWidth = SurfaceWidth = input.Width;
            BufferHeight = SurfaceHeight = input.Height;
            Viewport = new Viewport(0, 0, input.Width, input.Height, 0, 1);

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
            static (int, int) Sanitize(int width, int height)
            {
                if (width < 1 || height < 1)
                {
                    return (1, 1);
                }

                return (width, height);
            }

            (SurfaceWidth, SurfaceHeight) = Sanitize(Input.Width, Input.Height);
            Viewport = new Viewport(0, 0, SurfaceWidth, SurfaceHeight, 0, 1);
            ResizeSurface(SurfaceWidth, SurfaceHeight);

            var (bufferWidth, bufferHeight) = Sanitize(e.Width, e.Height);
            if (bufferWidth != BufferWidth || bufferHeight != BufferHeight)
            {
                BufferWidth = bufferWidth;
                BufferHeight = bufferHeight;
                ResizeBuffers(bufferWidth, bufferHeight);
            }
        }

        protected abstract void ResizeBuffers(int width, int height);
        protected abstract void ResizeSurface(int width, int height);

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
