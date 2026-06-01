using System;
using System.Diagnostics;
using System.IO;

namespace BlueStacks.Common
{
	public static class BlueStacksGL
	{
		public static int GLCheckInstallation(GLRenderer rendererToCheck, GLMode modeToCheck, out string glVendor, out string glRenderer, out string glVersion)
		{
			Logger.Info("Checking for GLRenderer: {0}, GLMode: {1}", new object[] { rendererToCheck, modeToCheck });
			string text = "";
			string text2 = "";
			string text3 = "";
			glVendor = text;
			glRenderer = text2;
			glVersion = text3;
			return BlueStacksGL.Run(BlueStacksGL.BinaryPath, BlueStacksGL.GetArgs(rendererToCheck, modeToCheck), true, true, out glVendor, out glRenderer, out glVersion, 10000);
		}

		private static string GetArgs(GLRenderer rendererToCheck, GLMode modeToCheck)
		{
			return string.Format("{0} {1}", (int)rendererToCheck, (int)modeToCheck);
		}

		private static int Run(string prog, string args, bool logOutput, bool getGLValues, out string glVendor, out string glRenderer, out string glVersion, int timeout = 10000)
		{
			int num = -1;
			string vendor = "";
			string renderer = "";
			string version = "";
			glVendor = vendor;
			glRenderer = renderer;
			glVersion = version;
			try
			{
				using (Process process = new Process())
				{
					process.StartInfo.FileName = prog;
					process.StartInfo.Arguments = args;
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.CreateNoWindow = true;
					if (getGLValues | logOutput)
					{
						if (logOutput && !getGLValues)
						{
							process.StartInfo.RedirectStandardOutput = true;
							process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs outLine)
							{
								try
								{
									string text = ((outLine.Data != null) ? outLine.Data : "");
									if (logOutput)
									{
										Logger.Info("OUT: " + text);
									}
								}
								catch (Exception ex2)
								{
									Logger.Error("A crash occured in the GLCheck delegate");
									Logger.Error(ex2.ToString());
								}
							};
						}
						else
						{
							process.StartInfo.RedirectStandardOutput = true;
							process.OutputDataReceived += delegate(object sender, DataReceivedEventArgs outLine)
							{
								try
								{
									string text2 = ((outLine.Data != null) ? outLine.Data : "");
									if (logOutput)
									{
										Logger.Info("OUT: " + text2);
									}
									if (text2.Contains("GL_VENDOR ="))
									{
										int num2 = text2.IndexOf('=');
										vendor = text2.Substring(num2 + 1).Trim();
										vendor = vendor.Replace(";", ";;");
									}
									if (text2.Contains("GL_RENDERER ="))
									{
										int num2 = text2.IndexOf('=');
										renderer = text2.Substring(num2 + 1).Trim();
										renderer = renderer.Replace(";", ";;");
									}
									if (text2.Contains("GL_VERSION ="))
									{
										int num2 = text2.IndexOf('=');
										version = text2.Substring(num2 + 1).Trim();
										version = version.Replace(";", ";;");
									}
								}
								catch (Exception ex3)
								{
									Logger.Error("A crash occured in the GLCheck delegate");
									Logger.Error(ex3.ToString());
								}
							};
						}
					}
					Logger.Info("{0}: {1}", new object[] { prog, args });
					process.Start();
					if (getGLValues | logOutput)
					{
						process.BeginOutputReadLine();
					}
					bool flag = process.WaitForExit(timeout);
					glVendor = vendor;
					glRenderer = renderer;
					glVersion = version;
					if (flag)
					{
						Logger.Info(process.Id.ToString() + " EXIT: " + process.ExitCode.ToString());
						num = process.ExitCode;
					}
					else
					{
						Logger.Fatal("Process {0} killed after timeout: {1}s", new object[]
						{
							process.StartInfo.FileName,
							timeout / 1000
						});
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Some error while running GLCheck. Ex: {0}", new object[] { ex });
			}
			return num;
		}

		private static string BinaryName = "HD-GLCheck.exe";

		private static string BinaryPath = Path.Combine(Directory.GetCurrentDirectory(), BlueStacksGL.BinaryName);
	}
}


