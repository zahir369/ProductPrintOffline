using MKSS.Service.UIBiaoDing;
using System;
using System.Configuration;

namespace MKSS.APP.UIBiaoDing.Signal
{


	public interface ISignal
	{
		byte Signal { get; }
		ushort[] GetSignalBytes();
	}

	public class SignalUtil {

		/// <summary>
		///  指令格式化，转化混淆
		/// </summary>
		/// <param name="srcArr"></param>
		/// <returns></returns>
		public static byte Confuse(params byte[] srcArr) {
            switch (SerialNoType)
			{
				case SerialNoTypeEnum.USER_DEF://对外发布指令转换
											   //	if (srcArr[0] == 0X01) return 0X11;
											   //	if (srcArr[0] == 0XFF) return 0XEE;
											   //	return (byte)((int)srcArr[0]-63);//偏移 63 十六进制3F
				case SerialNoTypeEnum.SCRWD://公司内部直接应用
				default://公司内部直接应用
					return srcArr[0];
			}
        }

		/// <summary>
		///  编号规则，发布类型
		/// </summary>
		public static SerialNoTypeEnum SerialNoType
		{
			get
			{
				SerialNoTypeEnum ret = SerialNoTypeEnum.SCRWD;
				string SerialNoType = ConfigurationManager.AppSettings["SerialNoType"];
				Enum.TryParse<SerialNoTypeEnum>(SerialNoType, true, out ret);
				return ret;
			}
		}

	}
}
