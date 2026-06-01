using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "Network", Namespace = "http://www.virtualbox.org/")]
	public class Network
	{
		[XmlElement(ElementName = "Adapter", Namespace = "http://www.virtualbox.org/")]
		public List<Adapter> Adapter { get; set; }
	}
}


