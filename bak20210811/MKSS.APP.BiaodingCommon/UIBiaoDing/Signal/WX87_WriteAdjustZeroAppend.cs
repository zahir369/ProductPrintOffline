using DeviceDataMonitorWPF.UIBiaoDing.Signal;
using System;

namespace DeviceDataMonitorWPF.UIBiaoDing.Signal
{
	public class WX87_WriteAdjustZeroAppend : ISignal
	{
		public int Zero
		{
			get;
			set;
		}

		public int DifferenceValue
		{
			get;
			set;
		}

		public int TemperaturePoint
		{
			get;
			set;
		}

		public ushort[] GetSignalBytes()
		{
			return new ushort[]
			{
				135,
				(ushort)(this.Zero / 256),
				(ushort)(this.Zero % 256),
				170,
				(ushort)this.TemperaturePoint,
				(ushort)this.DifferenceValue
			};
		}
	}
}
