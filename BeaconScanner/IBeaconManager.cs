using System;
using System.Collections.Generic;

namespace BeaconScanner
{
	public interface IBeaconManager
	{
		bool Init();

		void AddRegion(string id, string uuid, int major = -1, int minor = -1);

		void StartScan();

		void StopScan();

		void SetRangedAction(Action<IEnumerable<IBeacon>> action);
	}
}

