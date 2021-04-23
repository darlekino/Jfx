using Jfx.App.UI.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.App.UI.Operations
{
    public abstract class Operation : IDisposable
    {
        public IWindow Window { get; }

        public Operation(IWindow window)
        {
            Window = window;
        }

        public abstract void Dispose();
    }
}
