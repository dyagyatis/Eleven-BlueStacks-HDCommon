using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace BlueStacks.Common
{
	public static class VmCmdHandler
	{
		public static void SyncConfig(string keyMapParserVersion, string vmName)
		{
			VmCmdHandler.RunCommand("settime " + ((DateTime.UtcNow.Ticks - 621355968000000000L) / 10000L).ToString(), vmName);
			Utils.SetTimeZoneInGuest(vmName);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string currentKeyboardLayout = Utils.GetCurrentKeyboardLayout();
			dictionary.Add("keyboardlayout", currentKeyboardLayout);
			string text = "setkeyboardlayout";
			Logger.Info("Sending request for " + text + " with data : ");
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				Logger.Info("key : " + keyValuePair.Key + " value : " + keyValuePair.Value);
			}
			JObject jobject;
			string text2 = VmCmdHandler.SendRequest(text, dictionary, vmName, out jobject);
			if (text2 == null || text2.Contains("error"))
			{
				Logger.Info("Failed to set keyboard layout in sync config...checking for latinime");
				try
				{
					if (Utils.IsLatinImeSelected(vmName))
					{
						HTTPUtils.SendRequestToEngine("setPcImeWorkflow", null, vmName, 0, null, false, 1, 0, "");
					}
					else if (Oem.Instance.IsSendGameManagerRequest)
					{
						HTTPUtils.SendRequestToClient("showIMESwitchPrompt", null, vmName, 0, null, false, 1, 0, "bgp64");
					}
				}
				catch (Exception ex)
				{
					Logger.Warning("Error in informing engine/client. Ex: " + ex.Message);
				}
			}
			if (VmCmdHandler.RunCommand("setkeymappingparserversion " + keyMapParserVersion, vmName) == null)
			{
				Logger.Error("setkeymappingparserversion did not work, will try again on frontend restart");
				return;
			}
			RegistryManager.Instance.Guest[vmName].ConfigSynced = 1;
		}

		public static void SetMachineType(bool isDesktop, string vmName)
		{
			string text;
			if (isDesktop)
			{
				text = "isdesktop";
			}
			else
			{
				text = "istablet";
			}
			VmCmdHandler.RunCommand(text, vmName);
		}

		public static void SetKeyboard(bool isDesktop, string vmName)
		{
			string text;
			if (isDesktop)
			{
				text = "usehardkeyboard";
			}
			else
			{
				text = "usesoftkeyboard";
			}
			VmCmdHandler.RunCommand(text, vmName);
		}

		public static string FqdnSend(int port, string serverIn, string vmName)
		{
			string text3;
			try
			{
				string text;
				if (string.Compare(serverIn, "agent", StringComparison.OrdinalIgnoreCase) == 0)
				{
					text = "setWindowsAgentAddr";
				}
				else
				{
					if (string.Compare(serverIn, "frontend", StringComparison.OrdinalIgnoreCase) != 0)
					{
						Logger.Error("Unknown server: " + serverIn);
						return null;
					}
					text = "setWindowsFrontendAddr";
				}
				if (port == 0)
				{
					if (string.Compare(serverIn, "agent", StringComparison.OrdinalIgnoreCase) == 0)
					{
						port = RegistryManager.Instance.AgentServerPort;
					}
					else if (string.Compare(serverIn, "frontend", StringComparison.OrdinalIgnoreCase) == 0)
					{
						port = RegistryManager.Instance.Guest[vmName].FrontendServerPort;
					}
				}
				string text2 = "10.0.2.2:" + port.ToString(CultureInfo.InvariantCulture);
				text3 = VmCmdHandler.RunCommand(string.Format(CultureInfo.InvariantCulture, "{0} {1}", new object[] { text, text2 }), vmName);
			}
			catch (Exception ex)
			{
				Logger.Error("Exception when sending fqdn post request: " + ex.Message);
				text3 = null;
			}
			return text3;
		}

		public static string RunCommand(string cmd, string vmName)
		{
			int num = -1;
			if (cmd != null)
			{
				num = cmd.IndexOf(' ');
			}
			string text;
			string text2;
			if (num == -1)
			{
				text = cmd;
				text2 = "";
			}
			else
			{
				text = ((cmd != null) ? cmd.Substring(0, num) : null);
				text2 = ((cmd != null) ? cmd.Substring(num + 1) : null);
			}
			Logger.Info("Will send command: {0} to {1}", new object[] { text2, text });
			Dictionary<string, string> dictionary = new Dictionary<string, string> { { "arg", text2 } };
			JObject jobject;
			return VmCmdHandler.SendRequest(text, dictionary, vmName, out jobject);
		}

		public static string SendRequest(string path, Dictionary<string, string> data, string vmName, out JObject response)
		{
			int i = 60;
			int num = 3;
			response = null;
			while (i > 0)
			{
				try
				{
					if (num != 0)
					{
						num--;
					}
					string text;
					if (path == "runex" || path == "run" || path == "powerrun")
					{
						text = HTTPUtils.SendRequestToGuest(path, data, vmName, 3000, null, false, 1, 0, "bgp64");
					}
					else if (path == "setWindowsAgentAddr")
					{
						text = HTTPUtils.SendRequestToGuest(path, data, vmName, 1000, null, false, 1, 0, "bgp64");
					}
					else
					{
						text = HTTPUtils.SendRequestToGuest(path, data, vmName, 0, null, false, 1, 0, "bgp64");
					}
					Logger.Info("Got response for {0}: {1}", new object[] { path, text });
					response = JObject.Parse(text);
					VmCmdHandler.s_Received = (string)response["result"];
					if (VmCmdHandler.s_Received != "ok" && VmCmdHandler.s_Received != "error")
					{
						VmCmdHandler.s_Received = null;
					}
				}
				catch (Exception ex)
				{
					if (num != 0)
					{
						Logger.Error("Exception in SendRequest for {0}: {1}", new object[] { path, ex.Message });
					}
					VmCmdHandler.s_Received = null;
				}
				i--;
				if (VmCmdHandler.s_Received != null)
				{
					return VmCmdHandler.s_Received;
				}
				if (i > 0)
				{
					Thread.Sleep(1000);
				}
			}
			return null;
		}

		public static void RunCommandAsync(string cmd, UIHelper.Action continuation, Control control, string vmName)
		{
			new global::System.Threading.Timer(delegate(object obj)
			{
				VmCmdHandler.RunCommand(cmd, vmName);
				if (continuation != null)
				{
					UIHelper.RunOnUIThread(control, continuation);
				}
			}, null, 0, -1);
		}

		private static string s_Received;
	}
}


