using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.Test.UI.Inputs
{
    [Flags]
    internal enum MouseButtons
    {
        None = 0b_0000_0000,
        Left = 0b_0000_0001,
        Middle = 0b_0000_0010,
        Right = 0b_0000_0100,
        XButton1 = 0b_0000_1000,
        XButton2 = 0b_0001_0000,
    }
}
