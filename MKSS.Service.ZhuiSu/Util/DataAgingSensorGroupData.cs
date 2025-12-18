using System;
using System.Text;

namespace MKSS.Service.ZhuiSu.Util
{
    public class DataAgingSensorGroupData
	{
		public byte Address
		{
			get;
			private set;
		}
		public int AddressInt
		{
			get { return (int)Address; }
		}

		public DataAgingSensor[] SingleAddressData
		{
			get;
			set;
		}

		public DateTime Time
		{
			get;
			set;
		}

		public DataAging Qualified
		{
			get;
			set;
		}
		public ModBusBoardConnection Connection { get; set; }
		public DataAgingSensorGroupData(DataAging qui,byte Address, ModBusBoardConnection conn) {
			this.Qualified = qui;
			this.Address = Address;
			this.SingleAddressData = new DataAgingSensor[32];
            for (int i = 0; i < this.SingleAddressData.Length; i++)
            {
				this.SingleAddressData[i] = qui[AddressInt, i+1];

			}
			this.Connection = conn;
		}
		public void PushData(ushort[] Data, DateTime Time)
		{
			this.Time = Time;
			for (int i = 0; i < 32; i++)
			{
				this.SingleAddressData[i].Position = i + 1;
				this.SingleAddressData[i].PashData(((int)Data[i]), Time); 
			}
		}

        public override string ToString()
        {
			StringBuilder sf = new StringBuilder();
			sf.Append(this.Address + "[" +Time.ToString("mm:ss") + "]");

			sf.Append("[");
			foreach (var item in SingleAddressData)
            {
				sf.Append(string.Format("{0}:{1}",item.Position, item.Value));

			}
			sf.Append("]");

			return sf.ToString();
        }

    }

}
