using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace MKSS.Service.ZhuiSu.Util
{
    public class DataAgingRange : INotifyPropertyChanged
	{
		public DataAgingSensorValue Min { get; set; }
		public DataAgingSensorValue Max { get; set; }
		public TimeSpan Type { get; set; }
		public DateTime TypeFrom { get { return Parent.Now - Type; } }
		public DateTime TypeTo { get { return Parent.Now; } }
		public DataAgingSensor Parent { get; set; }
		public event PropertyChangedEventHandler PropertyChanged;
		/// <summary>
		///  已过期
		/// </summary>
		/// <param name="d"></param>
		/// <param name="now"></param>
		/// <returns></returns>
		public bool Overdue(DataAgingSensorValue dx) {

			if (dx == null) return true;
			DateTime d = dx.Time;
			if (d == DateTime.MinValue) return false;
			if (TypeTo - Type > d) return true;
			return true;
		}
		public DataAgingRange(DataAgingSensor p)
		{
			Parent = p; ResetMin(); ResetMax();
		}

		public void ResetMin() {
			IEnumerable<DataAgingSensorValue> xx = Parent.Values.Where(w => w.Time >= TypeFrom && w.Value != null);
			if (xx.Count() > 0)
			{
				int val = Parent.Values.Where(w => w.Time >= TypeFrom && w.Value != null).Min(x => x.Value.Value);
				Min = Parent.Values.FirstOrDefault(w => w.Value == val);
			}
			else {
				Min = null;
			} 
		}
		public void ResetMax()
		{
			IEnumerable<DataAgingSensorValue> xx = Parent.Values.Where(w => w.Time >= TypeFrom && w.Value != null);
			if (xx.Count() > 0)
			{
				int val = Parent.Values.Where(w => w.Time >= TypeFrom && w.Value != null).Max(x => x.Value.Value);
				Max = Parent.Values.FirstOrDefault(w => w.Value == val);
			}
			else
			{
				Max = null;
			}
		}

		public void Refresh(DataAgingSensorValue v)
		{

			if (this.Parent.IsEmpSensor) return;

			if (Overdue(Min)) {
				ResetMin();
			}
			if (Overdue(Max))
			{
				ResetMax();
			}

			if (Min==null || v.Value < Min.Value) {
				Min = v;
			}

			if (Max==null || v.Value > Max.Value)
			{
				Max = v;
			} 
		}


		public override string ToString()
		{
			return ToBdString();
			//return string.Format("{0}:{1}-{2}", Type, Min, Max);
		}


		public string ToBdString()
		{
			if (Min == null || Max == null) return null;
			int val = (Min.Value.Value + Max.Value.Value) / 2;
			int valbd = (Max.Value.Value - Min.Value.Value) / 2;
			return string.Format("{0}±{1}", val, valbd);
		}

	}

}
