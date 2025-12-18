using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{

	/// <summary>
	///  ¶Áµ¥ÆøÌåÃüÁî
	/// </summary>
	public class WX_StartRead : ISignal
	{
		/// <summary>
		///  ÃüÁî´Ê
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0X01); } }
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = (ushort)(int)Signal;
			return arr;
		}
	}

}
