using MKSS.APP.UIBiaoDing.Signal;
using System;

namespace MKSS.APP.UIBiaoDing.Signal
{
    public class RX_ReadDeviceDateTime : ISignal
	{
		/// <summary>
		///  √¸¡Ó¥ 
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0XB2); } }
		//   AD 00 …Ë÷√±‡∫≈£¨AD 01 ∂¡»°≤˙∆∑±‡∫≈
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = BitConverter.ToUInt16(new byte[] { (byte)Signal, (byte)0X00 }, 0);
			return arr;
		}
	}
}
