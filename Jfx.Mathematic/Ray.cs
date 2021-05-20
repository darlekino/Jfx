using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.Mathematic
{
    public struct Ray
    {
        public readonly Vector3F ThroughPoint;
        public readonly UnitVector3F Direction;

        public Ray(in Vector3F throughPoint, in UnitVector3F direction)
        {
            ThroughPoint = throughPoint;
            Direction = direction;
        }

        public Ray(in Vector3F throughPoint, in Vector3F direction) : this(throughPoint, direction.Normalize())
        {
        }
    }
}
