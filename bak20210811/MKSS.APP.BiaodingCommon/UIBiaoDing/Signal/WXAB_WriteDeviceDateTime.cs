using System;

namespace DeviceDataMonitorWPF.UIBiaoDing.Signal
{
    public class WXAB_WriteDeviceDateTime : ISignal
	{
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
				BitConverter.ToUInt16(new byte[] {(byte)0XB0, (byte)0X00 }, 0),
				BitConverter.ToUInt16(new byte[] {  (byte)0X00,(byte)0X00 }, 0),
				WXAC_WriteManufacturingDate.ToUshort(d.Substring(0,2),d.Substring(2,2)),
				WXAC_WriteManufacturingDate.ToUshort(d.Substring(4,2),d.Substring(6,2)),
				WXAC_WriteManufacturingDate.ToUshort(d.Substring(8,2),d.Substring(10,2)),

			};
		}
		 

	}

}
