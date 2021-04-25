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
            public Vector3F MousePositionInView;
            public Matrix4F LocalTransformation;
            public Matrix4F LocalTransformationInverse;
            public Vector3F EyeInLocal;
            public UnitVector3F EyeDirectionInLocal;
            public Vector3F CameraTargetInLocal;

            public MouseDownInfo(JfxPerspectiveCamera camera, MouseEventArgs e)
            {
                Camera = camera.Clone();
                MousePositionInView = Vector3F.Transform(new Vector3F(e.X, e.Y, 0), Camera.Viewport.MatrixInverse);
                var zAxis = Camera.UpVector;
                var yzPlane = Plane.FromPoints(Vector3F.Zero, Camera.NormilizedEyeVector().ToVector(), zAxis);
                var xAxis = yzPlane.Normal;
                var xzPlane = Plane.FromPoints(Vector3F.Zero, zAxis, xAxis.ToVector());
                var yAxis = xzPlane.Normal;

                LocalTransformation = new Matrix4F(
                    xAxis.X, yAxis.X, zAxis.X, 0,
                    xAxis.Y, yAxis.Y, zAxis.Y, 0,
                    xAxis.Z, yAxis.Z, zAxis.Z, 0,
                    0, 0, 0, 1);

                LocalTransformationInverse = LocalTransformation.Inverse();
                EyeInLocal = Vector3F.Transform(Camera.Position, LocalTransformation);
                CameraTargetInLocal = Vector3F.Transform(Camera.Target, LocalTransformation);
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
                var mouseMoveInView = Vector3F.Transform(new Vector3F(e.X, e.Y, 0), Window.Camera.Viewport.MatrixInverse);
                (float thetaDelta, float phiDelta) = SphereAngles(mouseMoveInView - mouseDown.MousePositionInView, mouseDown.EyeDirectionInLocal);

                // rotate horizontally
                var rotationHorizontal = Matrix4F.Rotate(UnitVector3F.ZAxis, thetaDelta).TransformAround(mouseDown.CameraTargetInLocal);
                var eye = Vector3F.Transform(mouseDown.EyeInLocal, rotationHorizontal);
                var target = Vector3F.Transform(mouseDown.CameraTargetInLocal, rotationHorizontal);

                // rotate vertically
                var phiPlane = Plane.FromPoints(eye, target, target + UnitVector3F.ZAxis);
                var rotationVertical = Matrix4F.Rotate(phiPlane.Normal, phiDelta).TransformAround(mouseDown.CameraTargetInLocal);
                eye = Vector3F.Transform(eye, rotationVertical);
                target = Vector3F.Transform(target, rotationVertical);

                eye = Vector3F.Transform(eye, mouseDown.LocalTransformationInverse);
                target = Vector3F.Transform(target, mouseDown.LocalTransformationInverse);

                Window.Camera.MoveTo(eye);
                Window.Camera.LookAt(target);
                Window.Camera.UpdateTransformMatrix();
            }
        }

        private static (float, float) SphereAngles(Vector3F mouseOffsetView, UnitVector3F eyeDirection)
        {
            // get deltas
            float thetaDelta = -mouseOffsetView.X * MathF.PI;  // horizontal (around z-axis)
            float phiDelta = mouseOffsetView.Y * MathF.PI;   // vertical

            var phiStart = UnitVector3F.ZAxis.AngleTo(-eyeDirection);
            var phiEnd = phiStart + phiDelta;

            // clamp phi so that new view vector won't match with upVector
            phiEnd = MathF.Max(MathF.Min(phiEnd, MathF.PI * 0.999f), 0.001f);
            phiDelta = phiEnd - phiStart;

            return (thetaDelta, phiDelta);
        }
    }
}
