using System;
using System.Collections.Generic;
using EstimoteSdk;
using Android.App;
using Java.Util.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace BeaconScanner.Droid
{
	public class Droid_IBeaconManager : Java.Lang.Object, IBeaconManager, BeaconManager.IServiceReadyCallback
	{
		BeaconManager _beaconManager;
		Region _region;

		List<Region> _itemsList;

		Action<IEnumerable<IBeacon>> _updateBeacons;
		IList<Beacon> beaconsInRange;

		public Droid_IBeaconManager()
		{
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

		public async Task<bool> Init()
		{
			_beaconManager = new BeaconManager(Xamarin.Forms.Forms.Context as Activity);
			if (_beaconManager != null)
			{
				_beaconManager.SetBackgroundScanPeriod(TimeUnit.Seconds.ToMillis(1), 0);
				_itemsList = new List<Region>();
				beaconsInRange = new List<Beacon>();
				_beaconManager.Connect(this);
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
				_beaconManager.Ranging += _beaconManager_Ranging;
				foreach (var regionItem in _itemsList)
				{
					_beaconManager.StartRanging(regionItem);
				}
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
			var beacons = e.Beacons.Select(x => new Beacon(x.Rssi, x.Major, x.Minor, x.Name, GetProximity(x), x.ProximityUUID.ToString()));
			List<Beacon> copy;

			lock (this.beaconsInRange)
			{
				foreach (var beacon in beacons)
				{
					var index = this.GetIndexOfBeacon(beacon);

					if (beacon.Proximity == Proximity.Unknown)
					{
						if (index > -1)
							this.beaconsInRange.RemoveAt(index);
					}
					else {
						if (index == -1)
							this.beaconsInRange.Add(beacon);

						else {
							var b = this.beaconsInRange[index];
							this.beaconsInRange.RemoveAt(index);
							this.beaconsInRange.Insert(index, beacon);
						}
					}
				}
				copy = this.beaconsInRange.ToList();
			}
			//this.OnRanged(copy);
			_updateBeacons?.Invoke(copy);
		}


		int GetIndexOfBeacon(Beacon beacon)
		{
			for (var i = 0; i < this.beaconsInRange.Count; i++)
			{
				var b = this.beaconsInRange[i];
				if (b.UUID.Equals(beacon.UUID, StringComparison.InvariantCultureIgnoreCase) && b.Major == beacon.Major && b.Minor == beacon.Minor)
					return i;
			}
			return -1;
		}

		private Proximity GetProximity(EstimoteSdk.Beacon beacon)
		{
			var nativeProx = Utils.ComputeProximity(beacon);

			Proximity prox;
			if (nativeProx == Utils.Proximity.Immediate)
			{
				prox = Proximity.Immediate;
			}
			else if (nativeProx == Utils.Proximity.Near)
			{
				prox = Proximity.Near;
			}
			else if (nativeProx == Utils.Proximity.Far)
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

