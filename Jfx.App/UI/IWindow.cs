using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.App.UI
{
    internal interface IWindow : IDisposable
    {
        void Render();
    }
}
