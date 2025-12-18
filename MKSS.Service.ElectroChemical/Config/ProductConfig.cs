using MKSS.Model;
using System.Collections.Generic;
using System.ComponentModel;

namespace MKSS.Service.ElectroChemical
{
    public class ProductConfig : INotifyPropertyChanged
    {
        public ProductConfig() {
            TimeTotal = 600;
            GradingTimePoint = 50;
            TimeInterval = 200;
            Name = "自定义"; TestTimePoints = "4,30,50,70,80,150,180,250,280,300";
            Grades = new List<ProductGrade>();

            if (SensorGroupData.ShowV)
            {
                YMin = 0;
                YMax = 3.3;
                XMin = 0;
                XMax = 1000;
            }
            else
            {
                YMin = 0;
                YMax = 1000;
                XMin = 0;
                XMax = 1000;
            }
        }

        public void AddDefaultGrade( ){
            //http://www.divcss5.com/html/h636.shtml
            if (SensorGroupData.ShowV)
            {
                Grades.Add(new ProductGrade() { Grade = 1, To = 0.26, From = 0, Color = "#FF2F07EF" });
                Grades.Add(new ProductGrade() { Grade = 2, To = 0.33, From = 0.26, Color = "#FF393636" });
                Grades.Add(new ProductGrade() { Grade = 3, To = 0.80, From = 0.33, Color = "#FF4DB9EF" });
                Grades.Add(new ProductGrade() { Grade = 4, To = 1.20, From = 0.80, Color = "#FFCFCB00" });
                Grades.Add(new ProductGrade() { Grade = 5, To = 1.61, From = 1.20, Color = "#FFFF9B00" });
                Grades.Add(new ProductGrade() { Grade = 6, To = 3.50, From = 1.61, Color = "#FFFD4300" });
            }
            else {
                Grades.Add(new ProductGrade() { Grade = 1, To = 320, From = 0, Color = "#FF2F07EF" });
                Grades.Add(new ProductGrade() { Grade = 2, To = 410, From = 320, Color = "#FF393636" });
                Grades.Add(new ProductGrade() { Grade = 3, To = 1000, From = 410, Color = "#FF4DB9EF" });
                Grades.Add(new ProductGrade() { Grade = 4, To = 1500, From = 1000, Color = "#FFCFCB00" });
                Grades.Add(new ProductGrade() { Grade = 5, To = 2000, From = 1500, Color = "#FFFF9B00" });
                Grades.Add(new ProductGrade() { Grade = 6, To = 4300, From = 2000, Color = "#FFFD4300" });
            }
        }

        public string Code { get; set; }
        public string Name { get; set; } 
        public int TimeTotal { get; set; }
        public double YMin { get; set; }  
        public double YMax { get; set; }  
        public double XMin { get; set; }  
        public double XMax { get; set; } 
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
        public double From { get; set; }
        public double To { get; set; }
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
