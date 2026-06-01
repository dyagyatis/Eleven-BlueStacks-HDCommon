using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using System.Web;

namespace BlueStacks.Common
{
	public class HTTPServer : IDisposable
	{
		public int Port { get; }

		public string RootDir { get; set; }

		public static bool FileWriteComplete { get; set; } = true;

		public HTTPServer(int port, Dictionary<string, HTTPServer.RequestHandler> routes, string rootDir)
		{
			this.Port = port;
			this.Routes = routes;
			this.RootDir = rootDir;
		}

		public void Start()
		{
			string text = string.Format(CultureInfo.InvariantCulture, "http://{0}:{1}/", new object[] { "*", this.Port });
			this.mListener = new HttpListener();
			this.mListener.Prefixes.Add(text);
			try
			{
				this.mShutDown = false;
				this.mListener.Start();
			}
			catch (HttpListenerException ex)
			{
				Logger.Error("Failed to start listener. err: " + ex.ToString());
				throw new ENoPortAvailableException("No free port available");
			}
		}

		public void Run()
		{
			while (!this.mShutDown)
			{
				HttpListenerContext httpListenerContext = null;
				try
				{
					httpListenerContext = this.mListener.GetContext();
				}
				catch (Exception ex)
				{
					Logger.Error("Exception while processing HTTP context: " + ex.ToString());
					continue;
				}
				ThreadPool.QueueUserWorkItem(new WaitCallback(new HTTPServer.Worker(httpListenerContext, this.Routes, this.RootDir).ProcessRequest));
			}
		}

		public void Stop()
		{
			bool flag = this.mListener != null;
			if (flag)
			{
				try
				{
					this.mShutDown = true;
					this.mListener.Close();
				}
				catch (HttpListenerException ex)
				{
					Logger.Error("Failed to stop listener. err: " + ex.ToString());
				}
			}
		}

		~HTTPServer()
		{
			this.Dispose(false);
		}

		protected virtual void Dispose(bool disposing)
		{
			bool flag = !this.disposedValue;
			if (flag)
			{
				HttpListener httpListener = this.mListener;
				bool flag2 = httpListener != null;
				if (flag2)
				{
					httpListener.Close();
				}
				this.disposedValue = true;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private HttpListener mListener;

		private bool mShutDown;

		private readonly Dictionary<string, HTTPServer.RequestHandler> Routes;

		private bool disposedValue;


		public delegate void RequestHandler(HttpListenerRequest req, HttpListenerResponse res);

		private class Worker
		{
			public Worker(HttpListenerContext ctx, Dictionary<string, HTTPServer.RequestHandler> routes, string rootDir)
			{
				this.mCtx = ctx;
				this.mRoutes = routes;
				this.mRootDir = rootDir;
			}

			[STAThread]
			public void ProcessRequest(object stateInfo)
			{
				try
				{
					bool flag = this.mCtx.Request.Url.AbsolutePath.StartsWith("/static/", StringComparison.OrdinalIgnoreCase);
					if (flag)
					{
						this.StaticFileHandler(this.mCtx.Request, this.mCtx.Response);
					}
					else
					{
						bool flag2 = this.mCtx.Request.Url.AbsolutePath.StartsWith("/static2/", StringComparison.OrdinalIgnoreCase);
						if (flag2)
						{
							this.StaticFileChunkHandler(this.mCtx.Request, this.mCtx.Response, "");
						}
						else
						{
							bool flag3 = this.mCtx.Request.Url.AbsolutePath.StartsWith("/staticicon/", StringComparison.OrdinalIgnoreCase);
							if (flag3)
							{
								bool flag4 = this.mCtx.Request.QueryString != null && this.mCtx.Request.QueryString.Count > 0;
								if (flag4)
								{
									string text = HttpUtility.ParseQueryString(this.mCtx.Request.Url.Query).Get("oem");
									bool flag5 = InstalledOem.InstalledCoexistingOemList.Contains(text);
									if (flag5)
									{
										this.StaticFileChunkHandler(this.mCtx.Request, this.mCtx.Response, Path.Combine(RegistryManager.RegistryManagers[text].EngineDataDir, "UserData\\Gadget"));
									}
								}
								else
								{
									this.StaticFileChunkHandler(this.mCtx.Request, this.mCtx.Response, Path.Combine(RegistryManager.Instance.EngineDataDir, "UserData\\Gadget"));
								}
							}
							else
							{
								bool flag6 = this.mRoutes.ContainsKey(this.mCtx.Request.Url.AbsolutePath);
								if (flag6)
								{
									HTTPServer.RequestHandler requestHandler = this.mRoutes[this.mCtx.Request.Url.AbsolutePath];
									bool flag7 = requestHandler != null;
									if (flag7)
									{
										bool flag8 = this.mCtx.Request.UserAgent != null;
										if (flag8)
										{
											Logger.Info("Request received {0}", new object[] { this.mCtx.Request.Url.AbsolutePath });
											Logger.Debug("UserAgent = {0}", new object[] { this.mCtx.Request.UserAgent });
										}
										bool flag9 = HTTPServer.Worker.IsTokenValid(this.mCtx.Request.Headers);
										if (flag9)
										{
											requestHandler(this.mCtx.Request, this.mCtx.Response);
										}
										else
										{
											Logger.Warning("Token validation check failed, unauthorized access");
											HTTPUtils.WriteErrorJson(this.mCtx.Response, "Unauthorized Access(401)");
											this.mCtx.Response.StatusCode = 401;
										}
									}
								}
								else
								{
									Logger.Warning("Exception: No Handler registered for " + this.mCtx.Request.Url.AbsolutePath);
									HTTPUtils.WriteErrorJson(this.mCtx.Response, "Request NotFound(404)");
									this.mCtx.Response.StatusCode = 404;
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					Logger.Error("Exception while processing HTTP handler: " + ex.ToString());
					HTTPUtils.WriteErrorJson(this.mCtx.Response, "Internal Server Error(500)");
					this.mCtx.Response.StatusCode = 500;
				}
				finally
				{
					try
					{
						this.mCtx.Response.OutputStream.Close();
					}
					catch (Exception ex2)
					{
						Logger.Warning("Exception during mCtx.Response.OutputStream.Close(): " + ex2.ToString());
					}
				}
			}

			private static bool IsTokenValid(NameValueCollection headers)
			{
				return headers["x_api_token"] != null && headers["x_api_token"].ToString(CultureInfo.InvariantCulture).Equals(RegistryManager.Instance.ApiToken, StringComparison.OrdinalIgnoreCase);
			}

			public void StaticFileHandler(HttpListenerRequest req, HttpListenerResponse res)
			{
				string text = req.Url.AbsolutePath;
				text = text.Substring(text.Substring(1).IndexOf("/", StringComparison.OrdinalIgnoreCase) + 2);
				string text2 = Path.Combine(this.mRootDir, text.Replace("/", "\\"));
				bool flag = File.Exists(text2);
				if (flag)
				{
					byte[] array = File.ReadAllBytes(text2);
					bool flag2 = text2.EndsWith(".css", StringComparison.OrdinalIgnoreCase);
					if (flag2)
					{
						res.Headers.Add("Content-Type: text/css");
					}
					else
					{
						bool flag3 = text2.EndsWith(".js", StringComparison.OrdinalIgnoreCase);
						if (flag3)
						{
							res.Headers.Add("Content-Type: application/javascript");
						}
					}
					res.OutputStream.Write(array, 0, array.Length);
				}
				else
				{
					Logger.Error(string.Format(CultureInfo.InvariantCulture, "File {0} doesn't exist", new object[] { text2 }));
					res.StatusCode = 404;
					res.StatusDescription = "Not Found.";
				}
			}

			public void StaticFileChunkHandler(HttpListenerRequest req, HttpListenerResponse res, string dir = "")
			{
				string text = req.Url.AbsolutePath;
				text = text.Substring(text.Substring(1).IndexOf("/", StringComparison.OrdinalIgnoreCase) + 2);
				string text2 = (string.IsNullOrEmpty(dir) ? Path.Combine(this.mRootDir, text.Replace("/", "\\")) : Path.Combine(dir, text.Replace("/", "\\")));
				int num = 0;
				while (!File.Exists(text2))
				{
					num++;
					Thread.Sleep(100);
					bool flag = num == 20;
					if (flag)
					{
						break;
					}
				}
				num = 0;
				bool flag2 = !File.Exists(text2);
				if (flag2)
				{
					Logger.Error(string.Format(CultureInfo.InvariantCulture, "File {0} doesn't exist", new object[] { text2 }));
					res.StatusCode = 404;
					res.StatusDescription = "Not Found.";
				}
				else
				{
					FileInfo fileInfo = new FileInfo(text2);
					long length = fileInfo.Length;
					DateTimeOffset dateTimeOffset = fileInfo.LastWriteTimeUtc;
					DateTimeOffset dateTimeOffset2 = new DateTimeOffset(dateTimeOffset.Year, dateTimeOffset.Month, dateTimeOffset.Day, dateTimeOffset.Hour, dateTimeOffset.Minute, dateTimeOffset.Second, dateTimeOffset.Offset).ToUniversalTime();
					long num2 = dateTimeOffset2.ToFileTime() ^ length;
					bool flag3 = string.Equals("\"" + Convert.ToString(num2, 16) + "\"", req.Headers.Get("If-None-Match"), StringComparison.InvariantCultureIgnoreCase) && string.Equals(Convert.ToString(dateTimeOffset2, CultureInfo.InvariantCulture), req.Headers.Get("If-Modified-Since"), StringComparison.InvariantCultureIgnoreCase);
					if (flag3)
					{
						res.StatusCode = 304;
						res.StatusDescription = "Not Modified.";
					}
					else
					{
						bool flag4 = text2.EndsWith(".flv", StringComparison.OrdinalIgnoreCase);
						if (flag4)
						{
							res.Headers.Add("Content-Type: video/x-flv");
						}
						bool flag5 = text2.EndsWith(".png", StringComparison.OrdinalIgnoreCase);
						if (flag5)
						{
							res.Headers.Add("Content-Type: image/png");
						}
						res.Headers.Add("Cache-Control: public,max-age=120");
						res.Headers.Add("ETag: \"" + Convert.ToString(num2, 16) + "\"");
						res.Headers.Add("Last-Modified: " + Convert.ToString(dateTimeOffset2, CultureInfo.InvariantCulture));
						int num3 = 1048576;
						bool flag6 = false;
						FileStream fileStream = new FileStream(text2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
						for (;;)
						{
							byte[] array = new byte[num3];
							int num4 = fileStream.Read(array, 0, num3);
							bool flag7 = num4 != 0;
							if (flag7)
							{
								res.OutputStream.Write(array, 0, num4);
								flag6 = true;
							}
							else
							{
								bool flag8 = num++ == 50 || flag6;
								if (flag8)
								{
									break;
								}
								Thread.Sleep(100);
							}
						}
						fileStream.Close();
					}
				}
			}

			private Dictionary<string, HTTPServer.RequestHandler> mRoutes;

			private HttpListenerContext mCtx;

			private string mRootDir;
		}
	}
}


