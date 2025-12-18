using System;

namespace MKSS.Service.UIBiaoDing
{


	/// <summary>
	/// 报警器设备编码是指单只报警器的编号，包括1位厂家编号+12位规定的数字+1 位校验位
	/// 金卡自产产品，厂家编号为空。"1"∶河南九格云天网络科技有限公司
	/// 
	/// 通讯方式∶0∶无通讯;1∶带NB 通讯功能;2∶带蓝牙通讯功能
	/// 输出方式∶0∶无输出;1∶有源;2∶无源;3∶有源+无源;……·其余待扩展
	/// 校验位∶该校验位根据ISO7064，MOD11，10校验系统校验码计算方法生成（依据前12/13 位数据 进行运算），X可以是数字0 ~9中的任一数字
	/// 1位厂家编号+报警器气体类型+年份+月份+通讯方式+输出方式+顺序号+1 位校验位
	/// 1
	/// </summary>
	public class SerialNoParserJK : SerialNoParser
	{

		/// <summary> 
		/// </summary>
		/// <param name="str"></param>
		public SerialNoParserJK(string str, SerialNoRule ruleDef, SerialNoNet nb) {
			StrValue = str;
			DateTime = DateTime.Now;
			Rule = ruleDef;
			Net=nb;	
		}
		
		public SerialNoRule Rule { get; set; }
		public SerialNoNet Net { get; set; }
		public DateTime DateTime { get; set; }

		/// <summary>
		/// 报警器气体类型∶1∶甲烷;2: CO;3∶甲烷+CO;……·其余待扩展
		/// </summary>
		public int RuleGasType
		{
			get
			{
				switch (Rule.Rule)
				{
					case SerialNoRuleEnnum.JK_12_CH4:
						return 1;
					case SerialNoRuleEnnum.JK_12_CO:
						return 2;
					case SerialNoRuleEnnum.JK_12_CH4_CO:
						return 3;
					default:
					case SerialNoRuleEnnum.COMMON_12:
						return 0;
				}
			}
		}

		/// <summary>
		/// 通讯方式∶0∶无通讯;1∶带NB 通讯功能;2∶带蓝牙通讯功能
		/// </summary>
		public int NetType
		{
			get
			{
                switch (Net.Net)
                {
                    case SerialNoNetEnnum.NB:
						return 1;
					case SerialNoNetEnnum.NO_NET:
					default:
						return 0;
				}
            }
		}
		 
		/// <summary>
		///  1位厂家编号+报警器气体类型+年份+月份+通讯方式+输出方式+顺序号+1 位校验位
		/// </summary>
		public override string Prefix {
			get { 
				return string.Format("{0}{1}{2}{3}{4}{5}",
					1, 
					RuleGasType.ToString("0"), 
					(this.DateTime.Year - 2000).ToString("00"),
					(this.DateTime.Month).ToString("00"),
					NetType.ToString("0"),
					2.ToString("0")); 
			} 
		}
		public override ulong GetMin() {
			return ulong.Parse(Prefix  + "000000");
		}
		public override ulong GetMax()
		{
			return ulong.Parse(Prefix  + "999999");
		}
		public override ulong GetNext(ulong seed)
		{
			seed = seed - 1;
			seed = seed / 10;
			seed = seed + 1;
			//校验位∶该校验位根据ISO7064，MOD11，10校验系统校验码计算方法生成（依据前12/13 位数据 进行运算），X可以是数字0 ~9中的任一数字
			var r11 = MKSS.Service.UIBiaoDing.ISO7064.CalculateHybridSystemCheckDigit(seed.ToString(), ISO7064.NumericCharSet);
			return ulong.Parse(r11);
		}

	}
}
 
