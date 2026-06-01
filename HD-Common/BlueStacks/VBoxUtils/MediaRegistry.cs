using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "MediaRegistry", Namespace = "http://www.virtualbox.org/")]
	public class MediaRegistry
	{
		[XmlElement(ElementName = "HardDisks", Namespace = "http://www.virtualbox.org/")]
		public HardDisks HardDisks { get; set; }
	}
}


