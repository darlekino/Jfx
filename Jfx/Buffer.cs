using Jfx.Mathematic;
using System;
using System.Runtime.InteropServices;

namespace Jfx
{
    public interface IVertexBuffer<TVSIn>
        where TVSIn : unmanaged
    {
        public int Count { get; }
        unsafe TVSIn* this[int index] { get; }
    }

    public class VertexBuffer : IVertexBuffer<Vector3F>, IDisposable
    {
        public readonly Vector3F[] array;
        private readonly GCHandle handle;
        private readonly IntPtr ptr;

        public VertexBuffer(Vector3F[] array)
        {
            this.array = array;
            handle = GCHandle.Alloc(array, GCHandleType.Pinned);
            ptr = handle.AddrOfPinnedObject();
        }

        public int Count => array.Length;
        public unsafe Vector3F* this[int index] => (Vector3F*)(ptr + (sizeof(Vector3F) * index));
        public void Dispose() => handle.Free();
    }
}
