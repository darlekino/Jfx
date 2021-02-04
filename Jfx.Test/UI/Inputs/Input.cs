using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Jfx.Test.UI.Inputs
{
    internal interface IInput : IDisposable
    {
        int Height { get; }
        int Width { get; }
        event EventHandler<SizeEventArgs> SizeChanged;
        event EventHandler<MouseEventArgs> MouseMove;
        event EventHandler<MouseEventArgs> MouseDown;
        event EventHandler<MouseEventArgs> MouseUp;
        event EventHandler<MouseEventArgs> MouseWheel;
        event EventHandler<KeyEventArgs> KeyDown;
        event EventHandler<KeyEventArgs> KeyUp;
    }

    internal class Input : IInput
    {
        private readonly Control control;

        public int Height => control.Size.Height;
        public int Width => control.Size.Width;

        public event EventHandler<SizeEventArgs> SizeChanged;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;
        public event EventHandler<MouseEventArgs> MouseWheel;
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyUp;

        public Input(Control control)
        {
            this.control = control;

            this.control.SizeChanged += ControlOnSizeChanged;
            this.control.MouseMove += ControlOnMouseMove;
            this.control.MouseDown += ControlOnMouseDown;
            this.control.MouseUp += ControlOnMouseUp;
            this.control.MouseWheel += ControlOnMouseWheel;
            this.control.KeyDown += ControlOnKeyDown;
            this.control.KeyUp += ControlOnKeyUp;

            //Test.Subscribe(this);
        }

        public void Dispose()
        {
            //Test.Unsubscribe(this);

            control.SizeChanged -= ControlOnSizeChanged;
            control.MouseMove -= ControlOnMouseMove;
            control.MouseDown -= ControlOnMouseDown;
            control.MouseUp -= ControlOnMouseUp;
            control.MouseWheel -= ControlOnMouseWheel;
            control.KeyDown -= ControlOnKeyDown;
            control.KeyUp -= ControlOnKeyUp;
        }

        private void ControlOnSizeChanged(object sender, EventArgs args) => SizeChanged?.Invoke(sender, new SizeEventArgs(Width, Height));
        private void ControlOnMouseMove(object sender, System.Windows.Forms.MouseEventArgs args) => MouseMove?.Invoke(sender, new MouseEventArgs(args));
        private void ControlOnMouseDown(object sender, System.Windows.Forms.MouseEventArgs args) => MouseDown?.Invoke(sender, new MouseEventArgs(args));
        private void ControlOnMouseUp(object sender, System.Windows.Forms.MouseEventArgs args) => MouseUp?.Invoke(sender, new MouseEventArgs(args));
        private void ControlOnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs args) => MouseWheel?.Invoke(sender, new MouseEventArgs(args));
        private void ControlOnKeyDown(object sender, System.Windows.Forms.KeyEventArgs args) => KeyDown?.Invoke(sender, new KeyEventArgs(args));
        private void ControlOnKeyUp(object sender, System.Windows.Forms.KeyEventArgs args) => KeyUp?.Invoke(sender, new KeyEventArgs(args));

        private static class Test
        {
            public static void Subscribe(IInput input)
            {
                input.SizeChanged += InputOnSizeChanged;
                input.MouseMove += InputOnMouseMove;
                input.MouseDown += InputOnMouseDown;
                input.MouseUp += InputOnMouseUp;
                input.MouseWheel += InputOnMouseWheel;
                input.KeyDown += InputOnKeyDown;
                input.KeyUp += InputOnKeyUp;
            }

            public static void Unsubscribe(IInput input)
            {
                input.SizeChanged -= InputOnSizeChanged;
                input.MouseMove -= InputOnMouseMove;
                input.MouseDown -= InputOnMouseDown;
                input.MouseUp -= InputOnMouseUp;
                input.MouseWheel -= InputOnMouseWheel;
                input.KeyDown -= InputOnKeyDown;
                input.KeyUp -= InputOnKeyUp;
            }

            private static void InputOnSizeChanged(object sender, SizeEventArgs args)
            {
                Debug.WriteLine($"{nameof(IInput.SizeChanged)} Height={args.Height}, Width={args.Width}");
            }

            private static void InputOnMouseMove(object sender, MouseEventArgs args)
            {
                Debug.WriteLine($"{nameof(IInput.MouseMove)} x={args.X}, y= {args.X}");
            }

            private static void InputOnMouseDown(object sender, MouseEventArgs args)
            {
                Debug.WriteLine($"{nameof(IInput.MouseDown)} x={args.X}, y= {args.X} {args.Buttons}");
            }

            private static void InputOnMouseUp(object sender, MouseEventArgs args)
            {
                Debug.WriteLine($"{nameof(IInput.MouseUp)} x={args.X}, y= {args.X} {args.Buttons}");
            }

            private static void InputOnMouseWheel(object sender, MouseEventArgs args)
            {
                Debug.WriteLine($"{nameof(IInput.MouseWheel)} x={args.X}, y= {args.X} {args.WheelDelta}");
            }

            private static void InputOnKeyDown(object sender, KeyEventArgs args)
            {
                Debug.WriteLine($"{nameof(IInput.KeyDown)} {args.Modifiers} {args.Key}");
            }

            private static void InputOnKeyUp(object sender, KeyEventArgs args)
            {
                Debug.WriteLine($"{nameof(IInput.KeyUp)} {args.Modifiers} {args.Key}");
            }
        }
    }
}
