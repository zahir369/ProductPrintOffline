using System.Collections.Generic;
using System.ComponentModel;

namespace MKSS.Service.SemiCatalysis
{
    public class ProductConfig : INotifyPropertyChanged
    {
        public ProductConfig() {
            TimeTotal = 50;
            GradingTimePoint = 30;
            TimeInterval = 200;
            Name = "自定义"; TestTimePoints = "2,4,6,8,10,15,20,25,30,45";
            Grades = new List<ProductGrade>();
            YMin = -100;
            YMax = 100;
            XMin = 0;
            XMax = 60;
        }

        public void AddDefaultGrade( ){
            //http://www.divcss5.com/html/h636.shtml
            Grades.Add(new ProductGrade() { Grade = 1, To = -40, From = -60, DtTo = 10, DtFrom = -4, Color = "#263126" });
            Grades.Add(new ProductGrade() { Grade = 2, To = -30, From = -40, DtTo = 20, DtFrom = 10, Color = "#69E93B" });
            Grades.Add(new ProductGrade() { Grade = 3, To = 0, From = -30, DtTo = 30, DtFrom = 20, Color = "#EF9696" });
            Grades.Add(new ProductGrade() { Grade = 4, To = 30, From = 0, DtTo = 40, DtFrom = 30, Color = "#FF0000" });
            Grades.Add(new ProductGrade() { Grade = 5, To = 80, From = 30, DtTo = 60, DtFrom = 40, Color = "#B5AA10" });
            Grades.Add(new ProductGrade() { Grade = 6, To = 100, From = 80, DtTo = 100, DtFrom = 60, Color = "#1C10A9" });
        }

        public string Code { get; set; }
        public string Name { get; set; } 
        public int TimeTotal { get; set; }
        public int ZeroTime { get; set; } = 5;
        
        public double YMin { get; set; } = -6;
        public double YMax { get; set; } = 6;
        public double XMin { get; set; } = 0;
        public double XMax { get; set; } = 830;
        public double YMinDelta { get; set; } = -6;
        public double YMaxDelta { get; set; } = 6;
        public double XMinDelta { get; set; } = 0;
        public double XMaxDelta { get; set; } = 830;

        public bool ShowDelta { get; set; } = false;
        public bool LockX { get; set; } = false;
        public bool LockY { get; set; } = false;
        public int GradingTimePoint { get; set; }
        public string TestTimePoints { get; set; }
        public int TimeInterval { get; set; }
        public List<ProductGrade> Grades { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public void RefreshPage()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(""));
            }
        }
    }
    public class ProductGrade : INotifyPropertyChanged
    {
        public ProductGrade()
        {

        }
        public int Grade { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public int DtFrom { get; set; }
        public int DtTo { get; set; }
        public string FromTo { 
            get { return string.Format("{0}~{1}", From,To); } 
            set { 
                if (!string.IsNullOrEmpty(value)) {
                    try
                    {
                        string[] arr = value.Split("~");
                        From = int.Parse(arr[0]);
                        To = int.Parse(arr[1]);
                    }
                    catch (System.Exception)
                    {
                         
                    }
                    
                } 
            } 
        }
        public string Color { get; set; }


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
