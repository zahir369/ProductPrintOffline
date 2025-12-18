using DeviceDataMonitorWPF.UIBiaoDing.Signal;
using System;

namespace DeviceDataMonitorWPF.UIBiaoDing.Signal
{
	public class RX7D_ReadAutoAdjustStatus : ISignal
	{
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = 125;
			return arr;
		}
	}
}
