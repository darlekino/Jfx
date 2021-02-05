using Jfx.Mathematic;

namespace Jfx
{
    public abstract class JfxProjection
    {
        protected internal float nearPlane;
        protected internal float farPlane;
        protected internal float aspectRatio;
        protected internal JfxMatrix4F transformation;

        public JfxProjection(float nearPlane, float farPlane)
        {
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
        }

        public float NearPlane
        {
            set
            {
                this.nearPlane = value;
                UpdateTransformation();
            }
            get => nearPlane;
        }

        public float FarPlane
        {
            set
            {
                this.farPlane = value;
                UpdateTransformation();
            }
            get => farPlane;
        }

        public float AspectRatio
        {
            set
            {
                this.aspectRatio = value;
                UpdateTransformation();
            }
            get => aspectRatio;
        }

        public ref readonly JfxMatrix4F Transformation => ref transformation;

        protected internal abstract void UpdateTransformation();
    }

    public class JfxPerspectiveProjection : JfxProjection
    {
        private float fieldOfViewY;

        public JfxPerspectiveProjection(float nearPlane, float farPlane, float fieldOfViewY, float aspectRatio)
            : base(nearPlane, farPlane)
        {
            this.fieldOfViewY = fieldOfViewY;
            this.aspectRatio = aspectRatio;
            UpdateTransformation();
        }

        public float FieldOfViewY
        {
            set
            {
                this.fieldOfViewY = value;
                UpdateTransformation();
            }
            get => fieldOfViewY;
        }

        protected internal override void UpdateTransformation()
        {
            transformation = JfxMatrix4F.PerspectiveFieldOfViewRH(fieldOfViewY, aspectRatio, nearPlane, farPlane);
        }

        public void Update(float nearPlane, float farPlane, float fieldOfViewY, float aspectRatio)
        {
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
            this.fieldOfViewY = fieldOfViewY;
            this.aspectRatio = aspectRatio;
            UpdateTransformation();
        }
    }

    public class JfxOrthographicProjection : JfxProjection
    {
        private float fieldHeight;

        public JfxOrthographicProjection(float nearPlane, float farPlane, float fieldHeight, float aspectRatio)
            : base(nearPlane, farPlane)
        {
            this.fieldHeight = fieldHeight;
            this.aspectRatio = aspectRatio;
            UpdateTransformation();
        }

        public float FieldHeight
        {
            set
            {
                this.fieldHeight = value;
                UpdateTransformation();
            }
            get => fieldHeight;
        }

        protected internal override void UpdateTransformation()
        {
            transformation = JfxMatrix4F.OrthographicRH(fieldHeight * aspectRatio, fieldHeight, nearPlane, farPlane);
        }

        public void Update(float nearPlane, float farPlane, float fieldHeight, float aspectRatio)
        {
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;
            this.fieldHeight = fieldHeight;
            this.aspectRatio = aspectRatio;
            UpdateTransformation();
        }
    }
}
