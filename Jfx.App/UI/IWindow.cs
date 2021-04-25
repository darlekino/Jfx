using Jfx.App.UI.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.App.UI
{
    public interface IWindow : IDisposable
    {
        public IInput Input { get; }
        JfxPerspectiveCamera Camera { get; }
        void Render();
        void Resize(in Size size);
    }
}
