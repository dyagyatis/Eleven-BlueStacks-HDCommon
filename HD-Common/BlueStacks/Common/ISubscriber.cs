using System;

namespace BlueStacks.Common
{
	public interface ISubscriber
	{
		void SubscribeTag(BrowserControlTags args);

		void UnsubscribeTag(BrowserControlTags args);

		void Message(EventArgs eventArgs);
	}
}


