using Jfx.Mathematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jfx
{
    public interface IVertexShader<TVIn, TVOut>
        where TVIn : unmanaged
        where TVOut : unmanaged
    {
        Vector4F ExecuteStage(in TVIn vsin, out TVOut fsin);
    }

    public interface IFragmentShader<TFIn>
        where TFIn : unmanaged
    {
        bool ExecuteStage(in Vector4F fragCoord, in TFIn fsin, out Vector4F color);
    }

    public interface IShaders<TVIn, TFIn>
        where TVIn : unmanaged
        where TFIn : unmanaged
    {
        public IVertexShader<TVIn, TFIn> VertexShader { get; }
        public IFragmentShader<TFIn> FragmentShader { get; }
    }
}