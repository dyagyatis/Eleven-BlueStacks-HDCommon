using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public static class HTTPUtils
	{
		public static string MultiInstanceServerUrl
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
				{
					"http://127.0.0.1",
					RegistryManager.Instance.MultiInstanceServerPort
				});
			}
		}

		public static string PartnerServerUrl
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
				{
					"http://127.0.0.1",
					RegistryManager.Instance.PartnerServerPort
				});
			}
		}

		public static string AgentServerUrl(string oem = "bgp64")
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				"http://127.0.0.1",
				RegistryManager.RegistryManagers[oem].AgentServerPort
			});
		}

		public static string FrontendServerUrl(string vmName = "Android")
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				"http://127.0.0.1",
				RegistryManager.Instance.Guest[vmName].FrontendServerPort
			});
		}

		public static string GuestServerUrl(string vmName = "Android", string oem = "bgp64")
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				"http://127.0.0.1",
				RegistryManager.RegistryManagers[oem].Guest[vmName].BstAndroidPort
			});
		}

		public static string BTvServerUrl
		{
			get
			{
				return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
				{
					"http://127.0.0.1",
					RegistryManager.Instance.BTVServerPort
				});
			}
		}

		public static string UrlForBstCommandProcessor(string url)
		{
			try
			{
				Uri uri = new Uri(url);
				foreach (string text in RegistryManager.Instance.VmList)
				{
					bool flag = uri.Segments.Length > 1 && string.Compare("ping", uri.Segments[1], StringComparison.OrdinalIgnoreCase) != 0 && uri.Port == RegistryManager.Instance.Guest[text].BstAndroidPort;
					if (flag)
					{
						return text;
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error Occured, Err: {0}", new object[] { ex.ToString() });
			}
			return null;
		}

		public static NameValueCollection GetRequestHeaderCollection(string vmName)
		{
			NameValueCollection nameValueCollection = new NameValueCollection
			{
				{
					"x_oem",
					RegistryManager.Instance.Oem
				},
				{
					"x_email",
					RegistryManager.Instance.RegisteredEmail
				},
				{
					"x_machine_id",
					GuidUtils.GetBlueStacksMachineId()
				},
				{
					"x_version_machine_id",
					GuidUtils.GetBlueStacksVersionId()
				}
			};
			bool flag = !string.IsNullOrEmpty(vmName);
			if (flag)
			{
				bool flag2 = vmName.Contains("Android");
				if (flag2)
				{
					nameValueCollection.Add("vmname", vmName);
					nameValueCollection.Add("x_google_aid", Utils.GetGoogleAdIdfromRegistry(vmName));
					nameValueCollection.Add("x_android_id", Utils.GetAndroidIdfromRegistry(vmName));
					bool flag3 = string.Equals(vmName, "Android", StringComparison.InvariantCultureIgnoreCase);
					if (flag3)
					{
						nameValueCollection.Add("vmid", "0");
					}
					else
					{
						nameValueCollection.Add("vmid", vmName.Split(new char[] { '_' })[1]);
					}
				}
				else
				{
					nameValueCollection.Add("vmid", vmName);
					bool flag4 = vmName.Equals("0", StringComparison.InvariantCultureIgnoreCase);
					if (flag4)
					{
						nameValueCollection.Add("vmname", "Android");
						nameValueCollection.Add("x_google_aid", Utils.GetGoogleAdIdfromRegistry("Android"));
						nameValueCollection.Add("x_android_id", Utils.GetAndroidIdfromRegistry("Android"));
					}
					else
					{
						nameValueCollection.Add("vmname", "Android_" + vmName);
						nameValueCollection.Add("x_google_aid", Utils.GetGoogleAdIdfromRegistry("Android_" + vmName));
						nameValueCollection.Add("x_android_id", Utils.GetAndroidIdfromRegistry("Android_" + vmName));
					}
				}
			}
			return nameValueCollection;
		}

		public static RequestData ParseRequest(HttpListenerRequest req)
		{
			return HTTPUtils.ParseRequest(req, true);
		}

		public static RequestData ParseRequest(HttpListenerRequest req, bool printData)
		{
			RequestData requestData = new RequestData();
			bool flag = false;
			string text = null;
			requestData.Headers = ((req != null) ? req.Headers : null);
			requestData.RequestVmId = 0;
			requestData.RequestVmName = "Android";
			foreach (string text2 in requestData.Headers.AllKeys)
			{
				bool flag2 = requestData.Headers[text2].Contains("multipart");
				if (flag2)
				{
					text = "--" + requestData.Headers[text2].Substring(requestData.Headers[text2].LastIndexOf("=", StringComparison.OrdinalIgnoreCase) + 1);
					Logger.Debug("boundary: {0}", new object[] { text });
					flag = true;
				}
				bool flag3 = text2.Contains("oem", StringComparison.InvariantCultureIgnoreCase) && requestData.Headers[text2] != null;
				if (flag3)
				{
					requestData.Oem = requestData.Headers[text2];
				}
				else
				{
					bool flag4 = text2 == "vmid" && requestData.Headers[text2] != null;
					if (flag4)
					{
						bool flag5 = !requestData.Headers[text2].Equals("0", StringComparison.OrdinalIgnoreCase);
						if (flag5)
						{
							requestData.RequestVmId = int.Parse(requestData.Headers["vmid"], CultureInfo.InvariantCulture);
							bool flag6 = requestData.RequestVmName == "Android";
							if (flag6)
							{
								RequestData requestData2 = requestData;
								requestData2.RequestVmName = requestData2.RequestVmName + "_" + requestData.Headers[text2].ToString(CultureInfo.InvariantCulture);
							}
						}
					}
					else
					{
						bool flag7 = text2 == "vmname" && requestData.Headers[text2] != null;
						if (flag7)
						{
							requestData.RequestVmName = requestData.Headers[text2].ToString(CultureInfo.InvariantCulture);
						}
					}
				}
			}
			requestData.QueryString = req.QueryString;
			bool flag8 = !req.HasEntityBody;
			RequestData requestData3;
			if (flag8)
			{
				requestData3 = requestData;
			}
			else
			{
				Stream inputStream = req.InputStream;
				byte[] array = new byte[16384];
				MemoryStream memoryStream = new MemoryStream();
				int num;
				while ((num = inputStream.Read(array, 0, array.Length)) > 0)
				{
					memoryStream.Write(array, 0, num);
				}
				byte[] array2 = memoryStream.ToArray();
				memoryStream.Close();
				inputStream.Close();
				Logger.Debug("byte array size {0}", new object[] { array2.Length });
				string @string = Encoding.UTF8.GetString(array2);
				bool flag9 = !flag;
				if (flag9)
				{
					bool flag10 = !req.ContentType.Contains("application/json", StringComparison.InvariantCultureIgnoreCase);
					if (flag10)
					{
						requestData.Data = HttpUtility.ParseQueryString(@string);
					}
					else
					{
						JObject jobject = JObject.Parse(@string);
						NameValueCollection nameValueCollection = new NameValueCollection();
						foreach (string text3 in (from p in jobject.Properties()
							select p.Name).ToList<string>())
						{
							nameValueCollection.Add(text3, jobject[text3].ToString());
						}
						requestData.Data = nameValueCollection;
					}
					requestData3 = requestData;
				}
				else
				{
					byte[] bytes = Encoding.UTF8.GetBytes(text);
					List<int> list = HTTPUtils.IndexOf(array2, bytes);
					int j = 0;
					while (j < list.Count - 1)
					{
						Logger.Info("Creating part");
						int num2 = list[j];
						int num3 = list[j + 1];
						int num4 = num3 - num2;
						byte[] array3 = new byte[num4];
						Logger.Debug("Start: {0}, End: {1}, Length: {2}", new object[] { num2, num3, num4 });
						Logger.Debug("byteData length: {0}", new object[] { array2.Length });
						Buffer.BlockCopy(array2, num2, array3, 0, num4);
						Logger.Debug("bytePart length: {0}", new object[] { array3.Length });
						string string2 = Encoding.UTF8.GetString(array3);
						Match match = new Regex("(?<=Content\\-Type:)(.*?)(?=\\r\\n)").Match(string2);
						Match match2 = new Regex("(?<=filename\\=\\\")(.*?)(?=\\\")").Match(string2);
						string text4 = new Regex("(?<=name\\=\\\")(.*?)(?=\\\")").Match(string2).Value.Trim();
						Logger.Info("Got name: {0}", new object[] { text4 });
						bool flag11 = match.Success && match2.Success;
						if (flag11)
						{
							Logger.Debug("Found file");
							string text5 = match.Value.Trim();
							Logger.Debug("Got contenttype: {0}", new object[] { text5 });
							string text6 = match2.Value.Trim();
							Logger.Info("Got filename: {0}", new object[] { text6 });
							int num5 = string2.IndexOf("\r\n\r\n", StringComparison.OrdinalIgnoreCase) + "\r\n\r\n".Length;
							Encoding.UTF8.GetBytes("\r\n" + text);
							int num6 = num4 - num5;
							byte[] array4 = new byte[num6];
							Logger.Debug("startindex: {0}, contentlength: {1}", new object[] { num5, num6 });
							Buffer.BlockCopy(array3, num5, array4, 0, num6);
							string text7 = RegistryStrings.BstUserDataDir;
							bool flag12 = text6.StartsWith("tombstone", StringComparison.OrdinalIgnoreCase);
							if (flag12)
							{
								text7 = RegistryStrings.BstLogsDir;
							}
							try
							{
								string text8 = Path.Combine(text7, text6);
								FileStream fileStream = File.OpenWrite(text8);
								fileStream.Write(array4, 0, num6);
								fileStream.Close();
								requestData.Files.Add(text4, text8);
								goto IL_062E;
							}
							catch (Exception ex)
							{
								Logger.Warning("Exception in generating file: " + ex.ToString());
								goto IL_062E;
							}
							goto IL_062C;
						}
						goto IL_062C;
						IL_062E:
						j++;
						continue;
						IL_062C:
						Logger.Info("No file in this part");
						int num7 = string2.LastIndexOf("\r\n\r\n", StringComparison.OrdinalIgnoreCase);
						string text9 = string2.Substring(num7, string2.Length - num7);
						text9 = text9.Trim();
						if (printData)
						{
							Logger.Info("Got value: {0}", new object[] { text9 });
						}
						else
						{
							Logger.Info("Value hidden");
						}
						requestData.Data.Add(text4, text9);
						goto IL_062E;
					}
					requestData3 = requestData;
				}
			}
			return requestData3;
		}

		private static List<int> IndexOf(byte[] searchWithin, byte[] searchFor)
		{
			List<int> list = new List<int>();
			int num = 0;
			int num2 = Array.IndexOf<byte>(searchWithin, searchFor[0], num);
			Logger.Debug("boundary size = {0}", new object[] { searchFor.Length });
			do
			{
				int num3 = 0;
				while (num2 + num3 < searchWithin.Length && searchWithin[num2 + num3] == searchFor[num3])
				{
					num3++;
					bool flag = num3 == searchFor.Length;
					if (flag)
					{
						list.Add(num2);
						Logger.Debug("Got boundary postion: {0}", new object[] { num2 });
						break;
					}
				}
				bool flag2 = num2 + num3 > searchWithin.Length;
				if (flag2)
				{
					break;
				}
				num2 = Array.IndexOf<byte>(searchWithin, searchFor[0], num2 + num3);
			}
			while (num2 != -1);
			return list;
		}

		public static string MergeQueryParams(string urlOriginal, string urlOverideParams, bool paramsOnly = false)
		{
			NameValueCollection nameValueCollection;
			if (paramsOnly)
			{
				nameValueCollection = HttpUtility.ParseQueryString(urlOverideParams);
			}
			else
			{
				nameValueCollection = HttpUtility.ParseQueryString(new UriBuilder(urlOverideParams).Query);
			}
			UriBuilder uriBuilder = new UriBuilder(urlOriginal);
			NameValueCollection nameValueCollection2 = HttpUtility.ParseQueryString(uriBuilder.Query);
			foreach (object obj in nameValueCollection.Keys)
			{
				nameValueCollection2.Set(obj.ToString(), nameValueCollection[obj.ToString()]);
			}
			uriBuilder.Query = nameValueCollection2.ToString();
			return uriBuilder.Uri.OriginalString;
		}

		public static void Write(StringBuilder sb, HttpListenerResponse res)
		{
			HTTPUtils.Write((sb != null) ? sb.ToString() : null, res);
		}

		public static void Write(string s, HttpListenerResponse res)
		{
			try
			{
				byte[] bytes = Encoding.UTF8.GetBytes(s);
				bool flag = res != null;
				if (flag)
				{
					res.ContentLength64 = (long)bytes.Length;
					res.OutputStream.Write(bytes, 0, bytes.Length);
					res.OutputStream.Flush();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Exception in writing response to http output stream:{0}", new object[] { ex });
			}
		}

		public static HTTPServer SetupServer(int startingPort, int maxPort, Dictionary<string, HTTPServer.RequestHandler> routes, string s_RootDir)
		{
			HTTPServer httpserver = null;
			int i;
			for (i = startingPort; i < maxPort; i++)
			{
				try
				{
					httpserver = new HTTPServer(i, routes, s_RootDir);
					httpserver.Start();
					i = httpserver.Port;
					Logger.Info("Server listening on port " + httpserver.Port.ToString());
					bool flag = !string.IsNullOrEmpty(httpserver.RootDir);
					if (flag)
					{
						Logger.Info("Serving static content from " + httpserver.RootDir);
					}
					break;
				}
				catch (Exception ex)
				{
					Logger.Warning("Error occured, port: {0} Err: {1}", new object[] { i, ex });
				}
			}
			bool flag2 = i == maxPort || httpserver == null;
			if (flag2)
			{
				Logger.Fatal("No free port available or server could not be started, exiting.");
				Environment.Exit(2);
			}
			return httpserver;
		}

		public static void SendRequestToClientAsync(string route, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0, string oem = "bgp64")
		{
			bool flag = retries == 1;
			if (flag)
			{
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					try
					{
						HTTPUtils.SendRequestToClient(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, oem);
					}
					catch (Exception ex)
					{
						Logger.Error("An exception in SendRequestToClient. route: {0}, \n{1}", new object[] { route, ex });
					}
				});
			}
			else
			{
				new Thread(new ThreadStart(delegate
				{
					try
					{
						HTTPUtils.SendRequestToClient(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, "bgp64");
					}
					catch (Exception ex2)
					{
						Logger.Error("An exception in SendRequestToClient. route: {0}, \n{1}", new object[] { route, ex2 });
					}
				}))
				{
					IsBackground = true
				}.Start();
			}
		}

		public static void SendRequestToEngineAsync(string route, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0)
		{
			bool flag = retries == 1;
			if (flag)
			{
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					try
					{
						HTTPUtils.SendRequestToEngine(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, "");
					}
					catch (Exception ex)
					{
						Logger.Error("An exception in SendRequestToEngine. route: {0}, \n{1}", new object[] { route, ex });
					}
				});
			}
			else
			{
				new Thread(new ThreadStart(delegate
				{
					try
					{
						HTTPUtils.SendRequestToEngine(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, "");
					}
					catch (Exception ex2)
					{
						Logger.Error("An exception in SendRequestToEngine. route: {0}, \n{1}", new object[] { route, ex2 });
					}
				}))
				{
					IsBackground = true
				}.Start();
			}
		}

		public static void SendRequestToAgentAsync(string route, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0, string oem = "bgp64")
		{
			bool flag = retries == 1;
			if (flag)
			{
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					try
					{
						HTTPUtils.SendRequestToAgent(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, oem, true);
					}
					catch (Exception ex)
					{
						Logger.Error("An exception in SendRequestToAgent. route: {0}, \n{1}", new object[] { route, ex });
					}
				});
			}
			else
			{
				new Thread(new ThreadStart(delegate
				{
					try
					{
						HTTPUtils.SendRequestToAgent(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, "bgp64", true);
					}
					catch (Exception ex2)
					{
						Logger.Error("An exception in SendRequestToAgent. route: {0}, \n{1}", new object[] { route, ex2 });
					}
				}))
				{
					IsBackground = true
				}.Start();
			}
		}

		public static void SendRequestToGuestAsync(string route, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0)
		{
			bool flag = retries == 1;
			if (flag)
			{
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					try
					{
						HTTPUtils.SendRequestToGuest(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, "bgp64");
					}
					catch (Exception ex)
					{
						Logger.Error("An exception in SendRequestToGuest. route: {0}, \n{1}", new object[] { route, ex });
					}
				});
			}
			else
			{
				new Thread(new ThreadStart(delegate
				{
					try
					{
						HTTPUtils.SendRequestToGuest(route, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, "bgp64");
					}
					catch (Exception ex2)
					{
						Logger.Error("An exception in SendRequestToGuest. route: {0}, \n{1}", new object[] { route, ex2 });
					}
				}))
				{
					IsBackground = true
				}.Start();
			}
		}

		public static void SendRequestToCloudAsync(string api, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0)
		{
			bool flag = retries == 1;
			if (flag)
			{
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					try
					{
						HTTPUtils.SendRequestToCloud(api, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false);
					}
					catch (Exception ex)
					{
						Logger.Error("An exception in SendRequestToCloud. route: {0}, \n{1}", new object[] { api, ex });
					}
				});
			}
			else
			{
				new Thread(new ThreadStart(delegate
				{
					try
					{
						HTTPUtils.SendRequestToCloud(api, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false);
					}
					catch (Exception ex2)
					{
						Logger.Error("An exception in SendRequestToCloud. route: {0}, \n{1}", new object[] { api, ex2 });
					}
				}))
				{
					IsBackground = true
				}.Start();
			}
		}

		public static void SendRequestToCloudWithParamsAsync(string api, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0)
		{
			bool flag = retries == 1;
			if (flag)
			{
				ThreadPool.QueueUserWorkItem(delegate(object obj)
				{
					try
					{
						HTTPUtils.SendRequestToCloudWithParams(api, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec);
					}
					catch (Exception ex)
					{
						Logger.Error("An exception in SendRequestToCloudWithParams. route: {0}, \n{1}", new object[] { api, ex });
					}
				});
			}
			else
			{
				new Thread(new ThreadStart(delegate
				{
					try
					{
						HTTPUtils.SendRequestToCloudWithParams(api, data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec);
					}
					catch (Exception ex2)
					{
						Logger.Error("An exception in SendRequestToCloudWithParams. route: {0}, \n{1}", new object[] { api, ex2 });
					}
				}))
				{
					IsBackground = true
				}.Start();
			}
		}

		public static string SendRequestToMultiInstance(string route, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0)
		{
			return HTTPUtils.SendHTTPRequest(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				HTTPUtils.MultiInstanceServerUrl,
				route
			}), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, "bgp64", false);
		}

		public static string SendRequestToClient(string route, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0, string oem = "bgp64")
		{
			bool flag = oem == null;
			if (flag)
			{
				oem = "bgp64";
			}
			string text = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				"http://127.0.0.1",
				RegistryManager.RegistryManagers[oem].PartnerServerPort
			});
			return HTTPUtils.SendHTTPRequest(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[] { text, route }), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, oem, false);
		}

		public static string SendRequestToEngine(string route, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0, string destinationVmName = "")
		{
			bool flag = string.IsNullOrEmpty(destinationVmName);
			if (flag)
			{
				destinationVmName = vmName;
			}
			return HTTPUtils.SendHTTPRequest(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				HTTPUtils.FrontendServerUrl(destinationVmName),
				route
			}), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, "bgp64", false);
		}

		public static string SendRequestToAgent(string route, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0, string oem = "bgp64", bool isCheckForAgentRunning = true)
		{
			bool flag = oem == null;
			if (flag)
			{
				oem = "bgp64";
			}
			bool flag2 = isCheckForAgentRunning && !ProcessUtils.IsLockInUse("Global\\BlueStacks_HDAgent_Lock" + oem);
			if (flag2)
			{
				Process process = new Process();
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.CreateNoWindow = true;
				process.StartInfo.FileName = Path.Combine(RegistryManager.RegistryManagers[oem].InstallDir, "HD-Agent.exe");
				Logger.Info("Utils: Starting Agent");
				process.Start();
				bool flag3 = !Utils.WaitForAgentPingResponse(vmName, oem);
				if (flag3)
				{
					return null;
				}
			}
			return HTTPUtils.SendHTTPRequest(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				HTTPUtils.AgentServerUrl(oem),
				route
			}), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, oem, false);
		}

		public static string SendRequestToGuest(string route, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0, string oem = "bgp64")
		{
			return HTTPUtils.SendHTTPRequest(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				HTTPUtils.GuestServerUrl(vmName, oem),
				route
			}), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, oem, false);
		}

		public static string SendRequestToBTv(string route, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0)
		{
			return HTTPUtils.SendHTTPRequest(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				HTTPUtils.BTvServerUrl,
				route
			}), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, "bgp64", false);
		}

		public static string SendRequestToCloud(string api, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0, bool isOnUIThreadOnPurpose = false)
		{
			return HTTPUtils.SendHTTPRequest(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				RegistryManager.Instance.Host,
				api
			}), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, isOnUIThreadOnPurpose, "bgp64", false);
		}

		public static string SendRequestToCloudWithParams(string api, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0)
		{
			return HTTPUtils.SendHTTPRequest(WebHelper.GetUrlWithParams(string.Format(CultureInfo.InvariantCulture, "{0}/{1}", new object[]
			{
				RegistryManager.Instance.Host,
				api
			})), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, "bgp64", false);
		}

		public static string SendRequestToNCSoftAgent(int port, string api, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0)
		{
			return HTTPUtils.SendHTTPRequest(string.Format(CultureInfo.InvariantCulture, "{0}:{1}/{2}", new object[] { "http://127.0.0.1", port, api }), data, vmName, timeout, headers, printResponse, retries, sleepTimeMSec, false, "bgp64", false);
		}

		public static void WriteSuccessArrayJson(HttpListenerResponse res, string reason = "")
		{
			bool flag = string.IsNullOrEmpty(reason);
			if (flag)
			{
				HTTPUtils.Write(JSonTemplates.SuccessArrayJSonTemplate, res);
			}
			else
			{
				JArray jarray = new JArray();
				JObject jobject = new JObject
				{
					{ "success", true },
					{ "reason", reason }
				};
				jarray.Add(jobject);
				HTTPUtils.Write(jarray.ToString(Formatting.None, new JsonConverter[0]), res);
			}
		}

		public static void WriteErrorArrayJson(HttpListenerResponse res, string reason = "")
		{
			bool flag = string.IsNullOrEmpty(reason);
			if (flag)
			{
				HTTPUtils.Write(JSonTemplates.FailedArrayJSonTemplate, res);
			}
			else
			{
				JArray jarray = new JArray();
				JObject jobject = new JObject
				{
					{ "success", false },
					{ "reason", reason }
				};
				jarray.Add(jobject);
				HTTPUtils.Write(jarray.ToString(Formatting.None, new JsonConverter[0]), res);
			}
		}

		public static void WriteArrayJson(HttpListenerResponse res, Dictionary<string, string> data)
		{
			JArray jarray = new JArray();
			bool flag = data != null;
			if (flag)
			{
				JObject jobject = new JObject();
				foreach (KeyValuePair<string, string> keyValuePair in data)
				{
					jobject.Add(keyValuePair.Key, keyValuePair.Value);
				}
				jarray.Add(jobject);
			}
			HTTPUtils.Write(jarray.ToString(Formatting.None, new JsonConverter[0]), res);
		}

		public static void WriteArrayJson(HttpListenerResponse res, Dictionary<string, object> data)
		{
			JArray jarray = new JArray();
			bool flag = data != null;
			if (flag)
			{
				JObject jobject = new JObject();
				foreach (KeyValuePair<string, object> keyValuePair in data)
				{
					jobject.Add(keyValuePair.Key, JToken.FromObject(keyValuePair.Value));
				}
				jarray.Add(jobject);
			}
			HTTPUtils.Write(jarray.ToString(Formatting.None, new JsonConverter[0]), res);
		}

		public static void WriteSuccessJson(HttpListenerResponse res, string reason = "")
		{
			bool flag = string.IsNullOrEmpty(reason);
			if (flag)
			{
				HTTPUtils.Write(JSonTemplates.SuccessJSonTemplate, res);
			}
			else
			{
				HTTPUtils.Write(new JObject
				{
					{ "success", true },
					{ "reason", reason }
				}.ToString(Formatting.None, new JsonConverter[0]), res);
			}
		}

		public static void WriteErrorJson(HttpListenerResponse res, string reason = "")
		{
			bool flag = string.IsNullOrEmpty(reason);
			if (flag)
			{
				HTTPUtils.Write(JSonTemplates.FailedJSonTemplate, res);
			}
			else
			{
				HTTPUtils.Write(new JObject
				{
					{ "success", false },
					{ "reason", reason }
				}.ToString(Formatting.None, new JsonConverter[0]), res);
			}
		}

		private static string SendHTTPRequest(string url, Dictionary<string, string> data = null, string vmName = "Android", int timeout = 0, Dictionary<string, string> headers = null, bool printResponse = false, int retries = 1, int sleepTimeMSec = 0, bool isOnUIThreadOnPurpose = false, string oem = "bgp64", bool fireAndForget = false)
		{
			bool flag = !fireAndForget && (url.IndexOf("mouse", StringComparison.OrdinalIgnoreCase) >= 0 || url.IndexOf("cursor", StringComparison.OrdinalIgnoreCase) >= 0 || url.IndexOf("input", StringComparison.OrdinalIgnoreCase) >= 0 || url.IndexOf("touch", StringComparison.OrdinalIgnoreCase) >= 0);
			if (flag)
			{
				fireAndForget = true;
			}
			bool flag2 = fireAndForget;
			string text;
			if (flag2)
			{
				ThreadPool.QueueUserWorkItem(delegate(object state)
				{
					try
					{
						bool flag4 = data == null;
						if (flag4)
						{
							BstHttpClient.Get(url, headers, false, vmName, timeout, retries, sleepTimeMSec, isOnUIThreadOnPurpose, oem);
						}
						else
						{
							BstHttpClient.Post(url, data, headers, false, vmName, timeout, retries, sleepTimeMSec, isOnUIThreadOnPurpose, oem);
						}
					}
					catch
					{
					}
				});
				text = string.Empty;
			}
			else
			{
				bool flag3 = data == null;
				string text2;
				if (flag3)
				{
					if (printResponse)
					{
						Logger.Info("Sending GET to {0}", new object[] { url });
					}
					text2 = BstHttpClient.Get(url, headers, false, vmName, timeout, retries, sleepTimeMSec, isOnUIThreadOnPurpose, oem);
				}
				else
				{
					if (printResponse)
					{
						Logger.Info("Sending POST to {0}", new object[] { url });
					}
					text2 = BstHttpClient.Post(url, data, headers, false, vmName, timeout, retries, sleepTimeMSec, isOnUIThreadOnPurpose, oem);
				}
				if (printResponse)
				{
					Logger.Info("Loopback resp: {0}", new object[] { text2 });
				}
				else
				{
					Logger.Debug("Loopback resp: {0}", new object[] { text2 });
				}
				text = text2;
			}
			return text;
		}

		private const string sLoopbackUrl = "http://127.0.0.1";
	}
}


