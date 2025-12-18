using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MKSS.Service.ZhuiSu.Util
{

    public class DataAgingxSensor : INotifyPropertyChanged
	{

		public string SensorId { get; set; }
		public int Address { get; set; }
		public int Position { get; set; }
		public DateTime Start { get; set; }
		public DateTime Now { get; set; } 
		public int? Value { get; set; }
		public Dictionary<TimeSpan,DataAgingxRange> Ranges { get; set; } 
		public bool IsEmpData
		{
			get { return this.Value == null || !this.Value.HasValue; }
		}
		// || (this.Value.HasValue && this.Value == 170) 
		public bool IsEmpSensor
		{
			get { return IsEmpData|| (this.Value.HasValue && this.Value == 0); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public DataAgingxSensor(int a,int p) {
			Address = a;
			Position = p;
			Start = DateTime.Now;
			Ranges = new Dictionary<TimeSpan,DataAgingxRange>();
			List<DataAgingxRange>  all = new List<DataAgingxRange>();
			all.Add(new DataAgingxRange(this) { Type = new TimeSpan(0, 0, 10) });
			all.Add(new DataAgingxRange(this) { Type = new TimeSpan(0, 1, 0) });
			all.Add(new DataAgingxRange(this) { Type = new TimeSpan(0, 5, 0) });
			all.Add(new DataAgingxRange(this) { Type = new TimeSpan(0, 15, 0) });
			all.Add(new DataAgingxRange(this) { Type = new TimeSpan(0, 30, 0) });
			all.Add(new DataAgingxRange(this) { Type = new TimeSpan(1, 0, 0) });
			all.Add(new DataAgingxRange(this) { Type = new TimeSpan(3, 0, 0) });
			all.Add(new DataAgingxRange(this) { Type = new TimeSpan(6, 0, 0) });
			all.Add(new DataAgingxRange(this) { Type = new TimeSpan(12, 0, 0) });
			all.Add(new DataAgingxRange(this) { Type = new TimeSpan(24, 0, 0) });
			all.Add(new DataAgingxRange(this) { Type = new TimeSpan(72, 0, 0) });
            foreach (DataAgingxRange item in all)
            {
				Ranges.Add(item.Type, item);
			}
		}


		public void Reset( )
		{
			foreach (DataAgingxRange item in Ranges.Values)
			{
				item.Min = 0;
				item.Max = 0;
			}
		}
		 
        public override string ToString()
        {
            return string.Format("{0}-{1}:{2}", Address, Position, Value);
        }

    }

}
