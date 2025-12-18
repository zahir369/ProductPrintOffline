using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MKSS.APP.LaoHua.Util
{
    public class PageBoardItem : INotifyPropertyChanged
	{

        public override string ToString()
        {
            return string.Format("{0}_{1}", Address, CaseNo);
        }
        public int CaseNo { get; set; }
		public int Address { get; set; }
		public DateTime LastUpdate { get; set; }
		public string AddressString { 
			get { if (Address == 0) return ""; return Address.ToString("00"); } 
			set { string str = (Address + "").TrimStart("0".ToCharArray()); int i = 0; if(int.TryParse(str,out i)) Address = i; } }
		
		public bool Empty {
			get
			{
				return ProductTable.Where(w => w.Value != null).Sum(w => w.Value.Value) == 0;
			}
		}
		 
		public List<PageSensorModel> ProductTable { get; set; }

		public PageBoardItem() {
			 
			ProductTable = new List<PageSensorModel>();
            for (int i = 1; i <= 15; i++)
            {
				ProductTable.Add(new PageSensorModel(0,i));
			} 
		}

		public event PropertyChangedEventHandler PropertyChanged;
		
		public void RefreshPage() {
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(""));
                foreach (PageSensorBase item in ProductTable)
                {
					 
				}
			}
		}
	}

}
