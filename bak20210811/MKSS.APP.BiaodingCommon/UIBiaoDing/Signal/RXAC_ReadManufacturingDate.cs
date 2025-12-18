using DeviceDataMonitorWPF.UIBiaoDing.Signal;
using System;

namespace DeviceDataMonitorWPF.UIBiaoDing.Signal
{
    public class RXAC_ReadManufacturingDate : ISignal
	{
		//   AD 00 设置编号，AD 01 读取产品编号
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = BitConverter.ToUInt16(new byte[] { (byte)0XAC, (byte)0X00 }, 0);
			return arr;
		}
	}
}
