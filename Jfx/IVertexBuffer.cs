using Jfx.Mathematic;
using System;
using System.Runtime.InteropServices;

namespace Jfx
{
    public interface IVertexBuffer<TVSIn>
        where TVSIn : unmanaged
    {
        public int Count { get; }
        public unsafe TVSIn* UnsafeVertexPtr();
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
        public unsafe Vector3F* UnsafeVertexPtr() => (Vector3F*)ptr;
        public void Dispose() => handle.Free();
    }
}
