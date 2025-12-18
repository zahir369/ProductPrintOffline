using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{
	public class RX_ReadLiangCheng : ISignal
	{
		/// <summary>
		///  √¸¡Ó¥ 
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0X9B); } }
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = (ushort)(int)Signal;
			return arr;
		}
	}
}
