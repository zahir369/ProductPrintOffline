using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MKSS.Service.ZhuiSu.Util
{
    public class DataAgingSensorGroupDataModel : INotifyPropertyChanged
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
		
		public DataAgingSensorGroupData Parent { get; set; }
		public List<DataAgingSensorModel> ProductTable { get; set; }

		public DataAgingSensorGroupDataModel() {
			 
			ProductTable = new List<DataAgingSensorModel>();
            for (int i = 1; i <= 15; i++)
            {
				ProductTable.Add(new DataAgingSensorModel(0,i));
			} 
		}

		public void Fill(DataAgingSensorGroupData parent)
		{
			Parent = parent;
			Address = parent.Address;
            for (int i = 0; i < ProductTable.Count; i++)
            {
				ProductTable[i].Fill(parent.SingleAddressData[i]);

			} 
		}

		public event PropertyChangedEventHandler PropertyChanged;
		
		public void RefreshPage() {
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(""));
                foreach (DataAgingSensor item in ProductTable)
                {
					 
				}
			}
		}
	}

}
