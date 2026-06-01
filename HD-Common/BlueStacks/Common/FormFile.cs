using System;
using System.IO;

namespace BlueStacks.Common
{
	public class FormFile
	{
		public string Name { get; set; }

		public string ContentType { get; set; }

		public string FilePath { get; set; }

		public Stream Stream { get; set; }
	}
}


