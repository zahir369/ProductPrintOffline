using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{

	/// <summary>
	///   Çå¿Õ×ÜÏß
	/// </summary>
	public class WX_StopRead : ISignal
	{

		/// <summary>
		///  ÃüÁî´Ê
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0XFF); } }

		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = (ushort)(int)Signal;
			return arr;
		}
	}
}
