using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{
	public class WX79_WriteAutoAdjust : ISignal
	{
		public bool Open
		{
			get;
			set;
		}

		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = 121;
			ushort[] temp = arr;
			bool open = this.Open;
			if (open)
			{
				temp[1] = 160;
			}
			return temp;
		}
	}
}
