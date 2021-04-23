using Jfx.Mathematic;
using System;

namespace Jfx
{
    public readonly struct JfxPerspectiveProjection
    {
        public readonly JfxMatrix4F Matrix;
        public readonly JfxMatrix4F MatrixInverse;
        public readonly float FieldOfViewY;
        public readonly float AspectRatio;
        public readonly float NearPlane;
        public readonly float FarPlane;

        public JfxPerspectiveProjection(float fieldOfViewY, float aspectRatio, float nearPlane, float farPlane)
        {
            Matrix = JfxMatrix4F.PerspectiveFieldOfViewRH(fieldOfViewY, aspectRatio, nearPlane, farPlane);
            MatrixInverse = Matrix.Inverse();
            FieldOfViewY = fieldOfViewY;
            AspectRatio = aspectRatio;
            NearPlane = nearPlane;
            FarPlane = farPlane;
        }

        public JfxPerspectiveProjection(in JfxPerspectiveProjection projection, float aspectRatio) : this(projection.FieldOfViewY, aspectRatio, projection.NearPlane, projection.FieldOfViewY)
        {
        }
    }

    public readonly struct JfxOrthographicProjection
    {
        public readonly JfxMatrix4F Matrix;
        public readonly float Width;
        public readonly float Height;
        public readonly float NearPlane;
        public readonly float FarPlane;

        public JfxOrthographicProjection(float width, float height, float nearPlane, float farPlane)
        {
            Matrix = JfxMatrix4F.OrthographicRH(width, height, nearPlane, farPlane);
            Width = width;
            Height = height;
            NearPlane = nearPlane;
            FarPlane = farPlane;
        }

        public JfxOrthographicProjection(in JfxOrthographicProjection projection, float width, float height) : this(width, height, projection.NearPlane, projection.FarPlane)
        {
        }
    }
}
