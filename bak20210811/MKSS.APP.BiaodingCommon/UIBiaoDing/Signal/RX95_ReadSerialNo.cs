using DeviceDataMonitorWPF.UIBiaoDing.Signal;
using System;

namespace DeviceDataMonitorWPF.UIBiaoDing.Signal
{
    public class RX95_ReadSerialNo : ISignal
	{
		//   AD 00 设置编号，AD 01 读取产品编号
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = BitConverter.ToUInt16(new byte[] { (byte)0X95, (byte)0X00 }, 0);
			return arr;
		}
	}
}
