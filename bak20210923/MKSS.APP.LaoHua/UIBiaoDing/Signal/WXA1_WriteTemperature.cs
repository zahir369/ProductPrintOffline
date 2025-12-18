using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{
	public class WXA1_WriteTemperature : ISignal
	{
		public ushort Temperature
		{
			get;
			set;
		}

		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = 161;
			arr[1] = this.Temperature;
			return arr;
		}
	}
}
