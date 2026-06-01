using System;

namespace BlueStacks.Common
{
	public static class EnumHelper
	{
		public static TEnum Parse<TEnum>(string value, TEnum defaultValue)
		{
			if (value == null || !Enum.IsDefined(typeof(TEnum), value))
			{
				return defaultValue;
			}
			return (TEnum)((object)Enum.Parse(typeof(TEnum), value));
		}

		public static bool TryParse<TEnum>(string value, out TEnum result) where TEnum : struct, IConvertible
		{
			bool flag = value != null && Enum.IsDefined(typeof(TEnum), value);
			result = (flag ? ((TEnum)((object)Enum.Parse(typeof(TEnum), value))) : default(TEnum));
			return flag;
		}
	}
}


