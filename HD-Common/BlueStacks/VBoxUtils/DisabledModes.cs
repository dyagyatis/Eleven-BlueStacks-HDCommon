using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "DisabledModes", Namespace = "http://www.virtualbox.org/")]
	public class DisabledModes
	{
		[XmlElement(ElementName = "InternalNetwork", Namespace = "http://www.virtualbox.org/")]
		public InternalNetwork InternalNetwork { get; set; }

		[XmlElement(ElementName = "NATNetwork", Namespace = "http://www.virtualbox.org/")]
		public NATNetwork NATNetwork { get; set; }
	}
}


