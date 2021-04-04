using Jfx.Mathematic;
using System;

namespace Jfx
{
    public abstract class JfxProjection
    {
        protected JfxMatrix4F transformation;
        protected float aspectRatio;
        public readonly float NearPlane;
        public readonly float FarPlane;

        public event EventHandler Changed;

        public float AspectRatio
        {
            get
            {
                return aspectRatio;
            }

            set
            {
                aspectRatio = value;
                UpdateTransformation();
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
        public ref readonly JfxMatrix4F Transformation => ref transformation;

        public JfxProjection(float nearPlane, float farPlane, float aspectRatio)
        {
            NearPlane = nearPlane;
            FarPlane = farPlane;
            this.aspectRatio = aspectRatio;
        }

        protected abstract void UpdateTransformation();
    }

    public class JfxPerspectiveProjection : JfxProjection
    {
        public readonly float FieldOfViewY;

        public JfxPerspectiveProjection(float nearPlane, float farPlane, float fieldOfViewY, float aspectRatio)
            : base(nearPlane, farPlane, aspectRatio)
        {
            FieldOfViewY = fieldOfViewY;
            UpdateTransformation();
        }

        protected override void UpdateTransformation()
        {
            transformation = JfxMatrix4F.PerspectiveFieldOfViewRH(FieldOfViewY, AspectRatio, NearPlane, FarPlane);
        }
    }

    public class JfxOrthographicProjection : JfxProjection
    {
        public readonly float FieldHeight;

        public JfxOrthographicProjection(float nearPlane, float farPlane, float fieldHeight, float aspectRatio)
            : base(nearPlane, farPlane, aspectRatio)
        {
            FieldHeight = fieldHeight;
            UpdateTransformation();
        }

        protected override void UpdateTransformation()
        {
            transformation = JfxMatrix4F.OrthographicRH(FieldHeight * AspectRatio, FieldHeight, NearPlane, FarPlane);
        }
    }
}
