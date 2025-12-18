using DeviceDataMonitorWPF.UIBiaoDing.Signal;
using System;

namespace DeviceDataMonitorWPF.UIBiaoDing.Signal
{
	public class WX88_WriteSpan : ISignal
	{
		public int Span
		{
			get;
			set;
		}
		//0x88	校准跨度点(SPAN)
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = 136;
			arr[1] = (ushort)(this.Span / 256);
			arr[2] = (ushort)(this.Span % 256);
			return arr;
		}
	}
}
