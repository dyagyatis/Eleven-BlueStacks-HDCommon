using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "SharedFolders", Namespace = "http://www.virtualbox.org/")]
	public class SharedFolders
	{
		[XmlElement(ElementName = "SharedFolder", Namespace = "http://www.virtualbox.org/")]
		public List<SharedFolder> SharedFolder { get; set; }
	}
}


