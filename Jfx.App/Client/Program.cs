using Jfx.App.UI;
using System;

namespace Jfx.App.Client
{
    internal class Program : System.Windows.Application, IDisposable
    {
        private IWindow window;

        public Program()
        {

            var s = new System.Numerics.Matrix4x4();


            Startup += (_, _) => Initialize();
            Exit += (_, _) => Dispose();
        }

        private void Initialize()
        {
            window = WindowFactory.CreateDefaultWindow();

            while (!Dispatcher.HasShutdownStarted)
            {
                window.Render();
                System.Windows.Forms.Application.DoEvents();
            }
        }

        public void Dispose()
        {
            window.Dispose();
            window = default;
        }
    }
}
