using Jfx.Mathematic;
using System;
using System.Runtime.InteropServices;

namespace Jfx
{
    public interface IVertexBuffer<TVSIn>
        where TVSIn : unmanaged
    {
        int Count { get; }
        unsafe TVSIn* UnsafeVertexPtr();
        ref readonly TVSIn this[int index] { get; }
    }

    public unsafe class VertexBuffer : IVertexBuffer<Vector3F>, IDisposable
    {
        public readonly Vector3F[] array;
        private readonly GCHandle handle;
        private readonly Vector3F* ptr;

        public VertexBuffer(Vector3F[] array)
        {
            this.array = array;
            handle = GCHandle.Alloc(array, GCHandleType.Pinned);
            ptr = (Vector3F*)handle.AddrOfPinnedObject();
        }

        public int Count => array.Length;

        public ref readonly Vector3F this[int index] => ref *(ptr + index);

        public unsafe Vector3F* UnsafeVertexPtr() => ptr;
        public void Dispose() => handle.Free();
    }
}
