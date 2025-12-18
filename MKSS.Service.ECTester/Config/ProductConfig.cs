using MKSS.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MKSS.Service.ECTester
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
        public int ContainerPPM { get; set; } = 100;
        public bool AutoPrintPPMSensibility { get; set; } = false;

        public bool ShowDelta { get; set; } = false;
        public bool LockX { get; set; } = false;
        public bool LockY { get; set; } = false;
        public int GradingTimePoint { get; set; }
        public string TestTimePoints { get; set; }
        public int TimeInterval { get; set; }
        public SensorItemEnum ColorDiagramFull { get; set; } = SensorItemEnum.SrcData; 
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
            //水性传感器
            //不良0 - 0.10
            //一档0.10 - 0.20
            //二档0.20 - 0.35
            //三档0.35 - 0.50
            //值高0.50以上
            List<ProductGrade> Grades = new List<ProductGrade>();
            Grades.Add(new ProductGrade() { Grade = 1,   NongDuValue = 1, ADValue = 125, DianYaValue = 0.1,  YuLiuColor = "#FF490808",  ADColor = "#FF990627",  DianYaColor = "#FF990627" });
            Grades.Add(new ProductGrade() { Grade = 2, NongDuValue = 6,  ADValue = 186, DianYaValue = 0.15,  YuLiuColor = "#FF1A1091", ADColor = "#FFC3C300", DianYaColor = "#FFC3C300" });
            Grades.Add(new ProductGrade() { Grade = 3,  NongDuValue = 10, ADValue = 248, DianYaValue = 0.2,   YuLiuColor = "#FFC1A615", ADColor = "#FFC3C300", DianYaColor = "#FFC3C300" });
            Grades.Add(new ProductGrade() { Grade = 4,  NongDuValue = 15, ADValue = 434, DianYaValue = 0.35,   YuLiuColor = "#FF147310", ADColor = "#FF089B02", DianYaColor = "#FF089B02" });
            Grades.Add(new ProductGrade() { Grade = 5,  NongDuValue = 20, ADValue = 621, DianYaValue = 0.5,  YuLiuColor = "#FFD70D3C", ADColor = "#FF02719B", DianYaColor = "#FF02719B" });
            Grades.Add(new ProductGrade() { Grade = 6,  NongDuValue = 5, ADValue = 4096, DianYaValue = 3.3,   YuLiuColor = "#FF06A9A1", ADColor = "#FF454F6A", DianYaColor = "#FF454F6A" });
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
            ProductGrade pre = new ProductGrade() {   NongDuValue = double.MinValue,  DianYaValue= double.MinValue, ADValue = double.MinValue, };
            foreach (ProductGrade item in Grades)
            {
                
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
                        case ShowModeEnum.AD:
                            if (value > pre.ADValue && value <= item.ADValue)
                            {
                                return item;
                            }
                            break;
                        case ShowModeEnum.YuLiu:
                            if (value > pre.NongDuValue && value <= item.NongDuValue)
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
        public double YMinAD { get; set; } = 0;
        public double YMaxAD { get; set; } = 1200;
        public double YMinDianYa { get; set; } = -0.1;
        public double YMaxDianYa { get; set; } = 0.6;
        public double YMinYuLiu { get; set; } = 0;
        public double YMaxYuLiu { get; set; } = 0.6;

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

        public string ADColor { get; set; }
        public string DianYaColor { get; set; }
        public string YuLiuColor { get; set; } 

        public string Color() {
            switch (SensorGroupData.ShowMode)
            {
                case ShowModeEnum.DuanDianYa:
                    return DianYaColor; 
                case ShowModeEnum.YuLiu:
                    return YuLiuColor;
                case ShowModeEnum.AD:
                    return ADColor;
                default:
                    break;
            }
            return ADColor;
        }

        public void ColorSet(string color)
        {
            switch (SensorGroupData.ShowMode)
            {
                case ShowModeEnum.DuanDianYa:
                    DianYaColor = color;
                    break;
                case ShowModeEnum.YuLiu:
                    YuLiuColor = color;
                    break; 
                case ShowModeEnum.AD:
                    ADColor = color;
                    break; 
                default:
                    break;
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
        /// 颜色图示
        /// </summary>
        ColorDiagram = 99
    }

}
