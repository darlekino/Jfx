using Jfx.App.UI.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.App.UI.Operations
{
    public class Resize : Operation
    {
        public Resize(IWindow window) : base(window)
        {
            Window.Input.SizeChanged += OnSizeChanged;
        }

        public override void Dispose()
        {
            Window.Input.SizeChanged -= OnSizeChanged;
        }

        private void OnSizeChanged(object _, Inputs.SizeEventArgs e)
        {
            static Size Sanitize(int width, int height)
            {
                if (width < 1 || height < 1)
                {
                    return new Size(1, 1);
                }

                return new Size(width, height);
            }

            var size = Sanitize(e.Width, e.Height);
            Window.Resize(size);
            Window.Camera.UpdateViewportSize(size);
        }
    }
}
