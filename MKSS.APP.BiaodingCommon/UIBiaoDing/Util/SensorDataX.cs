using MKSS.Service.UIBiaoDing;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
		int? _V01 = null;
		public int? V01
		{
			get { return !SensorDataFilter.Enabled ? _V01 : Filter["V01"]; }
			set { _V01 = value; if(SensorDataFilter.Enabled) Filter.PushData("V01", value); }
		}

		int? _V02 = null;
		public int? V02
		{
			get { return !SensorDataFilter.Enabled ? _V02 : Filter["V02"]; }
			set { _V02 = value; if (SensorDataFilter.Enabled) Filter.PushData("V02", value); }
		}

		int? _V03 = null;
		public int? V03
		{
			get { return !SensorDataFilter.Enabled ? _V03 : Filter["V03"]; }
			set { _V03 = value; if (SensorDataFilter.Enabled) Filter.PushData("V03", value); }
		}

		int? _V04 = null;
		public int? V04
		{
			get { return !SensorDataFilter.Enabled ? _V04 : Filter["V04"]; }
			set { _V04 = value; if (SensorDataFilter.Enabled) Filter.PushData("V04", value); }
		}

		int? _V05 = null;
		public int? V05
		{
			get { return !SensorDataFilter.Enabled ? _V05 : Filter["V05"]; }
			set { _V05 = value; if (SensorDataFilter.Enabled) Filter.PushData("V05", value); }
		}

		int? _V06 = null;
		public int? V06
		{
			get { return !SensorDataFilter.Enabled ? _V06 : Filter["V06"]; }
			set { _V06 = value; if (SensorDataFilter.Enabled) Filter.PushData("V06", value); }
		}

		int? _V07 = null;
		public int? V07
		{
			get { return !SensorDataFilter.Enabled ? _V07 : Filter["V07"]; }
			set { _V07 = value; if (SensorDataFilter.Enabled) Filter.PushData("V07", value); }
		}

		int? _V08 = null;
		public int? V08
		{
			get { return !SensorDataFilter.Enabled ? _V08 : Filter["V08"]; }
			set { _V08 = value; if (SensorDataFilter.Enabled) Filter.PushData("V08", value); }
		}

		int? _V09 = null;
		public int? V09
		{
			get { return !SensorDataFilter.Enabled ? _V09 : Filter["V09"]; }
			set { _V09 = value; if (SensorDataFilter.Enabled) Filter.PushData("V09", value); }
		}

		int? _V10 = null;
		public int? V10
		{
			get { return !SensorDataFilter.Enabled ? _V10 : Filter["V10"]; }
			set { _V10 = value; if (SensorDataFilter.Enabled) Filter.PushData("V10", value); }
		}

		public List<int> TemperaturePointZeroList
		{
			get; set;
		}

		int? _LiangCheng = null;
		public int? LiangCheng
		{
			get { return !SensorDataFilter.Enabled ? _LiangCheng : Filter["LiangCheng"]; }
			set { _LiangCheng = value; if (SensorDataFilter.Enabled) Filter.PushData("LiangCheng", value); }
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

		public bool SerialError
		{
			get; set;
		} = false;

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
			get { return (this.V01==null || !this.V01.HasValue ||this.V01.Value==0)&& (this.V02 == null || !this.V02.HasValue || this.V02.Value == 0); }
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
			get { return 
					(this.V01==null || (this.V01 != null && (this.V01.HasValue && this.V01 == 170)))
					&&
					(this.V02 == null || (this.V02 != null && (this.V02.HasValue && this.V02 == 170)))
					;
			}
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
			this.SerialError = false;
			this.DeviceDateTime = null;
			this.ManufacturingDate = null;
			this.AutoAdjustState = null;
			this.DianYaRange = null;
			this.OutPutVolage = null;
			this.TemperaturePointZeroList = null;
			Filter.Reset();
		}


		/// <summary>
		///  检测串号是否重复
		///  串号是否连续
		///  如果是 14 位串号，族后裔为可能是校验位
		/// </summary>
		public void TestSerial(ConcurrentDictionary<string, SensorDataX> cache) {
			if (string.IsNullOrEmpty(this.Serial)) return;
			if (cache.ContainsKey(this.Serial))
			{
				cache[this.Serial].SerialError = true;
				this.SerialError = true;
			}
			else {
                if (this.Serial.Length != 12 && this.Serial.Length != 14)
				{
					cache.TryAdd(this.Serial, this);
					cache[this.Serial].SerialError = true;
					this.SerialError = true;
				}
                else
                {
                    try
                    {
						cache.TryAdd(this.Serial, this);
						this.SerialError = false;
					}
                    catch (System.Exception ex)
                    {
						 
                    }
				}
		
			}

			if (!this.SerialError && !string.IsNullOrEmpty(Serial)) {

				//查看是否连续
				var all = cache.Values.Where(w => w.Address == this.Address).ToList();
				if (all.Count >= 14)
				{
					//满盘才有判断的必要

					//正常的最大最小值
					long cur = long.Parse(Serial);
					long min = (long)all.Where(w => !string.IsNullOrEmpty(w.Serial)).Min(w => long.Parse(w.Serial));
					long max = (long)all.Where(w => !string.IsNullOrEmpty(w.Serial)).Max(w => long.Parse(w.Serial));
					SensorDataX minEn = all.FirstOrDefault(w => w.Serial == min.ToString());
					SensorDataX maxEn = all.FirstOrDefault(w => w.Serial == max.ToString());

					bool trim13 = false;
					//如果是 14 位串号，并且最后一位是校验位，去掉最后一位校验位，再
					if (Serial.Length == 14) {
						//校验位∶该校验位根据ISO7064，MOD11，10校验系统校验码计算方法生成（依据前12/13 位数据 进行运算），X可以是数字0 ~9中的任一数字
						var r11 = MKSS.Service.UIBiaoDing.ISO7064.CalculateHybridSystemCheckDigit(Serial.Substring(0,13), ISO7064.NumericCharSet);
						if (r11 == Serial) {
							trim13 = true;
							cur = long.Parse(Serial.Substring(0, 13));
							min = (long)all.Where(w => !string.IsNullOrEmpty(w.Serial)).Min(w => long.Parse(w.Serial.Substring(0, 13)));
							max = (long)all.Where(w => !string.IsNullOrEmpty(w.Serial)).Max(w => long.Parse(w.Serial.Substring(0, 13)));
							minEn = all.FirstOrDefault(w => w.Serial.Substring(0, 13) == min.ToString());
							maxEn = all.FirstOrDefault(w => w.Serial.Substring(0, 13) == max.ToString());
						}
					}



					//判断 min 是否正确，只出现在第一位
					bool min_correct = minEn.Position == 1 || minEn.Position == 2;
					bool max_correct = maxEn.Position == 14 || maxEn.Position == 15;
					
                    //全部设置
                    foreach (var item in all)
                    {
						if (!string.IsNullOrEmpty(item.Serial)) {
							long cueSerial = long.Parse(item.Serial);
							if(trim13) cueSerial = long.Parse(item.Serial.Substring(0, 13));
							if (min_correct)
							{
								item.SerialError = (cueSerial - min != item.Position - minEn.Position);
							}
							if (max_correct)
							{
								item.SerialError = (cueSerial - max != item.Position - maxEn.Position);
							}
						}
						
					}

				}
			}

		}

		public static string VS(int? v) {
			if (v == null) return "";
			return v.Value.ToString();
		}
	}

}
