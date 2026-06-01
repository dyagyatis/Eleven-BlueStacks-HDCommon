using System;

namespace BlueStacks.Common
{
	[AttributeUsage(AttributeTargets.Field)]
	public class ArgAttribute : Attribute
	{
		public string Name { get; set; }

		public object Value { get; set; }

		public string Description { get; set; }
	}
}


