using System;

namespace BlueStacks.Common
{
	public class CmdRes
	{
		public string StdOut { get; set; } = string.Empty;

		public string StdErr { get; set; } = string.Empty;

		public int ExitCode { get; set; }
	}
}


