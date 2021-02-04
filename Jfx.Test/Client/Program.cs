using Jfx.Test.UI;
using System;

namespace Jfx.Test.Client
{
    internal class Program : System.Windows.Application, IDisposable
    {
        private IWindow window;

        public Program()
        {

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
