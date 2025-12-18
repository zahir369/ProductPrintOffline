using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{
    public class WX_WriteAdjustZero : ISignal
	{
		/// <summary>
		///  ÃüÁî´Ê
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0X87); } }
		public int Zero
		{
			get;
			set;
		}
		//0x87	Ð£×¼Áãµã(ZERO)
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[7];
			arr[0] = (ushort)(int)Signal;
			arr[1] = (ushort)(this.Zero / 256);
			arr[2] = (ushort)(this.Zero % 256);
			arr[6] = (ushort)(int)Config.BiaoDingConfgig.Instance.GasTunnel;
			return arr;
		}
	}
}
