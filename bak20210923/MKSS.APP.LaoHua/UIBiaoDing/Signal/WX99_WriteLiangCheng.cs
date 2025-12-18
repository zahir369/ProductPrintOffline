using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{
	public class WX99_WriteLiangCheng : ISignal
	{
		public int LiangCheng
		{
			get;
			set;
		}

		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = 153;
			arr[1] = (ushort)(this.LiangCheng / 65536);
			arr[2] = (ushort)(this.LiangCheng % 65536);
			return arr;
		}
	}
}
