using System;
using System.Runtime.InteropServices;

namespace Rasterizr.Util
{
	internal static class Interop
	{
        //public static unsafe void Read<T>(byte* pSrc, T[] data, int offset, int countInBytes)
        //    where T : struct
        //{
        //    var sizeOfT = SizeOf<T>();
        //    for (int i = 0; i < countInBytes / sizeOfT; i++)
        //    {
        //        data[offset + i] = (T)Marshal.PtrToStructure((IntPtr)pSrc, typeof(T));
        //        pSrc += sizeOfT;
        //    }
        //}

        [DllImport("kernel32.dll")]
        static extern void CopyMemory(IntPtr destination, IntPtr source, uint length);

        public static unsafe void Read<T>(byte* pSrc, T[] data, int offset, int countInBytes)
            where T : struct
        {
            var sizeOfT = SizeOf<T>();
            GCHandle handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                var pDst = handle.AddrOfPinnedObject() + (offset * sizeOfT);
                CopyMemory(pDst, (IntPtr)pSrc, (uint)countInBytes);
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
			for (int i = 0; i < count; i++)
            {
				Marshal.StructureToPtr(data[offset + i], (IntPtr)pDest, false);
				pDest += sizeOfT;
			}
		}
	}
}