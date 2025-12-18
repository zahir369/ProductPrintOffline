using MKSS.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MKSS.Model
{
    public class ProductConfig : INotifyPropertyChanged
    {

        public ProductConfig()
        {
            TimeInterval = 200;
            Name = "自定义"; 
        }
         

        public string Code { get; set; }
        public string Name { get; set; } 
         
        public int TimeInterval { get; set; }
        public SensorItemValueEnum ColorDiagram { get; set; } = SensorItemValueEnum.ValueDY; 
        public List<ProductModelConfig> Models { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RefreshPage()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(""));
            }
        }
    }

    public class ProductModelConfig
    {

        public ProductModelConfig()
        {
            GradesDictionary = new SerializableDictionary<string, ResistanceConfig>();
        }

        public string Name { get; set; } = "默认";
        public SerializableDictionary<string, ResistanceConfig> GradesDictionary { get; set; }

        public static List<ProductModelConfig> AddDefaultModelList()
        {
            List<ProductModelConfig> ret = new List<ProductModelConfig>();
            ret.Add(AddDefaultModel());
            return ret;
        }

        public static ProductModelConfig AddDefaultModel()
        {
            ProductModelConfig r = new ProductModelConfig()
            {
                GradesDictionary = AddDefaultGradeDic(),
            };
            return r;
        }
        public static SerializableDictionary<string, ResistanceConfig> AddDefaultGradeDic()
        {
            SerializableDictionary<string, ResistanceConfig> dic = new SerializableDictionary<string, ResistanceConfig>();
            List<ProductGrade> Grades = AddDefaultGradeItem();
            //http://www.divcss5.com/html/h636.shtml
            dic.Add("Default", new ResistanceConfig() { Grades = Grades });
            return dic;
        }
        static List<ProductGrade> AddDefaultGradeItem()
        {
            List<ProductGrade> Grades = new List<ProductGrade>();
            //http://www.divcss5.com/html/h636.shtml

            Grades.Add(new ProductGrade() { Grade = 1, ADValue = 5, NDValue = 10, XXValue = 0.09, XXColor = "#FF9D1212", NDColor = "#FF9D1212", ADColor = "#FF9D1212" });
            Grades.Add(new ProductGrade() { Grade = 2, ADValue = 10, NDValue = 19, XXValue = 2, XXColor = "#FF1A1091", NDColor = "#FF1A1091", ADColor = "#FF1A1091" });
            Grades.Add(new ProductGrade() { Grade = 3, ADValue = 15, NDValue = 23, XXValue = 2.4, XXColor = "#FFC1A615", NDColor = "#FFC1A615", ADColor = "#FFC1A615" });
            Grades.Add(new ProductGrade() { Grade = 4, ADValue = 20, NDValue = 35, XXValue = 3.12, XXColor = "#FF147310", NDColor = "#FF147310", ADColor = "#FF147310" });
            Grades.Add(new ProductGrade() { Grade = 5, ADValue = 25, NDValue = 55, XXValue = 4, XXColor = "#FFB914A7", NDColor = "#FFB914A7", ADColor = "#FFB914A7" });
            Grades.Add(new ProductGrade() { Grade = 6, ADValue = 30, NDValue = 75, XXValue = 5, XXColor = "#FF06A9A1", NDColor = "#FF06A9A1", ADColor = "#FF06A9A1" });

            return Grades;
        }

        [XmlIgnore()]
        public List<ProductGrade> Grades
        {
            get
            {
                if (GradesDictionary.Count==0)
                {
                    GradesDictionary.Add("Default", new ResistanceConfig() { Grades = ProductModelConfig.AddDefaultGradeItem() });
                }
                return GradesDictionary["Default"].Grades;
            }
        }
        [XmlIgnore()]
        public ResistanceConfig ResistanceConfig
        {
            get
            {
                if (!GradesDictionary.ContainsKey("Default"))
                {
                    GradesDictionary.Add("Default", new ResistanceConfig() { Grades = ProductModelConfig.AddDefaultGradeItem() });
                }
                return GradesDictionary["Default"];
            }
        }

        public ProductGrade FetchGrade(SensorItemValueEnum e, double value)
        {
            if (Grades == null) return null;
            ProductGrade pre = new ProductGrade() { XXValue = double.MinValue, NDValue = double.MinValue, ADValue = double.MinValue };
            foreach (ProductGrade item in Grades)
            {
                if (e == SensorItemValueEnum.ValueDY)
                {
                    if (value > pre.ADValue && value <= item.ADValue)
                    {
                        return item;
                    }
                } 
                if (e == SensorItemValueEnum.ValueND)
                {
                    if (value > pre.NDValue && value <= item.NDValue)
                    {
                        return item;
                    }
                }
                pre = item;
            }
            return null;
        }

    }

    public class ResistanceConfig : INotifyPropertyChanged
    {
        public List<ProductGrade> Grades
        {
            get; set;
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
    public class ProductGrade : INotifyPropertyChanged
    {
        public ProductGrade()
        {

        }
        public int Grade { get; set; }
        public double ADValue { get; set; }
        public double NDValue { get; set; }
        public double XXValue { get; set; }

        public string ADColor { get; set; }
        public string NDColor { get; set; }
        public string XXColor { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;
        public void RefreshPage()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(""));
            }
        }

    }


    /// <summary>
    /// 标题：支持 XML 序列化的 Dictionary
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    [XmlRoot("SerializableDictionary")]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {

        public SerializableDictionary()
            : base()
        {
        }
        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        public SerializableDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public SerializableDictionary(int capacity)
            : base(capacity)
        {
        }
        public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }
        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        /// <summary>
        /// 从对象的 XML 表示形式生成该对象
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        /**/
        /// <summary>
        /// 将对象转换为其 XML 表示形式
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }

    }

    public enum SensorItemValueEnum
    {
        /// <summary>
        ///  端电压显示
        /// </summary>
        ValueDY = 1,
        /// <summary>
        /// 浓度值显示
        /// </summary>
        ValueND = 2
    }
}
