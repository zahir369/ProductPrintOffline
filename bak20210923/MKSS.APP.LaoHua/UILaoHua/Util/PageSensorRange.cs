using MKSS.Service.LaoHua.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MKSS.APP.LaoHua.Util
{
    public class PageSensorRange : INotifyPropertyChanged
	{
		public int Min { get; set; }
		public int Max { get; set; }
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
			int val = (Min + Max) / 2;
			int valbd = (Max - Min) / 2;
			return string.Format("{0}±{1}", val, Math.Abs(valbd));
		}

	}
}
