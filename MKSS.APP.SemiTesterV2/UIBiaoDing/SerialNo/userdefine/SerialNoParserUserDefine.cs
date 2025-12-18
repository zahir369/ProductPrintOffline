using System;

namespace MKSS.Service.UIBiaoDing
{


    public class SerialNoParserUserDefine : SerialNoParser
	{

		/// <summary>
		///  SCRW-2021-07-31-01430
		///  990036990666
		/// </summary>
		/// <param name="str"></param>
		public SerialNoParserUserDefine(string str) {
			StrValue = str; 
		}
		 
		public override string Prefix { get { return StrValue; } }
		public override ulong GetMin() {
			return ulong.Parse(StrValue + "00000");
		}
		public override ulong GetMax()
		{
			return ulong.Parse(StrValue + "99999");
		}
	}
}
 
