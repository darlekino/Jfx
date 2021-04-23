using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx.Mathematic
{
    public struct JfxRay
    {
        public readonly JfxVector3F ThroughPoint;
        public readonly JfxUnitVector3F Direction;

        public JfxRay(in JfxVector3F throughPoint, in JfxUnitVector3F direction)
        {
            ThroughPoint = throughPoint;
            Direction = direction;
        }

        public JfxRay(in JfxVector3F throughPoint, in JfxVector3F direction) : this(throughPoint, direction.Normalize())
        {
        }
    }
}
