using MKSS.APP.UIBiaoDing.Signal;

namespace MKSS.APP.UIBiaoDing.Signal
{
    //0x86	读取气体浓度值
    public class RX86_ReadNongDu : ISignal
	{
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = 134;
			return arr;
		}
	}
}
