using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx
{
    public interface IJfx3DScene : IDisposable
    {
        JfxCamera Camera { get; }
        JfxProjection Projection { get; }
        JfxViewport Viewport { get; }

        event EventHandler<JfxSizeEventArgs> WindowResized;
    }
}
