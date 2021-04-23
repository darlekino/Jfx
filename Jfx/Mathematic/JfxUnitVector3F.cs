using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.Mathematic
{
    public readonly struct JfxUnitVector3F
    {
        private static readonly JfxUnitVector3F xAxis = new JfxUnitVector3F(1, 0, 0);
        private static readonly JfxUnitVector3F yAxis = new JfxUnitVector3F(0, 1, 0);
        private static readonly JfxUnitVector3F zAxis = new JfxUnitVector3F(0, 0, 1);

        public static ref readonly JfxUnitVector3F XAxis => ref xAxis;
        public static ref readonly JfxUnitVector3F YAxis => ref yAxis;
        public static ref readonly JfxUnitVector3F ZAxis => ref zAxis;

        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        private JfxUnitVector3F(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public JfxVector3F ToVector() => new JfxVector3F(X, Y, Z);

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

        public bool IsPerpendicularTo(JfxUnitVector3F othervector, float tolerance = float.Epsilon) 
            => Math.Abs(this.DotProduct(othervector)) < tolerance;

        public static JfxUnitVector3F Normalize(in JfxVector3F v)
        {
            var length = v.Length();
            return new JfxUnitVector3F(
                v.X / length,
                v.Y / length,
                v.Z / length
            );
        }

        public static JfxVector3F operator *(in JfxUnitVector3F left, float right)
        {
            return new JfxVector3F(
                left.X * right,
                left.Y * right,
                left.Z * right
            );
        }

        public static JfxVector3F operator *(float left, in JfxUnitVector3F right) => right * left;

        public static JfxVector3F operator +(in JfxUnitVector3F left, in JfxVector3F right)
        {
            return new JfxVector3F(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z
            );
        }

        public static JfxVector3F operator -(in JfxUnitVector3F left, in JfxVector3F right)
        {
            return new JfxVector3F(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z
            );
        }

        public static JfxVector3F operator -(in JfxUnitVector3F v) => new JfxVector3F(-v.X, -v.Y, -v.Z);

        public float AngleTo(JfxVector3F v) => AngleTo(v.Normalize());

        public float AngleTo(JfxUnitVector3F v)
        {
            var dp = DotProduct(v);
            return MathF.Acos(dp);
        }
    }
}
