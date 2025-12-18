using DeviceDataMonitorWPF.UIBiaoDing.Signal;
using System;

namespace DeviceDataMonitorWPF.UIBiaoDing.Signal
{
	public class WXA8_WriteLedStatus : ISignal
	{
		public ushort[] LedStatus
		{
			get;
			set;
		}

		public ushort[] GetSignalBytes()
		{
			return new ushort[]
			{
				168,
				this.LedStatus[0],
				this.LedStatus[1],
				this.LedStatus[2],
				this.LedStatus[3]
			};
		}
	}
}
