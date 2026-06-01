using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public static class BstHttpClient
	{
		public static string Get(string url, Dictionary<string, string> headers, bool gzip, string vmName = "Android", int timeout = 0, int retries = 1, int sleepTimeMSec = 0, bool isOnUIThreadOnPurpose = false, string oem = "bgp64")
		{
			string text;
			try
			{
				if (oem == null)
				{
					oem = "bgp64";
				}
				text = BstHttpClient.GetInternal(url, headers, gzip, retries, sleepTimeMSec, timeout, vmName, isOnUIThreadOnPurpose, oem);
			}
			catch (Exception ex)
			{
				if (url == null)
				{
					throw new Exception("url cannot be  null");
				}
				if (oem == null)
				{
					oem = "bgp64";
				}
				if (url.Contains(RegistryManager.Instance.Host))
				{
					Logger.Error("GET failed: {0}", new object[] { ex.Message });
					url = url.Replace(RegistryManager.Instance.Host, RegistryManager.Instance.Host2);
					text = BstHttpClient.GetInternal(url, headers, gzip, retries, sleepTimeMSec, timeout, vmName, isOnUIThreadOnPurpose, oem);
				}
				else
				{
					if (!url.Contains(RegistryManager.Instance.Host2))
					{
						throw;
					}
					Logger.Error("GET failed: {0}", new object[] { ex.Message });
					url = url.Replace(RegistryManager.Instance.Host2, RegistryManager.Instance.Host);
					text = BstHttpClient.GetInternal(url, headers, gzip, retries, sleepTimeMSec, timeout, vmName, isOnUIThreadOnPurpose, oem);
				}
			}
			return text;
		}

		private static string GetInternal(string url, Dictionary<string, string> headers, bool gzip, int retries, int sleepTimeMSec, int timeout, string vmName, bool isOnUIThreadOnPurpose, string oem = "bgp64")
		{
			if (Thread.CurrentThread.ManagedThreadId == 1 && !isOnUIThreadOnPurpose)
			{
				StackTrace stackTrace = new StackTrace();
				Logger.Warning("WARNING: This network call is from the UI thread. StackTrace: {0}", new object[] { stackTrace });
			}
			NameValueCollection requestHeaderCollection = HTTPUtils.GetRequestHeaderCollection(vmName);
			Uri uri = new Uri(url);
			if (uri.Host.Contains("localhost") || uri.Host.Contains("127.0.0.1"))
			{
				string text = (oem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : ("_" + oem));
				RegistryKey registryKey = RegistryUtils.InitKeyWithSecurityCheck("Software\\BlueStacks" + text);
				requestHeaderCollection.Add("x_api_token", (string)registryKey.GetValue("ApiToken", ""));
			}
			else
			{
				requestHeaderCollection.Remove("x_api_token");
			}
			return HTTP.Get(url, headers, gzip, retries, sleepTimeMSec, timeout, requestHeaderCollection, Utils.GetUserAgent(oem));
		}

		public static string Post(string url, Dictionary<string, string> data, Dictionary<string, string> headers = null, bool gzip = false, string vmName = "Android", int timeout = 0, int retries = 1, int sleepTimeMSec = 0, bool isOnUIThreadOnPurpose = false, string oem = "bgp64")
		{
			string text;
			try
			{
				if (oem == null)
				{
					oem = "bgp64";
				}
				if (url != null && Features.IsFeatureEnabled(536870912UL) && url.Contains(RegistryManager.Instance.Host))
				{
					url = url.Replace(RegistryManager.Instance.Host, RegistryManager.Instance.Host2);
				}
				text = BstHttpClient.PostInternal(url, data, headers, gzip, retries, sleepTimeMSec, timeout, vmName, isOnUIThreadOnPurpose, oem);
			}
			catch (Exception ex)
			{
				if (url == null)
				{
					throw new Exception("url cannot be  null");
				}
				if (oem == null)
				{
					oem = "bgp64";
				}
				if (url.Contains(RegistryManager.Instance.Host))
				{
					Logger.Error(ex.Message);
					url = url.Replace(RegistryManager.Instance.Host, RegistryManager.Instance.Host2);
					text = BstHttpClient.PostInternal(url, data, headers, gzip, retries, sleepTimeMSec, timeout, vmName, isOnUIThreadOnPurpose, oem);
				}
				else
				{
					if (!url.Contains(RegistryManager.Instance.Host2))
					{
						throw;
					}
					Logger.Error(ex.Message);
					url = url.Replace(RegistryManager.Instance.Host2, RegistryManager.Instance.Host);
					text = BstHttpClient.PostInternal(url, data, headers, gzip, retries, sleepTimeMSec, timeout, vmName, isOnUIThreadOnPurpose, oem);
				}
			}
			return text;
		}

		private static string PostInternal(string url, Dictionary<string, string> data, Dictionary<string, string> headers, bool gzip, int retries, int sleepTimeMSecs, int timeout, string vmName, bool isOnUIThreadOnPurpose, string oem = "bgp64")
		{
			if (Thread.CurrentThread.ManagedThreadId == 1 && !isOnUIThreadOnPurpose)
			{
				StackTrace stackTrace = new StackTrace();
				Logger.Warning("WARNING: This network call is from UI Thread and its stack trace is {0}", new object[] { stackTrace.ToString() });
			}
			NameValueCollection requestHeaderCollection = HTTPUtils.GetRequestHeaderCollection(vmName);
			if (data == null)
			{
				data = new Dictionary<string, string>();
			}
			Uri uri = new Uri(url);
			if (uri.Host.Contains("localhost") || uri.Host.Contains("127.0.0.1"))
			{
				string text = (oem.Equals("bgp", StringComparison.InvariantCultureIgnoreCase) ? "" : ("_" + oem));
				RegistryKey registryKey = RegistryUtils.InitKeyWithSecurityCheck("Software\\BlueStacks" + text);
				requestHeaderCollection.Add("x_api_token", (string)registryKey.GetValue("ApiToken", ""));
			}
			else
			{
				requestHeaderCollection.Remove("x_api_token");
				data = Utils.AddCommonData(data);
			}
			return HTTP.Post(url, data, headers, gzip, retries, sleepTimeMSecs, timeout, requestHeaderCollection, Utils.GetUserAgent(oem));
		}

		public static string HTTPGaeFileUploader(string url, Dictionary<string, string> data, Dictionary<string, string> headers, string filepath, string contentType, bool gzip, string vmName)
		{
			if (data == null)
			{
				data = new Dictionary<string, string>();
			}
			string text;
			try
			{
				if (url != null && Features.IsFeatureEnabled(536870912UL) && url.Contains(RegistryManager.Instance.Host))
				{
					url = url.Replace(RegistryManager.Instance.Host, RegistryManager.Instance.Host2);
				}
				text = BstHttpClient.HTTPGaeFileUploaderInternal(url, data, headers, filepath, contentType, gzip, vmName);
			}
			catch (Exception ex)
			{
				if (url == null)
				{
					throw new Exception("url cannot be  null");
				}
				if (url.Contains(RegistryManager.Instance.Host))
				{
					Logger.Error(ex.Message);
					url = url.Replace(RegistryManager.Instance.Host, RegistryManager.Instance.Host2);
					text = BstHttpClient.HTTPGaeFileUploaderInternal(url, data, headers, filepath, contentType, gzip, vmName);
				}
				else
				{
					if (!url.Contains(RegistryManager.Instance.Host2))
					{
						throw;
					}
					Logger.Error(ex.Message);
					url = url.Replace(RegistryManager.Instance.Host2, RegistryManager.Instance.Host);
					text = BstHttpClient.HTTPGaeFileUploaderInternal(url, data, headers, filepath, contentType, gzip, vmName);
				}
			}
			return text;
		}

		private static string HTTPGaeFileUploaderInternal(string url, Dictionary<string, string> data, Dictionary<string, string> headers, string filepath, string contentType, bool gzip, string vmName)
		{
			if (data == null)
			{
				data = new Dictionary<string, string>();
			}
			if (filepath == null || !File.Exists(filepath))
			{
				return BstHttpClient.Post(url, data, headers, gzip, vmName, 0, 1, 0, false, "bgp64");
			}
			JObject jobject = JObject.Parse(BstHttpClient.Get(url, null, false, vmName, 0, 1, 0, false, "bgp64"));
			string text = null;
			string text2 = null;
			string text3 = "";
			if (jobject["success"].ToObject<bool>())
			{
				text = jobject["url"].ToString();
				try
				{
					text2 = jobject["country"].ToString();
				}
				catch
				{
					try
					{
						text2 = new RegionInfo(CultureInfo.CurrentCulture.Name).TwoLetterISORegionName;
					}
					catch
					{
						text2 = "US";
					}
				}
			}
			data.Add("country", text2);
			if (Oem.Instance.IsOEMWithBGPClient)
			{
				text3 = RegistryManager.Instance.ClientVersion;
			}
			data.Add("client_ver", text3);
			return BstHttpClient.HttpUploadFile(text, filepath, "file", contentType, headers, data);
		}

		private static string HttpUploadFile(string url, string file, string paramName, string contentType, Dictionary<string, string> headers, Dictionary<string, string> data)
		{
			string text;
			try
			{
				if (Features.IsFeatureEnabled(536870912UL) && url.Contains(RegistryManager.Instance.Host))
				{
					url = url.Replace(RegistryManager.Instance.Host, RegistryManager.Instance.Host2);
				}
				text = BstHttpClient.HttpUploadFileInternal(url, file, paramName, contentType, headers, data);
			}
			catch (Exception ex)
			{
				if (url.Contains(RegistryManager.Instance.Host))
				{
					Logger.Error(ex.Message);
					url = url.Replace(RegistryManager.Instance.Host, RegistryManager.Instance.Host2);
					text = BstHttpClient.HttpUploadFileInternal(url, file, paramName, contentType, headers, data);
				}
				else
				{
					if (!url.Contains(RegistryManager.Instance.Host2))
					{
						throw;
					}
					Logger.Error(ex.Message);
					url = url.Replace(RegistryManager.Instance.Host2, RegistryManager.Instance.Host);
					text = BstHttpClient.HttpUploadFileInternal(url, file, paramName, contentType, headers, data);
				}
			}
			return text;
		}

		private static string HttpUploadFileInternal(string url, string file, string paramName, string contentType, Dictionary<string, string> headers, Dictionary<string, string> data)
		{
			Logger.Info("Uploading {0} to {1}", new object[] { file, url });
			string text = "---------------------------" + DateTime.Now.Ticks.ToString("x", CultureInfo.InvariantCulture);
			byte[] bytes = Encoding.ASCII.GetBytes("\r\n--" + text + "\r\n");
			Uri uri = new Uri(url);
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.ContentType = "multipart/form-data; boundary=" + text;
			httpWebRequest.Method = "POST";
			httpWebRequest.KeepAlive = true;
			httpWebRequest.Timeout = 300000;
			httpWebRequest.UserAgent = Utils.GetUserAgent("bgp64");
			if (!uri.Host.Contains("localhost") && !uri.Host.Contains("127.0.0.1"))
			{
				string text2 = "URI of proxy = ";
				Uri proxy = httpWebRequest.Proxy.GetProxy(uri);
				Logger.Debug(text2 + ((proxy != null) ? proxy.ToString() : null));
			}
			if (headers != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in headers)
				{
					httpWebRequest.Headers.Set(StringUtils.GetControlCharFreeString(keyValuePair.Key), StringUtils.GetControlCharFreeString(keyValuePair.Value));
				}
			}
			httpWebRequest.Headers.Add(HTTPUtils.GetRequestHeaderCollection(""));
			if (data == null)
			{
				data = new Dictionary<string, string>();
			}
			Stream requestStream = httpWebRequest.GetRequestStream();
			string text3 = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
			foreach (KeyValuePair<string, string> keyValuePair2 in data)
			{
				requestStream.Write(bytes, 0, bytes.Length);
				string text4 = string.Format(CultureInfo.InvariantCulture, text3, new object[] { keyValuePair2.Key, keyValuePair2.Value });
				byte[] bytes2 = Encoding.UTF8.GetBytes(text4);
				requestStream.Write(bytes2, 0, bytes2.Length);
			}
			requestStream.Write(bytes, 0, bytes.Length);
			string text5 = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
			string text6 = string.Format(CultureInfo.InvariantCulture, text5, new object[] { paramName, file, contentType });
			byte[] bytes3 = Encoding.UTF8.GetBytes(text6);
			requestStream.Write(bytes3, 0, bytes3.Length);
			string text7 = Environment.ExpandEnvironmentVariables("%TEMP%");
			text7 = Path.Combine(text7, Path.GetFileName(file)) + "_bst";
			File.Copy(file, text7);
			if (contentType.Equals("text/plain", StringComparison.InvariantCultureIgnoreCase))
			{
				int num = 1048576;
				string text8 = File.ReadAllText(text7);
				byte[] array = new byte[num];
				array = Encoding.UTF8.GetBytes(text8);
				requestStream.Write(array, 0, array.Length);
			}
			else
			{
				FileStream fileStream = new FileStream(text7, FileMode.Open, FileAccess.Read);
				byte[] array2 = new byte[4096];
				int num2;
				while ((num2 = fileStream.Read(array2, 0, array2.Length)) != 0)
				{
					requestStream.Write(array2, 0, num2);
				}
				fileStream.Close();
			}
			File.Delete(text7);
			byte[] bytes4 = Encoding.ASCII.GetBytes("\r\n--" + text + "--\r\n");
			requestStream.Write(bytes4, 0, bytes4.Length);
			requestStream.Close();
			string text9 = null;
			WebResponse webResponse = null;
			try
			{
				webResponse = httpWebRequest.GetResponse();
				using (Stream responseStream = webResponse.GetResponseStream())
				{
					using (StreamReader streamReader = new StreamReader(responseStream))
					{
						text9 = streamReader.ReadToEnd();
						Logger.Info("File uploaded, server response is: {0}", new object[] { text9 });
					}
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error uploading file", new object[] { ex });
				if (webResponse != null)
				{
					webResponse.Close();
					webResponse = null;
				}
				throw;
			}
			finally
			{
				httpWebRequest = null;
			}
			return text9;
		}

		public static string PostMultipart(string url, Dictionary<string, object> parameters, out byte[] dataArray)
		{
			string text = "---------------------------" + DateTime.Now.Ticks.ToString("x", CultureInfo.InvariantCulture);
			byte[] bytes = Encoding.ASCII.GetBytes("\r\n--" + text + "\r\n");
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.ContentType = "multipart/form-data; boundary=" + text;
			httpWebRequest.Method = "POST";
			httpWebRequest.KeepAlive = true;
			httpWebRequest.Credentials = CredentialCache.DefaultCredentials;
			if (parameters != null && parameters.Count > 0)
			{
				using (Stream requestStream = httpWebRequest.GetRequestStream())
				{
					foreach (KeyValuePair<string, object> keyValuePair in parameters)
					{
						requestStream.Write(bytes, 0, bytes.Length);
						if (keyValuePair.Value is FormFile)
						{
							FormFile formFile = keyValuePair.Value as FormFile;
							string text2 = string.Concat(new string[] { "Content-Disposition: form-data; name=\"", keyValuePair.Key, "\"; filename=\"", formFile.Name, "\"\r\nContent-Type: ", formFile.ContentType, "\r\n\r\n" });
							byte[] bytes2 = Encoding.UTF8.GetBytes(text2);
							requestStream.Write(bytes2, 0, bytes2.Length);
							byte[] array = new byte[32768];
							int num;
							if (formFile.Stream == null)
							{
								using (FileStream fileStream = File.OpenRead(formFile.FilePath))
								{
									while ((num = fileStream.Read(array, 0, array.Length)) != 0)
									{
										requestStream.Write(array, 0, num);
									}
									fileStream.Close();
									continue;
								}
								goto IL_019C;
							}
							IL_01A8:
							if ((num = formFile.Stream.Read(array, 0, array.Length)) == 0)
							{
								continue;
							}
							IL_019C:
							requestStream.Write(array, 0, num);
							goto IL_01A8;
						}
						string text3 = "Content-Disposition: form-data; name=\"";
						string key = keyValuePair.Key;
						string text4 = "\"\r\n\r\n";
						object value = keyValuePair.Value;
						string text5 = text3 + key + text4 + ((value != null) ? value.ToString() : null);
						byte[] bytes3 = Encoding.UTF8.GetBytes(text5);
						requestStream.Write(bytes3, 0, bytes3.Length);
					}
					byte[] bytes4 = Encoding.ASCII.GetBytes("\r\n--" + text + "--\r\n");
					requestStream.Write(bytes4, 0, bytes4.Length);
					requestStream.Close();
				}
			}
			HttpStatusCode httpStatusCode = HttpStatusCode.BadRequest;
			string text6 = ((HttpWebResponse)httpWebRequest.GetResponse()).ToString();
			int num2;
			using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
			{
				httpStatusCode = httpWebResponse.StatusCode;
				if (int.TryParse(httpWebResponse.Headers.Get("Content-Length"), out num2))
				{
					Logger.Info("content lenght.." + num2.ToString());
				}
				using (BinaryReader binaryReader = new BinaryReader(httpWebResponse.GetResponseStream()))
				{
					using (MemoryStream memoryStream = new MemoryStream())
					{
						byte[] array2 = binaryReader.ReadBytes(16384);
						while (array2.Length != 0)
						{
							memoryStream.Write(array2, 0, array2.Length);
							array2 = binaryReader.ReadBytes(16384);
						}
						dataArray = new byte[(int)memoryStream.Length];
						memoryStream.Position = 0L;
						memoryStream.Read(dataArray, 0, dataArray.Length);
					}
				}
			}
			if (httpStatusCode != HttpStatusCode.OK || num2 < 2000)
			{
				text6 = "error";
			}
			return text6;
		}
	}
}


