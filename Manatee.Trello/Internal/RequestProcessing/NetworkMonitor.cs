using System;
using System.Net.NetworkInformation;

namespace Manatee.Trello.Internal.RequestProcessing
{
	internal static class NetworkMonitor
	{
		public static bool IsConnected { get; private set; }

#if IOS
		private static System.Action _connectionStatusChangedInvoker;

		public static event System.Action ConnectionStatusChanged
		{
			add { _connectionStatusChangedInvoker += value; }
			remove { _connectionStatusChangedInvoker -= value; }
		}
#else
		public static event System.Action ConnectionStatusChanged;
#endif
		static NetworkMonitor()
		{
			IsConnected = NetworkInterface.GetIsNetworkAvailable();
#if IOS
			NetworkChange.NetworkAddressChanged += HandleNetworkAvailabilityChange;
#else
			NetworkChange.NetworkAvailabilityChanged += HandleNetworkAvailabilityChange;
#endif
		}

#if IOS
		private static void HandleNetworkAvailabilityChange(object sender, EventArgs eventArgs)
		{
			var isAvailable = NetworkInterface.GetIsNetworkAvailable();
			if (IsConnected == isAvailable) return;
			IsConnected = isAvailable;
			var handler = _connectionStatusChangedInvoker;
			handler?.Invoke();
		}
#else
		private static void HandleNetworkAvailabilityChange(object sender, NetworkAvailabilityEventArgs e)
		{
			if (IsConnected == e.IsAvailable) return;
			IsConnected = e.IsAvailable;
			var handler = ConnectionStatusChanged;
			handler?.Invoke();
		}
#endif
	}
}