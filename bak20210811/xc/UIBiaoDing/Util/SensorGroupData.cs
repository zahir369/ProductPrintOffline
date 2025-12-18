using System;

namespace DeviceDataMonitorWPF.UIBiaoDing.Util
{
    public class SensorGroupData
	{
		public Address Address
		{
			get;
			set;
		}
		public int AddressInt
		{
			get { return  Address.V; }
		}

		public SensorDataX[] SingleAddressData
		{
			get;
			set;
		}

		public DateTime Time
		{
			get;
			set;
		}

		public DataQualified Qualified
		{
			get;
			set;
		}
		public SensorGroupData(DataQualified qui, SensorDataX[] Data, bool LargeValue, Address Address, DateTime Time) {
			this.Qualified = qui;
			this.Address = Address;
			this.SingleAddressData = Data;
			this.Time = Time; 
		}
		public SensorGroupData(DataQualified qui,ushort[] Data, bool LargeValue, Address Address, DateTime Time)
		{
			this.Qualified = qui;
			this.Address = Address;

			this.SingleAddressData = new SensorDataX[]
			{
				new SensorDataX(Address,1),
				new SensorDataX(Address,2),
				new SensorDataX(Address,3),
				new SensorDataX(Address,4),
				new SensorDataX(Address,5),
				new SensorDataX(Address,6),
				new SensorDataX(Address,7),
				new SensorDataX(Address,8),
				new SensorDataX(Address,9),
				new SensorDataX(Address,10),
				new SensorDataX(Address,11),
				new SensorDataX(Address,12),
				new SensorDataX(Address,13),
				new SensorDataX(Address,14),
				new SensorDataX(Address,15)
			}; 
			this.Time = Time;
			for (int i = 0; i < 15; i++)
			{
				this.SingleAddressData[i].Position = i + 1;
				this.SingleAddressData[i].V01 = new int?((int)Data[i * 10]);
				this.SingleAddressData[i].V02 = new int?((int)Data[i * 10 + 1]);
				this.SingleAddressData[i].V03 = new int?((int)Data[i * 10 + 2]);
				this.SingleAddressData[i].V04 = new int?((int)Data[i * 10 + 3]);
				this.SingleAddressData[i].V05 = new int?((int)Data[i * 10 + 4]);
				this.SingleAddressData[i].V06 = new int?((int)Data[i * 10 + 5]);
				this.SingleAddressData[i].V07 = new int?((int)Data[i * 10 + 6]);
				this.SingleAddressData[i].V08 = new int?((int)Data[i * 10 + 7]);
				this.SingleAddressData[i].V09 = new int?((int)Data[i * 10 + 8]);
				this.SingleAddressData[i].V10 = new int?((int)Data[i * 10 + 9]);
			}
		}

		public ushort[] GetProductQualified()
		{
			ushort[] Qualified = new ushort[4];
			bool flag = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 0);
			if (flag)
			{
				Qualified[0] = (ushort)(20480 | Qualified[0]);
			}
			bool flag2 = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 1);
			if (flag2)
			{
				Qualified[0] = (ushort)(1280 | Qualified[0]);
			}
			bool flag3 = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 2);
			if (flag3)
			{
				Qualified[0] = (ushort)(80 | Qualified[0]);
			}
			bool flag4 = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 3);
			if (flag4)
			{
				Qualified[0] = (ushort)(5 | Qualified[0]);
			}
			bool flag5 = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 4);
			if (flag5)
			{
				Qualified[1] = (ushort)(20480 | Qualified[1]);
			}
			bool flag6 = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 5);
			if (flag6)
			{
				Qualified[1] = (ushort)(1280 | Qualified[1]);
			}
			bool flag7 = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 6);
			if (flag7)
			{
				Qualified[1] = (ushort)(80 | Qualified[1]);
			}
			bool flag8 = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 7);
			if (flag8)
			{
				Qualified[1] = (ushort)(5 | Qualified[1]);
			}
			bool flag9 = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 8);
			if (flag9)
			{
				Qualified[2] = (ushort)(20480 | Qualified[2]);
			}
			bool flag10 = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 9);
			if (flag10)
			{
				Qualified[2] = (ushort)(1280 | Qualified[2]);
			}
			bool flag11 = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 10);
			if (flag11)
			{
				Qualified[2] = (ushort)(80 | Qualified[2]);
			}
			bool flag12 = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 11);
			if (flag12)
			{
				Qualified[2] = (ushort)(5 | Qualified[2]);
			}
			bool flag13 = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 12);
			if (flag13)
			{
				Qualified[3] = (ushort)(20480 | Qualified[3]);
			}
			bool flag14 = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 13);
			if (flag14)
			{
				Qualified[3] = (ushort)(1280 | Qualified[3]);
			}
			bool flag15 = this.Qualified.NotQualified(this.Address.APP,this.AddressInt, 14);
			if (flag15)
			{
				Qualified[3] = (ushort)(80 | Qualified[3]);
			}
			return Qualified;
		}
	}


}
