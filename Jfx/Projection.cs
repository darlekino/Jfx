using Jfx.Mathematic;
using System;

namespace Jfx
{
    public readonly struct PerspectiveProjection
    {
        public readonly Matrix4F Matrix;
        public readonly Matrix4F MatrixInverse;
        public readonly float FieldOfViewY;
        public readonly float AspectRatio;
        public readonly float NearPlane;
        public readonly float FarPlane;

        public PerspectiveProjection(float fieldOfViewY, float aspectRatio, float nearPlane, float farPlane)
        {
            Matrix = Matrix4F.PerspectiveFieldOfViewRH(fieldOfViewY, aspectRatio, nearPlane, farPlane);
            MatrixInverse = Matrix.Inverse();
            FieldOfViewY = fieldOfViewY;
            AspectRatio = aspectRatio;
            NearPlane = nearPlane;
            FarPlane = farPlane;
        }

        public PerspectiveProjection(in PerspectiveProjection projection, float aspectRatio) : this(projection.FieldOfViewY, aspectRatio, projection.NearPlane, projection.FieldOfViewY)
        {
        }
    }

    public readonly struct OrthographicProjection
    {
        public readonly Matrix4F Matrix;
        public readonly float Width;
        public readonly float Height;
        public readonly float NearPlane;
        public readonly float FarPlane;

        public OrthographicProjection(float width, float height, float nearPlane, float farPlane)
        {
            Matrix = Matrix4F.OrthographicRH(width, height, nearPlane, farPlane);
            Width = width;
            Height = height;
            NearPlane = nearPlane;
            FarPlane = farPlane;
        }

        public OrthographicProjection(in OrthographicProjection projection, float width, float height) : this(width, height, projection.NearPlane, projection.FarPlane)
        {
        }
    }
}
