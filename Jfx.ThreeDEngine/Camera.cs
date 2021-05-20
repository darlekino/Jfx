using Jfx.Mathematic;

namespace Jfx.ThreeDEngine
{
    public class JfxPerspectiveCamera
    {
        public Vector3F Position;
        public Vector3F Target;
        public Vector3F UpVector;
        public Matrix4F CameraMatrix;
        public Matrix4F CameraMatrixInverse;
        public Matrix4F MatrixToClip;
        public Matrix4F TransformMatrix;
        public Matrix4F TransformMatrixInverse;
        public Viewport Viewport;
        public PerspectiveProjection Projection;

        private JfxPerspectiveCamera(JfxPerspectiveCamera camera)
        {
            Position = camera.Position;
            Target = camera.Target;
            UpVector = camera.UpVector;
            CameraMatrix = camera.CameraMatrix;
            CameraMatrixInverse = camera.CameraMatrixInverse;
            MatrixToClip = camera.MatrixToClip;
            TransformMatrix = camera.TransformMatrix;
            TransformMatrixInverse = camera.TransformMatrixInverse;
            Viewport = camera.Viewport;
            Projection = camera.Projection;
        }

        public JfxPerspectiveCamera(in Vector3F position, in Vector3F target, in Vector3F upVector, in Viewport viewport, in PerspectiveProjection projection)
        {
            Position = position;
            Target = target;
            UpVector = upVector;
            Viewport = viewport;
            Projection = projection;
            UpdateTransformMatrix();
        }

        public Vector3F EyeVector() => Target - Position;
        public UnitVector3F NormilizedEyeVector() => EyeVector().Normalize();


        public void UpdateViewportSize(in Size size)
        {
            Viewport = new Viewport(Viewport, size);
            Projection = new PerspectiveProjection(Projection, Viewport.AspectRatio);
            MatrixToClip = CameraMatrix * Projection.Matrix;
        }

        public void UpdateTransformMatrix()
        {
            CameraMatrix = Matrix4F.LookAtRH(Position, Target, UpVector);
            CameraMatrixInverse = CameraMatrix.Inverse();

            MatrixToClip = CameraMatrix * Projection.Matrix;

            TransformMatrix = CameraMatrix * Projection.Matrix * Viewport.Matrix;
            TransformMatrixInverse = TransformMatrix.Inverse();
        }

        public void MoveTo(in Vector3F position) => Position = position;

        public void LookAt(in Vector3F target) => Target = target;

        public void Rotate(in Vector3F upVector) => UpVector = upVector;

        public JfxPerspectiveCamera Clone() => new JfxPerspectiveCamera(this);
    }
}
