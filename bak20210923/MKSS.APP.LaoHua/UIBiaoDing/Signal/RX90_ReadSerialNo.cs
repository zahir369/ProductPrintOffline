using MKSS.APP.UIBiaoDing.Signal;

namespace MKSS.APP.UIBiaoDing.Signal
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
