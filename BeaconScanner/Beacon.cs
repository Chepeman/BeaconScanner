using System;
namespace BeaconScanner
{
	public class Beacon : IBeacon
	{
		internal Beacon(double rssi, int major, int minor, string name, Proximity proximity, string uuid)
		{
			_rssi = rssi;
			_major = major;
			_minor = minor;
			_name = name;
			_proximity = proximity;
			_uuid = uuid;
		}

		double _rssi;

		int _major;

		int _minor;

		string _name;

		Proximity _proximity;

		string _uuid;

		#region IBeacon

		public double Distance => _rssi;

		public int Major => _major;

		public int Minor => _minor;

		public string Name => _name;

		public Proximity Proximity => _proximity;

		public string UUID => _uuid;

		#endregion
	}
}

