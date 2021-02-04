using Wpf = System.Windows;
using Jfx.App.UI;
using Jfx.App.UI.Gdi;
using System;
using Jfx.App.UI.Inputs;

namespace Jfx.App.Client
{
    internal static class WindowFactory
    {
        private static System.Windows.Forms.Control CreateHostControl()
        {
            var hostControl = new System.Windows.Forms.Panel
            {
                Dock = System.Windows.Forms.DockStyle.Fill,
                BackColor = System.Drawing.Color.Transparent,
                ForeColor = System.Drawing.Color.Transparent,
            };

            // focus control, so that we could capture mousewheel events
            void EnsureFocus(System.Windows.Forms.Control control)
            {
                if (!control.Focused)
                {
                    control.Focus();
                }
            }

            hostControl.MouseEnter += (sender, args) => EnsureFocus(hostControl);
            hostControl.MouseClick += (sender, args) => EnsureFocus(hostControl);

            return hostControl;
        }

        internal static IWindow CreateDefaultWindow()
        {
            const int width = 720;
            const int height = 480;
            const string title = "wpf gdi";

            var window = new System.Windows.Forms.Form
            {
                Size = new System.Drawing.Size(width, height),
                Text = title,
            };

            var hostControl = CreateHostControl();
            window.Controls.Add(hostControl);

            window.Closed += (sender, args) => System.Windows.Application.Current.Shutdown();

            window.Show();


            return new GdiWindow(hostControl.Handle, new Input(hostControl));
        }
    }
}
