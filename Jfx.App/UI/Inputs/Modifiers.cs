using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.App.UI.Inputs
{
    [Flags]
    public enum Modifiers
    {
        None = 0b_0000_0000,
        Control = 0b_0000_0001,
        Alt = 0b_0000_0010,
        Shift = 0b_0000_0100,
        Windows = 0b_0000_1000,
    }
}
