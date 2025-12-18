using System.Collections.Generic;
using System.ComponentModel;

namespace MKSS.APP.UIBiaoDing.Util
{
    public class SensorDataX : INotifyPropertyChanged
	{

		public Address Address
		{
			get; set;
		}

		public int Position
		{
			get;set;
		}

		public int? V01
		{
			get { return Filter["V01"]; }
			set { Filter.PushData("V01", value); }
		}

		public int? V02
		{
			get { return Filter["V02"]; }
			set { Filter.PushData("V02", value); }
		}

		public int? V03
		{
			get { return Filter["V03"]; }
			set { Filter.PushData("V03", value); }
		}

		public int? V04
		{
			get { return Filter["V04"]; }
			set { Filter.PushData("V04", value); }
		}

		public int? V05
		{
			get { return Filter["V05"]; }
			set { Filter.PushData("V05", value); }
		}

		public int? V06
		{
			get { return Filter["V06"]; }
			set { Filter.PushData("V06", value); }
		}

		public int? V07
		{
			get { return Filter["V07"]; }
			set { Filter.PushData("V07", value); }
		}

		public int? V08
		{
			get { return Filter["V08"]; }
			set { Filter.PushData("V08", value); }
		}

		public int? V09
		{
			get { return Filter["V09"]; }
			set { Filter.PushData("V09", value); }
		}

		public int? V10
		{
			get { return Filter["V10"]; }
			set { Filter.PushData("V10", value); }
		}

		public List<int> TemperaturePointZeroList
		{
			get; set;
		}

		public int? LiangCheng
		{
			get { return Filter["LiangCheng"]; }
			set { Filter.PushData("LiangCheng", value); }
		}

		public string DianYaRange
		{
			get; set;
		}

		public bool? AutoAdjustState
		{
			get; set;
		}

		public int? OutPutVolage
		{
			get; set;
		}

		public string Serial
		{
			get; set;
		}

		/// <summary>
		/// 设备时间
		/// </summary>
		public string DeviceDateTime
		{
			get; set;
		}

		/// <summary>
		/// 出厂日期
		/// </summary>
		public string ManufacturingDate
		{
			get; set;
		}

		public bool IsEmpData {
			get { return this.V01==null || !this.V01.HasValue ||this.V01.Value==0; }
		}

		/// <summary>
		/// // || (this.V01.HasValue && this.V01 == 170)
		/// </summary>
		public bool IsEmpSensor
		{
			get { return IsEmpData ; }
		}

		public bool IsEmpValue(int? v)
		{
			return v == null || !v.HasValue ;
		}

		/// <summary>
		/// // || (this.V01.HasValue && this.V01 == 170)
		/// </summary>
		public bool IsEmp170
		{
			get { return this.V01==null || (this.V01 != null && (this.V01.HasValue && this.V01 == 170)); }
		}

		public bool? IsQualifiedV1
		{
			get; set;
		}
		public bool? IsQualifiedV7
		{
			get; set;
		}

		SensorDataFilter Filter
        {
			get { 
				return SensorDataFilter.CreateSensorDataFilter(Address, Position, arr);
			}
		}
		string[] arr = new string[] {
			"V01", "V02", "V03", "V04", "V05",
			"V06", "V07", "V08","V09","V10","LiangCheng",
		};
		public SensorDataX(Address addr,int pos)
		{
			this.Address = addr;
			this.Position = pos;
			TemperaturePointZeroList = new List<int>();
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public void RefreshPage()
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(""));
			}
		}

		public void Reset() {
			this.V01 = null;
			this.V02 = null;
			this.V03 = null;
			this.V04 = null;
			this.V05 = null;
			this.V06 = null;
			this.V07 = null;
			this.V08 = null;
			this.V09 = null;
			this.V10 = null;
			this.LiangCheng = null;
			this.Serial = null;
			this.DeviceDateTime = null;
			this.ManufacturingDate = null;
			this.AutoAdjustState = null;
			this.DianYaRange = null;
			this.OutPutVolage = null;
			this.TemperaturePointZeroList = null;
			Filter.Reset();
		}

	

		public static string VS(int? v) {
			if (v == null) return "";
			return v.Value.ToString();
		}
	}

}
