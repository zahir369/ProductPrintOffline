using MKSS.Model;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MKSS.APP.UIBiaoDing.Util
{
    public class SensorGroupDataModel : INotifyPropertyChanged
	{
		public Board Board { get; set; }
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
		public List<SensormData> ProductTable { get; set; }
		public SensorGroupDataModel() {
			ProductTable = new List<SensormData>();
            for (int i = 0; i < 15; i++)
            {
				ProductTable.Add(new SensormData());
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		
		public void RefreshPage() {
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(""));
                foreach (SensormData item in ProductTable)
                {
					item.RefreshPage();
				}
			}
		}
	}

}
