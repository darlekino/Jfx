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
        where TFIn : unmanaged
    {
        Vector4F VertexShader(in TVIn vsin, out TFIn fsin);
        Vector4F FragmentShader(in TFIn fsin, in Vector4F fragCoord);
    }
}
