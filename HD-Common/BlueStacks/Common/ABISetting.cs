using System;
using System.ComponentModel;

namespace BlueStacks.Common
{
	public enum ABISetting
	{
		[Description("4")]
		ARM,
		[Description("15")]
		Auto,
		[Description("15")]
		ARM64,
		[Description("7")]
		Auto64,
		[Description("-1")]
		Custom
	}
}


