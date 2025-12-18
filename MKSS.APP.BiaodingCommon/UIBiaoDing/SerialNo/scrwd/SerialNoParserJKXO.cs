using System;

namespace MKSS.Service.UIBiaoDing
{


	/// <summary>
	/// 新奥新奥定制
	/// 烟台新奥报警器编号规则(1)
	/// 型号 2位数字 由厂商自行编码提交烟台新奥维护 （金卡NB 01 金卡普通02 ）
	/// 类型 2位数字 01-普通、02-蓝牙、03-NB、04-4G
	/// 出厂年月 4位数字 2204代表22年4月，对应出厂日期记为2022-04-01
	/// 序列号	6位数字	厂家自行编制的出厂顺序号000001
	/// </summary>
	public class SerialNoParserJKXO : SerialNoParser
	{

		/// <summary>
		/// 新奥新奥定制
		/// </summary>
		/// <param name="str"></param>
		public SerialNoParserJKXO(string str, SerialNoRule ruleDef, SerialNoNet nb) {
			StrValue = str;
			DateTime = DateTime.Now;
			Rule = ruleDef;
			Net=nb;	
		}
		
		public SerialNoRule Rule { get; set; }
		public SerialNoNet Net { get; set; }
		public DateTime DateTime { get; set; }

		/// <summary>
		/// 型号由厂商自行编码提交烟台新奥维护（金卡NB01 金卡普通02）
		/// </summary>
		public int ModelType
		{
			get
			{
				switch (Net.Net)
				{
					case SerialNoNetEnnum.NB:
						return 1;
					case SerialNoNetEnnum.NO_NET:
					default:
						return 2;
				}
			}
		}

		/// <summary>
		/// 类型 2位数字 01-普通、02-蓝牙、03-NB、04-4G
		/// </summary>
		public int NetType
		{
			get
			{
                switch (Net.Net)
                {
                    case SerialNoNetEnnum.NB:
						return 3;
					case SerialNoNetEnnum.NO_NET:
					default:
						return 1;
				}
            }
		}

		/// <summary>
		///  型号+类型+出厂年+月,
		///  前面特定加 1 回避 13 位的问题
		/// </summary>
		public override string Prefix {
			get { 
				return string.Format("1{0}{1}{2}{3}",
					ModelType.ToString("0"),
					NetType.ToString("00"),  
					(this.DateTime.Year - 2000).ToString("00"),
					(this.DateTime.Month).ToString("00")
				); 
			} 
		}

		public override ulong GetMin() {
			return ulong.Parse(Prefix  + "000000");
		}
		public override ulong GetMax()
		{
			return ulong.Parse(Prefix  + "999999");
		}


		public override string ToStringValue(long no)
		{
			return no.ToString("00000000000000");
		}

	}
}
 
