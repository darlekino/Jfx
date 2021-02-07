using Jfx.App.UI;
using System;

namespace Jfx.App.Client
{
    internal class Program : System.Windows.Application, IDisposable
    {
        private IWindow window;

        public Program()
        {
            Startup += (_, _) => Initialize();
            Exit += (_, _) => Dispose();
        }

        private static float GetDeltaTime(DateTime timestamp, TimeSpan periodDuration)
        {
            var result = (timestamp.Second * 1000 + timestamp.Millisecond) % periodDuration.TotalMilliseconds / periodDuration.TotalMilliseconds;
            return (float)result;
        }

        private void Initialize()
        {
            window = WindowFactory.CreateDefaultWindow();

            while (!Dispatcher.HasShutdownStarted)
            {
                DateTime utcNow = DateTime.UtcNow;
                const int radius = 2;
                float t = GetDeltaTime(utcNow, new TimeSpan(0, 0, 0, 10));
                float angle = t * MathF.PI * 2;

                window.Camera.MoveTo(new Mathematic.JfxVector3F(MathF.Sin(angle) * radius, MathF.Cos(angle) * radius, 1));

                window.Render();
                System.Windows.Forms.Application.DoEvents();
            }
        }

        public void Dispose()
        {
            window.Dispose();
        }
    }
}
