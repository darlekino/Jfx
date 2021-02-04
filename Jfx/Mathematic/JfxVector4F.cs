using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.Mathematic
{
    public readonly struct JfxVector4F
    {
        private static readonly JfxVector4F zero = new JfxVector4F(0, 0, 0, 0);
        public static ref readonly JfxVector4F Zero => ref zero;

        public readonly float X;
        public readonly float Y;
        public readonly float Z;
        public readonly float W /* 1 = place, 0 = direction */;

        public JfxVector4F(float x, float y, float z, float w = 1)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public static JfxVector4F operator +(in JfxVector4F left, in JfxVector4F right)
        {
            return new JfxVector4F(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z,
                left.W + right.W
            );
        }

        public static JfxVector4F operator -(in JfxVector4F left, in JfxVector4F right)
        {
            return new JfxVector4F(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z,
                left.W - right.W
            );
        }

        public static JfxVector4F operator *(in JfxVector4F left, float right)
        {
            return new JfxVector4F(
                left.X * right,
                left.Y * right,
                left.Z * right,
                left.W * right
            );
        }

        public static JfxVector4F Transform(in JfxVector4F position, in JfxMatrix4F matrix)
        {
            float x = position.X * matrix.M11 + position.Y * matrix.M21 + position.Z * matrix.M31 + position.W * matrix.M41;
            float y = position.X * matrix.M12 + position.Y * matrix.M22 + position.Z * matrix.M32 + position.W * matrix.M42;
            float z = position.X * matrix.M13 + position.Y * matrix.M23 + position.Z * matrix.M33 + position.W * matrix.M43;
            float w = position.X * matrix.M14 + position.Y * matrix.M24 + position.Z * matrix.M34 + position.W * matrix.M44;

            return new JfxVector4F(
                x,
                y,
                z,
                w
            );
        }
    }
}
