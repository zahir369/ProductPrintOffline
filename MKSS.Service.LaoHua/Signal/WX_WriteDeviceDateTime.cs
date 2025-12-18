using System;

namespace MKSS.APP.UIBiaoDing.Signal
{
    public class WX_WriteDeviceDateTime : ISignal
	{

		/// <summary>
		///  命令词
		/// </summary>
		public byte Signal { get { return SignalUtil.Confuse(0XB0); } }

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
				BitConverter.ToUInt16(new byte[] {(byte)Signal, (byte)0X00 }, 0),
				BitConverter.ToUInt16(new byte[] {  (byte)0X00,(byte)0X00 }, 0),
				WX_WriteManufacturingDate.ToUshort(d.Substring(0,2),d.Substring(2,2)),
				WX_WriteManufacturingDate.ToUshort(d.Substring(4,2),d.Substring(6,2)),
				WX_WriteManufacturingDate.ToUshort(d.Substring(8,2),d.Substring(10,2)),

			};
		}
		 

	}

}
