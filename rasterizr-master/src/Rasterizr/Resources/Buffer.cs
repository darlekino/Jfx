﻿using Rasterizr.Util;

namespace Rasterizr.Resources
{
	public class Buffer : Resource
	{
		private readonly byte[] _data;

	    internal override ResourceType ResourceType
		{
			get { return ResourceType.Buffer; }
		}

	    internal override int Size
	    {
	        get { return _data.Length; }
	    }

	    internal byte[] Data
		{
			get { return _data; }
		}

		internal Buffer(Device device, BufferDescription description)
			: base(device)
		{
			_data = new byte[description.SizeInBytes];
		}

		internal void GetData<T>(int dataOffset, T[] data, int startIndex, int countInBytes)
			where T : unmanaged
		{
			Utilities.FromByteArray(data, dataOffset, _data, startIndex, countInBytes);
		}

        internal void GetData<T>(T[] data, int startIndex, int countInBytes)
			where T : unmanaged
		{
			GetData(0, data, startIndex, countInBytes);
		}

        internal void GetData<T>(out T data, int startIndex, int countInBytes)
			where T : unmanaged
		{
			Utilities.FromByteArray(out data, _data, startIndex, countInBytes);
		}

        internal void GetData<T>(T[] data)
			where T : unmanaged
		{
			GetData(data, 0, _data.Length * Utilities.SizeOf<T>());
		}

	    internal void SetData(byte[] data, int offsetInBytes = 0)
	    {
	        Utilities.ToByteArray(data, _data, offsetInBytes);
	    }
	}
}