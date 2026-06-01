using System;
using System.Globalization;
using System.Reflection;

namespace BlueStacks.Common
{
	public static class TaskScheduler
	{
		public static int CreateTask(string taskName, string binaryToRun, Tasks.Frequency frequency, int modifierOrIdleTime, DateTime timeToStart)
		{
			if (string.IsNullOrEmpty(binaryToRun))
			{
				binaryToRun = Assembly.GetEntryAssembly().Location;
			}
			DateTimeFormatInfo dateTimeFormat = new CultureInfo("en-US", false).DateTimeFormat;
			string text = string.Format(CultureInfo.InvariantCulture, string.Concat(new string[]
			{
				"/",
				Tasks.Parameter.Create.ToString(),
				" /SC ",
				frequency.ToString(),
				" /TN ",
				taskName,
				string.Format(CultureInfo.InvariantCulture, " /TR \"{0}\"", new object[] { binaryToRun }),
				" /F"
			}), new object[0]);
			if (frequency != Tasks.Frequency.ONIDLE)
			{
				text = string.Format(CultureInfo.InvariantCulture, "{0} /ST {1}", new object[]
				{
					text,
					timeToStart.ToString("HH:mm", dateTimeFormat)
				});
				text = string.Format(CultureInfo.InvariantCulture, "{0} /MO {1}", new object[]
				{
					text,
					modifierOrIdleTime.ToString(CultureInfo.InvariantCulture)
				});
			}
			else
			{
				text = string.Format(CultureInfo.InvariantCulture, "{0} /I " + modifierOrIdleTime.ToString(CultureInfo.InvariantCulture), new object[] { text });
			}
			int num = TaskScheduler.RunSchedulerCommand(text);
			if (num != 0)
			{
				Logger.Error("An error occured while creating the task, exit code: {0}", new object[] { num });
			}
			return num;
		}

		public static int DeleteTask(string taskName)
		{
			int num = TaskScheduler.RunSchedulerCommand(string.Format(CultureInfo.InvariantCulture, string.Concat(new string[]
			{
				"/",
				Tasks.Parameter.Delete.ToString(),
				" /TN ",
				taskName,
				" /F"
			}), new object[0]));
			if (num != 0)
			{
				Logger.Error("An error occured while deleting the task, exit code: {0}", new object[] { num });
			}
			return num;
		}

		private static string QueryTaskArguments(string taskName)
		{
			return string.Format(CultureInfo.InvariantCulture, "/" + Tasks.Parameter.Query.ToString() + " /FO LIST /V  /TN " + taskName, new object[0]);
		}

		public static int QueryTask(string taskName)
		{
			int num = TaskScheduler.RunSchedulerCommand(TaskScheduler.QueryTaskArguments(taskName));
			if (num != 0)
			{
				Logger.Error("An error occured while querying the task, exit code: {0}", new object[] { num });
			}
			return num;
		}

		private static int RunSchedulerCommand(string args)
		{
			return RunCommand.RunCmd(TaskScheduler.BinaryName, args, true, true, false, 0).ExitCode;
		}

		public static CmdRes GetTaskQueryCommandOutput(string taskName)
		{
			return RunCommand.RunCmd(TaskScheduler.BinaryName, TaskScheduler.QueryTaskArguments(taskName), false, true, false, 0);
		}

		private static string BinaryName = "schtasks.exe";
	}
}


