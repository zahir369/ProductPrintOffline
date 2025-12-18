using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;

namespace MKSS.APP.ZhuiSu.Util
{

    public class PageSensorBase : INotifyPropertyChanged
	{

		public string SensorId { get; set; }
		public string SerialNO { get; set; }
		public int Address { get; set; }
		public int Position { get; set; }
		public DateTime Start { get; set; }
		public DateTime Now { get; set; }

		public int? Value { get; set; }
		public double ValueFormate { get { return Value == null ? 0 : 5 - (double)Value.Value / (double)10000 * 5; } }


		public double? Humidity { get; set; }
		public double? Temperature { get; set; }
		public DateTime? HTDateTime { get; set; }
		public string HTString()
		{
			return string.Format("/{1}℃/{0}%", Humidity == null ? "--" : Humidity.Value.ToString("f1"), Temperature == null ? "--" : Temperature.Value.ToString("f1"));
		}


		public Dictionary<TimeSpan,PageSensorRange> Ranges { get; set; } 
		public bool IsEmpData
		{
			get { return this.Value == null || !this.Value.HasValue; }
		}
		// || (this.Value.HasValue && this.Value == 170) 
		public bool IsEmpSensor
		{
			get { return IsEmpData|| (this.Value.HasValue && this.Value == 0); }
		}

		public static int[] Seconds { get;   set; }= new int[] { 100, 730, 1210, 1690, 1930, 3610, 4330, 6010, 6730, 6970 };
		public event PropertyChangedEventHandler PropertyChanged;
		public PageSensorBase(int a,int p) {
			Address = a;
			Position = p;
			Start = DateTime.Now;
			Ranges = new Dictionary<TimeSpan, PageSensorRange>();
			Reset();
		}

		public void Reset( )
		{

			Ranges.Clear();

			foreach (PageSensorRange item in Ranges.Values)
			{
				item.Value = 0;
			}


			List<PageSensorRange> all = new List<PageSensorRange>();
			foreach (var item in Seconds)
			{
				all.Add(new PageSensorRange(this) { Type = TimeSpan.FromSeconds(item) });
			}
			foreach (PageSensorRange item in all)
			{
				Ranges.Add(item.Type, item);
			} 

		}
		 
        public override string ToString()
        {
            return string.Format("{0}-{1}:{2}", Address, Position, Value);
        }

    }

}
