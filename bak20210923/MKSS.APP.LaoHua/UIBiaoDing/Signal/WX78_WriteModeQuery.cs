using MKSS.APP.UIBiaoDing.Signal;

namespace MKSS.APP.UIBiaoDing.Signal
{
    public class WX78_WriteModeQuery : ISignal
	{
		public bool IsQueryMode {
			get;set;
		}

		//0x78-切换主被动上传模式（通讯模式：0x03-主动上传；0x04-问询模式）
		public ushort[] GetSignalBytes()
		{
			ushort[] arr = new ushort[5];
			arr[0] = 120;
			arr[1] = (ushort)(IsQueryMode?0x04:0x03);
			arr[2] = 0;
			return arr;
		}
	}
}
