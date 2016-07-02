using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BeaconScanner
{
	public interface IBeaconManager
	{
		Task<bool> Init();

		void AddRegion(string id, string uuid, int major = -1, int minor = -1);

		void StartScan();

		void StopScan();

		void SetRangedAction(Action<IEnumerable<IBeacon>> action);
	}
}

