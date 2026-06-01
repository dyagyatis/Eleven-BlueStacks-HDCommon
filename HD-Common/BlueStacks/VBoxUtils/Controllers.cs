using System;
using System.Xml.Serialization;

namespace BlueStacks.VBoxUtils
{
	[XmlRoot(ElementName = "Controllers", Namespace = "http://www.virtualbox.org/")]
	public class Controllers
	{
		[XmlElement(ElementName = "Controller", Namespace = "http://www.virtualbox.org/")]
		public Controller Controller { get; set; }
	}
}


