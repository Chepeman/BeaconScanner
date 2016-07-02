using System;
namespace BeaconScanner
{
	public class Beacon : IBeacon
	{
		internal Beacon(double rssi, int major, int minor, string name, Proximity proximity, string uuid)
		{
		}

		#region IBeacon

		public double Distance
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int Major
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public int Minor
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string Name
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Proximity Proximity
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public string UUID
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}

