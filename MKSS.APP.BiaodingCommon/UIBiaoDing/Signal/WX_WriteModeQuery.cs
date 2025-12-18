using MKSS.APP.UIBiaoDing.Signal;

namespace MKSS.APP.UIBiaoDing.Signal
{
    public class WX_WriteModeQuery : ISignal
	{

		/// <summary>
		///  命令词
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0X78); } }
		public bool IsQueryMode {
			get;set;
		}

		//0x78-切换主被动上传模式（通讯模式：0x03-主动上传；0x04-问询模式）
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = (ushort)(int)Signal;
			arr[1] = (ushort)(IsQueryMode?0x04:0x03);
			arr[2] = 0;
			return arr;
		}

	}
}
