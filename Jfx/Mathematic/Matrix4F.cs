using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.Mathematic
{
    public readonly struct Matrix4F
    {
        private static readonly Matrix4F identity = new Matrix4F(
            1, 0, 0, 0,
            0, 1, 0, 0,
            0, 0, 1, 0,
            0, 0, 0, 1
        );
        public static ref readonly Matrix4F Identity => ref identity;

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

        public Matrix4F(
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

        public static Matrix4F operator *(in Matrix4F left, in Matrix4F right)
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

            return new Matrix4F(
                m11, m12, m13, m14,
                m21, m22, m23, m24,
                m31, m32, m33, m34,
                m41, m42, m43, m44
            );
        }

        public Matrix4F TransformAround(in Vector3F transformationOrigin)
        {
            var translate = Translate(transformationOrigin);
            return translate.Inverse() * this * translate;
        }

        public Matrix4F Inverse()
        {
            float a = M11, b = M12, c = M13, d = M14;
            float e = M21, f = M22, g = M23, h = M24;
            float i = M31, j = M32, k = M33, l = M34;
            float m = M41, n = M42, o = M43, p = M44;

            float kp_lo = k * p - l * o;
            float jp_ln = j * p - l * n;
            float jo_kn = j * o - k * n;
            float ip_lm = i * p - l * m;
            float io_km = i * o - k * m;
            float in_jm = i * n - j * m;

            float a11 = +(f * kp_lo - g * jp_ln + h * jo_kn);
            float a12 = -(e * kp_lo - g * ip_lm + h * io_km);
            float a13 = +(e * jp_ln - f * ip_lm + h * in_jm);
            float a14 = -(e * jo_kn - f * io_km + g * in_jm);

            float det = a * a11 + b * a12 + c * a13 + d * a14;

            if (MathF.Abs(det) < float.Epsilon)
            {
                throw new InvalidOperationException("Determinant equal to zero!");
            }

            float invDet = 1.0f / det;

            float gp_ho = g * p - h * o;
            float fp_hn = f * p - h * n;
            float fo_gn = f * o - g * n;
            float ep_hm = e * p - h * m;
            float eo_gm = e * o - g * m;
            float en_fm = e * n - f * m;
            float gl_hk = g * l - h * k;
            float fl_hj = f * l - h * j;
            float fk_gj = f * k - g * j;
            float el_hi = e * l - h * i;
            float ek_gi = e * k - g * i;
            float ej_fi = e * j - f * i;

            return new Matrix4F(
                m11: a11 * invDet,
                m21: a12 * invDet,
                m31: a13 * invDet,
                m41: a14 * invDet,
                m12: -(b * kp_lo - c * jp_ln + d * jo_kn) * invDet,
                m22: +(a * kp_lo - c * ip_lm + d * io_km) * invDet,
                m32: -(a * jp_ln - b * ip_lm + d * in_jm) * invDet,
                m42: +(a * jo_kn - b * io_km + c * in_jm) * invDet,
                m13: +(b * gp_ho - c * fp_hn + d * fo_gn) * invDet,
                m23: -(a * gp_ho - c * ep_hm + d * eo_gm) * invDet,
                m33: +(a * fp_hn - b * ep_hm + d * en_fm) * invDet,
                m43: -(a * fo_gn - b * eo_gm + c * en_fm) * invDet,
                m14: -(b * gl_hk - c * fl_hj + d * fk_gj) * invDet,
                m24: +(a * gl_hk - c * el_hi + d * ek_gi) * invDet,
                m34: -(a * fl_hj - b * el_hi + d * ej_fi) * invDet,
                m44: +(a * fk_gj - b * ek_gi + c * ej_fi) * invDet
            );
        }

        public bool TryInverse(out Matrix4F result)
        {
            float a = M11, b = M12, c = M13, d = M14;
            float e = M21, f = M22, g = M23, h = M24;
            float i = M31, j = M32, k = M33, l = M34;
            float m = M41, n = M42, o = M43, p = M44;

            float kp_lo = k * p - l * o;
            float jp_ln = j * p - l * n;
            float jo_kn = j * o - k * n;
            float ip_lm = i * p - l * m;
            float io_km = i * o - k * m;
            float in_jm = i * n - j * m;

            float a11 = +(f * kp_lo - g * jp_ln + h * jo_kn);
            float a12 = -(e * kp_lo - g * ip_lm + h * io_km);
            float a13 = +(e * jp_ln - f * ip_lm + h * in_jm);
            float a14 = -(e * jo_kn - f * io_km + g * in_jm);

            float det = a * a11 + b * a12 + c * a13 + d * a14;

            if (MathF.Abs(det) < float.Epsilon)
            {
                result = new Matrix4F(
                    float.NaN, float.NaN, float.NaN, float.NaN,
                    float.NaN, float.NaN, float.NaN, float.NaN,
                    float.NaN, float.NaN, float.NaN, float.NaN,
                    float.NaN, float.NaN, float.NaN, float.NaN
                );

                return false;
            }

            float invDet = 1.0f / det;

            float gp_ho = g * p - h * o;
            float fp_hn = f * p - h * n;
            float fo_gn = f * o - g * n;
            float ep_hm = e * p - h * m;
            float eo_gm = e * o - g * m;
            float en_fm = e * n - f * m;
            float gl_hk = g * l - h * k;
            float fl_hj = f * l - h * j;
            float fk_gj = f * k - g * j;
            float el_hi = e * l - h * i;
            float ek_gi = e * k - g * i;
            float ej_fi = e * j - f * i;

            result = new Matrix4F(
                m11: a11 * invDet,
                m21: a12 * invDet,
                m31: a13 * invDet,
                m41: a14 * invDet,
                m12: -(b * kp_lo - c * jp_ln + d * jo_kn) * invDet,
                m22: +(a * kp_lo - c * ip_lm + d * io_km) * invDet,
                m32: -(a * jp_ln - b * ip_lm + d * in_jm) * invDet,
                m42: +(a * jo_kn - b * io_km + c * in_jm) * invDet,
                m13: +(b * gp_ho - c * fp_hn + d * fo_gn) * invDet,
                m23: -(a * gp_ho - c * ep_hm + d * eo_gm) * invDet,
                m33: +(a * fp_hn - b * ep_hm + d * en_fm) * invDet,
                m43: -(a * fo_gn - b * eo_gm + c * en_fm) * invDet,
                m14: -(b * gl_hk - c * fl_hj + d * fk_gj) * invDet,
                m24: +(a * gl_hk - c * el_hi + d * ek_gi) * invDet,
                m34: -(a * fl_hj - b * el_hi + d * ej_fi) * invDet,
                m44: +(a * fk_gj - b * ek_gi + c * ej_fi) * invDet
            );

            return true;
        }

        public static Matrix4F Scale(float uniform)
            => Scale(uniform, uniform, uniform);

        public static Matrix4F Scale(float sx, float sy, float sz)
        {
            return new Matrix4F(
                sx, 0, 0, 0,
                0, sy, 0, 0,
                0, 0, sz, 0,
                0, 0, 0, 1
            );
        }

        public static Matrix4F Translate(Vector3F v) => Translate(v.X, v.Y, v.Z);

        public static Matrix4F Translate(float dx, float dy, float dz)
        {
            return new Matrix4F(
                1, 0, 0, 0,
                0, 1, 0, 0,
                0, 0, 1, 0,
                dx, dy, dz, 1
            );
        }

        public static Matrix4F Rotate(in Vector3F axis, float angle) => Rotate(axis.Normalize(), angle);

        public static Matrix4F Rotate(in UnitVector3F axis, float angle)
        {
            float x = axis.X, y = axis.Y, z = axis.Z;
            float sa = MathF.Sin(angle), ca = MathF.Cos(angle);
            float xx = x * x, yy = y * y, zz = z * z;
            float xy = x * y, xz = x * z, yz = y * z;

            return new Matrix4F(
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

        public static Matrix4F LookAtRH(in Vector3F cameraPosition, in Vector3F cameraTarget, in Vector3F cameraUpVector)
        {
            var zaxis = (cameraPosition - cameraTarget).Normalize();
            var xaxis = cameraUpVector.CrossProduct(zaxis).Normalize();
            var yaxis = zaxis.CrossProduct(xaxis).Normalize();

            //var transformation = new JfxMatrix4F(
            //    xaxis.X, yaxis.X, zaxis.X, 0,
            //    xaxis.Y, yaxis.Y, zaxis.Y, 0,
            //    xaxis.Z, yaxis.Z, zaxis.Z, 0,
            //    0, 0, 0, 1
            //);

            //var translation = new JfxMatrix4F(
            //    1, 0, 0, 0,
            //    0, 1, 0, 0,
            //    0, 0, 1, 0,
            //    -cameraPosition.X, -cameraPosition.Y, -cameraPosition.Z, 1
            //);

            //  translation * transformation == new JfxMatrix4F(
            //    xaxis.X, yaxis.X, zaxis.X, 0,
            //    xaxis.Y, yaxis.Y, zaxis.Y, 0,
            //    xaxis.Z, yaxis.Z, zaxis.Z, 0,
            //    -xaxis.DotProduct(cameraPosition), -yaxis.DotProduct(cameraPosition), -zaxis.DotProduct(cameraPosition), 1
            //);
            // that's why we have dot product for translations (just try to multiply by yourself)

            /*
             * x = x' + a;  ==> x' = x - a;
             * y = y' + b;  ==> y' = y - b;
             * that's why we have minus for translations
             */

            return new Matrix4F(
                xaxis.X, yaxis.X, zaxis.X, 0,
                xaxis.Y, yaxis.Y, zaxis.Y, 0,
                xaxis.Z, yaxis.Z, zaxis.Z, 0,
                -xaxis.DotProduct(cameraPosition), -yaxis.DotProduct(cameraPosition), -zaxis.DotProduct(cameraPosition), 1
            );
        }

        public static Matrix4F PerspectiveFieldOfViewRH(float fieldOfViewY, float aspectRatio, float znearPlane, float zfarPlane)
        {
            float yScale = 1 / MathF.Tan(fieldOfViewY * 0.5f);
            float xScale = yScale / aspectRatio;
            return new Matrix4F(
                xScale, 0, 0, 0,
                0, yScale, 0, 0,
                0, 0, zfarPlane / (znearPlane - zfarPlane), -1,
                0, 0, znearPlane * zfarPlane / (znearPlane - zfarPlane), 0
            );
        }

        public static Matrix4F OrthographicRH(float width, float height, float znearPlane, float zfarPlane)
        {
            return new Matrix4F(
                2 / width, 0, 0, 0,
                0, 2 / height, 0, 0,
                0, 0, 1 / (znearPlane - zfarPlane), 0,
                0, 0, znearPlane / (znearPlane - zfarPlane), 1
            );
        }
    }
}
