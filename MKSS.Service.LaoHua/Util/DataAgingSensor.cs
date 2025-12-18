using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace MKSS.Service.LaoHua.Util
{

    public class DataAgingSensor : INotifyPropertyChanged
	{

		public int Address { get; set; }
		public int Position { get; set; }
		public DateTime Start { get; set; }
		public DateTime Now { get; set; } 
		public int? Value { get; set; }
		public int? ValueDbl { get; set; }
		public Dictionary<TimeSpan,DataAgingRange> Ranges { get; set; }
		public List<DataAgingSensorValue> Values { get; set; } 
		public bool IsEmpData
		{
			get { return this.Value == null || !this.Value.HasValue; }
		}

		public bool IsEmpSensor
		{
			get { return IsEmpData || (this.Value.HasValue && this.Value == 170) || (this.Value.HasValue && this.Value == 0); }
		}

		public event PropertyChangedEventHandler PropertyChanged;
		public DataAgingSensor(int a,int p) {
			Address = a;
			Position = p;
			Start = DateTime.Now;
			Ranges = new Dictionary<TimeSpan,DataAgingRange>();
			Values = new List<DataAgingSensorValue>(); 
			List<DataAgingRange>  all = new List<DataAgingRange>();
			all.Add(new DataAgingRange(this) { Type = new TimeSpan(0, 0, 10) });
			all.Add(new DataAgingRange(this) { Type = new TimeSpan(0, 1, 0) });
			all.Add(new DataAgingRange(this) { Type = new TimeSpan(0, 5, 0) });
			all.Add(new DataAgingRange(this) { Type = new TimeSpan(0, 15, 0) });
			all.Add(new DataAgingRange(this) { Type = new TimeSpan(0, 30, 0) });
			all.Add(new DataAgingRange(this) { Type = new TimeSpan(1, 0, 0) });
			all.Add(new DataAgingRange(this) { Type = new TimeSpan(3, 0, 0) });
			all.Add(new DataAgingRange(this) { Type = new TimeSpan(6, 0, 0) });
			all.Add(new DataAgingRange(this) { Type = new TimeSpan(12, 0, 0) });
			all.Add(new DataAgingRange(this) { Type = new TimeSpan(24, 0, 0) });
			all.Add(new DataAgingRange(this) { Type = new TimeSpan(72, 0, 0) });
            foreach (DataAgingRange item in all)
            {
				Ranges.Add(item.Type, item);
			}
		}

		public void Reset( )
		{
			Values.Clear(); 
			foreach (DataAgingRange item in Ranges.Values)
			{
				item.ResetMin();
				item.ResetMax();
			}
		}

		public void PashData(ushort[] Data, ushort[] DataDbl, int val, int valDbl, DateTime now) {
			Now = now;
			Value = val;
			ValueDbl = val;
			DataAgingSensorValue v = new DataAgingSensorValue() { Time = Now, Value = val, ValueDbl= valDbl, Data= Data, DataDbl= DataDbl };
			Values.Add(v); 
			foreach (DataAgingRange item in Ranges.Values)
			{
				item.Refresh(v); 
			}
		}
 

		public override string ToString()
        {
            return string.Format("{0}_{1}:{2}", Address, Position, Value);
        }

    }

}
