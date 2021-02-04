using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.Mathematic
{
    public readonly struct JfxVector3F
    {
        private static readonly JfxVector3F zero = new JfxVector3F(0, 0, 0);
        public static ref readonly JfxVector3F Zero => ref zero;

        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        public JfxVector3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float DotProduct(in JfxUnitVector3F right)
            => X * right.X + Y * right.Y + Z * right.Z;

        public float DotProduct(in JfxVector3F right)
            => X * right.X + Y * right.Y + Z * right.Z;

        public JfxVector3F CrossProduct(in JfxUnitVector3F right)
        {
            return new JfxVector3F(
                Y * right.Z - Z * right.Y,
                Z * right.X - X * right.Z,
                X * right.Y - Y * right.X
            );
        }

        public JfxVector3F CrossProduct(in JfxVector3F right)
        {
            return new JfxVector3F(
                Y * right.Z - Z * right.Y,
                Z * right.X - X * right.Z,
                X * right.Y - Y * right.X
            );
        }

        public float Length()
            => MathF.Sqrt(X * X + Y * Y + Z * Z);

        public JfxUnitVector3F Normilize()
            => JfxUnitVector3F.Normalize(this);

        public static JfxVector3F operator +(in JfxVector3F left, in JfxVector3F right)
        {
            return new JfxVector3F(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z
            );
        }

        public static JfxVector3F operator -(in JfxVector3F left, in JfxVector3F right)
        {
            return new JfxVector3F(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z
            );
        }

        public static JfxVector3F operator *(in JfxVector3F left, float right)
        {
            return new JfxVector3F(
                left.X * right,
                left.Y * right,
                left.Z * right
            );
        }

        public static JfxVector3F Transform(in JfxVector3F position, in JfxMatrix4F matrix)
        {
            float x = position.X * matrix.M11 + position.Y * matrix.M21 + position.Z * matrix.M31 + matrix.M41;
            float y = position.X * matrix.M12 + position.Y * matrix.M22 + position.Z * matrix.M32 + matrix.M42;
            float z = position.X * matrix.M13 + position.Y * matrix.M23 + position.Z * matrix.M33 + matrix.M43;
            float wInv = 1 / (position.X * matrix.M14 + position.Y * matrix.M24 + position.Z * matrix.M34 + matrix.M44);

            return new JfxVector3F(
                x * wInv,
                y * wInv,
                z * wInv
            );
        }

    }
}
