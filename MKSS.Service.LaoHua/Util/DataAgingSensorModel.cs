using System;

namespace MKSS.Service.LaoHua.Util
{
    public class DataAgingSensorModel : DataAgingSensor {

		public new int Address { get { return Parent.Address; } }
		public new int Position { get { return Parent.Position; } }
		public new DateTime Start { get { return Parent.Start; } }
		public new DateTime Now { get { return Parent.Now; } }
		public new int? Value { get { return Parent.Value; } }
		public new bool IsEmpData
		{
			get { return this.Value == null || !this.Value.HasValue; }
		}

		public new bool IsEmpSensor
		{
			get { return IsEmpData || (this.Value.HasValue && this.Value == 170) || (this.Value.HasValue && this.Value == 0); }
		}
		public DataAgingSensor Parent { get; set; }

		public DataAgingSensorModel(int a,int p) : base(a, p) {
			Parent = new DataAgingSensor(a, p);
		}

		public void Fill(DataAgingSensor parent) {
			Parent = parent;
		}

		public static string VS(int? v)
		{
			if (v == null) return "";
			return v.Value.ToString();
		}
	}

}
