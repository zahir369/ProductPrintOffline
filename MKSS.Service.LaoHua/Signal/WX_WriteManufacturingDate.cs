using System;

namespace MKSS.APP.UIBiaoDing.Signal
{

	/// <summary>
	///  配置寿命起始数据
	/// </summary>
	public class WX_WriteManufacturingDate : ISignal
	{
		/// <summary>
		///  命令词
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0XB1); } }

		public DateTime DateTime
		{
			get;
			set;
		} 

		public ushort[] GetSignalBytes()
		{
			if (DateTime == DateTime.MinValue) 
				DateTime = DateTime
					   .Now;
			string d = DateTime.ToString("yyyyMMddHHmm");
			return new ushort[]
			{
				BitConverter.ToUInt16(new byte[] {  (byte)Signal, (byte)0X00 }, 0),
				BitConverter.ToUInt16(new byte[] {  (byte)0X00,(byte)0X00 }, 0),
				ToUshort(d.Substring(0,2),d.Substring(2,2)),
				ToUshort(d.Substring(4,2),d.Substring(6,2)),
				ToUshort(d.Substring(8,2),d.Substring(10,2)),
			};

		}

		public static ushort ToUshort(string hex, string hex2) {
			int b = Int32.Parse(hex, System.Globalization.NumberStyles.HexNumber);
			int b2 = Int32.Parse(hex2, System.Globalization.NumberStyles.HexNumber);
			return BitConverter.ToUInt16(new byte[] { (byte)b2, (byte)b }, 0);
		}
		ushort ToUshort(string s) {
			return BitConverter.ToUInt16(new byte[] { }, 0);
		}

	}

}
