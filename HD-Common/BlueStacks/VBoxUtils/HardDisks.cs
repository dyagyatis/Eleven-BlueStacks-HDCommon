using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "HardDisks", Namespace = "http://www.virtualbox.org/")]
	public class HardDisks
	{
		[XmlElement(ElementName = "HardDisk", Namespace = "http://www.virtualbox.org/")]
		public List<HardDisk> HardDisk { get; set; }
	}
}


