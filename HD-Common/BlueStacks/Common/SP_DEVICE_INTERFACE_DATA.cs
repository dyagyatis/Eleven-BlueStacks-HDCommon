using System;

namespace BlueStacks.Common
{
	public struct SP_DEVICE_INTERFACE_DATA
	{
		public int CbSize { readonly get; set; }

		public Guid InterfaceClassGuid { readonly get; set; }

		public int Flags { readonly get; set; }

		public int Reserved { readonly get; set; }
	}
}


