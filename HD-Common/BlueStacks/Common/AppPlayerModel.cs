using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Threading;
using Newtonsoft.Json;

namespace BlueStacks.Common
{
	[Serializable]
	public class AppPlayerModel
	{
		[JsonProperty(PropertyName = "app_player_win_version", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public string AppPlayerWinVersion { get; set; }

		[JsonProperty(PropertyName = "source", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public string Source { get; set; }

		[JsonProperty(PropertyName = "app_player_os_arch", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public string AppPlayerOsArch { get; set; }

		[JsonProperty(PropertyName = "oem", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public string AppPlayerOem { get; set; }

		[JsonProperty(PropertyName = "prod_ver", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public string AppPlayerProdVer { get; set; }

		[JsonProperty(PropertyName = "app_player_language", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public string AppPlayerLanguage { get; set; }

		[JsonProperty(PropertyName = "display_name", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public string AppPlayerOemDisplayName { get; set; }

		[JsonProperty(PropertyName = "download_url", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public string DownLoadUrl { get; set; }

		[JsonProperty(PropertyName = "abi_value", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public int AbiValue { get; set; }

		[JsonProperty(PropertyName = "suffix", DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate, NullValueHandling = NullValueHandling.Include)]
		public string Suffix { get; set; }

		[JsonIgnore]
		private string DownloadPath { get; set; } = string.Empty;

		[JsonIgnore]
		private Downloader MDownloader { get; set; }

		[JsonIgnore]
		private bool IsOemDownloadCancelling { get; set; }

		public bool DownLoadOem(Downloader.DownloadExceptionEventHandler downloadException, Downloader.DownloadProgressChangedEventHandler downloadProgressChanged, Downloader.DownloadFileCompletedEventHandler downloadFileCompleted, Downloader.FilePayloadInfoReceivedHandler filePayloadInfoReceived, Downloader.UnsupportedResumeEventHandler unsupportedResume, bool isRetry = false)
		{
			try
			{
				if (!string.IsNullOrEmpty(this.DownLoadUrl))
				{
					Logger.Info("The new engine url is : " + this.DownLoadUrl);
					if (!isRetry)
					{
						string fileName = Path.GetFileName(new Uri(this.DownLoadUrl).LocalPath);
						this.DownloadPath = Path.Combine(Path.GetTempPath(), fileName);
					}
					new Thread(new ThreadStart(delegate
					{
						while (this.IsOemDownloadCancelling)
						{
							Thread.Sleep(1000);
						}
						if (isRetry || !File.Exists(this.DownloadPath))
						{
							this.MDownloader = new Downloader();
							this.MDownloader.DownloadException += this.Downloader_DownloadException;
							this.MDownloader.DownloadException += downloadException;
							this.MDownloader.DownloadProgressChanged += downloadProgressChanged;
							this.MDownloader.DownloadFileCompleted += downloadFileCompleted;
							this.MDownloader.FilePayloadInfoReceived += filePayloadInfoReceived;
							this.MDownloader.UnsupportedResume += this.Downloader_UnsupportedResume;
							this.MDownloader.UnsupportedResume += unsupportedResume;
							this.MDownloader.DownloadCancelled += this.Downloader_Cancelled;
							this.MDownloader.DownloadFile(this.DownLoadUrl, this.DownloadPath);
							return;
						}
						Downloader.DownloadFileCompletedEventHandler downloadFileCompleted2 = downloadFileCompleted;
						if (downloadFileCompleted2 == null)
						{
							return;
						}
						downloadFileCompleted2(null, null);
					})).Start();
				}
			}
			catch (Exception ex)
			{
				Logger.Error("Error while download file: {0}", new object[] { ex });
				return false;
			}
			return true;
		}

		private void Downloader_DownloadException(Exception e)
		{
			this.DeleteFiles();
		}

		private void Downloader_UnsupportedResume(HttpStatusCode sc)
		{
			this.DeleteFiles();
		}

		private void Downloader_Cancelled()
		{
			this.IsOemDownloadCancelling = false;
		}

		public void CancelOemDownload()
		{
			if (this.MDownloader != null && this.MDownloader.IsDownloadInProgress)
			{
				this.IsOemDownloadCancelling = true;
				this.MDownloader.IsDownLoadCanceled = true;
			}
		}

		private void DeleteFiles()
		{
			try
			{
				if (File.Exists(this.DownloadPath))
				{
					File.Delete(this.DownloadPath);
				}
				if (File.Exists(this.DownloadPath + ".tmp"))
				{
					File.Delete(this.DownloadPath + ".tmp");
				}
			}
			catch (Exception ex)
			{
				string text = "Error while deleting files from temp folder ";
				string downloadPath = this.DownloadPath;
				string text2 = " ";
				Exception ex2 = ex;
				Logger.Error(text + downloadPath + text2 + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		public int InstallOem()
		{
			string text = Path.Combine(Path.GetDirectoryName(RegistryManager.Instance.UserDefinedDir), "BlueStacks" + ((this.AppPlayerOem == "bgp") ? string.Empty : ("_" + this.AppPlayerOem)));
			Process process = Process.Start(new ProcessStartInfo
			{
				Arguments = string.Format(CultureInfo.InvariantCulture, "-s -pddir:\"{0}\"", new object[] { text }),
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.Hidden,
				FileName = this.DownloadPath,
				UseShellExecute = false
			});
			process.WaitForExit();
			if (process.ExitCode == 0 && RegistryManager.CheckOemInRegistry(this.AppPlayerOem, "Android"))
			{
				this.DeleteFiles();
			}
			return process.ExitCode;
		}
	}
}


