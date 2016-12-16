using System;
using System.Net.NetworkInformation;

namespace Manatee.Trello.Internal.RequestProcessing
{
	internal static class NetworkMonitor
	{
		public static bool IsConnected { get; private set; }

		public static event System.Action ConnectionStatusChanged;

		static NetworkMonitor()
		{
			IsConnected = NetworkInterface.GetIsNetworkAvailable();
#if IOS || CORE
			NetworkChange.NetworkAddressChanged += HandleNetworkAvailabilityChange;
#else
			NetworkChange.NetworkAvailabilityChanged += HandleNetworkAvailabilityChange;
#endif
		}

#if IOS || CORE
		private static void HandleNetworkAvailabilityChange(object sender, EventArgs eventArgs)
		{
			var isAvailable = NetworkInterface.GetIsNetworkAvailable();
			if (IsConnected == isAvailable) return;
			IsConnected = isAvailable;
			var handler = ConnectionStatusChanged;
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