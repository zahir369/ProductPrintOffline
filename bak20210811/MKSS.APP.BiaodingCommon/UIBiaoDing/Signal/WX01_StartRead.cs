using DeviceDataMonitorWPF.UIBiaoDing.Signal;
using System;

namespace DeviceDataMonitorWPF.UIBiaoDing.Signal
{

	/// <summary>
	///  84 85 86 ¶Áµ¥ÆøÌåÃüÁî
	/// </summary>
	public class WX01_StartRead : ISignal
	{
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = 1;
			return arr;
		}
	}

}
