using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using BlueStacks.Common;

public class LegacyDownloader
{
	public LegacyDownloader(int nrWorkers, string url, string fileName)
	{
		this.mUrl = url;
		this.mFileName = fileName;
		this.mNrWorkers = nrWorkers;
	}

	public void Download(LegacyDownloader.UpdateProgressCallback updateProgressCb, LegacyDownloader.DownloadCompletedCallback downloadedCb, LegacyDownloader.ExceptionCallback exceptionCb, LegacyDownloader.ContentTypeCallback contentTypeCb = null, LegacyDownloader.SizeDownloadedCallback sizeDownloadedCb = null, LegacyDownloader.PayloadInfoCallback pInfoCb = null)
	{
		this.mUpdateProgressCallback = updateProgressCb;
		this.mDownloadCompletedCallback = downloadedCb;
		this.mExceptionCallback = exceptionCb;
		this.mContentTypeCallback = contentTypeCb;
		this.mSizeDownloadedCallback = sizeDownloadedCb;
		this.mPayloadInfoCallback = pInfoCb;
		Logger.Info("Downloading {0} to: {1}", new object[] { this.mUrl, this.mFileName });
		string text = this.mFileName;
		global::PayloadInfo payloadInfo = null;
		try
		{
			string text2 = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Path.GetFileName(this.mFileName));
			if (File.Exists(text2))
			{
				Logger.Info("{0} already downloaded to {1}", new object[] { this.mUrl, text2 });
				text = text2;
			}
			else
			{
				try
				{
					payloadInfo = this.GetRemotePayloadInfo(this.mUrl);
					if (pInfoCb != null)
					{
						pInfoCb(payloadInfo.Size);
					}
					if (payloadInfo.InvalidHTTPStatusCode)
					{
						Logger.Error("Invalid http status code.");
						exceptionCb(new Exception(Convert.ToString(ReturnCodesUInt.DOWNLOAD_FAILED_INVALID_STATUS_CODE, CultureInfo.InvariantCulture)));
						return;
					}
					string text3 = this.mResponseHeaders["Content-Type"];
					if (text3 == "application/vnd.android.package-archive")
					{
						this.mFileName = Path.ChangeExtension(this.mFileName, ".apk");
					}
					text = this.mFileName;
					if (contentTypeCb != null && !contentTypeCb(text3))
					{
						Logger.Info("Cancelling download");
						return;
					}
				}
				catch (WebException ex)
				{
					if (ex.Status == WebExceptionStatus.NameResolutionFailure)
					{
						Logger.Error("The hostname could not be resolved. Url = " + this.mUrl);
						exceptionCb(new Exception(Convert.ToString(ReturnCodesUInt.DOWNLOAD_FAILED_HOSTNAME_NOT_RESOLVED, CultureInfo.InvariantCulture)));
						return;
					}
					if (ex.Status == WebExceptionStatus.Timeout)
					{
						Logger.Error("The operation has timed out. Url = " + this.mUrl);
						exceptionCb(new Exception(Convert.ToString(ReturnCodesUInt.DOWNLOAD_FAILED_OPERATION_TIMEOUT, CultureInfo.InvariantCulture)));
						return;
					}
					Logger.Error("A WebException has occured. Url = " + this.mUrl);
					exceptionCb(ex);
					return;
				}
				catch (Exception)
				{
					Logger.Error(string.Format(CultureInfo.InvariantCulture, "Unable to send to {0}", new object[] { this.mUrl }));
					throw;
				}
				if (File.Exists(this.mFileName))
				{
					if (LegacyDownloader.IsPayloadOk(this.mFileName, payloadInfo.Size))
					{
						Logger.Info(this.mUrl + " already downloaded");
						goto IL_034C;
					}
					File.Delete(this.mFileName);
				}
				if (!payloadInfo.SupportsRangeRequest)
				{
					this.mNrWorkers = 1;
				}
				this.mWorkers = this.MakeWorkers(this.mNrWorkers, this.mUrl, this.mFileName, payloadInfo.Size);
				Logger.Info("Starting download of " + this.mFileName);
				int prevAverageTotalPercent = 0;
				LegacyDownloader.StartWorkers(this.mWorkers, delegate
				{
					int num = 0;
					long num2 = 0L;
					foreach (KeyValuePair<Thread, LegacyDownloader.Worker> keyValuePair in this.mWorkers)
					{
						num += keyValuePair.Value.PercentComplete;
						num2 += keyValuePair.Value.TotalFileDownloaded;
					}
					LegacyDownloader.SizeDownloadedCallback sizeDownloadedCb2 = sizeDownloadedCb;
					if (sizeDownloadedCb2 != null)
					{
						sizeDownloadedCb2(num2);
					}
					int num3 = num / this.mWorkers.Count;
					if (num3 != prevAverageTotalPercent)
					{
						updateProgressCb(num3);
					}
					prevAverageTotalPercent = num3;
				});
				LegacyDownloader.WaitForWorkers(this.mWorkers);
				LegacyDownloader.MakePayload(this.mNrWorkers, this.mFileName);
				if (!LegacyDownloader.IsPayloadOk(this.mFileName, payloadInfo.Size))
				{
					string text4 = "Downloaded file not of the correct size";
					Logger.Info(text4);
					File.Delete(this.mFileName);
					throw new Exception(text4);
				}
				Logger.Info("File downloaded correctly");
				LegacyDownloader.DeletePayloadParts(this.mNrWorkers, this.mFileName);
			}
			IL_034C:
			downloadedCb(text);
		}
		catch (Exception ex2)
		{
			Logger.Error("Exception in Download. err: " + ex2.ToString());
			exceptionCb(ex2);
		}
	}

	public static string MakePartFileName(string fileName, int id)
	{
		return string.Format(CultureInfo.InvariantCulture, "{0}_part_{1}", new object[] { fileName, id });
	}

	private static long GetSizeFromContentRange(HttpWebResponse res)
	{
		string text = res.Headers["Content-Range"];
		char[] array = new char[] { '/' };
		string[] array2 = text.Split(array);
		return Convert.ToInt64(array2[array2.Length - 1], CultureInfo.InvariantCulture);
	}

	private global::PayloadInfo GetRemotePayloadInfo(string url)
	{
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.Method = "Head";
		httpWebRequest.KeepAlive = false;
		HttpWebResponse httpWebResponse = null;
		global::PayloadInfo payloadInfo = null;
		try
		{
			LegacyDownloader.Add64BitRange(httpWebRequest, 0L, 0L);
			httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			string httpresponseHeaders = LegacyDownloader.GetHTTPResponseHeaders(httpWebResponse);
			this.mResponseHeaders = httpWebResponse.Headers;
			Logger.Warning(httpresponseHeaders);
			if (httpWebResponse.StatusCode == HttpStatusCode.PartialContent)
			{
				long sizeFromContentRange = LegacyDownloader.GetSizeFromContentRange(httpWebResponse);
				payloadInfo = new global::PayloadInfo(true, sizeFromContentRange, false);
			}
			else if (httpWebResponse.StatusCode == HttpStatusCode.OK)
			{
				if (httpresponseHeaders.Contains("Accept-Ranges: bytes"))
				{
					payloadInfo = new global::PayloadInfo(true, httpWebResponse.ContentLength, false);
				}
				else
				{
					payloadInfo = new global::PayloadInfo(false, httpWebResponse.ContentLength, false);
				}
			}
			else
			{
				payloadInfo = new global::PayloadInfo(false, 0L, true);
			}
		}
		catch (Exception ex)
		{
			Logger.Error(ex.ToString());
			throw;
		}
		httpWebResponse.Close();
		return payloadInfo;
	}

	private List<KeyValuePair<Thread, LegacyDownloader.Worker>> MakeWorkers(int nrWorkers, string url, string payloadFileName, long payloadSize)
	{
		long num = payloadSize / (long)nrWorkers;
		List<KeyValuePair<Thread, LegacyDownloader.Worker>> list = new List<KeyValuePair<Thread, LegacyDownloader.Worker>>();
		for (int i = 0; i < nrWorkers; i++)
		{
			long num2 = (long)i * num;
			long num3;
			if (i == nrWorkers - 1)
			{
				num3 = (long)(i + 1) * num + payloadSize % (long)nrWorkers - 1L;
			}
			else
			{
				num3 = (long)(i + 1) * num - 1L;
			}
			Thread thread = new Thread(new ParameterizedThreadStart(this.DoWork))
			{
				IsBackground = true
			};
			LegacyDownloader.Worker worker = new LegacyDownloader.Worker(i, url, payloadFileName, new Range(num2, num3));
			KeyValuePair<Thread, LegacyDownloader.Worker> keyValuePair = new KeyValuePair<Thread, LegacyDownloader.Worker>(thread, worker);
			list.Add(keyValuePair);
		}
		return list;
	}

	private static void StartWorkers(List<KeyValuePair<Thread, LegacyDownloader.Worker>> workers, LegacyDownloader.ProgressCallback progressCallback)
	{
		foreach (KeyValuePair<Thread, LegacyDownloader.Worker> keyValuePair in workers)
		{
			keyValuePair.Value.ProgressCallback = progressCallback;
			keyValuePair.Key.Start(keyValuePair.Value);
		}
	}

	private static void MakePayload(int nrWorkers, string payloadName)
	{
		Stream stream = new FileStream(payloadName, FileMode.Create, FileAccess.Write, FileShare.None);
		int num = 16384;
		byte[] array = new byte[num];
		for (int i = 0; i < nrWorkers; i++)
		{
			Stream stream2 = new FileStream(LegacyDownloader.MakePartFileName(payloadName, i), FileMode.Open, FileAccess.Read);
			int num2;
			while ((num2 = stream2.Read(array, 0, num)) > 0)
			{
				stream.Write(array, 0, num2);
			}
			stream2.Close();
		}
		stream.Flush();
		stream.Close();
	}

	private static void DeletePayloadParts(int nrParts, string payloadName)
	{
		for (int i = 0; i < nrParts; i++)
		{
			File.Delete(LegacyDownloader.MakePartFileName(payloadName, i));
		}
	}

	private static string GetHTTPResponseHeaders(HttpWebResponse res)
	{
		string text = "HTTP Response Headers\n" + string.Format(CultureInfo.InvariantCulture, "StatusCode: {0}\n", new object[] { (int)res.StatusCode });
		WebHeaderCollection headers = res.Headers;
		return text + ((headers != null) ? headers.ToString() : null);
	}

	public void DoWork(object data)
	{
		LegacyDownloader.Worker worker = (LegacyDownloader.Worker)data;
		Range range = ((worker != null) ? worker.Range : null);
		Stream stream = null;
		HttpWebResponse httpWebResponse = null;
		Stream stream2 = null;
		try
		{
			Logger.Info("WorkerId {0} range.From = {1}, range.To = {2}", new object[] { worker.Id, range.From, range.To });
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(worker.URL);
			httpWebRequest.KeepAlive = true;
			if (File.Exists(worker.PartFileName))
			{
				stream = new FileStream(worker.PartFileName, FileMode.Append, FileAccess.Write, FileShare.None);
				if (stream.Length == range.Length)
				{
					worker.TotalFileDownloaded = stream.Length;
					worker.PercentComplete = 100;
					Logger.Info("WorkerId {0} already downloaded", new object[] { worker.Id });
					return;
				}
				worker.TotalFileDownloaded = stream.Length;
				worker.PercentComplete = (int)(stream.Length * 100L / range.Length);
				Logger.Info("WorkerId {0} Resuming from range.From = {1}, range.To = {2}", new object[]
				{
					worker.Id,
					range.From + stream.Length,
					range.To
				});
				if (this.mNrWorkers > 1)
				{
					LegacyDownloader.Add64BitRange(httpWebRequest, range.From + stream.Length, range.To);
				}
			}
			else
			{
				worker.TotalFileDownloaded = 0L;
				worker.PercentComplete = 0;
				stream = new FileStream(worker.PartFileName, FileMode.Create, FileAccess.Write, FileShare.None);
				if (this.mNrWorkers > 1)
				{
					LegacyDownloader.Add64BitRange(httpWebRequest, range.From + stream.Length, range.To);
				}
			}
			httpWebRequest.ReadWriteTimeout = 60000;
			httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			long contentLength = httpWebResponse.ContentLength;
			stream2 = httpWebResponse.GetResponseStream();
			int num = 65536;
			byte[] array = new byte[num];
			long num2 = 0L;
			Logger.Warning(string.Format(CultureInfo.InvariantCulture, "WorkerId {0}\n", new object[] { worker.Id }) + LegacyDownloader.GetHTTPResponseHeaders(httpWebResponse));
			int num3;
			while ((num3 = stream2.Read(array, 0, num)) > 0)
			{
				if (worker.Cancelled)
				{
					throw new OperationCanceledException("Download cancelled by user.");
				}
				stream.Write(array, 0, num3);
				num2 += (long)num3;
				worker.TotalFileDownloaded = stream.Length;
				worker.PercentComplete = (int)(stream.Length * 100L / range.Length);
			}
			if (contentLength != num2)
			{
				throw new Exception(string.Format(CultureInfo.InvariantCulture, "totalContentRead({0}) != contentLength({1})", new object[] { num2, contentLength }));
			}
		}
		catch (Exception ex)
		{
			worker.Exception = ex;
			Logger.Error(ex.ToString());
		}
		finally
		{
			if (stream2 != null)
			{
				stream2.Close();
			}
			if (httpWebResponse != null)
			{
				httpWebResponse.Close();
			}
			if (stream != null)
			{
				stream.Flush();
				stream.Close();
			}
		}
		Logger.Info("WorkerId {0} Finished", new object[] { worker.Id });
	}

	private static bool IsPayloadOk(string payloadFileName, long remoteSize)
	{
		long length = new FileInfo(payloadFileName).Length;
		Logger.Info("payloadSize = " + length.ToString() + " remoteSize = " + remoteSize.ToString());
		return length == remoteSize;
	}

	public void AbortDownload()
	{
		if (this.mWorkers == null)
		{
			return;
		}
		Logger.Info("Downloader: Aborting all threads...");
		foreach (KeyValuePair<Thread, LegacyDownloader.Worker> keyValuePair in this.mWorkers)
		{
			try
			{
				keyValuePair.Value.Cancelled = true;
			}
			catch (Exception ex)
			{
				Logger.Error("Downloader: could not abort thread. Error: " + ex.Message);
			}
		}
	}

	private static void WaitForWorkers(List<KeyValuePair<Thread, LegacyDownloader.Worker>> workers)
	{
		foreach (KeyValuePair<Thread, LegacyDownloader.Worker> keyValuePair in workers)
		{
			keyValuePair.Key.Join();
		}
		foreach (KeyValuePair<Thread, LegacyDownloader.Worker> keyValuePair2 in workers)
		{
			if (keyValuePair2.Value.Exception != null)
			{
				throw new WorkerException(keyValuePair2.Value.Exception.Message, keyValuePair2.Value.Exception);
			}
		}
	}

	private static void Add64BitRange(HttpWebRequest req, long start, long end)
	{
		MethodInfo method = typeof(WebHeaderCollection).GetMethod("AddWithoutValidate", BindingFlags.Instance | BindingFlags.NonPublic);
		string text = "Range";
		string text2 = string.Format(CultureInfo.InvariantCulture, "bytes={0}-{1}", new object[] { start, end });
		method.Invoke(req.Headers, new object[] { text, text2 });
	}

	private List<KeyValuePair<Thread, LegacyDownloader.Worker>> mWorkers;

	private WebHeaderCollection mResponseHeaders;

	private readonly string mUrl;

	private string mFileName;

	private int mNrWorkers;

	private LegacyDownloader.UpdateProgressCallback mUpdateProgressCallback;

	private LegacyDownloader.DownloadCompletedCallback mDownloadCompletedCallback;

	private LegacyDownloader.ExceptionCallback mExceptionCallback;

	private LegacyDownloader.ContentTypeCallback mContentTypeCallback;

	private LegacyDownloader.SizeDownloadedCallback mSizeDownloadedCallback;

	private LegacyDownloader.PayloadInfoCallback mPayloadInfoCallback;


	public delegate void UpdateProgressCallback(int percent);


	public delegate void DownloadCompletedCallback(string filePath);


	public delegate void ExceptionCallback(Exception e);


	public delegate bool ContentTypeCallback(string contentType);


	public delegate void SizeDownloadedCallback(long size);


	public delegate void PayloadInfoCallback(long pInfo);


	private delegate void ProgressCallback();

	private class Worker
	{
		public Worker(int id, string url, string payloadName, Range range)
		{
			this.Id = id;
			this.URL = url;
			this.m_PayloadName = payloadName;
			this.Range = range;
		}

		public bool Cancelled { get; set; }

		public int Id { get; }

		public string PartFileName
		{
			get
			{
				return LegacyDownloader.MakePartFileName(this.m_PayloadName, this.Id);
			}
		}

		public Range Range { get; }

		public string URL { get; }

		public int PercentComplete
		{
			get
			{
				return this.m_PercentComplete;
			}
			set
			{
				this.m_PercentComplete = value;
				this.ProgressCallback();
			}
		}

		public long TotalFileDownloaded { get; set; }

		public LegacyDownloader.ProgressCallback ProgressCallback { get; set; }

		public Exception Exception { get; set; }

		private readonly string m_PayloadName;

		private int m_PercentComplete;
	}
}


