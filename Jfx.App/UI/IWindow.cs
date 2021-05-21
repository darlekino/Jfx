using Jfx.App.UI.Inputs;
using Jfx.ThreeDEngine;
using System;
using System.Collections.Generic;

namespace Jfx.App.UI
{
    public interface IWindow : IDisposable
    {
        public IInput Input { get; }
        JfxPerspectiveCamera Camera { get; }
        void Render(IEnumerable<IModel> models);
        void Resize(in Size size);
    }
}
