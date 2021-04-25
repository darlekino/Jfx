using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.Mathematic
{
    public readonly struct Vector3F: IEquatable<Vector3F> 
    {
        private static readonly Vector3F zero = new Vector3F(0, 0, 0);
        public static ref readonly Vector3F Zero => ref zero;

        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        public Vector3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float DotProduct(in UnitVector3F right)
            => X * right.X + Y * right.Y + Z * right.Z;

        public float DotProduct(in Vector3F right)
            => X * right.X + Y * right.Y + Z * right.Z;

        public Vector3F CrossProduct(in UnitVector3F right)
        {
            return new Vector3F(
                Y * right.Z - Z * right.Y,
                Z * right.X - X * right.Z,
                X * right.Y - Y * right.X
            );
        }

        public Vector3F CrossProduct(in Vector3F right)
        {
            return new Vector3F(
                Y * right.Z - Z * right.Y,
                Z * right.X - X * right.Z,
                X * right.Y - Y * right.X
            );
        }

        public float Length() => MathF.Sqrt(X * X + Y * Y + Z * Z);

        public UnitVector3F Normalize() => UnitVector3F.Normalize(this);

        public static Vector3F operator +(in Vector3F left, in Vector3F right)
        {
            return new Vector3F(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z
            );
        }

        public static Vector3F operator -(in Vector3F left, in Vector3F right)
        {
            return new Vector3F(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z
            );
        }

        public static Vector3F operator +(in Vector3F left, in UnitVector3F right)
        {
            return new Vector3F(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z
            );
        }

        public static Vector3F operator -(in Vector3F left, in UnitVector3F right)
        {
            return new Vector3F(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z
            );
        }

        public static Vector3F operator *(in Vector3F left, float right)
        {
            return new Vector3F(
                left.X * right,
                left.Y * right,
                left.Z * right
            );
        }

        public static Vector3F operator *(float left, in Vector3F right) => right * left;

        public static bool operator ==(in Vector3F left, in Vector3F right) => left.Equals(right);
        public static bool operator !=(in Vector3F left, in Vector3F right) => !(left == right);

        public static Vector3F Transform(in Vector3F position, in Matrix4F matrix)
        {
            float x = position.X * matrix.M11 + position.Y * matrix.M21 + position.Z * matrix.M31 + matrix.M41;
            float y = position.X * matrix.M12 + position.Y * matrix.M22 + position.Z * matrix.M32 + matrix.M42;
            float z = position.X * matrix.M13 + position.Y * matrix.M23 + position.Z * matrix.M33 + matrix.M43;
            float wInv = 1 / (position.X * matrix.M14 + position.Y * matrix.M24 + position.Z * matrix.M34 + matrix.M44);

            return new Vector3F(
                x * wInv,
                y * wInv,
                z * wInv
            );
        }

        public bool Equals(Vector3F other) => X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);

        public override bool Equals(object obj) => (obj is Vector3F u && Equals(u));

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
