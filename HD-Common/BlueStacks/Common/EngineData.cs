using System;

namespace BlueStacks.Common
{
	public class EngineData
	{
		public GraphicsMode GraphicsMode { get; set; }

		public bool UseAdvancedGraphicEngine { get; set; }

		public bool UseDedicatedGPU { get; set; }

		public ASTCOption ASTCOption { get; set; }

		public int Ram { get; set; }

		public int CpuCores { get; set; }

		public int FrameRate { get; set; }

		public bool EnableHighFrameRates { get; set; }

		public bool DisplayFPS { get; set; }

		public bool AutoLockFps { get; set; }

		public bool EnableAdb { get; set; }


		public int SelectedAutoLockFps { get; set; }
		public int SelectedUnlockFps { get; set; }

		public ABISetting ABISetting { get; set; }

		public string AutoUnlockKey { get; set; } = "";
		public bool AutoUnlockEnabled { get; set; }
	}
}


