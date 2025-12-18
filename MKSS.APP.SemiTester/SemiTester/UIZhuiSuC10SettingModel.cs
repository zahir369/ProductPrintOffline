using MKSS.Service.SemiTester;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace MKSS.APP.SemiTester
{
    public class UIZhuiSuC10SettingModel: INotifyPropertyChanged {

        public string TxtSerialPorts { get; set; } 
        public int TxtTotalSpan { get; set; } = 30;
		public int TxtZeroPoint { get; set; } = 3;
		public int TxtSpanPoint { get; set; } = 25;
		public double TxtCurrentSpan { get; set; } = 0;
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
		public bool CanTxtTotalSpan { get { return !Started; } }
		public bool CanTxtZeroPoint { get { return !Started; } }
		public bool CanTxtSpanPoint { get { return !Started; } }
		public bool CanTxtGradingTimePoint { get { return true; } }
		public bool CanTxtSensorGrougAddress { get { return !Started; } }
		public bool CanTxtProductList { get { return !Started; } }
		public bool CanTxtModelList { get { return !Started; } }
		public bool CanBtnStart { get { return !BtnStartIng ; } }
		public bool BtnStartIng { get; set; } = false;
		public bool CanBtnPause { get { return Started; } }
		public bool CanBtnFinish { get { return Started; } }
		
		public bool CanBtnExcelExport { get { return !Started; } }

		public string TxtG1Final { get { var g = OfGrade(1); return g == null ? "0" : g.FinalValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Final, 1, value); } }
		public string TxtG2Final { get { var g = OfGrade(2); return g == null ? "0" : g.FinalValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Final, 2, value); } }
		public string TxtG3Final { get { var g = OfGrade(3); return g == null ? "0" : g.FinalValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Final, 3, value); } }
		public string TxtG4Final { get { var g = OfGrade(4); return g == null ? "0" : g.FinalValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Final, 4, value); } }
		public string TxtG5Final { get { var g = OfGrade(5); return g == null ? "0" : g.FinalValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Final, 5, value); } }
		public string TxtG6Final { get { var g = OfGrade(6); return g == null ? "0" : g.FinalValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Final, 6, value); } }

		public string TxtG1Delta { get { var g = OfGrade(1); return g == null ? "0" : g.DeltaValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Delta, 1, value); } }
		public string TxtG2Delta { get { var g = OfGrade(2); return g == null ? "0" : g.DeltaValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Delta, 2, value); } }
		public string TxtG3Delta { get { var g = OfGrade(3); return g == null ? "0" : g.DeltaValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Delta, 3, value); } }
		public string TxtG4Delta { get { var g = OfGrade(4); return g == null ? "0" : g.DeltaValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Delta, 4, value); } }
		public string TxtG5Delta { get { var g = OfGrade(5); return g == null ? "0" : g.DeltaValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Delta, 5, value); } }
		public string TxtG6Delta { get { var g = OfGrade(6); return g == null ? "0" : g.DeltaValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Delta, 6, value); } }

		public string TxtG1Start { get { var g = OfGrade(1); return g == null ? "0" : g.StartValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Start, 1, value); } }
		public string TxtG2Start { get { var g = OfGrade(2); return g == null ? "0" : g.StartValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Start, 2, value); } }
		public string TxtG3Start { get { var g = OfGrade(3); return g == null ? "0" : g.StartValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Start, 3, value); } }
		public string TxtG4Start { get { var g = OfGrade(4); return g == null ? "0" : g.StartValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Start, 4, value); } }
		public string TxtG5Start { get { var g = OfGrade(5); return g == null ? "0" : g.StartValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Start, 5, value); } }
		public string TxtG6Start { get { var g = OfGrade(6); return g == null ? "0" : g.StartValue.ToString("f2"); } set { var g = OfGradeSetFrom(SensorItemEnum.Start, 6, value); } }

		List<ProductGradeExtEdit> GradeListExt = new List<ProductGradeExtEdit>();
		public List<ProductGradeExtEdit> GradeList
        {
			get {
				//if (GradeListExt.Count == 0) {
				GradeListExt.Clear();
				UIZhuiSuC10SettingModel m = UIZhuiSuC10Model.Instance.SettingModel;
					for (int i = 1; i <= 6; i++)
					{ 
						GradeListExt.Add(new ProductGradeExtEdit() { Pre = i == 1 ? null : m.OfGrade(i - 1), Current = m.OfGrade(i), ItemEnum = SensorItemEnum.Start });
						GradeListExt.Add(new ProductGradeExtEdit() { Pre = i == 1 ? null : m.OfGrade(i - 1), Current = m.OfGrade(i), ItemEnum = SensorItemEnum.Final });
						GradeListExt.Add(new ProductGradeExtEdit() { Pre = i == 1 ? null : m.OfGrade(i - 1),  Current = m.OfGrade(i), ItemEnum = SensorItemEnum.Delta });
					}
                    foreach (ProductGradeExtEdit ext in GradeListExt)
                    {
						ProductGradeExtEdit next = GradeListExt.FirstOrDefault(w => w.ItemEnum==ext.ItemEnum && w.Current.Grade == ext.Current.Grade + 1);
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
		ProductGrade OfGradeSetFrom(SensorItemEnum e,int i, string vvStr)
		{
			var g = OfGrade(i);
			double vv = 0;
			if (!double.TryParse(vvStr, out vv)) return g;
            switch (e)
            {
                case SensorItemEnum.Start:
					g.StartValue = vv;
					break;
                case SensorItemEnum.Final:
					g.FinalValue = vv;
					break;
                case SensorItemEnum.Delta:
					g.DeltaValue = vv;
					break;
                case SensorItemEnum.ColorDiagram:
                    break;
                default:
                    break;
            }
			SemiTesterConfgig.Save();
			return g;
		}
		 


		public string TxtGrading1FinalColor { get { var g = OfBrush(1, SensorItemEnum.Final); return g; } set { OfBrushSet(1, value, SensorItemEnum.Final); } }
		public string TxtGrading2FinalColor { get { var g = OfBrush(2, SensorItemEnum.Final); return g; } set { OfBrushSet(2, value, SensorItemEnum.Final); } }
		public string TxtGrading3FinalColor { get { var g = OfBrush(3, SensorItemEnum.Final); return g; } set { OfBrushSet(3, value, SensorItemEnum.Final); } }
		public string TxtGrading4FinalColor { get { var g = OfBrush(4, SensorItemEnum.Final); return g; } set { OfBrushSet(4, value, SensorItemEnum.Final); } }
		public string TxtGrading5FinalColor { get { var g = OfBrush(5, SensorItemEnum.Final); return g; } set { OfBrushSet(5, value, SensorItemEnum.Final); } }
		public string TxtGrading6FinalColor { get { var g = OfBrush(6, SensorItemEnum.Final); return g; } set { OfBrushSet(6, value, SensorItemEnum.Final); } }

		public string TxtGrading1StartColor { get { var g = OfBrush(1, SensorItemEnum.Start); return g; } set { OfBrushSet(1, value, SensorItemEnum.Start); } }
		public string TxtGrading2StartColor { get { var g = OfBrush(2, SensorItemEnum.Start); return g; } set { OfBrushSet(2, value, SensorItemEnum.Start); } }
		public string TxtGrading3StartColor { get { var g = OfBrush(3, SensorItemEnum.Start); return g; } set { OfBrushSet(3, value, SensorItemEnum.Start); } }
		public string TxtGrading4StartColor { get { var g = OfBrush(4, SensorItemEnum.Start); return g; } set { OfBrushSet(4, value, SensorItemEnum.Start); } }
		public string TxtGrading5StartColor { get { var g = OfBrush(5, SensorItemEnum.Start); return g; } set { OfBrushSet(5, value, SensorItemEnum.Start); } }
		public string TxtGrading6StartColor { get { var g = OfBrush(6, SensorItemEnum.Start); return g; } set { OfBrushSet(6, value, SensorItemEnum.Start); } }

		public string TxtGrading1DeltaColor { get { var g = OfBrush(1, SensorItemEnum.Delta); return g; } set { OfBrushSet(1, value, SensorItemEnum.Delta); } }
		public string TxtGrading2DeltaColor { get { var g = OfBrush(2, SensorItemEnum.Delta); return g; } set { OfBrushSet(2, value, SensorItemEnum.Delta); } }
		public string TxtGrading3DeltaColor { get { var g = OfBrush(3, SensorItemEnum.Delta); return g; } set { OfBrushSet(3, value, SensorItemEnum.Delta); } }
		public string TxtGrading4DeltaColor { get { var g = OfBrush(4, SensorItemEnum.Delta); return g; } set { OfBrushSet(4, value, SensorItemEnum.Delta); } }
		public string TxtGrading5DeltaColor { get { var g = OfBrush(5, SensorItemEnum.Delta); return g; } set { OfBrushSet(5, value, SensorItemEnum.Delta); } }
		public string TxtGrading6DeltaColor { get { var g = OfBrush(6, SensorItemEnum.Delta); return g; } set { OfBrushSet(6, value, SensorItemEnum.Delta); } }


		void OfBrushSet(int g, string value, SensorItemEnum e)
		{
			var grade = OfGrade(1);
			if (grade != null && e == SensorItemEnum.Final) grade.FinalColor = value;
			if (grade != null && e == SensorItemEnum.Delta) grade.DeltaColor = value;
			if (grade != null && e == SensorItemEnum.Start) grade.StartColor = value;
		}

		public string OfBrush(int g, SensorItemEnum e)
		{
			if (ProductConfig == null || ProductModelConfig == null) return "#d0d0d0";
			if (g <= ProductModelConfig.Grades.Count)
			{
				string c = "#d0d0d0";
				if (e == SensorItemEnum.Final) c = ProductModelConfig.Grades[g - 1].FinalColor;
				if (e == SensorItemEnum.Delta) c = ProductModelConfig.Grades[g - 1].DeltaColor;
				if (e == SensorItemEnum.Start) c = ProductModelConfig.Grades[g - 1].StartColor;
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
					case SensorItemEnum.Start:
						return Pre.StartValue.ToString("f2");
					case SensorItemEnum.Final:
						return Pre.FinalValue.ToString("f2");
					case SensorItemEnum.Delta:
						return Pre.DeltaValue.ToString("f2");
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
					case SensorItemEnum.Start:
						Pre.StartValue = d; break;
					case SensorItemEnum.Final:
						Pre.FinalValue = d; break;
					case SensorItemEnum.Delta:
						Pre.DeltaValue = d; break;
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
					case SensorItemEnum.Start:
						return Current.StartValue.ToString("f2");
					case SensorItemEnum.Final:
						return Current.FinalValue.ToString("f2");
					case SensorItemEnum.Delta:
						return Current.DeltaValue.ToString("f2");
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
					case SensorItemEnum.Start:
						Current.StartValue = d; break;
					case SensorItemEnum.Final:
						Current.FinalValue = d; break;
					case SensorItemEnum.Delta:
						Current.DeltaValue = d; break;
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
					case SensorItemEnum.Start:
						return Current.StartColor;
					case SensorItemEnum.Final:
						return Current.FinalColor;
					case SensorItemEnum.Delta:
						return Current.DeltaColor;
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
					case SensorItemEnum.Start:
						Current.StartColor = d; break;
					case SensorItemEnum.Final:
						Current.FinalColor = d; break;
					case SensorItemEnum.Delta:
						Current.DeltaColor = d; break;
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
