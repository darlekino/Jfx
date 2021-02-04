using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.Mathematic
{
    public readonly struct JfxMatrix4F
    {
        private static readonly JfxMatrix4F identity = new JfxMatrix4F(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );
        public static ref readonly JfxMatrix4F Identity => ref identity;

        public readonly float M11;
        public readonly float M12;
        public readonly float M13;
        public readonly float M14;

        public readonly float M21;
        public readonly float M22;
        public readonly float M23;
        public readonly float M24;

        public readonly float M31;
        public readonly float M32;
        public readonly float M33;
        public readonly float M34;

        public readonly float M41;
        public readonly float M42;
        public readonly float M43;
        public readonly float M44;

        public JfxMatrix4F(
            float m11, float m12, float m13, float m14,
            float m21, float m22, float m23, float m24,
            float m31, float m32, float m33, float m34,
            float m41, float m42, float m43, float m44
            )
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M14 = m14;

            M21 = m21;
            M22 = m22;
            M23 = m23;
            M24 = m24;

            M31 = m31;
            M32 = m32;
            M33 = m33;
            M34 = m34;

            M41 = m41;
            M42 = m42;
            M43 = m43;
            M44 = m44;
        }

        public static JfxMatrix4F operator *(in JfxMatrix4F left, in JfxMatrix4F right)
        {
            float m11 = left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31 + left.M14 * right.M41;
            float m12 = left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32 + left.M14 * right.M42;
            float m13 = left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33 + left.M14 * right.M43;
            float m14 = left.M11 * right.M14 + left.M12 * right.M24 + left.M13 * right.M34 + left.M14 * right.M44;

            float m21 = left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31 + left.M24 * right.M41;
            float m22 = left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32 + left.M24 * right.M42;
            float m23 = left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33 + left.M24 * right.M43;
            float m24 = left.M21 * right.M14 + left.M22 * right.M24 + left.M23 * right.M34 + left.M24 * right.M44;

            float m31 = left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31 + left.M34 * right.M41;
            float m32 = left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32 + left.M34 * right.M42;
            float m33 = left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33 + left.M34 * right.M43;
            float m34 = left.M31 * right.M14 + left.M32 * right.M24 + left.M33 * right.M34 + left.M34 * right.M44;

            float m41 = left.M41 * right.M11 + left.M42 * right.M21 + left.M43 * right.M31 + left.M44 * right.M41;
            float m42 = left.M41 * right.M12 + left.M42 * right.M22 + left.M43 * right.M32 + left.M44 * right.M42;
            float m43 = left.M41 * right.M13 + left.M42 * right.M23 + left.M43 * right.M33 + left.M44 * right.M43;
            float m44 = left.M41 * right.M14 + left.M42 * right.M24 + left.M43 * right.M34 + left.M44 * right.M44;

            return new JfxMatrix4F(
                m11, m12, m13, m14,
                m21, m22, m23, m24,
                m31, m32, m33, m34,
                m41, m42, m43, m44
            );
        }

        public static JfxMatrix4F Scale(float uniform)
            => Scale(uniform, uniform, uniform);

        public static JfxMatrix4F Scale(float sx, float sy, float sz)
        {
            return new JfxMatrix4F(
                sx, 0, 0, 0,
                0, sy, 0, 0,
                0, 0, sz, 0,
                0, 0, 0, 1
            );
        }

        public static JfxMatrix4F Translate(float dx, float dy, float dz)
        {
            return new JfxMatrix4F(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                dx, dy, dz, 1
            );
        }

        public static JfxMatrix4F Rotate(in JfxUnitVector3F axis, float angle)
        {
            float x = axis.X, y = axis.Y, z = axis.Z;
            float sa = MathF.Sin(angle), ca = MathF.Cos(angle);
            float xx = x * x, yy = y * y, zz = z * z;
            float xy = x * y, xz = x * z, yz = y * z;

            return new JfxMatrix4F(
                xx + ca * (1.0f - xx),
                xy - ca * xy + sa * z,
                xz - ca * xz - sa * y,
                0.0f,
                xy - ca * xy - sa * z,
                yy + ca * (1.0f - yy),
                yz - ca * yz + sa * x,
                0.0f,
                xz - ca * xz + sa * y,
                yz - ca * yz - sa * x,
                zz + ca * (1.0f - zz),
                0.0f,
                0.0f,
                0.0f,
                0.0f,
                1.0f
            );
        }
    }
}
