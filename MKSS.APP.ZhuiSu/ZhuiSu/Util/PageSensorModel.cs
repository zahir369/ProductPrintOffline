using System;

namespace MKSS.APP.ZhuiSu.Util
{
    public class PageSensorModel : PageSensorBase {

		public new string SensorId { get { return Parent.SensorId; } }
		public new int Address { get { return Parent.Address; } }
		public new int Position { get { return Parent.Position; } } 
		public new int? Value { get { return Parent.Value; } }

		public double ValueFormate { get { return Value == null ? 0 :  (double)Value.Value / (double)10000 * 5; } }
		public new bool IsEmpData
		{
			get { return this.Value == null || !this.Value.HasValue; }
		}

		//|| (this.Value.HasValue && this.Value == 170)
		public new bool IsEmpSensor
		{
			get { return IsEmpData  || (this.Value.HasValue && this.Value == 0); }
		}
		public PageSensorBase Parent { get; set; }

		public PageSensorModel(int a,int p) : base(a, p) {
			Parent = new PageSensorBase(a, p);
		}

		public void Fill(PageSensorBase parent) {
			Parent = parent;
		}

		public static string VS(int? v)
		{
			if (v == null) return "";
			return v.Value.ToString();
		}
		public override string ToString()
		{
			return string.Format("{0}-{1}:{2}", Address, Position, Value);
		}
	}

}
