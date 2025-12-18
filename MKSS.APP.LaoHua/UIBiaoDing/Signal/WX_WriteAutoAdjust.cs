using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{
	public class WX_WriteAutoAdjust : ISignal
	{
		/// <summary>
		///  √¸¡Ó¥ 
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0X79); } }
		public bool Open
		{
			get;
			set;
		}

		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = (ushort)(int)Signal;
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
