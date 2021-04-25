using Jfx.Mathematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx
{
    //public abstract class JfxCamera
    //{


    //    public ref readonly JfxVector3F Position => ref position;
    //    public ref readonly JfxVector3F Target => ref target;
    //    public ref readonly JfxUnitVector3F UpVector => ref upVector;
    //    public ref readonly JfxViewport Viewport => ref viewport;
    //    public ref readonly JfxMatrix4F TransformMatrix => ref transformMatrix;
    //    public abstract ref readonly JfxMatrix4F ProjectionMatrix { get; }


    //    public JfxCamera(in JfxVector3F position, in JfxVector3F target, in JfxUnitVector3F upVector, in JfxViewport viewport)
    //    {
    //        this.position = position;
    //        this.target = target;
    //        this.upVector = upVector;
    //        this.viewport = viewport;
    //    }

    //    public JfxUnitVector3F EyeVector() => (target - position).Normilize();

    //    public void UpdateTransformMatrix()
    //    {
    //        this.cameraMatrix = JfxMatrix4F.LookAtRH(position, target, upVector);
    //        transformMatrix = cameraMatrix * ProjectionMatrix * viewport.Matrix;
    //    }

    //    public void MoveTo(in JfxVector3F position) => this.position = position;

    //    public void LookAt(in JfxVector3F target) => this.target = target;

    //    public void Rotate(in JfxUnitVector3F upVector) => this.upVector = upVector;

    //    public static JfxPerspectiveCamera CreatePerspective(in JfxVector3F position, in JfxVector3F target, in JfxUnitVector3F upVector, in JfxViewport viewport, in JfxPerspectiveProjection projection)
    //    {
    //        return new JfxPerspectiveCamera(position, target, upVector, viewport, projection);
    //    }

    //    public static JfxOrthographicCamera CreateOrthographic(in JfxVector3F position, in JfxVector3F target, in JfxUnitVector3F upVector, in JfxViewport viewport, in JfxOrthographicProjection projection)
    //    {
    //        throw new NotImplementedException();
    //        //return new JfxOrthographicCamera(position, target, upVector, viewport, projection);
    //    }
    //}

    public class JfxPerspectiveCamera
    {
        public Vector3F Position;
        public Vector3F Target;
        public Vector3F UpVector;
        public Matrix4F CameraMatrix;
        public Matrix4F CameraMatrixInverse;
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
        }

        public void UpdateTransformMatrix()
        {
            CameraMatrix = Matrix4F.LookAtRH(Position, Target, UpVector);
            CameraMatrixInverse = CameraMatrix.Inverse();
            TransformMatrix = CameraMatrix * Projection.Matrix * Viewport.Matrix;
            TransformMatrixInverse = TransformMatrix.Inverse();
        }

        public void MoveTo(in Vector3F position) => Position = position;

        public void LookAt(in Vector3F target) => Target = target;

        public void Rotate(in Vector3F upVector) => UpVector = upVector;

        public JfxPerspectiveCamera Clone() => new JfxPerspectiveCamera(this);
    }

    //public class JfxOrthographicCamera
    //{
    //    private JfxVector3F position;
    //    private JfxVector3F target;
    //    private JfxUnitVector3F upVector;
    //    private JfxMatrix4F cameraMatrix;
    //    public readonly JfxViewport Viewport;
    //    public readonly JfxMatrix4F TransformMatrix;
    //    private JfxOrthographicProjection Projection;

    //    public JfxOrthographicCamera(in JfxVector3F position, in JfxVector3F target, in JfxUnitVector3F upVector, in JfxViewport viewport, in JfxOrthographicProjection projection)
    //    {
    //        this.position = position;
    //        this.target = target;
    //        this.upVector = upVector;
    //        this.viewport = viewport;
    //        this.projection = projection;
    //        UpdateTransformMatrix();
    //    }

    //    public JfxUnitVector3F EyeVector() => (target - position).Normilize();

    //    public void UpdateTransformMatrix()
    //    {
    //        this.cameraMatrix = JfxMatrix4F.LookAtRH(position, target, upVector);
    //        transformMatrix = cameraMatrix * ProjectionMatrix * viewport.Matrix;
    //    }

    //    public void MoveTo(in JfxVector3F position) => this.position = position;

    //    public void LookAt(in JfxVector3F target) => this.target = target;

    //    public void Rotate(in JfxUnitVector3F upVector) => this.upVector = upVector;
    //}
}
