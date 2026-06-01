using System;

namespace BlueStacks.Common
{
	public struct SP_DEVINFO_DATA
	{
		public int cbSize { readonly get; set; }

		public Guid ClassGuid { readonly get; set; }

		public int DevInst { readonly get; set; }

		public int Reserved { readonly get; set; }
	}
}


