using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MKSS.Model
{
    public class ProductItem
    {
        public ProductItem() {

            TimeTotal = 300;
            TimeInterval = 1000;
            Id = DateTime.Now.ToString("yyyyMMddHHmmss");
            Code = Id;
            Name = "自定义";
            Fomula = "";
            TimeList = "4,30,50,70,80,150,180,280";

            GradePoint = 35;
            Grades = new List<ProductGrade>();
            //http://www.divcss5.com/html/h636.shtml
            Grades.Add(new ProductGrade() { Grade = 1, From = 100, To = 200, Color = "#0072E3" });
            Grades.Add(new ProductGrade() { Grade = 2, From = 200, To = 300, Color = "#00E3E3" });
            Grades.Add(new ProductGrade() { Grade = 3, From = 300, To = 400, Color = "#02DF82" });
            Grades.Add(new ProductGrade() { Grade = 4, From = 400, To = 500, Color = "#9AFF02" });
            Grades.Add(new ProductGrade() { Grade = 5, From = 500, To = 600, Color = "#C4C400" });
            Grades.Add(new ProductGrade() { Grade = 6, From = 600, To = 700, Color = "#FF8000" });

        }

        public string Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Fomula { get; set; }
        public int TimeTotal { get; set; }
        public string TimeList { get; set; }
        public int TimeInterval { get; set; }
        public int GradePoint { get; set; }
        public List<ProductGrade> Grades { get; set; }

        public string ToXml() {
            XmlSerializer serializer = new XmlSerializer(this.GetType());
            string content = string.Empty;
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, this);
                content = writer.ToString();
            }
            return content;
        }

        public static ProductItem FromXml(string content)
        {
            if (string.IsNullOrEmpty(content)) {
                return new ProductItem();
            }
            StringReader reader = new StringReader(content);
            XmlSerializer serializer = new XmlSerializer(typeof(ProductItem));
            ProductItem p = (ProductItem)serializer.Deserialize(reader);
            return p;
        }

    }
    public class ProductGrade
    {
        public ProductGrade()
        {
             
        }
        public int Grade { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string Color { get; set; } 
    }
}
