using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "Boot", Namespace = "http://www.virtualbox.org/")]
	public class Boot
	{
		[XmlElement(ElementName = "Order", Namespace = "http://www.virtualbox.org/")]
		public List<Order> Order { get; set; }
	}
}


