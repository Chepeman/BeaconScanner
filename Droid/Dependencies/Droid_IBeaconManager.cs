using System;
using System.Collections.Generic;
using EstimoteSdk;
using Android.App;
using Java.Util.Concurrent;
using System.Linq;

namespace BeaconScanner.Droid
{
	public class Droid_IBeaconManager : IBeaconManager, BeaconManager.IServiceReadyCallback
	{
		BeaconManager _beaconManager;
		Region _region;

		List<Region> _itemsList;

		Action<IEnumerable<IBeacon>> _updateBeacons;

		public Droid_IBeaconManager()
		{
		}

		public IntPtr Handle
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public void AddRegion(string id, string uuid, int major = -1, int minor = -1)
		{
			Region regionItem = null;
			if (major == -1 && minor == -1)
				regionItem = new Region(id, uuid);
			else if (major != -1 && minor != -1)
				regionItem = new Region(id, uuid, major, minor);
			else if (major != -1 && minor == -1)
				regionItem = new Region(id, uuid, major);

			if(regionItem != null)
				_itemsList.Add(regionItem);
		}

		public void Dispose()
		{
			_beaconManager.Disconnect();
			_beaconManager.Dispose();
		}

		public bool Init()
		{
			_beaconManager = new BeaconManager(Xamarin.Forms.Forms.Context as Activity);
			if (_beaconManager != null)
			{
				_beaconManager.SetBackgroundScanPeriod(TimeUnit.Seconds.ToMillis(1), 0);
				_itemsList = new List<Region>();
				return true;
			}

			return false;
		}

		public void OnServiceReady()
		{
			Console.WriteLine("Scanning mode activated");
		}

		public void SetRangedAction(Action<IEnumerable<IBeacon>> action)
		{
			_updateBeacons = action;
		}

		public void StartScan()
		{
			if (_beaconManager != null)
			{
				foreach (var regionItem in _itemsList)
				{
					_beaconManager.StartRanging(regionItem);
				}
				_beaconManager.Ranging += _beaconManager_Ranging;
			}
		}

		public void StopScan()
		{
			foreach (var regionItem in _itemsList)
			{
				_beaconManager.StopRanging(regionItem);
			}
			_beaconManager.Ranging -= _beaconManager_Ranging;
		}

		void _beaconManager_Ranging(object sender, BeaconManager.RangingEventArgs e)
		{
			var beacons = e.Beacons.Select(x => new Beacon(x.Rssi, x.Major, x.Minor, x.Name, GetProximity(x.ProximityUUID.ToString()), e.Region.Identifier));
			_updateBeacons?.Invoke(beacons);
		}

		private Proximity GetProximity(string nativeProximity)
		{
			Proximity prox;
			if (nativeProximity == Proximity.Immediate.ToString())
			{
				prox = Proximity.Immediate;
			}
			else if (nativeProximity == Proximity.Near.ToString())
			{
				prox = Proximity.Near;
			}
			else if (nativeProximity == Proximity.Far.ToString())
			{
				prox = Proximity.Far;
			}
			else {
				prox = Proximity.Unknown;
			}

			return prox;
		}
	}
}

