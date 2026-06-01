using System;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;

namespace BlueStacks.Common
{
	public static class BlueStacksUtils
	{
		public static bool IsSignedByBlueStacks(string filePath)
		{
			Logger.Info("Checking if {0} is signed", new object[] { filePath });
			bool flag = false;
			try
			{
				X509Certificate2 x509Certificate = new X509Certificate2(X509Certificate.CreateFromSignedFile(filePath));
				string name = x509Certificate.IssuerName.Name;
				Logger.Debug("Certificate issuer name is: " + name);
				string name2 = x509Certificate.SubjectName.Name;
				Logger.Debug("Certificate issued by: " + name2);
				if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(name2, "Bluestack Systems, Inc.", CompareOptions.IgnoreCase) >= 0)
				{
					Logger.Info("File signed by BlueStacks");
					if (x509Certificate.Verify())
					{
						Logger.Info("Certificate verified");
						flag = true;
					}
					else
					{
						Logger.Warning("Certificate not verified");
					}
				}
				else
				{
					Logger.Warning("File not signed by BlueStacks. Signed by {0}", new object[] { name2 });
				}
			}
			catch (Exception ex)
			{
				Logger.Error("File not signed");
				Logger.Error(ex.ToString());
			}
			return flag;
		}
	}
}


