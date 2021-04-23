using Jfx.App.UI.Inputs;
using System;

namespace Jfx.App.UI.Operations
{
    public class CameraZoom : Operation
    {
        public readonly float Scale;
        private readonly float scaleForward;
        private readonly float scaleBackwards;

        public CameraZoom(IWindow window, float scale) : base(window)
        {
            Scale = scale;
            scaleForward = 1.0f + scale;
            scaleBackwards = 2.0f - 1.0f / (1.0f - scale);
            Window.Input.MouseWheel += OnMouseWheel;
        }

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            var eyeVector = Window.Camera.NormilizedEyeVector();
            var offset = ((e.WheelDelta > 0 ? scaleForward : scaleBackwards) * eyeVector) - eyeVector;
            Window.Camera.MoveTo(Window.Camera.Position + offset);
            Window.Camera.UpdateTransformMatrix();
        }

        public override void Dispose()
        {
            Window.Input.MouseWheel -= OnMouseWheel;
        }
    }
}
