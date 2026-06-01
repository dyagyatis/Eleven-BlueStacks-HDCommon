using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BlueStacks.Common
{
	public class FpsChangedMessage
	{
		public int NewFps { get; set; }
		public string VmName { get; set; }
	}

	public static class EventAggregator
	{
		public static void Publish<TMessageType>(TMessageType message)
		{
			Type typeFromHandle = typeof(TMessageType);
			if (EventAggregator.mSubscriber.ContainsKey(typeFromHandle))
			{
				foreach (object obj in ((IEnumerable)new List<Subscription<TMessageType>>(EventAggregator.mSubscriber[typeFromHandle].Cast<Subscription<TMessageType>>())))
				{
					((Subscription<TMessageType>)obj).Action(message);
				}
			}
		}

		public static Subscription<TMessageType> Subscribe<TMessageType>(Action<TMessageType> action)
		{
			Type typeFromHandle = typeof(TMessageType);
			Subscription<TMessageType> subscription = new Subscription<TMessageType>(action);
			IList list;
			if (!EventAggregator.mSubscriber.TryGetValue(typeFromHandle, out list))
			{
				list = new List<Subscription<TMessageType>>();
				list.Add(subscription);
				EventAggregator.mSubscriber.Add(typeFromHandle, list);
			}
			else
			{
				list.Add(subscription);
			}
			return subscription;
		}

		public static void Unsubscribe<TMessageType>(Subscription<TMessageType> subscription)
		{
			Type typeFromHandle = typeof(TMessageType);
			if (EventAggregator.mSubscriber.ContainsKey(typeFromHandle))
			{
				EventAggregator.mSubscriber[typeFromHandle].Remove(subscription);
			}
		}

		private static Dictionary<Type, IList> mSubscriber = new Dictionary<Type, IList>();
	}
}


