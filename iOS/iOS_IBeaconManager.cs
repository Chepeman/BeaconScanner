using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreLocation;
using Estimote;
using UIKit;

namespace BeaconScanner.iOS
{
	public class iOS_IBeaconManager:IBeaconManager
	{
		readonly BeaconManager beaconManager;
		
		public iOS_IBeaconManager()
		{
			_regions = new List<CLBeaconRegion>();

			this.beaconManager = new BeaconManager
			{
				ReturnAllRangedBeaconsAtOnce = true
			};

			this.beaconManager.RangedBeacons += (sender, args) =>
			{
				var region = args.Region?.Identifier;
				var beacons = args.Beacons.Select(x => new Beacon(x.Rssi, x.Major.UInt16Value, x.Minor.UInt16Value, region ?? x.ProximityUuid.AsString(), GetProximity(x.Proximity), x.ProximityUuid.AsString(), GetDistance(x)));
				beacons = beacons.Where(x => x.Proximity != Proximity.Unknown);
				_updateBeacons?.Invoke(beacons);
			};
		}

		Proximity GetProximity(CLProximity prox)
		{
			switch (prox) {
				case CLProximity.Far:
					return Proximity.Far;
				case CLProximity.Immediate:
					return Proximity.Immediate;
				case CLProximity.Near:
					return Proximity.Near;
				case CLProximity.Unknown:
					return Proximity.Unknown;
				default:
					return Proximity.Unknown;
			}
		}

		Action<IEnumerable<IBeacon>> _updateBeacons;

		bool IsGoodStatus(CLAuthorizationStatus status)
		{
			return (
				status == CLAuthorizationStatus.Authorized ||
				status == CLAuthorizationStatus.AuthorizedAlways ||
				status == CLAuthorizationStatus.AuthorizedWhenInUse
			);
		}


		List<CLBeaconRegion> _regions;

		private double GetDistance(CLBeacon beacon)
		{
			double distance = -1;

			distance = beacon.Accuracy;

			return Math.Round(distance, 2, MidpointRounding.AwayFromZero);
		}

		#region IBeaconManager

		public void AddRegion(string id, string uuid, int major = -1, int minor = -1)
		{
			CLBeaconRegion region;
			if (major > 0 && minor > 0)
				region = new CLBeaconRegion(new Foundation.NSUuid(uuid), (ushort)major, (ushort)minor, id);
			else if (major > 0)
				region = new CLBeaconRegion(new Foundation.NSUuid(uuid), (ushort)major, id);
			else
				region = new CLBeaconRegion(new Foundation.NSUuid(uuid), id);
			region.NotifyEntryStateOnDisplay = true;
			region.NotifyOnEntry = true;
			region.NotifyOnExit = true;
			_regions.Add(region);
		}

		public async Task<bool> Init()
		{
			if (!UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
				return false;

			if (!CLLocationManager.LocationServicesEnabled)
				return false;

			var preau = this.IsGoodStatus(BeaconManager.AuthorizationStatus);
			if (preau)
			{
				return preau;
			}
			var tcs = new TaskCompletionSource<bool>();
			var funcPnt = new EventHandler<AuthorizationStatusChangedEventArgs>((sender, args) =>
			{
				if (args.Status == CLAuthorizationStatus.NotDetermined)
					return;
				var success = this.IsGoodStatus(args.Status);
				tcs.TrySetResult(success);
			});
			this.beaconManager.AuthorizationStatusChanged += funcPnt;
			Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
			{
				this.beaconManager.RequestAlwaysAuthorization();
			});
			var status = await tcs.Task;
			this.beaconManager.AuthorizationStatusChanged -= funcPnt;
			return status;
		}

		public void SetRangedAction(Action<IEnumerable<IBeacon>> action)
		{
			_updateBeacons = action;
		}

		public void StartScan()
		{
			foreach (var region in _regions)
			{
				this.beaconManager.StartRangingBeaconsInRegion(region);
			}
		}

		public void StopScan()
		{
			foreach (var region in _regions)
			{
				this.beaconManager.StopRangingBeaconsInRegion(region);
			}
			_regions.Clear();
		}

		#endregion
	}
}

