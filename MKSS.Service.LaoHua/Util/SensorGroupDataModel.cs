using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MKSS.Service.LaoHua.Util
{
    public class SensorGroupDataModel : INotifyPropertyChanged
	{
		public int Address { get; set; }
		public string AddressString { 
			get { if (Address == 0) return ""; return Address.ToString("00"); } 
			set { string str = (Address + "").TrimStart("0".ToCharArray()); int i = 0; if(int.TryParse(str,out i)) Address = i; } }
		
		public bool Empty {
			get
			{
				return ProductTable.Where(w => w.V01 != null).Sum(w => w.V01.Value) == 0;
			}
		}
		public bool EmptyPre { get; set; }
		public bool EmptyPrePre { get; set; }
		public bool EmptyPrePrePre { get; set; }
		public List<SensorData> ProductTable { get; set; }
		public SensorGroupDataModel() {
			ProductTable = new List<SensorData>();
            for (int i = 0; i < 15; i++)
            {
				ProductTable.Add(new SensorData());
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		
		public void RefreshPage() {
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(""));
                foreach (SensorData item in ProductTable)
                {
					item.RefreshPage();
				}
			}
		}
	}

}
