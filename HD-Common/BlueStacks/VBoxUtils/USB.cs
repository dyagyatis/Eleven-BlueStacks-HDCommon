using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "USB", Namespace = "http://www.virtualbox.org/")]
	public class USB
	{
		[XmlElement(ElementName = "Controllers", Namespace = "http://www.virtualbox.org/")]
		public Controllers Controllers { get; set; }
	}
}


