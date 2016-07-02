using System;
namespace BeaconScanner
{
	public enum Proximity
	{
		Near,
		Immediate,
		Far,
		Unknown
	}

	public interface IBeacon
	{
		string UUID { get; }

		double Distance { get; }

		Proximity Proximity { get; }

		int Major { get; }

		int Minor { get; }

		string Name { get; }
	}
}

