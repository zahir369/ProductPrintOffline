using MKSS.APP.LaoHuaService.MQTT;
using MKSS.Model;
using MKSS.Service.LaoHuaElectroChemical;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

namespace MKSS.APP.SemiTester
{
    public class UIZhuiSuC10SettingModel: INotifyPropertyChanged {


		public double TxtCurrentSpan { get; set; } = 0;
		public double TxtCurrentSpanValue { get; set; } = 20.9;
		public int TxtCurrentSpanInt { get { return (int)TxtCurrentSpan; } }
		public double TxtCurrentSpanLatest { get; set; } = 0;   
		public ProductConfig ProductConfig { get; set; }
		public ProductModelConfig ProductModelConfig { get; set; }
		public TimeSpan CurrentTimeSpan { get; set; } = new TimeSpan();

		public SensorItemValueEnum ValueMode { get; set; } = SensorItemValueEnum.ValueND;
		public bool CanBtnStart { get { return !BtnStartIng ; } }
		public bool BtnStartIng { get; set; } = false;
		public bool CanBtnPause { get { return Started!= LaHuaTaskStatus.Stoped; } }
		public bool CanBtnFinish { get { return Started != LaHuaTaskStatus.Stoped; } }
		
		public bool CanBtnExcelExport { get { return Started == LaHuaTaskStatus.Stoped; } }
 
		List<ProductGradeExtEdit> GradeListExt = new List<ProductGradeExtEdit>();
		public List<ProductGradeExtEdit> GradeList
        {
			get {
				GradeListExt.Clear();
				
				UIZhuiSuC10SettingModel m = UIZhuiSuC10Model.Instance.SettingModel;
				for (int i = 1; i <= 6; i++)
				{ 
					if(ValueMode== SensorItemValueEnum.ValueDY)
						GradeListExt.Add(
						new ProductGradeExtEdit() { Pre = i == 1 ? null : m.OfGrade(i - 1), 
							Current = m.OfGrade(i), ItemEnum = SensorItemValueEnum.ValueDY });
					if (ValueMode == SensorItemValueEnum.ValueND)
						GradeListExt.Add(
						new ProductGradeExtEdit() { Pre = i == 1 ? null : m.OfGrade(i - 1), 
							Current = m.OfGrade(i), ItemEnum = SensorItemValueEnum.ValueND });
				}
                foreach (ProductGradeExtEdit ext in GradeListExt)
                {
					ProductGradeExtEdit next = GradeListExt.FirstOrDefault(w => w.ItemEnum==ext.ItemEnum && w.Current.Grade == ext.Current.Grade + 1);
					ext.Next = next;
				}
				GradeListExt = GradeListExt.OrderBy(w => (int)w.ItemEnum * 100 + w.Current.Grade).ToList();
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
		ProductGrade OfGradeSetFrom(SensorItemValueEnum e,int i, string vvStr)
		{
			var g = OfGrade(i);
			double vv = 0;
			if (!double.TryParse(vvStr, out vv)) return g;
            switch (e)
            {
                case SensorItemValueEnum.ValueDY:
					g.ADValue = vv;
					break;
                case SensorItemValueEnum.ValueND:
					g.NDValue = vv;
					break; 
            }
			ElectroChemicalConfgig.Save();
			return g;
		}

		 
		void OfBrushSet(int g, string value, SensorItemValueEnum e)
		{
			var grade = OfGrade(1);
			if (grade != null && e == SensorItemValueEnum.ValueND) grade.NDColor = value; 
			if (grade != null && e == SensorItemValueEnum.ValueDY) grade.ADColor = value;
		}

		public string OfBrush(int g, SensorItemValueEnum e)
		{
			if (ProductConfig == null || ProductModelConfig == null) return "#d0d0d0";
			if (g <= ProductModelConfig.Grades.Count)
			{
				string c = "#d0d0d0";
				if (e == SensorItemValueEnum.ValueND) c = ProductModelConfig.Grades[g - 1].NDColor; 
				if (e == SensorItemValueEnum.ValueDY) c = ProductModelConfig.Grades[g - 1].ADColor;
				if (string.IsNullOrEmpty(c)) return "#d0d0d0";
				return c;
			}
			return "#d0d0d0";
		}
		 

        /// <summary>
        ///  一开始检测，正在检测
        /// </summary>
        public LaHuaTaskStatus Started { get { return LaoHuaDataProvider.Instance.TaskStatus; } }
	 

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
		public SensorItemValueEnum ItemEnum { get; set; }
		public bool ReadOnly { get { return Pre == null; } set { } }
		public string GradeStart
		{
			get
			{
                if (Pre == null)
                {
					switch (ItemEnum)
					{
						case SensorItemValueEnum.ValueDY:
							return "0.00";
						case SensorItemValueEnum.ValueND:
							return "0.00";
						default:
							break;
					}
				}
				switch (ItemEnum)
				{
					case SensorItemValueEnum.ValueDY: 
						return Pre.ADValue.ToString("f2");
					case SensorItemValueEnum.ValueND: 
						return Pre.NDValue.ToString("f2"); 
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
					case SensorItemValueEnum.ValueDY:
						Pre.ADValue = d; break;
					case SensorItemValueEnum.ValueND:
						Pre.NDValue = d; break; 
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
					case SensorItemValueEnum.ValueDY: 
						return Current.ADValue.ToString("f2");
					case SensorItemValueEnum.ValueND: 
						return Current.NDValue.ToString("f2"); 
					default:
						break;
				}
				return "0";
			}
			set
			{
				double d = D(value);
				switch (ItemEnum)
				{
					case SensorItemValueEnum.ValueDY:
						Current.ADValue = d; break;
					case SensorItemValueEnum.ValueND:
						Current.NDValue = d; break; 
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
					case SensorItemValueEnum.ValueDY:
						return Current.ADColor;
					case SensorItemValueEnum.ValueND:
						return Current.NDColor; 
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
					case SensorItemValueEnum.ValueDY:
						Current.ADColor = d; break;
					case SensorItemValueEnum.ValueND:
						Current.NDColor = d; break; 
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
