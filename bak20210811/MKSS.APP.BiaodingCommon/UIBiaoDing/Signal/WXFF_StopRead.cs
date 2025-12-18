using DeviceDataMonitorWPF.UIBiaoDing.Signal;
using System;

namespace DeviceDataMonitorWPF.UIBiaoDing.Signal
{

	/// <summary>
	///   Çå¿Õ×ÜÏß
	/// </summary>
	public class WXFF_StopRead : ISignal
	{
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = 255;
			return arr;
		}
	}
}
