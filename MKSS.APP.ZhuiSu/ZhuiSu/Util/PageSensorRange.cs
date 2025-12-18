using MKSS.Service.ZhuiSu.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MKSS.APP.ZhuiSu.Util
{
    public class PageSensorRange : INotifyPropertyChanged
	{
		public int Value { get; set; }
		public double? Humidity { get; set; }
		public double? Temperature { get; set; }
		public DateTime? HTDateTime { get; set; }
		public string HTString()
		{
			return string.Format("/{1}℃/{0}%", Humidity == null ? "--" : Humidity.Value.ToString("f1"), Temperature == null ? "--" : Temperature.Value.ToString("f1"));
		}
		public TimeSpan Type { get; set; }
		public DateTime TypeFrom { get { return Parent.Now - Type; } }
		public DateTime TypeTo { get { return Parent.Now; } }
		public PageSensorBase Parent { get; set; }
		public event PropertyChangedEventHandler PropertyChanged;
		public PageSensorRange() { 
		
		}
		/// <summary>
		///  已过期
		/// </summary>
		/// <param name="d"></param>
		/// <param name="now"></param>
		/// <returns></returns>
		public bool Overdue(PageSensorValue dx) {

			if (dx == null) return true;
			DateTime d = dx.Time;
			if (d == DateTime.MinValue) return false;
			if (TypeTo - Type > d) return true;
			return true;
		}
		public PageSensorRange(PageSensorBase p)
		{
			Parent = p;  
		}
		  

		public override string ToString()
		{
			return ToBdString(); 
		}

		public string ToBdString()
		{ 
			return string.Format("{0}", Value==0 ? "" :( (double)Value/10000 * 5).ToString("f2") );
		}

	}
}
