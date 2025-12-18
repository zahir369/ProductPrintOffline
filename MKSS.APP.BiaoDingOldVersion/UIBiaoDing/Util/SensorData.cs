using System.Collections.Generic;
using System.ComponentModel;

namespace DeviceDataMonitorWPF.UIBiaoDing.Util
{
    public class SensorData : INotifyPropertyChanged
	{
		 

		public int Position
		{
			get;set;
		}

		public int? V01
		{
			get; set;
		}

		public int? V02
		{
			get; set;
		}

		public int? V03
		{
			get; set;
		}

		public int? V04
		{
			get; set;
		}

		public int? V05
		{
			get; set;
		}

		public int? V06
		{
			get; set;
		}

		public int? V07
		{
			get; set;
		}

		public int? V08
		{
			get; set;
		}

		public int? V09
		{
			get; set;
		}

		public int? V10
		{
			get; set;
		}

		public List<int> TemperaturePointZeroList
		{
			get; set;
		}

		public int? LiangCheng
		{
			get; set;
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

		public SensorData()
		{
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

		public static string VS(int? v) {
			if (v == null) return "";
			return v.Value.ToString();
		}
	}

}
