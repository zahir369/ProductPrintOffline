using MKSS.APP.UIBiaoDing.Signal;

namespace MKSS.APP.UIBiaoDing.Signal
{

	// 是读取其他数据
    public class RX_ReadSerialNoXX : ISignal
	{
		/// <summary>
		///  命令词
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0X90); } }
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = (ushort)(int)Signal;
			return arr;
		}
	}

}
