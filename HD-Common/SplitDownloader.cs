using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using BlueStacks.Common;

public class SplitDownloader
{
	public SplitDownloader(string manifestURL, string dirPath, string userGUID, int nrWorkers)
	{
		this.m_ManifestURL = manifestURL;
		this.m_DirPath = dirPath;
		this.m_UserGUID = userGUID;
		this.m_UserAgent = string.Format(CultureInfo.InvariantCulture, "SplitDownloader {0}/{1}/{2}", new object[] { "BlueStacks", "4.220.0.4001", this.m_UserGUID });
		this.m_NrWorkers = nrWorkers;
		this.m_Workers = new SerialWorkQueue[nrWorkers];
		for (int i = 0; i < this.m_NrWorkers; i++)
		{
			this.m_Workers[i] = new SerialWorkQueue();
		}
		this.m_WorkersStarted = false;
	}

	public void Download(SplitDownloader.ProgressCb progressCb, SplitDownloader.CompletedCb completedCb, SplitDownloader.ExceptionCb exceptionCb)
	{
		this.Download(progressCb, completedCb, exceptionCb, null);
	}

	public void Download(SplitDownloader.ProgressCb progressCb, SplitDownloader.CompletedCb completedCb, SplitDownloader.ExceptionCb exceptionCb, SplitDownloader.FileSizeCb fileSizeCb)
	{
		this.m_ProgressCb = progressCb;
		this.m_CompletedCb = completedCb;
		this.m_ExceptionCb = exceptionCb;
		this.m_FileSizeCb = fileSizeCb;
		try
		{
			this.m_Manifest = this.GetManifest();
			this.GetManifestFilePath();
			if (this.m_FileSizeCb != null)
			{
				this.m_FileSizeCb(this.m_Manifest.FileSize);
			}
			this.StartWorkers();
			this.m_ProgressCb(this.m_Manifest.PercentDownloaded());
			int num = 0;
			while ((long)num < this.m_Manifest.Count)
			{
				FilePart filePart = this.m_Manifest[num];
				SerialWorkQueue.Work work = this.MakeWork(filePart);
				this.m_Workers[num % this.m_NrWorkers].Enqueue(work);
				num++;
			}
			this.StopAndWaitWorkers();
			if (!this.m_Manifest.Check())
			{
				throw new CheckFailedException();
			}
			string text = this.m_Manifest.MakeFile();
			this.m_Manifest.DeleteFileParts();
			this.m_Manifest.DeleteManifest();
			this.m_CompletedCb(text);
		}
		catch (Exception ex)
		{
			Logger.Error(ex.ToString());
			this.m_ExceptionCb(ex);
		}
		finally
		{
			if (this.m_WorkersStarted)
			{
				this.StopAndWaitWorkers();
			}
		}
	}

	private void StartWorkers()
	{
		for (int i = 0; i < this.m_NrWorkers; i++)
		{
			this.m_Workers[i].Start();
		}
		this.m_WorkersStarted = true;
	}

	private void StopAndWaitWorkers()
	{
		for (int i = 0; i < this.m_NrWorkers; i++)
		{
			this.m_Workers[i].Stop();
		}
		for (int j = 0; j < this.m_NrWorkers; j++)
		{
			this.m_Workers[j].Join();
		}
		this.m_WorkersStarted = false;
	}

	private SerialWorkQueue.Work MakeWork(FilePart filePart)
	{
		return delegate
		{
			try
			{
				if (filePart.Check())
				{
					Logger.Info(filePart.Path + " is already downloaded");
				}
				else
				{
					this.DownloadFilePart(filePart);
				}
			}
			catch (Exception ex)
			{
				Logger.Error(ex.ToString());
			}
		};
	}

	private string GetManifestFilePath()
	{
		string fileName = Path.GetFileName(new Uri(this.m_ManifestURL).AbsolutePath);
		return Path.Combine(this.m_DirPath, fileName);
	}

	private Manifest GetManifest()
	{
		string manifestFilePath = this.GetManifestFilePath();
		Logger.Info("Downloading " + this.m_ManifestURL + " to " + manifestFilePath);
		bool downloaded = false;
		Exception capturedException = null;
		SplitDownloader.DownloadFile(this.m_ManifestURL, manifestFilePath, this.m_UserAgent, delegate(long downloadedSize, long totalSize)
		{
			Logger.Info("Downloaded (" + downloadedSize.ToString() + " bytes) out of " + totalSize.ToString());
		}, delegate(string filePath)
		{
			downloaded = true;
			Logger.Info("Downloaded " + this.m_ManifestURL + " to " + filePath);
		}, delegate(Exception e)
		{
			downloaded = false;
			capturedException = e;
			Logger.Error(e.ToString());
		});
		if (!downloaded)
		{
			throw capturedException;
		}
		Manifest manifest = new Manifest(manifestFilePath);
		manifest.Build();
		return manifest;
	}

	private void DownloadFilePart(FilePart filePart)
	{
		string filePartURL = filePart.URL(this.m_ManifestURL);
		Logger.Info("Downloading " + filePartURL + " to " + filePart.Path);
		bool downloaded = false;
		Exception capturedException = null;
		SplitDownloader.DownloadFile(filePartURL, filePart.Path, this.m_UserAgent, delegate(long downloadedSize, long totalSize)
		{
			filePart.DownloadedSize = downloadedSize;
			if (this.m_PercentDownloaded != this.m_Manifest.PercentDownloaded())
			{
				this.m_ProgressCb(this.m_Manifest.PercentDownloaded());
			}
			this.m_PercentDownloaded = this.m_Manifest.PercentDownloaded();
		}, delegate(string filePath)
		{
			downloaded = true;
			Logger.Info("Downloaded " + filePartURL + " to " + filePart.Path);
		}, delegate(Exception e)
		{
			downloaded = false;
			capturedException = e;
			Logger.Error(e.ToString());
		});
		if (!downloaded)
		{
			throw capturedException;
		}
	}

	private static void DownloadFile(string url, string filePath, string userAgent, SplitDownloader.DownloadFileProgressCb progressCb, SplitDownloader.DownloadFileCompletedCb completedCb, SplitDownloader.DownloadFileExceptionCb exceptionCb)
	{
		FileStream fileStream = null;
		HttpWebResponse httpWebResponse = null;
		Stream stream = null;
		bool flag = false;
		try
		{
			if (File.Exists(filePath))
			{
				File.Delete(filePath);
			}
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.UserAgent = userAgent;
			httpWebRequest.KeepAlive = false;
			httpWebRequest.ReadWriteTimeout = 60000;
			httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
			long contentLength = httpWebResponse.ContentLength;
			stream = httpWebResponse.GetResponseStream();
			Logger.Warning(string.Format(CultureInfo.InvariantCulture, "HTTP Response Header\nStatusCode: {0}\n{1}", new object[]
			{
				(int)httpWebResponse.StatusCode,
				httpWebResponse.Headers
			}));
			int num = 4096;
			byte[] array = new byte[num];
			long num2 = 0L;
			fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
			int num3;
			while ((num3 = stream.Read(array, 0, num)) > 0)
			{
				fileStream.Write(array, 0, num3);
				num2 += (long)num3;
				progressCb(num2, contentLength);
			}
			if (contentLength != num2)
			{
				throw new Exception(string.Format(CultureInfo.InvariantCulture, "totalContentRead({0}) != contentLength({1})", new object[] { num2, contentLength }));
			}
			flag = true;
		}
		catch (Exception ex)
		{
			Logger.Error(ex.ToString());
			exceptionCb(ex);
		}
		finally
		{
			if (stream != null)
			{
				stream.Close();
			}
			if (httpWebResponse != null)
			{
				httpWebResponse.Close();
			}
			if (fileStream != null)
			{
				fileStream.Flush();
				fileStream.Close();
				Thread.Sleep(1000);
			}
		}
		if (flag)
		{
			completedCb(filePath);
		}
	}

	private string m_ManifestURL;

	private string m_DirPath;

	private string m_UserGUID;

	private string m_UserAgent;

	private SplitDownloader.ProgressCb m_ProgressCb;

	private SplitDownloader.CompletedCb m_CompletedCb;

	private SplitDownloader.ExceptionCb m_ExceptionCb;

	private SplitDownloader.FileSizeCb m_FileSizeCb;

	private int m_NrWorkers;

	private SerialWorkQueue[] m_Workers;

	private bool m_WorkersStarted;

	private Manifest m_Manifest;

	private float m_PercentDownloaded;


	public delegate void ProgressCb(float percent);


	public delegate void CompletedCb(string filePath);


	public delegate void ExceptionCb(Exception e);


	public delegate void FileSizeCb(long fileSize);


	public delegate void DownloadFileProgressCb(long downloaded, long size);


	public delegate void DownloadFileCompletedCb(string filePath);


	public delegate void DownloadFileExceptionCb(Exception e);
}


