using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.Mathematic
{
    public struct JfxPlane
    {
        public readonly JfxUnitVector3F Normal;
        public readonly float Distance;

        public JfxPlane(in JfxUnitVector3F normal, float offset = 0)
        {
            Normal = normal;
            Distance = -offset;
        }

        public JfxPlane(in JfxVector3F rootPoint, in JfxUnitVector3F normal) : this(normal, normal.DotProduct(rootPoint))
        {
        }

        public JfxVector3F Project(in JfxVector3F p)
        {
            var projectionVector = (Normal.DotProduct(p) + Distance) * Normal;
            return p - projectionVector;
        }

        public float SignedDistanceTo(in JfxVector3F point)
        {
            var p = Project(point);
            return (point - p).DotProduct(Normal);
        }

        public JfxVector3F IntersectionWith(in JfxRay ray)
        {
            if (Normal.IsPerpendicularTo(ray.Direction))
            {
                throw new InvalidOperationException("Ray is parallel to the plane.");
            }

            var d = SignedDistanceTo(ray.ThroughPoint);
            var t = -1 * d / ray.Direction.DotProduct(Normal);
            return ray.ThroughPoint + (t * ray.Direction);
        }

        public static JfxPlane FromPoints(in JfxVector3F p1, in JfxVector3F p2, in JfxVector3F p3)
        {
            if (p1 == p2 || p1 == p3 || p2 == p3)
            {
                throw new ArgumentException("Must use three different points");
            }

            var v1 = p2 - p1;
            var v2 = p3 - p1;
            var cross = v1.CrossProduct(v2);

            if (cross.Length() <= float.Epsilon)
            {
                throw new ArgumentException("The 3 points should not be on the same line");
            }

            return new JfxPlane(p1, cross.Normalize());
        }
    }
}
