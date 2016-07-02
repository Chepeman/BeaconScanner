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
		List<IBeacon> _beaconsFoundList;

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
				_beaconsFoundList = new List<IBeacon>();
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
			throw new NotImplementedException();
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
			var Nearnables = e.Beacons;

		}
	}
}

