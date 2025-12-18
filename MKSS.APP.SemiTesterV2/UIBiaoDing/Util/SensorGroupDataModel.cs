using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MKSS.APP.UIBiaoDing.Util
{
    public class SensorGroupDataModel : INotifyPropertyChanged
	{
		Address _Address = new Address(0,0);
		public Address Address {
			get { return _Address; }
			set {
				_Address = value;
                foreach (var item in ProductTable)
                {
					item.Address = value;
				}
			}
		}

		public string AddressString { 
			get { if (Address.V == 0) return ""; return Address.V.ToString("00"); } 
		}
		
		public bool Empty {
			get
			{
				return ProductTable.Where(w => w.V01 != null).Sum(w => w.V01.Value) == 0;
			}
		}

		public bool EmptyPre { get; set; }
		public bool EmptyPrePre { get; set; }
		public bool EmptyPrePrePre { get; set; }
		public DateTime LastSendTime { get; private set; }
		public DateTime LastReceiveTime { get;private set; }
		public void QuertReceiveData() {
			LastReceiveTime = DateTime.Now;
		}
		public void QuertStart()
		{
			LastReceiveTime = DateTime.Now;
		}
		public List<SensorDataX> ProductTable { get; set; }
		public SensorGroupDataModel() {
			ProductTable = new List<SensorDataX>();
            for (int i = 0; i < 15; i++)
            {
				ProductTable.Add(new SensorDataX(Address,i+1));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		
		public void RefreshPage() {
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(""));
                foreach (SensorDataX item in ProductTable)
                {
					item.RefreshPage();
				}
			}
		}
	}

}
