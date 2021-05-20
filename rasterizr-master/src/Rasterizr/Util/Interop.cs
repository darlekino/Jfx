using System;
using System.Runtime.InteropServices;

namespace Rasterizr.Util
{
	internal static class Interop
	{
        [DllImport("kernel32.dll")]
        static extern void CopyMemory(IntPtr destination, IntPtr source, uint length);

        public static unsafe void Read<T>(byte* pSrc, T[] data, int offset, int countInBytes)
            where T : struct
        {
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                var pDest = handle.AddrOfPinnedObject() + (offset * SizeOf<T>());
                CopyMemory(pDest, (IntPtr)pSrc, (uint)countInBytes);
            }
            finally
            {
                handle.Free();
            }
        }

        public static int SizeOf<T>() where T: struct
		{
			return Marshal.SizeOf(typeof(T));
		}

		public static unsafe void Write<T>(byte* pDest, ref T data) where T : struct
		{
			Marshal.StructureToPtr(data, (IntPtr)pDest, false);
		}

        public static unsafe void Write<T>(byte* pDest, T[] data, int offset, int count)
            where T : struct
        {
            var sizeOfT = SizeOf<T>();
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                var pSrc = handle.AddrOfPinnedObject() + (offset * sizeOfT);
                var countInBytes = count * sizeOfT;
                CopyMemory((IntPtr)pDest, pSrc, (uint)countInBytes);
            }
            finally
            {
                handle.Free();
            }

        }
	}
}