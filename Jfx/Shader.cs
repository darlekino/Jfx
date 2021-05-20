using Jfx.Mathematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx
{
    public interface IShader<TVIn, TFIn>
        where TVIn : unmanaged
        where TFIn : unmanaged, IFSIn
    {
        void VertexShader(in TVIn vsin, out TFIn fsin);
        void FragmentShader(in TFIn fsin, out Vector4F color);
    }

    public interface IFSIn
    {
        public Vector4F Position { get; set; }
    }
}
