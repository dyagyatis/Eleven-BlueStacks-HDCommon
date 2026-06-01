using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using Microsoft.Win32;

namespace BlueStacks.Common
{
	public static class ServiceManager
	{
		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr OpenSCManager(string lpMachineName, string lpDatabaseName, ServiceManager.ServiceManagerRights dwDesiredAccess);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, ServiceManager.ServiceRights dwDesiredAccess);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CreateService(IntPtr hSCManager, string lpServiceName, string lpDisplayName, ServiceManager.ServiceRights dwDesiredAccess, int dwServiceType, ServiceManager.ServiceBootFlag dwStartType, ServiceManager.ServiceError dwErrorControl, string lpBinaryPathName, string lpLoadOrderGroup, IntPtr lpdwTagId, string lpDependencies, string lp, string lpPassword);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int CloseServiceHandle(IntPtr hSCObject);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int QueryServiceStatus(IntPtr hService, ServiceManager.SERVICE_STATUS lpServiceStatus);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int DeleteService(IntPtr hService);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int ControlService(IntPtr hService, ServiceManager.ServiceControl dwControl, ServiceManager.SERVICE_STATUS lpServiceStatus);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern int StartService(IntPtr hService, int dwNumServiceArgs, int lpServiceArgVectors);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool QueryServiceObjectSecurity(SafeHandle serviceHandle, SecurityInfos secInfo, byte[] lpSecDesrBuf, uint bufSize, out uint bufSizeNeeded);

		[DllImport("advapi32.dll", SetLastError = true)]
		private static extern bool SetServiceObjectSecurity(SafeHandle serviceHandle, SecurityInfos secInfos, byte[] lpSecDesrBuf);

		public static bool UninstallService(string serviceName, bool isKernelDriverService = false)
		{
			Logger.Info("{0} {1}", new object[]
			{
				MethodBase.GetCurrentMethod().Name,
				serviceName
			});
			IntPtr intPtr = ServiceManager.OpenSCManager(ServiceManager.ServiceManagerRights.Connect);
			bool flag = true;
			try
			{
				IntPtr intPtr2 = ServiceManager.OpenService(intPtr, serviceName, (ServiceManager.ServiceRights)983076);
				if (intPtr2 == IntPtr.Zero)
				{
					Logger.Info("Service " + serviceName + " is not installed or inaccessible.");
					return true;
				}
				try
				{
					ServiceManager.StopService(intPtr2, isKernelDriverService);
					if (ServiceManager.DeleteService(intPtr2) == 0)
					{
						throw new Exception("Could not delete service " + Marshal.GetLastWin32Error().ToString());
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Failed to uninstall service... Err : " + ex.ToString());
					flag = false;
				}
				finally
				{
					ServiceManager.CloseServiceHandle(intPtr2);
				}
			}
			finally
			{
				ServiceManager.CloseServiceHandle(intPtr);
			}
			return flag;
		}

		public static bool ServiceIsInstalled(string serviceName)
		{
			IntPtr intPtr = ServiceManager.OpenSCManager(ServiceManager.ServiceManagerRights.Connect);
			bool flag;
			try
			{
				IntPtr intPtr2 = ServiceManager.OpenService(intPtr, serviceName, ServiceManager.ServiceRights.QueryStatus);
				if (intPtr2 == IntPtr.Zero)
				{
					flag = false;
				}
				else
				{
					ServiceManager.CloseServiceHandle(intPtr2);
					flag = true;
				}
			}
			finally
			{
				ServiceManager.CloseServiceHandle(intPtr);
			}
			return flag;
		}

		public static void InstallKernelDriver(string serviceName, string displayName, string fileName)
		{
			Logger.Info("{0} {1}", new object[]
			{
				MethodBase.GetCurrentMethod().Name,
				serviceName
			});
			IntPtr intPtr = ServiceManager.OpenSCManager(ServiceManager.ServiceManagerRights.AllAccess);
			try
			{
				if (ServiceManager.OpenService(intPtr, serviceName, ServiceManager.ServiceRights.QueryStatus | ServiceManager.ServiceRights.Start) != IntPtr.Zero)
				{
					Logger.Info("service is already installed...uninstalling it first");
					ServiceManager.UninstallService(serviceName, true);
				}
				IntPtr intPtr2 = ServiceManager.CreateService(intPtr, serviceName, displayName, ServiceManager.ServiceRights.AllAccess, 1, ServiceManager.ServiceBootFlag.AutoStart, ServiceManager.ServiceError.Normal, fileName, null, IntPtr.Zero, null, null, null);
				int lastWin32Error = Marshal.GetLastWin32Error();
				if (intPtr2 == IntPtr.Zero)
				{
					Logger.Info("Error in creating kernel driver service...last win32 error = " + lastWin32Error.ToString());
					throw new Exception("Failed to create service.");
				}
				Logger.Info("Successfully created service = " + serviceName + "...setting DACL now");
				ServiceManager.SetServicePermissions(serviceName);
				Logger.Info("Successfully set DACL");
			}
			catch (Exception ex)
			{
				ServiceManager.CloseServiceHandle(intPtr);
				Logger.Error("Failed to install kernel driver... Err : " + ex.ToString());
				throw new Exception(ex.Message);
			}
		}

		public static void Install(string serviceName, string displayName, string fileName)
		{
			Logger.Info("{0} {1}", new object[]
			{
				MethodBase.GetCurrentMethod().Name,
				serviceName
			});
			IntPtr intPtr = ServiceManager.OpenSCManager(ServiceManager.ServiceManagerRights.AllAccess);
			try
			{
				if (ServiceManager.OpenService(intPtr, serviceName, ServiceManager.ServiceRights.QueryStatus | ServiceManager.ServiceRights.Start) != IntPtr.Zero)
				{
					Logger.Info("service is already installed...uninstalling it first");
					ServiceManager.UninstallService(serviceName, false);
				}
				IntPtr intPtr2 = ServiceManager.CreateService(intPtr, serviceName, displayName, ServiceManager.ServiceRights.AllAccess, 16, ServiceManager.ServiceBootFlag.AutoStart, ServiceManager.ServiceError.Normal, fileName, null, IntPtr.Zero, null, null, null);
				Logger.Info("Successfully created service = " + serviceName + "...setting DACL now");
				ServiceManager.SetServicePermissions(serviceName);
				Logger.Info("Successfully set DACL");
				if (intPtr2 == IntPtr.Zero)
				{
					throw new Exception("Failed to install service.");
				}
			}
			catch (Exception ex)
			{
				ServiceManager.CloseServiceHandle(intPtr);
				Logger.Error("Failed to install kernel driver... Err : " + ex.ToString());
				throw new Exception(ex.Message);
			}
		}

		public static void StartService(string name, bool isKernelDriverService = false)
		{
			IntPtr intPtr = ServiceManager.OpenSCManager(ServiceManager.ServiceManagerRights.Connect);
			try
			{
				IntPtr intPtr2 = ServiceManager.OpenService(intPtr, name, ServiceManager.ServiceRights.QueryStatus | ServiceManager.ServiceRights.Start);
				if (intPtr2 == IntPtr.Zero)
				{
					throw new Exception("Could not open service.");
				}
				try
				{
					ServiceManager.StartService(intPtr2, isKernelDriverService);
				}
				finally
				{
					ServiceManager.CloseServiceHandle(intPtr2);
				}
			}
			finally
			{
				ServiceManager.CloseServiceHandle(intPtr);
			}
		}

		public static void StopService(string name, bool isKernelDriverService = false)
		{
			Logger.Info("{0} {1}", new object[]
			{
				MethodBase.GetCurrentMethod().Name,
				name
			});
			IntPtr intPtr = ServiceManager.OpenSCManager(ServiceManager.ServiceManagerRights.Connect);
			try
			{
				IntPtr intPtr2 = ServiceManager.OpenService(intPtr, name, ServiceManager.ServiceRights.QueryStatus | ServiceManager.ServiceRights.Stop);
				if (!(intPtr2 == IntPtr.Zero))
				{
					try
					{
						ServiceManager.StopService(intPtr2, isKernelDriverService);
					}
					finally
					{
						ServiceManager.CloseServiceHandle(intPtr2);
					}
				}
			}
			finally
			{
				ServiceManager.CloseServiceHandle(intPtr);
			}
		}

		private static void StartService(IntPtr hService, bool isKernelDriverService = false)
		{
			Logger.Info("{0} {1}", new object[]
			{
				MethodBase.GetCurrentMethod().Name,
				Convert.ToString(hService, CultureInfo.InvariantCulture)
			});
			int num = ServiceManager.StartService(hService, 0, 0);
			if (num == 0)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				Logger.Warning("Error in starting service, StartService ret: {0}, Last win32 error: {1}", new object[] { num, lastWin32Error });
			}
			ServiceManager.SERVICE_STATUS service_STATUS = new ServiceManager.SERVICE_STATUS(isKernelDriverService);
			ServiceManager.WaitForServiceStatus(hService, ServiceManager.ServiceState.StartPending, ServiceManager.ServiceState.Running, service_STATUS);
		}

		private static void StopService(IntPtr hService, bool isKernelDriverService = false)
		{
			Logger.Info("{0} {1}", new object[]
			{
				MethodBase.GetCurrentMethod().Name,
				Convert.ToString(hService, CultureInfo.InvariantCulture)
			});
			ServiceManager.SERVICE_STATUS service_STATUS = new ServiceManager.SERVICE_STATUS(isKernelDriverService);
			ServiceManager.ControlService(hService, ServiceManager.ServiceControl.Stop, service_STATUS);
			ServiceManager.WaitForServiceStatus(hService, ServiceManager.ServiceState.StopPending, ServiceManager.ServiceState.Stopped, service_STATUS);
		}

		private static bool WaitForServiceStatus(IntPtr hService, ServiceManager.ServiceState waitStatus, ServiceManager.ServiceState desiredStatus, ServiceManager.SERVICE_STATUS ssStatus)
		{
			ServiceManager.QueryServiceStatus(hService, ssStatus);
			if (ssStatus.dwCurrentState == desiredStatus)
			{
				return true;
			}
			int num = Environment.TickCount;
			int num2 = ssStatus.dwCheckPoint;
			while (ssStatus.dwCurrentState == waitStatus)
			{
				int num3 = ssStatus.dwWaitHint / 10;
				if (num3 < 1000)
				{
					num3 = 1000;
				}
				else if (num3 > 10000)
				{
					num3 = 10000;
				}
				Thread.Sleep(num3);
				if (ServiceManager.QueryServiceStatus(hService, ssStatus) == 0)
				{
					break;
				}
				if (ssStatus.dwCheckPoint > num2)
				{
					num = Environment.TickCount;
					num2 = ssStatus.dwCheckPoint;
				}
				else if (Environment.TickCount - num > ssStatus.dwWaitHint)
				{
					break;
				}
			}
			return ssStatus.dwCurrentState == desiredStatus;
		}

		private static IntPtr OpenSCManager(ServiceManager.ServiceManagerRights rights)
		{
			Logger.Info("{0}", new object[] { MethodBase.GetCurrentMethod().Name });
			IntPtr intPtr = ServiceManager.OpenSCManager(null, null, rights);
			if (intPtr == IntPtr.Zero)
			{
				throw new Exception("Could not connect to service control manager.");
			}
			return intPtr;
		}

		public static void SetServicePermissions(string serviceName)
		{
			Logger.Info("{0} {1}", new object[]
			{
				MethodBase.GetCurrentMethod().Name,
				serviceName
			});
			using (ServiceController serviceController = new ServiceController(serviceName, "."))
			{
				ServiceControllerStatus status = serviceController.Status;
				byte[] array = new byte[0];
				uint num;
				bool flag = ServiceManager.QueryServiceObjectSecurity(serviceController.ServiceHandle, SecurityInfos.DiscretionaryAcl, array, 0U, out num);
				if (!flag)
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error != 122 && lastWin32Error != 0)
					{
						throw new Exception("error calling QueryServiceObjectSecurity() to get DACL : error code=" + lastWin32Error.ToString());
					}
					array = new byte[num];
					flag = ServiceManager.QueryServiceObjectSecurity(serviceController.ServiceHandle, SecurityInfos.DiscretionaryAcl, array, num, out num);
				}
				if (!flag)
				{
					throw new Exception("error calling QueryServiceObjectSecurity(2) to get DACL : error code=" + Marshal.GetLastWin32Error().ToString());
				}
				RawSecurityDescriptor rawSecurityDescriptor = new RawSecurityDescriptor(array, 0);
				RawAcl discretionaryAcl = rawSecurityDescriptor.DiscretionaryAcl;
				DiscretionaryAcl discretionaryAcl2 = new DiscretionaryAcl(false, false, discretionaryAcl);
				SecurityIdentifier securityIdentifier = new SecurityIdentifier(WellKnownSidType.InteractiveSid, null);
				discretionaryAcl2.AddAccess(AccessControlType.Allow, securityIdentifier, 983551, InheritanceFlags.None, PropagationFlags.None);
				byte[] array2 = new byte[discretionaryAcl2.BinaryLength];
				discretionaryAcl2.GetBinaryForm(array2, 0);
				rawSecurityDescriptor.DiscretionaryAcl = new RawAcl(array2, 0);
				byte[] array3 = new byte[rawSecurityDescriptor.BinaryLength];
				rawSecurityDescriptor.GetBinaryForm(array3, 0);
				if (!ServiceManager.SetServiceObjectSecurity(serviceController.ServiceHandle, SecurityInfos.DiscretionaryAcl, array3))
				{
					throw new Exception("error calling SetServiceObjectSecurity(); error code=" + Marshal.GetLastWin32Error().ToString());
				}
			}
		}

		public static int InstallBstkDrv(string installDir, string driverName = "")
		{
			Logger.Info("InstallService start");
			try
			{
				int num = ServiceManager.InstallPlusDriver(installDir, driverName);
				if (num != 0)
				{
					return num;
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in installing BstkDrv, Err: " + ex.Message);
			}
			return 0;
		}

		private static int CheckStatusAndReturnResult(int result)
		{
			try
			{
				Logger.Info("Install failed due to: {0}", new object[] { (InstallerCodes)result });
			}
			catch
			{
			}
			return result;
		}

		private static int InstallPlusDriver(string installDir, string driverName = "")
		{
			Logger.Info("Installing driver");
			ServiceController[] devices = ServiceController.GetDevices();
			string text = Strings.BlueStacksDriverName;
			if (!string.IsNullOrEmpty(driverName))
			{
				text = driverName;
			}
			string blueStacksDriverDisplayName = Strings.BlueStacksDriverDisplayName;
			string text2 = Path.Combine(installDir, Strings.BlueStacksDriverFileName);
			Logger.Info("Registering driver with params: file path : {0}, DriverName {1}, DisplayName: {2}", new object[] { text2, text, blueStacksDriverDisplayName });
			if (ServiceManager.IsServiceAlreadyExists(devices, text) && !ServiceManager.GetImagePathOfService(text).Equals(text2, StringComparison.InvariantCultureIgnoreCase))
			{
				Logger.Info("Image path of driver is not same");
				if (ServiceManager.QueryDriverStatus(text, true) != 1)
				{
					return -59;
				}
				string text3 = "sc.exe delete " + text;
				using (Process process = new Process())
				{
					process.StartInfo.FileName = "cmd.exe";
					process.StartInfo.Arguments = "/c \"" + text3 + "\"";
					Logger.Info("Calling {0} {1}", new object[]
					{
						process.StartInfo.FileName,
						process.StartInfo.Arguments
					});
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.CreateNoWindow = true;
					process.Start();
					process.WaitForExit();
					if (process.ExitCode != 0)
					{
						return -58;
					}
				}
				if (ServiceManager.CheckForBlueStacksServicesMarkForDeletion(new List<string> { Strings.BlueStacksDriverName }) != 0)
				{
					return -58;
				}
			}
			IL_0153:
			if (!ServiceManager.InstallDriver(text, text2, blueStacksDriverDisplayName))
			{
				Logger.Error("Failed to install driver");
				return -40;
			}
			Logger.Info("Successfully Installed Driver");
			return 0;
		}

		public static int QueryDriverStatus(string name, bool isKernelDriverService = false)
		{
			Logger.Info("{0} {1}", new object[]
			{
				MethodBase.GetCurrentMethod().Name,
				name
			});
			IntPtr intPtr = ServiceManager.OpenSCManager(ServiceManager.ServiceManagerRights.Connect);
			IntPtr intPtr2 = IntPtr.Zero;
			int num;
			try
			{
				intPtr2 = ServiceManager.OpenService(intPtr, name, ServiceManager.ServiceRights.QueryStatus | ServiceManager.ServiceRights.Stop);
				if (intPtr2 == IntPtr.Zero)
				{
					Logger.Info("service handle not created");
					num = -1;
				}
				else
				{
					ServiceManager.SERVICE_STATUS service_STATUS = new ServiceManager.SERVICE_STATUS(isKernelDriverService);
					if (ServiceManager.QueryServiceStatus(intPtr2, service_STATUS) != 0)
					{
						Logger.Info("current service state is: {0} for service: {1}", new object[] { service_STATUS.dwCurrentState, name });
						num = (int)service_STATUS.dwCurrentState;
					}
					else
					{
						Logger.Info("Error in getting service status.." + Marshal.GetLastWin32Error().ToString());
						num = -1;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error in querying driver status err: " + ex.ToString());
				num = -1;
			}
			finally
			{
				ServiceManager.CloseServiceHandle(intPtr2);
				ServiceManager.CloseServiceHandle(intPtr);
			}
			return num;
		}

		private static bool InstallDriver(string driverName, string driverPath, string driverDisplayName)
		{
			try
			{
				ServiceManager.StopService(driverName, false);
				ServiceManager.UninstallService(driverName, true);
			}
			catch (Exception ex)
			{
				Logger.Info("Ignore Error, when stopping and uninstalling driver ex : {0}", new object[] { ex.ToString() });
			}
			bool flag;
			try
			{
				ServiceManager.InstallKernelDriver(driverName, driverDisplayName, driverPath);
				flag = true;
			}
			catch (Exception ex2)
			{
				Logger.Error(string.Format(CultureInfo.InvariantCulture, "Error Occured, Err: {0}", new object[] { ex2.ToString() }));
				flag = false;
			}
			return flag;
		}

		private static string GetImagePathOfService(string serviceName)
		{
			RegistryKey registryKey = null;
			string text3;
			try
			{
				Logger.Info("In GetImagePathOfService {0}", new object[] { serviceName });
				string text = Path.Combine("System\\CurrentControlSet\\Services", serviceName);
				registryKey = Registry.LocalMachine.OpenSubKey(text);
				string text2 = (string)registryKey.GetValue("ImagePath");
				registryKey.Close();
				text3 = text2;
			}
			catch (Exception ex)
			{
				Logger.Error("Could not get the image path for service {0}, ex: {1}", new object[]
				{
					serviceName,
					ex.ToString()
				});
				text3 = null;
			}
			finally
			{
				if (registryKey != null)
				{
					registryKey.Close();
				}
			}
			return text3;
		}

		private static bool CheckIfInstalledServicePathAndInstallDirPathMatch(string serviceName, string installDir)
		{
			Logger.Info("Checking file path for {0}", new object[] { serviceName });
			string imagePathOfService = ServiceManager.GetImagePathOfService(serviceName);
			if (string.IsNullOrEmpty(imagePathOfService))
			{
				Logger.Error("The code checking image path of service returned null");
				return false;
			}
			string installDirOfService = ServiceManager.GetInstallDirOfService(imagePathOfService);
			if (installDirOfService != installDir)
			{
				Logger.Error("Service {0} is already installed but at incorrect path {1}, required path is {2}", new object[] { serviceName, installDirOfService, installDir });
				return false;
			}
			return true;
		}

		private static string GetInstallDirOfService(string servicePath)
		{
			int num = servicePath.IndexOf(".sys", StringComparison.OrdinalIgnoreCase);
			if (num != -1)
			{
				return Path.GetDirectoryName(servicePath.Substring(4, num));
			}
			num = servicePath.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
			if (num == -1)
			{
				return null;
			}
			return Path.GetDirectoryName(servicePath.Substring(0, num + 4).Replace("\"", ""));
		}

		private static bool IsServiceAlreadyExists(ServiceController[] services, string serviceName)
		{
			Logger.Info("Checking if service {0} exists on user's machine", new object[] { serviceName });
			try
			{
				for (int i = 0; i < services.Length; i++)
				{
					if (services[i].ServiceName.Equals(serviceName, StringComparison.OrdinalIgnoreCase))
					{
						Logger.Info("Found service: " + serviceName);
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Warning("Exception in checking if service {0} is installed ex: {1}", new object[]
				{
					serviceName,
					ex.ToString()
				});
			}
			return false;
		}

		public static int CheckForBlueStacksServicesMarkForDeletion(List<string> servicesName)
		{
			if (servicesName != null)
			{
				using (List<string>.Enumerator enumerator = servicesName.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						if (ServiceManager.CheckIfServiceHasBeenMarkedForDeletion(enumerator.Current))
						{
							return -30;
						}
					}
				}
				return 0;
			}
			return 0;
		}

		private static bool CheckIfServiceHasBeenMarkedForDeletion(string serviceName)
		{
			try
			{
				Logger.Info("checking for marked for deletion flag in service {0}", new object[] { serviceName });
				string text = Path.Combine("system\\CurrentControlSet\\services", serviceName);
				int i = 10;
				while (i > 0)
				{
					int num = (int)Registry.LocalMachine.OpenSubKey(text).GetValue("DeleteFlag");
					i--;
					Logger.Info("delete flag : " + num.ToString() + " and retry number = " + (10 - i).ToString());
					if (num != 1)
					{
						break;
					}
					if (i == 0)
					{
						Logger.Warning("the  service {0} has been marked for deletion.", new object[] { serviceName });
						return true;
					}
					Thread.Sleep(1000);
				}
			}
			catch (Exception)
			{
				Logger.Info("Could not check for service marked for deletion. should be safe to ignore in most cases.");
			}
			return false;
		}

		private const int SERVICE_WIN32_OWN_PROCESS = 16;

		private const int SERVICE_KERNEL_DRIVER = 1;

		[Flags]
		public enum ServiceManagerRights
		{
			Connect = 1,
			CreateService = 2,
			EnumerateService = 4,
			Lock = 8,
			QueryLockStatus = 16,
			ModifyBootConfig = 32,
			StandardRightsRequired = 983040,
			AllAccess = 983103
		}

		[Flags]
		public enum ServiceRights
		{
			QueryConfig = 1,
			ChangeConfig = 2,
			QueryStatus = 4,
			EnumerateDependants = 8,
			Start = 16,
			Stop = 32,
			PauseContinue = 64,
			Interrogate = 128,
			UserDefinedControl = 256,
			Delete = 65536,
			StandardRightsRequired = 983040,
			ReadControl = 131072,
			AllAccess = 983551
		}

		public enum ServiceBootFlag
		{
			BootStart,
			SystemStart,
			AutoStart,
			DemandStart,
			Disabled
		}

		public enum ServiceControl
		{
			Stop = 1,
			Pause,
			Continue,
			Interrogate,
			Shutdown,
			ParamChange,
			NetBindAdd,
			NetBindRemove,
			NetBindEnable,
			NetBindDisable
		}

		public enum ServiceError
		{
			Ignore,
			Normal,
			Severe,
			Critical
		}

		public enum ServiceState
		{
			Unknown = -1,
			NotFound,
			Stopped,
			StartPending,
			StopPending,
			Running,
			ContinuePending,
			PausePending,
			Paused
		}

		[StructLayout(LayoutKind.Sequential)]
		private class SERVICE_STATUS
		{
			public SERVICE_STATUS(bool isKernelDriver)
			{
				if (isKernelDriver)
				{
					this.dwServiceType = 1;
				}
			}

			public int dwServiceType = 16;

			public ServiceManager.ServiceState dwCurrentState;

			public int dwControlsAccepted;

			public int dwWin32ExitCode;

			public int dwServiceSpecificExitCode;

			public int dwCheckPoint;

			public int dwWaitHint;
		}
	}
}


