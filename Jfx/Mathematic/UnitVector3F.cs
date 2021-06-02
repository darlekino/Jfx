using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.Mathematic
{
    public readonly struct UnitVector3F
    {
        private static readonly UnitVector3F xAxis = new UnitVector3F(1, 0, 0);
        private static readonly UnitVector3F yAxis = new UnitVector3F(0, 1, 0);
        private static readonly UnitVector3F zAxis = new UnitVector3F(0, 0, 1);

        public static ref readonly UnitVector3F XAxis => ref xAxis;
        public static ref readonly UnitVector3F YAxis => ref yAxis;
        public static ref readonly UnitVector3F ZAxis => ref zAxis;

        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        private UnitVector3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3F ToVector() => new Vector3F(X, Y, Z);

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

        public bool IsPerpendicularTo(UnitVector3F othervector, float tolerance = float.Epsilon) 
            => MathF.Abs(this.DotProduct(othervector)) < tolerance;

        public static UnitVector3F Normalize(in Vector3F v)
        {
            var length = v.Length();
            return new UnitVector3F(
                v.X / length,
                v.Y / length,
                v.Z / length
            );
        }

        public static Vector3F operator *(in UnitVector3F left, float right)
        {
            return new Vector3F(
                left.X * right,
                left.Y * right,
                left.Z * right
            );
        }

        public static Vector3F operator *(float left, in UnitVector3F right) => right * left;

        public static Vector3F operator +(in UnitVector3F left, in Vector3F right)
        {
            return new Vector3F(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z
            );
        }

        public static Vector3F operator -(in UnitVector3F left, in Vector3F right)
        {
            return new Vector3F(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z
            );
        }

        public static Vector3F operator -(in UnitVector3F v) => new Vector3F(-v.X, -v.Y, -v.Z);

        public float AngleTo(Vector3F v) => AngleTo(v.Normalize());

        public float AngleTo(UnitVector3F v)
        {
            var dp = DotProduct(v);
            return MathF.Acos(dp);
        }
    }
}
