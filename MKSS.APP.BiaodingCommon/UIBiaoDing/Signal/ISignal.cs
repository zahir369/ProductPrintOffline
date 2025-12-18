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


	public enum GasTunnel
	{
		未指定 = 0x00,//CH4
		甲烷 = 0x01,//CH4
		氨气 = 0x02,//NH3
		硫化氢 = 0x03,//H2S
		一氧化碳 = 0x04,//CO
		氧气 = 0x05,//O2
		氢气 = 0x06,//H2
		乙烷 = 0x07,//C2H6
		乙烯 = 0x08,//C2H4
		乙焕 = 0x09,//C2H2
		丙烷 = 0x0A,//C3H8
		丙烯 = 0x0B,//C3H6
		丁烷 = 0x0C,//C4H10
		丁烯 = 0x0D,//C4H8
		丁二烯 = 0x0E,//C4H6
		轻油 = 0x0F,//轻油
		重油 = 0x10,//重油
		汽油 = 0x11,//汽油
		柴油 = 0x12,//柴油
		煤油 = 0x13,//煤油
		甲醇 = 0x14,//CH30H
		乙醇 = 0x15,//C2H50H
		异丙醇 = 0x16,//(CH3)2CH0H
		甲醛 = 0x17,//HCHO
		丁醛 = 0x18,//C3H7CH0
		丙酮 = 0x19,//C3H60
		丁酮 = 0x1A,//CH3C0C2H5
		苯 = 0x1B,//苯
		甲苯 = 0x1C,//甲苯
		二甲苯 = 0x1D,//二甲苯
		苯乙烯 = 0x1E,//苯乙烯
		苯酚 = 0x1F,//苯酚
		乙醚 = 0x20,//乙醚
		二甲醚 = 0x21,//二甲醚
		石油醚 = 0x22,//石油醚
		二甲胺 = 0x23,//二甲胺
		三甲胺 = 0x24,//三甲胺
		甲酰胺 = 0x25,//甲酰胺
		四氢呋喃 = 0x26,//四氢呋喃
		醋酸乙酯 = 0x27,//醋酸乙酯
		氯代甲苯 = 0x28,//氯代甲苯
		环氧乙烷 = 0x29,//环氧乙烷
		臭氧 = 0x2A,//臭氧
		二氧化硫 = 0x2B,//二氧化硫
		二氧化氮 = 0x2C,//二氧化氮
		一氧化氮 = 0x2D,//一氧化氮
		氯化氢 = 0x2E,//氯化氢
		氰化氢 = 0x2F,//氰化氢
		二氧化碳 = 0x30,//二氧化碳
		氯气 = 0x31,//氯气
		可燃气体 = 0x32,//可燃气体
		丙稀腊 = 0x33,//C3H3N
		氟化氢 = 0x34,//HF
		磷化氢 = 0x35,//PH3
		二氧化氯 = 0x36,//CLO2
		四氢噻酚 = 0x37,//C4H8S
		碘甲烷 = 0x38,//CH3I
		三氯甲烷 = 0x39,//CHCL3
		硅烷 = 0x3A,//SiH4
		氯乙烯 = 0x3B,//C2H3CL
		光气 = 0x3C,//COCL2
		三氢化砷 = 0x3D,//AsH3
		漠化氢 = 0x3E,//HBr
		二硫化碳 = 0x3F,//CS2
		环己烷 = 0x40,//C6H12
		毒性气体 = 0x41,//毒性气体
		一甲胺 = 0x42,//一甲胺
		甲胺 = 0x43,//甲胺
		DMF = 0x44,//DMF
		有机胺 = 0x45,//有机胺
		六氟化硫 = 0x46,//SF6
		异丁烯 = 0x47,//异丁烯
		苯胺 = 0x48,//苯胺
		双氧水 = 0x49,//H2O2
		双光气 = 0x4A,//双光气
		三乙胺 = 0x4B,//三乙胺
		乙腊 = 0x4C,//乙腊
		硝酸 = 0x4D,//硝酸
		环氧氯丙烷 = 0x4E,//C3H5OCL
		二氯丙醇 = 0x4F,//C3H6CL2O
		四氯化碳 = 0x50,//CCL4

	}

}
