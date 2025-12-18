using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{
	public class WX_WriteLiangCheng : ISignal
	{

		/// <summary>
		///  √¸¡Ó¥ 
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0X99); } }

		public int LiangCheng
		{
			get;
			set;
		}

		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = (ushort)(int)Signal;
			arr[1] = (ushort)(this.LiangCheng / 65536);
			arr[2] = (ushort)(this.LiangCheng % 65536);
			return arr;
		}
	}
}
