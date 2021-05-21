using Jfx.Mathematic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Jfx
{
    public interface IVertexBuffer<TVSIn>
        where TVSIn : unmanaged
    {
        public int Count { get; }
        unsafe TVSIn this[int index] { get; }
    }

    public class VertexBuffer : IVertexBuffer<Vector3F>
    {
        public readonly Vector3F[] array;

        public VertexBuffer(Vector3F[] array)
        {
            this.array = array;
        }

        public int Count => array.Length;
        public unsafe Vector3F this[int index] => array[index];
    }
}
