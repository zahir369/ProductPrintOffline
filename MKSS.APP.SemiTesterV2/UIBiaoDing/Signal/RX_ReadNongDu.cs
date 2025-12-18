using MKSS.APP.UIBiaoDing.Signal;

namespace MKSS.APP.UIBiaoDing.Signal
{
    //0x86	读取气体浓度值
    public class RX_ReadNongDu : ISignal
	{
		/// <summary>
		///  命令词
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0X86); } }
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = (ushort)(int)Signal;
			return arr;
		}
	}
}
