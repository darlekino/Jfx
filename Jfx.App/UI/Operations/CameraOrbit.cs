using Jfx.App.UI.Inputs;
using Jfx.Mathematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.App.UI.Operations
{
    class CameraOrbit : Operation
    {
        private struct MouseDownInfo
        {
            public JfxPerspectiveCamera Camera;
            public JfxVector3F MousePositionInView;
            public JfxMatrix4F LocalTransformation;
            public JfxMatrix4F LocalTransformationInverse;
            public JfxVector3F EyeInLocal;
            public JfxUnitVector3F EyeDirectionInLocal;
            public JfxVector3F CameraTargetInLocal;

            public MouseDownInfo(JfxPerspectiveCamera camera, MouseEventArgs e)
            {
                Camera = camera.Clone();
                MousePositionInView = JfxVector3F.Transform(new JfxVector3F(e.X, e.Y, 0), Camera.Viewport.MatrixInverse);
                var zAxis = Camera.UpVector;
                var yzPlane = JfxPlane.FromPoints(JfxVector3F.Zero, Camera.EyeVector(), zAxis);
                var xAxis = yzPlane.Normal;
                var xzPlane = JfxPlane.FromPoints(JfxVector3F.Zero, zAxis, xAxis.ToVector());
                var yAxis = xzPlane.Normal;

                LocalTransformation = new JfxMatrix4F(
                    xAxis.X, yAxis.X, zAxis.X, 0,
                    xAxis.Y, yAxis.Y, zAxis.Y, 0,
                    xAxis.Z, yAxis.Z, zAxis.Z, 0,
                    0, 0, 0, 1);

                LocalTransformationInverse = LocalTransformation.Inverse();
                EyeInLocal = JfxVector3F.Transform(Camera.EyeVector(), LocalTransformation);
                CameraTargetInLocal = JfxVector3F.Transform(Camera.Target, LocalTransformation);
                EyeDirectionInLocal = (CameraTargetInLocal - EyeInLocal).Normalize(); // ???
            }
        }

        private bool moving = default;
        private MouseDownInfo mouseDown = default;

        public CameraOrbit(IWindow window) : base(window)
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

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Buttons != MouseButtons.Left)
                return;

            mouseDown = new MouseDownInfo(Window.Camera, e);
            moving = true;
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            if (e.Buttons != MouseButtons.Left)
                return;

            moving = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (moving)
            {
                var mouseMoveInView = JfxVector3F.Transform(new JfxVector3F(e.X, e.Y, 0), Window.Camera.Viewport.MatrixInverse);
                (float thetaDelta, float phiDelta) = SphereAngles(mouseMoveInView, mouseDown.EyeDirectionInLocal);

                // rotate horizontally
                var rotationHorizontal = JfxMatrix4F.Rotate(JfxUnitVector3F.ZAxis, thetaDelta).TransformAround(mouseDown.CameraTargetInLocal);
                var eye = JfxVector3F.Transform(mouseDown.EyeInLocal, rotationHorizontal);
                var target = JfxVector3F.Transform(mouseDown.CameraTargetInLocal, rotationHorizontal);

                // rotate vertically
                var phiPlane = JfxPlane.FromPoints(eye, target, target + JfxUnitVector3F.ZAxis);
                var rotationVertical = JfxMatrix4F.Rotate(phiPlane.Normal, phiDelta).TransformAround(mouseDown.CameraTargetInLocal);
                eye = JfxVector3F.Transform(mouseDown.EyeInLocal, rotationVertical);
                target = JfxVector3F.Transform(mouseDown.CameraTargetInLocal, rotationVertical);

                var eyeInWorld = JfxVector3F.Transform(eye, mouseDown.LocalTransformationInverse);
                var targerInWorld = JfxVector3F.Transform(target, mouseDown.LocalTransformationInverse);

                Window.Camera.MoveTo(eye);
                Window.Camera.LookAt(target);
                Window.Camera.UpdateTransformMatrix();
            }
        }

        private static (float, float) SphereAngles(JfxVector3F mouseOffsetView, JfxUnitVector3F eyeDirection)
        {
            // get deltas
            float thetaDelta = -mouseOffsetView.X * MathF.PI;  // horizontal (around z-axis)
            float phiDelta = mouseOffsetView.Y * MathF.PI;   // vertical

            var phiStart = JfxUnitVector3F.ZAxis.AngleTo(-eyeDirection);
            var phiEnd = phiStart + phiDelta;

            // clamp phi so that new view vector won't match with upVector
            phiEnd = MathF.Max(MathF.Min(phiEnd, MathF.PI * 0.999f), 0.001f);
            phiDelta = phiEnd - phiStart;

            return (thetaDelta, phiDelta);
        }
    }
}
