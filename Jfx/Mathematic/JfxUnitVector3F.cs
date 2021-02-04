using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.Mathematic
{
    public readonly struct JfxUnitVector3F
    {
        public readonly float X;
        public readonly float Y;
        public readonly float Z;

        private JfxUnitVector3F(float x, float y, float z)
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

        public static JfxUnitVector3F Normalize(in JfxVector3F v)
        {
            var length = v.Length();
            return new JfxUnitVector3F(
                v.X / length,
                v.Y / length,
                v.Z / length
            );
        }
    }
}
