using System;

namespace BlueStacks.Common
{
	public static class Tasks
	{
		public enum Parameter
		{
			Create,
			Delete,
			Query,
			Run,
			End
		}

		public enum Frequency
		{
			MINUTE,
			HOURLY,
			DAILY,
			WEEKLY,
			MONTHLY,
			ONCE,
			ONSTART,
			ONLOGON,
			ONIDLE,
			ONEVENT
		}

		public enum Modifiers
		{
			MON,
			TUE,
			WED,
			THU,
			FRI,
			SAT,
			SUN
		}

		public enum Days
		{
			MON,
			TUE,
			WED,
			THU,
			FRI,
			SAT,
			SUN
		}
	}
}


