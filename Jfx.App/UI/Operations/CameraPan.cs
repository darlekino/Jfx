using Jfx.App.UI.Inputs;
using Jfx.Mathematic;
using System;

namespace Jfx.App.UI.Operations
{
    public class CameraPan : Operation
    {
        private struct MouseDownInfo
        {
            public JfxPerspectiveCamera Camera;
            public JfxPlane Plane;
            public JfxVector3F PointOfIntersection;
        }

        private bool moving = default;
        private MouseDownInfo mouseDown = default;
        
        public CameraPan(IWindow window) : base(window)
        {
            Window.Input.MouseDown += OnMouseDown;
            Window.Input.MouseUp += OnMouseUp;
            Window.Input.MouseMove += OnMouseMove;
        }

        public override void Dispose()
        {
            Window.Input.MouseMove -= OnMouseMove;
            Window.Input.MouseUp -= OnMouseUp;
            Window.Input.MouseDown -= OnMouseDown;
        }

        private JfxRay GetMouseRay(int x, int y)
        {
            var mousePositionInScreen = new JfxVector3F(x, y, 0);
            var mousePositionInWorld = JfxVector3F.Transform(mousePositionInScreen, mouseDown.Camera.TransformMatrixInverse);
            return new JfxRay(mousePositionInWorld, (mousePositionInWorld - mouseDown.Camera.Position).Normalize());
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Buttons != MouseButtons.Right)
                return;

            mouseDown.Camera = Window.Camera.Clone();
            mouseDown.Plane = new JfxPlane(Window.Camera.Target, Window.Camera.NormilizedEyeVector());
            mouseDown.PointOfIntersection = mouseDown.Plane.IntersectionWith(GetMouseRay(e.X, e.Y));
            moving = true;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Buttons != MouseButtons.Right)
                return;

            moving = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (moving)
            {
                var mouseRay = GetMouseRay(e.X, e.Y);
                var mouseMoveOnPlane = mouseDown.Plane.IntersectionWith(mouseRay);
                var offset = mouseMoveOnPlane - mouseDown.PointOfIntersection;

                Window.Camera.MoveTo(mouseDown.Camera.Position - offset);
                Window.Camera.LookAt(mouseDown.Camera.Target - offset);
                Window.Camera.UpdateTransformMatrix();
            }
        }
    }
}
