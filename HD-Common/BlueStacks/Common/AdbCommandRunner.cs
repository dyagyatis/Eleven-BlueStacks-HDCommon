using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public class AdbCommandRunner
	{
		public int Port { get; set; }

		public string Path { get; set; }

		public bool IsHostConnected { get; set; }

		public AdbCommandRunner(string vmName = "Android")
		{
			if (!string.IsNullOrEmpty(vmName))
			{
				this.mVmName = vmName;
				AdbCommandRunner.GUEST_URL = string.Format(CultureInfo.InvariantCulture, "http://127.0.0.1:{0}", new object[] { RegistryManager.Instance.Guest[this.mVmName].BstAndroidPort });
			}
			this.IsHostConnected = this.EnableADB();
		}

		private static bool CheckIfGuestCommandSuccess(string res)
		{
			string text = JObject.Parse(res)["result"].ToString().Trim();
			if (string.Equals(text, "ok", StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			Logger.Error("result: {0}", new object[] { text });
			return false;
		}

		public void Dispose()
		{
			this.DisableADB();
		}

		private bool EnableADB()
		{
			string text = "connectHost";
			return this.HitGuestAPI(text);
		}

		private bool DisableADB()
		{
			string text = "disconnectHost";
			return this.HitGuestAPI(text);
		}

		private bool HitGuestAPI(string api)
		{
			try
			{
				return AdbCommandRunner.CheckIfGuestCommandSuccess(HTTPUtils.SendRequestToGuest(api, null, this.mVmName, 0, null, false, 1, 0, "bgp64"));
			}
			catch (Exception ex)
			{
				Logger.Error("Error in Sending request {0} to guest {1}", new object[]
				{
					api,
					ex.ToString()
				});
			}
			return false;
		}

		public AdbCommandRunner.OutputLineHandlerDelegate OutputLineHandler { get; set; }

		public bool Connect(string vmName)
		{
			this.Port = RegistryManager.Instance.Guest[vmName].BstAdbPort;
			this.Path = global::System.IO.Path.Combine(RegistryStrings.InstallDir, "HD-Adb.exe");
			if (!this.RunInternal(string.Format(CultureInfo.InvariantCulture, "connect 127.0.0.1:{0}", new object[] { this.Port }), true))
			{
				return false;
			}
			this.RunInternal("devices", true);
			return true;
		}

		private bool RunInternal(string cmd, bool retry = true)
		{
			Logger.Info("ADB CMD: " + cmd);
			bool flag;
			using (Process process = new Process())
			{
				process.StartInfo.FileName = this.Path;
				process.StartInfo.Arguments = cmd;
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.RedirectStandardError = true;
				process.StartInfo.CreateNoWindow = true;
				process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs evt)
				{
					Logger.Info("ADB OUT: " + evt.Data);
				};
				process.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs evt)
				{
					if (!string.IsNullOrEmpty(evt.Data))
					{
						Logger.Info("ERR: " + evt.Data);
					}
				};
				process.Start();
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit();
				int num = process.ExitCode;
				Logger.Info("ADB EXIT: " + num.ToString());
				if (num != 0 && retry)
				{
					Thread.Sleep(4000);
					num = (this.RunInternal(cmd, false) ? 0 : 1);
				}
				flag = num == 0;
			}
			return flag;
		}

		public bool Push(string localPath, string remotePath)
		{
			Logger.Info("Pushing {0} to {1}", new object[] { localPath, remotePath });
			return this.RunInternal(string.Format(CultureInfo.InvariantCulture, "-s 127.0.0.1:{0} push \"{1}\" \"{2}\"", new object[] { this.Port, localPath, remotePath }), true);
		}

		public bool Pull(string filePath, string destPath)
		{
			Logger.Info("Pull file {0} in {1}", new object[] { filePath, destPath });
			return this.RunInternal(string.Format(CultureInfo.InvariantCulture, "-s 127.0.0.1:{0} pull \"{1}\" \"{2}\"", new object[] { this.Port, filePath, destPath }), true);
		}

		public bool RunShell(string fmt, params object[] args)
		{
			return this.RunShell(string.Format(CultureInfo.InvariantCulture, fmt, args));
		}

		public bool RunShell(string cmd)
		{
			Logger.Info("RunShell: " + cmd);
			return this.RunInternal(string.Format(CultureInfo.InvariantCulture, "-s 127.0.0.1:{0} shell {1}", new object[] { this.Port, cmd }), true);
		}

		public bool RunShellScript(string[] cmdList)
		{
			if (cmdList != null)
			{
				foreach (string text in cmdList)
				{
					if (!this.RunShell(text))
					{
						return false;
					}
				}
			}
			return true;
		}

		public bool RunShellPrivileged(string fmt, params object[] cmd)
		{
			return this.RunShellPrivileged(string.Format(CultureInfo.InvariantCulture, fmt, cmd));
		}

		public bool RunShellPrivileged(string cmd)
		{
			Logger.Info("RunShellPrivileged: " + cmd);
			return this.RunInternal(string.Format(CultureInfo.InvariantCulture, "-s 127.0.0.1:{0} shell {1} -c {2}", new object[] { this.Port, "/system/xbin/bstk/su", cmd }), true);
		}

		public bool RunShellScriptPrivileged(string[] cmdList)
		{
			if (cmdList != null)
			{
				foreach (string text in cmdList)
				{
					if (!this.RunShellPrivileged(text))
					{
						return false;
					}
				}
			}
			return true;
		}

		private const string SU_PATH = "/system/xbin/bstk/su";

		private static string GUEST_URL = string.Format(CultureInfo.InvariantCulture, "http://127.0.0.1:{0}", new object[] { RegistryManager.Instance.Guest["Android"].BstAndroidPort });

		private string mVmName = "Android";

		public delegate void OutputLineHandlerDelegate(string line);
	}
}


