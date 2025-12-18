using MKSS.Model;
using MKSS.Service.O2Tester;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace MKSS.APP.O2Tester
{
    public class UIZhuiSuC10SettingModel: INotifyPropertyChanged {

		public int FullQueryTimesText { get; set; } = 0;
		public string TxtSerialPorts { get; set; }
		public int TxtTotalSpan { get; set; } = 21; 
		public double TxtCurrentSpan { get; set; } = 0;
		public double TxtCurrentSpanValue { get; set; } = 20.9;
		public int TxtCurrentSpanInt { get { return (int)TxtCurrentSpan; } }
		public double TxtCurrentSpanLatest { get; set; } = 0;
		public int TxtGradingTimePoint { get; set; } = 50; 
        public string TxtSensorGrougAddress { get; set; } = "1-2";
		public string TxtProductList { get; set; }
		public string TxtModelList { get; set; }
		public ProductConfig ProductConfig { get; set; }
		public ProductModelConfig ProductModelConfig { get; set; }
		public TimeSpan CurrentTimeSpan { get; set; } = new TimeSpan();

		public bool CanTxtSerialPorts { get { return !Started; } } 
		public bool CanTxtGradingTimePoint { get { return true; } }
		public bool CanTxtSensorGrougAddress { get { return !Started; } }
		public bool CanTxtProductList { get { return !Started; } }
		public bool CanTxtModelList { get { return !Started; } }
		public bool CanBtnStart { get { return !BtnStartIng ; } }
		public bool BtnStartIng { get; set; } = false;
		public bool CanBtnPause { get { return Started; } }
		public bool CanBtnFinish { get { return Started; } }
		
		public bool CanBtnExcelExport { get { return !Started; } }
		 
		List<ProductGradeExtEdit> GradeListExt = new List<ProductGradeExtEdit>();
		public List<ProductGradeExtEdit> GradeList
        {
			get
			{
				//if (GradeListExt.Count == 0) {
				GradeListExt.Clear();
				UIZhuiSuC10SettingModel m = UIZhuiSuC10Model.Instance.SettingModel;
				for (int i = 1; i <= 6; i++)
				{
					GradeListExt.Add(new ProductGradeExtEdit() { Pre = i == 1 ? null : m.OfGrade(i - 1), Current = m.OfGrade(i), ItemEnum = SensorItemEnum.SrcData });
					GradeListExt.Add(new ProductGradeExtEdit() { Pre = i == 1 ? null : m.OfGrade(i - 1), Current = m.OfGrade(i), ItemEnum = SensorItemEnum.T90 });
					GradeListExt.Add(new ProductGradeExtEdit() { Pre = i == 1 ? null : m.OfGrade(i - 1), Current = m.OfGrade(i), ItemEnum = SensorItemEnum.T10 });
				}
				foreach (ProductGradeExtEdit ext in GradeListExt)
				{
					ProductGradeExtEdit next = GradeListExt.FirstOrDefault(w => w.ItemEnum == ext.ItemEnum && w.Current.Grade == ext.Current.Grade + 1);
					ext.Next = next;
				}
				GradeListExt = GradeListExt.OrderBy(w => (int)w.ItemEnum * 100 + w.Current.Grade).ToList();
				//}
				return GradeListExt;
			}
		}

		public ProductGrade OfGrade(int g)
		{
			if (ProductConfig == null || ProductModelConfig == null) return null;
			if (ProductModelConfig == null) return null;
			if (g <= ProductModelConfig.Grades.Count)
				return ProductModelConfig.Grades[g - 1];
			return null;
		}
		  

		public string OfBrush(int g, SensorItemEnum e)
		{
			if (ProductConfig == null || ProductModelConfig == null) return "#d0d0d0";
			if (g <= ProductModelConfig.Grades.Count)
			{
				string c = "#d0d0d0";
				if (e == SensorItemEnum.SrcData) {

					switch (SensorGroupData.ShowMode)
					{
						case ShowModeEnum.DuanDianYa:
							c = ProductModelConfig.Grades[g - 1].DianYaColor; break;
						case ShowModeEnum.NongDu:
							c = ProductModelConfig.Grades[g - 1].NongDuColor; break;
						case ShowModeEnum.AD:
							c = ProductModelConfig.Grades[g - 1].ADColor; break;
						default:
							break;
					} 
				} 
				if (e == SensorItemEnum.T10) c = ProductModelConfig.Grades[g - 1].T10Color;
				if (e == SensorItemEnum.T90) c = ProductModelConfig.Grades[g - 1].T90Color;
				if (string.IsNullOrEmpty(c)) return "#d0d0d0";
				return c;
			}
			return "#d0d0d0";
		}
		 

        /// <summary>
        ///  一开始检测，正在检测
        /// </summary>
        public bool Started { get; set; }
		public List<int> Address()
		{
			List<int> ret = new List<int>();
			if (!string.IsNullOrEmpty(TxtSensorGrougAddress))
			{
				string src = TxtSensorGrougAddress;
				src = src.Replace("-", "-");
				string[] linge = src.Split(",".ToCharArray());
				foreach (var item in linge)
				{
					int itemInt = -1;
					if (int.TryParse(item, out itemInt))
					{
						if (itemInt <= 255)
						{
							 ret.Add((byte)itemInt);
						}
					}
					else
					{
						string[] from_to = item.Split("-".ToCharArray());
						if (from_to.Length == 2)
						{
							int itemIntFrom = -1;
							int itemIntTo = -1;
							if (int.TryParse(from_to[0], out itemIntFrom) && int.TryParse(from_to[1], out itemIntTo))
							{
								if (itemIntFrom <= 255 && itemIntTo <= 255)
								{
									for (int i = itemIntFrom; i <= itemIntTo; i++)
									{
										if (!ret.Contains((byte)i)) ret.Add((byte)i);
									}
								}
							}
						}
					}
				}
			}
			return ret;
		}


		public string TxtTestTimePoints { get; set; }  
		public int[] TxtTestTimePointsArr {
			get {
				List<int> ret = new List<int>();
				string[] arr =  (TxtTestTimePoints+"").Split(","); 
                foreach (var item in arr)
                {
					if (string.IsNullOrEmpty(item)) continue;
					ret.Add(int.Parse(item));
				}
				return ret.ToArray();
			}
		}


		public event PropertyChangedEventHandler PropertyChanged;
		public void RefreshPage()
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(""));
			}
		}
	}

	public class ProductGradeExtEdit : INotifyPropertyChanged
	{
		public ProductGradeExtEdit Tag { get { return this; } }
		public ProductGrade Pre { get; set; }
		public ProductGradeExtEdit Next { get; set; }
		public ProductGrade Current { get; set; }
		public SensorItemEnum ItemEnum { get; set; }
		public bool ReadOnly { get { return Pre == null; } set { } }
		public string GradeStart
		{
			get
			{
				if (ReadOnly) return "0.00";
				switch (ItemEnum)
				{
					case SensorItemEnum.T90:
						return Pre.T90Value.ToString("f2");
					case SensorItemEnum.SrcData:
						switch (SensorGroupData.ShowMode)
						{
							case ShowModeEnum.DuanDianYa:
								return Pre.DianYaValue.ToString("f2");
							case ShowModeEnum.NongDu:
								return Pre.NongDuValue.ToString("f2");
							case ShowModeEnum.AD:
								return Pre.ADValue.ToString("f0");
							default:
								break;
						}
						break;
					case SensorItemEnum.T10:
						return Pre.T10Value.ToString("f2");
					default:
						break;
				}
				return "0.00";
			}
			set
			{
				if (ReadOnly) return;
				double d = D(value);
				switch (ItemEnum)
				{
					case SensorItemEnum.T90:
						Pre.T90Value = d; break;
					case SensorItemEnum.SrcData:
						
						switch (SensorGroupData.ShowMode)
						{
							case ShowModeEnum.DuanDianYa:
								Pre.DianYaValue = d; break;
							case ShowModeEnum.NongDu:
								Pre.NongDuValue = d; break;
							case ShowModeEnum.AD:
								Pre.ADValue = d; break;
							default:
								break;
						}
						break;
					case SensorItemEnum.T10:
						Pre.T10Value = d; break;
					default:
						break;
				}
			}
		}
		public string GradeEnd
		{
			get
			{
				switch (ItemEnum)
				{
					case SensorItemEnum.T90:
						return Current.T90Value.ToString("f2");
					case SensorItemEnum.SrcData:
						switch (SensorGroupData.ShowMode)
						{
							case ShowModeEnum.DuanDianYa:
								return Current.DianYaValue.ToString("f2");
							case ShowModeEnum.NongDu:
								return Current.NongDuValue.ToString("f2");
							case ShowModeEnum.AD:
								return Current.ADValue.ToString("f0");
							default:
								break;
						}
						break;
					case SensorItemEnum.T10:
						return Current.T10Value.ToString("f2");
					default:
						break;
				}
				return "0.00";
			}
			set
			{
				double d = D(value);
				switch (ItemEnum)
				{
					case SensorItemEnum.T90:
						Current.T90Value = d; break;
					case SensorItemEnum.SrcData:
						switch (SensorGroupData.ShowMode)
						{
							case ShowModeEnum.DuanDianYa:
								Current.DianYaValue = d; break;
							case ShowModeEnum.NongDu:
								Current.NongDuValue = d; break;
							case ShowModeEnum.AD:
								Current.ADValue = d; break;
							default:
								break;
						}
						break;
					case SensorItemEnum.T10:
						Current.T10Value = d; break;
					default:
						break;
				}
				if (Next != null) {
					Next.RefreshPage();
				}
			}
		}
		public string GradeColor
		{
			get
			{
				switch (ItemEnum)
				{
					case SensorItemEnum.T90:
						return Current.T90Color;
					case SensorItemEnum.SrcData:
						switch (SensorGroupData.ShowMode)
						{
							case ShowModeEnum.DuanDianYa:
								return Current.DianYaColor;
							case ShowModeEnum.NongDu:
								return Current.NongDuColor;
							case ShowModeEnum.AD:
								return Current.ADColor;
							default:
								break;
						}
						break;
						
					case SensorItemEnum.T10:
						return Current.T10Color;
					default:
						break;
				}
				return "#ffffff";
			}
			set
			{
				string d = (value);
				switch (ItemEnum)
				{
					case SensorItemEnum.T90:
						Current.T90Color = d; break;
					case SensorItemEnum.SrcData:
						
						switch (SensorGroupData.ShowMode)
						{
							case ShowModeEnum.DuanDianYa:
								Current.DianYaColor = d; break;
							case ShowModeEnum.NongDu:
								Current.NongDuColor = d; break;
							case ShowModeEnum.AD:
								Current.ADColor = d; break;
							default:
								break;
						}
						break;
					case SensorItemEnum.T10:
						Current.T10Color = d; break;
					default:
						break;
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;
		public void RefreshPage()
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(""));
			}
		}

		double D(string str)
		{
			double d = 0.00;
			if (double.TryParse(str, out d))
			{
				return d;
			}
			return d;
		}
	}
}
