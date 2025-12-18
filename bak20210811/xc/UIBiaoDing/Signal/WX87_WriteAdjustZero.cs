using DeviceDataMonitorWPF.UIBiaoDing.Signal;
using System;

namespace DeviceDataMonitorWPF.UIBiaoDing.Signal
{
    public class WX87_WriteAdjustZero : ISignal
	{
		public int Zero
		{
			get;
			set;
		}
		//0x87	Ð£×¼Áãµã(ZERO)
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = 135;
			arr[1] = (ushort)(this.Zero / 256);
			arr[2] = (ushort)(this.Zero % 256);
			return arr;
		}
	}
}
