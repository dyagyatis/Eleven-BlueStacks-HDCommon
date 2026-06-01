using System;
using System.IO;

namespace BlueStacks.Common
{
	public static class Logger
	{
		public static void InitUserLog() { }
		private static string GetLogDir() { return Path.GetTempPath(); }
		private static void HdLogger(int prio, uint tid, string tag, string msg) { }
		public static void InitLog(string logFileName, string tag, bool doLogRotation = true) { }
		public static void InitLogAtPath(string logFilePath, string _, bool doLogRotation) { }
		public static Logger.HdLoggerCallback GetHdLoggerCallback() { return Logger.s_HdLoggerCallback; }
		private static void LogLevelsInit() { }
		public static void EnableDebugLogs() { }
		private static bool IsLogLevelEnabled(string tag, string level) { return false; }
		private static void DoLogRotation() { }
		private static void Open() { }
		private static void Close() { }
		public static TextWriter GetWriter() { return TextWriter.Null; }
		private static string GetLogFromLevel(int level) { return string.Empty; }
		private static void Print(string s, string tag, string fmt, params object[] args) { }
		private static void Print(string fmt, params object[] args) { }
		private static void Print(string msg) { }
		public static void Fatal(string fmt, params object[] args) { }
		public static void Fatal(string msg) { }
		public static void Error(string fmt, params object[] args) { }
		public static void Error(string msg) { }
		public static void Warning(string fmt, params object[] args) { }
		public static void Warning(string msg) { }
		public static void Info(string fmt, params object[] args) { }
		public static void Info(string msg) { }
		public static void Debug(string fmt, params object[] args) { }
		public static void Debug(string msg) { }
		private static string GetPrefix(string tag, string logLevel) { return string.Empty; }
		public static void InitVmInstanceName(string vmName) { }

		private const int HDLOG_PRIORITY_FATAL = 0;
		private const int HDLOG_PRIORITY_ERROR = 1;
		private const int HDLOG_PRIORITY_WARNING = 2;
		private const int HDLOG_PRIORITY_INFO = 3;
		private const int HDLOG_PRIORITY_DEBUG = 4;
		private static int s_OpenCloseAfter = 300;
		private static int s_OpenCloseAfterCount = 0;
		private static object s_sync = new object();
		private static TextWriter sWriter = TextWriter.Null;
		private static int s_logRotationTime = 30000;
		public static readonly int s_logFileSize = 10485760;
		public static readonly int s_totalLogFileNum = 5;
		private static string s_logFilePath = null;
		private static bool s_loggerInited = false;
		private static int s_processId = -1;
		private static string s_processName = "Unknown";
		private static string s_logLevels = null;
		private static FileStream s_fileStream;
		private static string s_logDir = null;
		private const string DEFAULT_FILE_NAME = "BlueStacksUsers";
		private static string s_logStringFatal = "FATAL";
		private static string s_logStringError = "ERROR";
		private static string s_logStringWarning = "WARNING";
		private static string s_logStringInfo = "INFO";
		private static string s_logStringDebug = "DEBUG";
		private static string s_vmNameTextToLog = "";
		private static Logger.HdLoggerCallback s_HdLoggerCallback = new Logger.HdLoggerCallback(Logger.HdLogger);
		public delegate void HdLoggerCallback(int prio, uint tid, string tag, string msg);
	}
}


