using MKSS.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MKSS.Service.O2Tester
{
    public class ProductConfig : INotifyPropertyChanged
    {

        public ProductConfig()
        {
            TimeInterval = 200;
            Name = "自定义";
            TestTimePoints = AutoTestTimePoints(TimeTotal, ZeroPoint, SpanPoint);
        }

        public static string AutoTestTimePoints(int total, int zero, int span)
        {
            int[] ps = new int[] { 16, 25, 40, 55, 70, 85 };
            List<int> retList = new List<int>();
            retList.Add(zero);
            retList.Add(span);
            foreach (var p in ps)
            {
                int p1 = ((int)((double)total * (double)p / (double)100));
                if (!retList.Contains(p1)) retList.Add(p1);
            }
            retList = retList.OrderBy(w => w).ToList();
            string ret = "";
            foreach (var p1 in retList)
            {
                ret = ret + p1 + ",";
            }
            return ret.TrimEnd(',');
        }


        public string Code { get; set; }
        public string Name { get; set; }
        public int TimeTotal { get; set; } = 21;
        public int ZeroPoint { get; set; } = 3;
        public int SpanPoint { get; set; } = 17;


        public bool ShowDelta { get; set; } = false;
        public bool LockX { get; set; } = false;
        public bool LockY { get; set; } = false;
        public int GradingTimePoint { get; set; }
        public string TestTimePoints { get; set; }
        public int TimeInterval { get; set; }
        public SensorItemEnum ColorDiagramTop { get; set; } = SensorItemEnum.SrcData;
        public SensorItemEnum ColorDiagramLeft { get; set; } = SensorItemEnum.T90;
        public SensorItemEnum ColorDiagramRight { get; set; } = SensorItemEnum.T10;
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
            List<ProductGrade> Grades = AddDefaultGradeItem();
            GradesConfig = new ShowModeConfig() { Grades = Grades };
        }

        public string Name { get; set; } = "默认";
        public ShowModeConfig GradesConfig { get; set; }

        public static List<ProductModelConfig> AddDefaultModelList()
        {
            List<ProductModelConfig> ret = new List<ProductModelConfig>();
            ret.Add(AddDefaultModel());
            return ret;
        }

        public static ProductModelConfig AddDefaultModel()
        {
            List<ProductGrade> Grades = AddDefaultGradeItem();
            ProductModelConfig r = new ProductModelConfig()
            {
                GradesConfig = new ShowModeConfig() { Grades = Grades }
            };
            return r;
        }
        
        public static List<ProductGrade> AddDefaultGradeItem()
        {
            List<ProductGrade> Grades = new List<ProductGrade>();
            Grades.Add(new ProductGrade() { Grade = 1, T90Value = 3, T10Value = 3, NongDuValue = 1,   ADValue = 1500,   DianYaValue = 6, T10Color = "#FF490808", T90Color = "#FF490808", NongDuColor = "#FF490808",  ADColor = "#FF490808",  DianYaColor = "#FF490808" });
            Grades.Add(new ProductGrade() { Grade = 2, T90Value = 8, T10Value = 8, NongDuValue = 6,  ADValue = 1800, DianYaValue = 8, T10Color = "#FF1A1091", T90Color = "#FF1A1091", NongDuColor = "#FF1A1091", ADColor = "#FF1A1091", DianYaColor = "#FF1A1091" });
            Grades.Add(new ProductGrade() { Grade = 3, T90Value = 12, T10Value = 12, NongDuValue = 10, ADValue = 2100, DianYaValue = 10, T10Color = "#FFC1A615", T90Color = "#FFC1A615", NongDuColor = "#FFC1A615", ADColor = "#FFC1A615", DianYaColor = "#FFC1A615" });
            Grades.Add(new ProductGrade() { Grade = 4, T90Value = 15, T10Value = 15, NongDuValue = 15, ADValue = 2500, DianYaValue = 11, T10Color = "#FF147310", T90Color = "#FF147310", NongDuColor = "#FF147310", ADColor = "#FF147310", DianYaColor = "#FF147310" });
            Grades.Add(new ProductGrade() { Grade = 5, T90Value = 20, T10Value = 20, NongDuValue = 20, ADValue = 3000, DianYaValue = 13, T10Color = "#FFD70D3C", T90Color = "#FFD70D3C", NongDuColor = "#FFD70D3C", ADColor = "#FFD70D3C", DianYaColor = "#FFD70D3C" });
            Grades.Add(new ProductGrade() { Grade = 6, T90Value = 50, T10Value = 50, NongDuValue = 5, ADValue = 4096, DianYaValue = 20, T10Color = "#FF06A9A1", T90Color = "#FF06A9A1", NongDuColor = "#FF06A9A1", ADColor = "#FF06A9A1", DianYaColor = "#FF06A9A1" });
            return Grades;
        }

        [XmlIgnore()]
        public List<ProductGrade> Grades
        {
            get
            { 
                return GradesConfig.Grades;
            }
        }
        [XmlIgnore()]
        public ShowModeConfig ShowModeConfig
        {
            get
            { 
                return GradesConfig;
            }
        }

        public ProductGrade FetchGrade(SensorItemEnum e, double value)
        {
            if (Grades == null) return null;
            ProductGrade pre = new ProductGrade() { T10Value = double.MinValue, NongDuValue = double.MinValue, T90Value = double.MinValue,DianYaValue= double.MinValue, ADValue = double.MinValue, };
            foreach (ProductGrade item in Grades)
            {
                if (e == SensorItemEnum.T90)
                {
                    if (value > pre.T90Value && value <= item.T90Value)
                    {
                        return item;
                    }
                }
                if (e == SensorItemEnum.T10)
                {
                    if (value > pre.T10Value && value <= item.T10Value)
                    {
                        return item;
                    }
                }
                if (e == SensorItemEnum.SrcData)
                {
                    switch (SensorGroupData.ShowMode)
                    {
                        case ShowModeEnum.DuanDianYa:
                            if (value > pre.DianYaValue && value <= item.DianYaValue)
                            {
                                return item;
                            }
                            break;
                        case ShowModeEnum.NongDu:
                            if (value > pre.NongDuValue && value <= item.NongDuValue)
                            {
                                return item;
                            }
                            break;
                        case ShowModeEnum.AD:
                            if (value > pre.ADValue && value <= item.ADValue)
                            {
                                return item;
                            }
                            break;
                        default:
                            break;
                    } 
                }
                pre = item;
            }
            return null;
        }

    }

    public class ShowModeConfig : INotifyPropertyChanged
    {
        public List<ProductGrade> Grades
        {
            get; set;
        }
        public double XMin { get; set; } = -2;
        public double XMax { get; set; } = 25;
        public double YMinAD { get; set; } = 1500;
        public double YMaxAD { get; set; } = 3400;
        public double YMinDianYa { get; set; } = 2;
        public double YMaxDianYa { get; set; } = 16.8;
        public double YMinNongDu { get; set; } = 9;
        public double YMaxNongDu { get; set; } = 23;

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
        public double DianYaValue { get; set; }
        public double NongDuValue { get; set; }
        public double T10Value { get; set; }
        public double T90Value { get; set; }

        public string ADColor { get; set; }
        public string DianYaColor { get; set; }
        public string NongDuColor { get; set; }
        public string T10Color { get; set; }
        public string T90Color { get; set; }


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

    public enum SensorItemEnum
    {
        /// <summary>
        /// 反应值
        /// </summary>
        SrcData = 1,
        /// <summary>
        ///  初始值
        /// </summary>
        T90 = 2,
        /// <summary>
        /// 变化值
        /// </summary>
        T10 = 3,
        /// <summary>
        /// 颜色图示
        /// </summary>
        ColorDiagram = 99
    }
}
