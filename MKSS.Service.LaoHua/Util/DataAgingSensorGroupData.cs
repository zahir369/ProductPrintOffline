using System;
using System.Text;

namespace MKSS.Service.LaoHua.Util
{
    public class DataAgingSensorGroupData
	{
		public TaskAddress Address
		{
			get;
			private set;
		}
		public int AddressInt
		{
			get { return (int)Address.Address; }
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
		public DataAgingSensorGroupData(DataAging qui,TaskAddress Address, ModBusBoardConnection conn) {
			this.Qualified = qui;
			this.Address = Address;
			this.SingleAddressData = new DataAgingSensor[]
			{
				qui[AddressInt,1],
				qui[AddressInt,2],
				qui[AddressInt,3],
				qui[AddressInt,4],
				qui[AddressInt,5],
				qui[AddressInt,6],
				qui[AddressInt,7],
				qui[AddressInt,8],
				qui[AddressInt,9],
				qui[AddressInt,10],
				qui[AddressInt,11],
				qui[AddressInt,12],
				qui[AddressInt,13],
				qui[AddressInt,14],
				qui[AddressInt,15]
			};
			this.Connection = conn;
		}
		public void PushData(ushort[] Data, ushort[] DataDbl, DateTime Time)
		{
			this.Time = Time;
			
			for (int i = 0; i < 15; i++)
			{
				this.SingleAddressData[i].Position = i + 1;
				this.SingleAddressData[i].PashData( Data,  DataDbl, ((int)Data[i * 10]), ((int)DataDbl[i * 10]), Time); 
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
