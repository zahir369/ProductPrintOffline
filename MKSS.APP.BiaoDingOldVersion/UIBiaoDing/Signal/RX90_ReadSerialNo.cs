using DeviceDataMonitorWPF.UIBiaoDing.Signal;

namespace DeviceDataMonitorWPF.UIBiaoDing.Signal
{
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
