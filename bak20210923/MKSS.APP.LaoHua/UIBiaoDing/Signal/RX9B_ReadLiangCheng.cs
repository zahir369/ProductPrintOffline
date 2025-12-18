using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{
	public class RX9B_ReadLiangCheng : ISignal
	{
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = 155;
			return arr;
		}
	}
}
