using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{
	public class WX_WriteSpan : ISignal
	{
		/// <summary>
		///  命令词
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0X88); } }
		public int Span
		{
			get;
			set;
		}
		//0x88	校准跨度点(SPAN)
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = (ushort)(int)Signal;
			arr[1] = (ushort)(this.Span / 256);
			arr[2] = (ushort)(this.Span % 256);
			return arr;
		}
	}
}
