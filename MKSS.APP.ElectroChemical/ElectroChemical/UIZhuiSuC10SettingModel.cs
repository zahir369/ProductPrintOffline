using MKSS.Service.ElectroChemical;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace MKSS.APP.ElectroChemical
{
    public class UIZhuiSuC10SettingModel: INotifyPropertyChanged {
        public string TxtSerialPorts { get; set; } 
        public int TxtTotalSpan { get; set; } = 300;
		public int TxtCurrentSpan { get; set; } = 0;
		public int TxtCurrentSpanLatest { get; set; } = 0;
		public int TxtGradingTimePoint { get; set; } = 50; 
        public string TxtSensorGrougAddress { get; set; } = "1-4";
		public string TxtProductList { get; set; }
		public ProductConfig ProductConfig { get; set; }

		public TimeSpan CurrentTimeSpan { get; set; } = new TimeSpan();
		


		public bool CanTxtSerialPorts { get { return !Started; } }
		public bool CanTxtTotalSpan { get { return !Started; } }
		public bool CanTxtGradingTimePoint { get { return true; } }
		public bool CanTxtSensorGrougAddress { get { return !Started; } }
		public bool CanTxtProductList { get { return !Started; } }
		public bool CanBtnStart { get { return !Started; } }
		public bool CanBtnPause { get { return Started; } }
		public bool CanBtnFinish { get { return Started; } }
		public bool CanBtnExcelExport { get { return !Started; } }

		public string TxtGrading1Range { get { var g = OfGrade(1); return g == null ? "0~0" : g.FromTo; }  set { var g = OfGradeSet(1, value); } }
		public string TxtGrading2Range { get { var g = OfGrade(2); return g == null ? "0~0" : g.FromTo; } set { var g = OfGradeSet(2, value);  } }
		public string TxtGrading3Range { get { var g = OfGrade(3); return g == null ? "0~0" : g.FromTo; } set { var g = OfGradeSet(3, value);   } }
		public string TxtGrading4Range { get { var g = OfGrade(4); return g == null ? "0~0" : g.FromTo; } set { var g = OfGradeSet(4, value);  } }
		public string TxtGrading5Range { get { var g = OfGrade(5); return g == null ? "0~0" : g.FromTo; } set { var g = OfGradeSet(5, value); } }
		public string TxtGrading6Range { get { var g = OfGrade(6); return g == null ? "0~0" : g.FromTo; } set { var g = OfGradeSet(6, value);  } }


		public double TxtGrading1From { get { var g = OfGrade(1); return g == null ? 0 : g.From; } set { var g = OfGradeSetFrom(1, value); } }
		public double TxtGrading2From { get { var g = OfGrade(2); return g == null ? 0 : g.From; } set { var g = OfGradeSetFrom(2, value); } }
		public double TxtGrading3From { get { var g = OfGrade(3); return g == null ? 0 : g.From; } set { var g = OfGradeSetFrom(3, value); } }
		public double TxtGrading4From { get { var g = OfGrade(4); return g == null ? 0 : g.From; } set { var g = OfGradeSetFrom(4, value); } }
		public double TxtGrading5From { get { var g = OfGrade(5); return g == null ? 0 : g.From; } set { var g = OfGradeSetFrom(5, value); } }
		public double TxtGrading6From { get { var g = OfGrade(6); return g == null ? 0 : g.From; } set { var g = OfGradeSetFrom(6, value); } }


		public double TxtGrading1To { get { var g = OfGrade(1); return g == null ? 0 : g.To; } set { var g = OfGradeSetTo(1, value); } }
		public double TxtGrading2To { get { var g = OfGrade(2); return g == null ? 0 : g.To; } set { var g = OfGradeSetTo(2, value); } }
		public double TxtGrading3To { get { var g = OfGrade(3); return g == null ? 0 : g.To; } set { var g = OfGradeSetTo(3, value); } }
		public double TxtGrading4To { get { var g = OfGrade(4); return g == null ? 0 : g.To; } set { var g = OfGradeSetTo(4, value); } }
		public double TxtGrading5To { get { var g = OfGrade(5); return g == null ? 0 : g.To; } set { var g = OfGradeSetTo(5, value); } }
		public double TxtGrading6To { get { var g = OfGrade(6); return g == null ? 0 : g.To; } set { var g = OfGradeSetTo(6, value); } }

		ProductGrade OfGradeSet(int i,string fromTo) {
			var g = OfGrade(i);
			g.FromTo = fromTo;
			if (i > 1) {
				var g0 = OfGrade(i-1);
				g0.From = g.To;
			}
			if (i < 6)
			{
				var g0 = OfGrade(i + 1);
				g0.To = g.From;
			}
			return g;
		}


		ProductGrade OfGradeSetFrom(int i, double vv)
		{
			var g = OfGrade(i);
			g.From = vv;
			if (i > 1)
			{
				var g0 = OfGrade(i - 1);
				g0.To = vv;
			} 
			return g;
		}

		ProductGrade OfGradeSetTo(int i, double vv)
		{
			var g = OfGrade(i);
			g.To = vv;
			if (i < 6)
			{
				var g0 = OfGrade(i + 1);
				g0.From = vv;
			}
			return g;
		}

		//TxtGrading6HeGe
		public string TxtGrading1HeGe { get; set; } = "一级";
		public string TxtGrading2HeGe { get; set; } = "二级";
		public string TxtGrading3HeGe { get; set; } = "三级";
		public string TxtGrading4HeGe { get; set; } = "四级";
		public string TxtGrading5HeGe { get; set; } = "五级";
		public string TxtGrading6HeGe { get; set; } = "六级";
		public void CalcHeGe()
		{
			

			int[] c6 = new int[] { 0, 0, 0, 0, 0, 0 };

			try
			{
				if (ProductConfig == null || UIZhuiSuC10Model.Instance.ECService.BatchSensorDataLastDictionary == null) 
					return;
				foreach (var item in UIZhuiSuC10Model.Instance.ECService.BatchSensorDataLastDictionary.Data.Values)
				{
					var vv = item.F_DataValue;
					for (int i = 0; i < ProductConfig.Grades.Count; i++)
					{
						ProductGrade ci = ProductConfig.Grades[i];
						if (vv <= ci.To && vv > ci.From)
						{
							c6[i] = c6[i] + 1;
						}
					}
				}
			}
			catch (System.Exception)
			{
				 
			}
			finally {
				TxtGrading1HeGe = string.Format("一级{0}个", c6[0]);
				TxtGrading2HeGe = string.Format("二级{0}个", c6[1]);
				TxtGrading3HeGe = string.Format("三级{0}个", c6[2]);
				TxtGrading4HeGe = string.Format("四级{0}个", c6[3]);
				TxtGrading5HeGe = string.Format("五级{0}个", c6[4]);
				TxtGrading6HeGe = string.Format("六级{0}个", c6[5]);
			}
			

		}


		public string TxtGrading1Color { get { var g = OfBrush(1); return g; } set { OfBrushSet(1, value); } }
		public string TxtGrading2Color { get { var g = OfBrush(2); return g; } set { OfBrushSet(2, value); } }
		public string TxtGrading3Color { get { var g = OfBrush(3); return g; } set { OfBrushSet(3, value); } }
		public string TxtGrading4Color { get { var g = OfBrush(4); return g; } set { OfBrushSet(4, value); } }
		public string TxtGrading5Color { get { var g = OfBrush(5); return g; } set { OfBrushSet(5, value); } }
		public string TxtGrading6Color { get { var g = OfBrush(6); return g; } set { OfBrushSet(6, value); } }

		void OfBrushSet(int g, string value)
		{
			var grade = OfGrade(1); if(grade!=null) grade.Color = value;
		}
		string OfBrush(int g)
		{
			if (ProductConfig == null) return "#d0d0d0";
			if (g <= ProductConfig.Grades.Count)
			{
				string c = ProductConfig.Grades[g - 1].Color;
				if (string.IsNullOrEmpty(c)) return "#d0d0d0";
				return ProductConfig.Grades[g - 1].Color;
			}
			return "#d0d0d0";
		}

		//System.Windows.Media.BrushConverter converter = new System.Windows.Media.BrushConverter();
		//public Brush TxtGrading1Color { get { var g = OfBrush(1); return g; } set { OfBrushSet(1,value); } }
		//public Brush TxtGrading2Color { get { var g = OfBrush(2); return g; } set { OfBrushSet(2,value); } }
		//public Brush TxtGrading3Color { get { var g = OfBrush(3); return g; } set { OfBrushSet(3,value); } }
		//public Brush TxtGrading4Color { get { var g = OfBrush(4); return g; } set { OfBrushSet(4,value); } }
		//public Brush TxtGrading5Color { get { var g = OfBrush(5); return g; } set { OfBrushSet(5,value); } }
		//public Brush TxtGrading6Color { get { var g = OfBrush(6); return g; } set { OfBrushSet(6,value); } }
		//void OfBrushSet(int g, Brush value)
		//{
		//	var grade = OfGrade(1);
		//	if (value is SolidBrush) {
		//		grade.Color = System.Drawing.ColorTranslator.ToHtml(((SolidBrush)value).Color);
		//	} 
		//}
		//SolidBrush OfBrush(int g) {
		//	if (ProductConfig == null) return new SolidBrush(Color.Gray);
		//	if (g <= ProductConfig.Grades.Count) {
		//		string c = ProductConfig.Grades[g - 1].Color;
		//		if(string.IsNullOrEmpty(c)) return new SolidBrush(Color.Gray);
		//		return (SolidBrush)converter.ConvertFromString(ProductConfig.Grades[g - 1].Color);
		//	} 
		//	return new SolidBrush(Color.Gray);
		//}


		public ProductGrade OfGrade(int g) {
			if (ProductConfig == null)  return null;
			if (g <= ProductConfig.Grades.Count) 
				return ProductConfig.Grades[g - 1];
			return null;
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

}
