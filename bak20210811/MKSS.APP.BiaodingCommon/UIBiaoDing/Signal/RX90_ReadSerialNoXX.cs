using DeviceDataMonitorWPF.UIBiaoDing.Signal;

namespace DeviceDataMonitorWPF.UIBiaoDing.Signal
{

	// 90 - 93 是读取其他数据
    public class RX90_ReadSerialNo : ISignal
	{
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = 144;
			return arr;
		}
	}
}
