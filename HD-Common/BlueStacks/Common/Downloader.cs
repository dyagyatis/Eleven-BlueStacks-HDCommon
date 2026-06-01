using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;

namespace BlueStacks.Common
{
	public class Downloader
	{
		public event Downloader.DownloadRetryEventHandler DownloadRetry;
		public event Downloader.DownloadExceptionEventHandler DownloadException;
		public event Downloader.UnsupportedResumeEventHandler UnsupportedResume;
		public event Downloader.FilePayloadInfoReceivedHandler FilePayloadInfoReceived;
		public event Downloader.DownloadFileCompletedEventHandler DownloadFileCompleted;
		public event Downloader.DownloadProgressChangedEventHandler DownloadProgressChanged;
		public event Downloader.DownloadProgressPercentChangedEventHandler DownloadProgressPercentChanged;
		public event Downloader.DownloadCancelledEventHandler DownloadCancelled;

		public bool IsDownLoadCanceled { get; set; }

		public bool IsDownloadInProgress { get; private set; }

		protected virtual void OnDownloadProgressChanged(long bytes)
		{
			Downloader.DownloadProgressChangedEventHandler downloadProgressChanged = this.DownloadProgressChanged;
			if (downloadProgressChanged == null)
			{
				return;
			}
			downloadProgressChanged(bytes);
		}

		protected virtual void OnDownloadPercentProgressChanged(double percent)
		{
			Downloader.DownloadProgressPercentChangedEventHandler downloadProgressPercentChanged = this.DownloadProgressPercentChanged;
			if (downloadProgressPercentChanged == null)
			{
				return;
			}
			downloadProgressPercentChanged(percent);
		}

		protected virtual void OnDownloadException(Exception e)
		{
			Downloader.DownloadExceptionEventHandler downloadException = this.DownloadException;
			if (downloadException == null)
			{
				return;
			}
			downloadException(e);
		}

		protected virtual void OnDownloadFileCompleted()
		{
			Downloader.DownloadFileCompletedEventHandler downloadFileCompleted = this.DownloadFileCompleted;
			if (downloadFileCompleted == null)
			{
				return;
			}
			downloadFileCompleted(this, new EventArgs());
		}

		protected virtual void OnFilePayloadInfoReceived(long size)
		{
			Downloader.FilePayloadInfoReceivedHandler filePayloadInfoReceived = this.FilePayloadInfoReceived;
			if (filePayloadInfoReceived == null)
			{
				return;
			}
			filePayloadInfoReceived(size);
		}

		protected virtual void OnUnsupportedResume(HttpStatusCode code)
		{
			Downloader.UnsupportedResumeEventHandler unsupportedResume = this.UnsupportedResume;
			if (unsupportedResume == null)
			{
				return;
			}
			unsupportedResume(code);
		}

		protected virtual void OnDownloadCancelled()
		{
			Downloader.DownloadCancelledEventHandler downloadCancelled = this.DownloadCancelled;
			if (downloadCancelled == null)
			{
				return;
			}
			downloadCancelled();
		}

		protected virtual void OnDownloadRetryEvent()
		{
			Downloader.DownloadRetryEventHandler downloadRetry = this.DownloadRetry;
			if (downloadRetry == null)
			{
				return;
			}
			downloadRetry(this, new EventArgs());
		}

		public void DownloadFile(string url, string fileDestination)
		{
			this.IsDownloadInProgress = true;
			FileStream fileStream = null;
			HttpWebRequest httpWebRequest = null;
			HttpWebResponse httpWebResponse = null;
			try
			{
				if (File.Exists(fileDestination))
				{
					Logger.Info("{0} already downloaded to {1}", new object[] { url, fileDestination });
					this.OnDownloadFileCompleted();
				}
				else
				{
					string text = fileDestination + ".tmp";
					try
					{
						fileStream = new FileStream(text, FileMode.Append, FileAccess.Write, FileShare.None);
					}
					catch (Exception ex)
					{
						this.OnDownloadException(ex);
						return;
					}
					long num = fileStream.Length;
					int num2 = 0;
					for (;;)
					{
						long num3 = num;
						try
						{
							httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
							Downloader.AddRangeToRequest(httpWebRequest, string.Format(CultureInfo.InvariantCulture, "{0}-", new object[] { num }), "bytes");
							httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
						}
						catch (WebException ex2)
						{
							HttpStatusCode statusCode = ((HttpWebResponse)ex2.Response).StatusCode;
							if (statusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
							{
								Logger.Warning("Unsupported resume! {0}", new object[] { statusCode });
								if (fileStream != null)
								{
									fileStream.Close();
								}
								this.OnUnsupportedResume(statusCode);
								return;
							}
							Logger.Warning("An error occured while creating a request. WebEx: {0}", new object[] { ex2.Message });
							goto IL_034B;
						}
						catch (Exception ex3)
						{
							Logger.Warning("An error occured while creating a request. Ex: {0}", new object[] { ex3.Message });
							goto IL_034B;
						}
						goto IL_0143;
						IL_034B:
						this.OnDownloadRetryEvent();
						if (num3 == num)
						{
							num2++;
						}
						if (num2 == 20)
						{
							this.OnDownloadException(new UnknownErrorException());
						}
						if (httpWebRequest != null)
						{
							httpWebRequest.Abort();
						}
						httpWebRequest = null;
						if (httpWebResponse != null)
						{
							httpWebResponse.Close();
						}
						httpWebResponse = null;
						int num4;
						if (num2 > 10)
						{
							num4 = 1800;
						}
						else
						{
							num4 = Convert.ToInt32(Math.Pow(2.0, (double)num2));
						}
						Logger.Info("Will retry after {0}s", new object[] { num4 });
						Thread.Sleep(num4 * 1000);
						continue;
						IL_0143:
						if (httpWebResponse.StatusCode != HttpStatusCode.PartialContent && httpWebResponse.StatusCode != HttpStatusCode.OK)
						{
							Logger.Warning("Got an unexpected status code: {0}", new object[] { httpWebResponse.StatusCode });
							goto IL_034B;
						}
						if (num != 0L && httpWebResponse.StatusCode != HttpStatusCode.PartialContent)
						{
							break;
						}
						long num5 = httpWebResponse.ContentLength + num;
						this.OnFilePayloadInfoReceived(num5);
						Stream responseStream;
						try
						{
							responseStream = httpWebResponse.GetResponseStream();
						}
						catch (Exception ex4)
						{
							Logger.Warning("An error occured while getting a response stream: {0}", new object[] { ex4.Message });
							goto IL_034B;
						}
						byte[] array = new byte[10485760];
						for (;;)
						{
							int num6;
							try
							{
								if (this.IsDownLoadCanceled)
								{
									fileStream.Close();
									fileStream = null;
									if (File.Exists(fileDestination))
									{
										File.Delete(fileDestination);
									}
									if (File.Exists(text))
									{
										File.Delete(text);
									}
									this.OnDownloadCancelled();
									return;
								}
								num6 = responseStream.Read(array, 0, 10485760);
							}
							catch (Exception ex5)
							{
								Logger.Warning("Some error while reading from the stream. Ex: {0}", new object[] { ex5.Message });
								goto IL_034B;
							}
							if (num6 == 0)
							{
								break;
							}
							try
							{
								fileStream.Write(array, 0, num6);
							}
							catch (Exception ex6)
							{
								Logger.Warning("Some error while writing the stream to file. Ex: {0}", new object[] { ex6.Message });
								this.OnDownloadException(ex6);
								return;
							}
							num += (long)num6;
							this.OnDownloadProgressChanged(num);
							this.OnDownloadPercentProgressChanged((double)Math.Round(decimal.Divide(num, num5) * 100m, 2));
						}
						if (num != num5)
						{
							Logger.Error("Stream does not have more bytes to read. {0} != {1}", new object[] { num, num5 });
							goto IL_034B;
						}
						goto IL_028B;
					}
					this.OnUnsupportedResume(httpWebResponse.StatusCode);
					return;
					IL_028B:
					try
					{
						fileStream.Close();
						fileStream = null;
						File.Move(text, fileDestination);
						this.OnDownloadFileCompleted();
					}
					catch (Exception ex7)
					{
						Logger.Warning("Could not move file to destination. Ex: {0}", new object[] { ex7.Message });
						this.OnDownloadException(ex7);
					}
				}
			}
			catch (Exception ex8)
			{
				Logger.Error("Unable to download the file: {0}", new object[] { ex8.Message });
				Downloader.ThrowOnFatalException(ex8);
				this.OnDownloadException(ex8);
			}
			finally
			{
				if (fileStream != null)
				{
					fileStream.Close();
				}
				if (httpWebRequest != null)
				{
					httpWebRequest.Abort();
				}
				if (httpWebResponse != null)
				{
					httpWebResponse.Close();
				}
				this.IsDownloadInProgress = false;
			}
		}

		private long GetSizeFromResponseHeaders(WebHeaderCollection headers)
		{
			return Convert.ToInt64(headers["Content-Range"].Split(new char[] { '/' })[1], CultureInfo.InvariantCulture);
		}

		private static void ThrowOnFatalException(Exception e)
		{
			if (e is ThreadAbortException || e is StackOverflowException || e is OutOfMemoryException)
			{
				throw e;
			}
		}

		private static void AddRangeToRequest(WebRequest req, string range, string rangeSpecifier = "bytes")
		{
			MethodInfo method = typeof(WebHeaderCollection).GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);
			string text = "Range";
			string text2 = string.Format(CultureInfo.InvariantCulture, "{0}={1}", new object[] { rangeSpecifier, range });
			method.Invoke(req.Headers, new object[] { text, text2 });
		}

		private long GetSizeFromContentRange(HttpWebResponse webResponse)
		{
			string text = webResponse.Headers["Content-Range"];
			char[] array = new char[] { '/' };
			string[] array2 = text.Split(array);
			return Convert.ToInt64(array2[array2.Length - 1], CultureInfo.InvariantCulture);
		}

		private const int DEFAULT_BUFFER_LENGTH = 10485760;


		public delegate void DownloadRetryEventHandler(object sender, EventArgs args);


		public delegate void DownloadFileCompletedEventHandler(object sender, EventArgs args);


		public delegate void FilePayloadInfoReceivedHandler(long size);


		public delegate void DownloadExceptionEventHandler(Exception e);


		public delegate void DownloadProgressChangedEventHandler(long bytes);


		public delegate void UnsupportedResumeEventHandler(HttpStatusCode sc);


		public delegate void DownloadCancelledEventHandler();


		public delegate void DownloadProgressPercentChangedEventHandler(double percent);
	}
}


