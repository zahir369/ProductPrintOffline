using System;

namespace MKSS.Service.UIBiaoDing
{


	public class SerialNoParserScrwd : SerialNoParser
	{

		/// <summary>
		///  SCRW-2021-07-31-01430
		///  990036990666
		/// </summary>
		/// <param name="str"></param>
		public SerialNoParserScrwd(string str)
		{
			StrValue = str;
			DateTime = DateTime.Parse(StrValue.Substring(5, 10));
			MainNo = int.Parse(StrValue.Substring(16, 5));
		}

		public DateTime DateTime { get; set; }
		public int MainNo { get; set; }
		public override string Prefix { get { return (this.DateTime.Year - 2000) + "" + this.MainNo.ToString("00000"); } }
		public override ulong GetMin()
		{
			return ulong.Parse((this.DateTime.Year - 2000) + "" + this.MainNo.ToString("00000") + "00000");
		}
		public override ulong GetMax()
		{
			return ulong.Parse((this.DateTime.Year - 2000) + "" + this.MainNo.ToString("00000") + "99999");
		}
	}
}

