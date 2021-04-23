using System;
using System.Drawing;

namespace Jfx.App.UI.Inputs
{
    public class MouseEventArgs : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public MouseButtons Buttons { get; }
        public int WheelDelta { get; }
        public int ClickCount { get; }

        public MouseEventArgs(in Point position, bool buttonLeft, bool buttonMiddle, bool buttonRight, bool buttonX1, bool buttonX2, int wheelDelta, int clickCount)
        {
            X = position.X;
            Y = position.Y;
            Buttons |= buttonLeft ? MouseButtons.Left : MouseButtons.None;
            Buttons |= buttonMiddle ? MouseButtons.Middle : MouseButtons.None;
            Buttons |= buttonRight ? MouseButtons.Right : MouseButtons.None;
            Buttons |= buttonX1 ? MouseButtons.XButton1 : MouseButtons.None;
            Buttons |= buttonX2 ? MouseButtons.XButton2 : MouseButtons.None;
            WheelDelta = wheelDelta;
            ClickCount = clickCount;
        }

        public MouseEventArgs(System.Windows.Forms.MouseEventArgs args) :
            this
            (
                args.Location,
                (args.Button & System.Windows.Forms.MouseButtons.Left) != 0,
                (args.Button & System.Windows.Forms.MouseButtons.Middle) != 0,
                (args.Button & System.Windows.Forms.MouseButtons.Right) != 0,
                (args.Button & System.Windows.Forms.MouseButtons.XButton1) != 0,
                (args.Button & System.Windows.Forms.MouseButtons.XButton2) != 0,
                args.Delta,
                args.Clicks
            )
        {
        }
    }
}