using MKSS.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace MKSS.Service.SemiTester
{
    public class ProductConfig : INotifyPropertyChanged
    {

        public ProductConfig()
        {
            TimeInterval = 200;
            Name = "自定义"; 
            TestTimePoints = AutoTestTimePoints(TimeTotal); 
        }

        public static string AutoTestTimePoints(int total)
        {
            string ret = "";
            int[] ps = new int[] {  16,   25, 40, 55, 70,  85 };
            foreach (var p in ps)
            {
                int p1 = ((int)((double)total * (double)p / (double)100));
                if (string.IsNullOrEmpty(ret) && p1<=1) {
                    p1 = 3;
                }
                ret = ret + p1 + ",";
            }
            return ret.TrimEnd(',');
        }


        public string Code { get; set; }
        public string Name { get; set; }
        public int TimeTotal { get; set; } = 21;

        public bool ShowDelta { get; set; } = false;
        public bool LockX { get; set; } = false;
        public bool LockY { get; set; } = false;
        public int GradingTimePoint { get; set; }
        public string TestTimePoints { get; set; }
        public int TimeInterval { get; set; }
        public SensorItemEnum ColorDiagramTop { get; set; } = SensorItemEnum.Start;
        public SensorItemEnum ColorDiagramLeft { get; set; } = SensorItemEnum.Final;
        public SensorItemEnum ColorDiagramRight { get; set; } = SensorItemEnum.Final;
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

    public class ProductModelConfig {

        public ProductModelConfig() {
            GradesDictionary = new SerializableDictionary<ResistanceEnum, ResistanceConfig>();
        }

        public string Name { get; set; } = "默认";
        public SerializableDictionary<ResistanceEnum, ResistanceConfig> GradesDictionary { get; set; }

        public static List<ProductModelConfig> AddDefaultModelList() {
            List<ProductModelConfig> ret = new List<ProductModelConfig>();
            ret.Add(AddDefaultModel());
            return ret;
        }

        public static ProductModelConfig AddDefaultModel() {
            ProductModelConfig r = new ProductModelConfig() { 
              GradesDictionary = AddDefaultGradeDic(),   
            };
            return r;
        }
        public static SerializableDictionary<ResistanceEnum, ResistanceConfig> AddDefaultGradeDic()
        {
            SerializableDictionary<ResistanceEnum, ResistanceConfig> dic = new SerializableDictionary<ResistanceEnum, ResistanceConfig>();
            foreach (ResistanceEnum item in Enum.GetValues(typeof(ResistanceEnum)))
            {
                List<ProductGrade> Grades = AddDefaultGradeItem();
                //http://www.divcss5.com/html/h636.shtml
                dic.Add(item, new ResistanceConfig() { Grades = Grades });
            }
            return dic;
        }
        static List<ProductGrade> AddDefaultGradeItem()
        {
            List<ProductGrade> Grades = new List<ProductGrade>();
            //http://www.divcss5.com/html/h636.shtml
            //Grades.Add(new ProductGrade() { Grade = 1, StartValue = 0.01, FinalValue = 0.01, DeltaValue = 0.01, DeltaColor = "#FF040E39", FinalColor = "#FF040E39", StartColor = "#FF040E39" });
            //Grades.Add(new ProductGrade() { Grade = 2, StartValue = 0.25, FinalValue = 0.25, DeltaValue = 0.25,   DeltaColor = "#FF0E0E67", FinalColor = "#FF0E0E67", StartColor = "#FF0E0E67" });
            //Grades.Add(new ProductGrade() { Grade = 3, StartValue = 0.98, FinalValue = 0.98, DeltaValue = 0.98, DeltaColor = "#FF22157B", FinalColor = "#FF22157B", StartColor = "#FF22157B" });
            //Grades.Add(new ProductGrade() { Grade = 4, StartValue = 1.6, FinalValue = 1.6, DeltaValue = 1.6,  DeltaColor = "#FF3525A5", FinalColor = "#FF3525A5", StartColor = "#FF3525A5" });
            //Grades.Add(new ProductGrade() { Grade = 5, StartValue = 3, FinalValue = 3, DeltaValue = 3,   DeltaColor = "#FF5039BD", FinalColor = "#FF5039BD", StartColor = "#FF5039BD" });
            //Grades.Add(new ProductGrade() { Grade = 6, StartValue = 5, FinalValue = 5, DeltaValue = 5,  DeltaColor = "#FF5757E5", FinalColor = "#FF5757E5", StartColor = "#FF5757E5" });

            Grades.Add(new ProductGrade() { Grade = 1, StartValue = 0.01, FinalValue = 1, DeltaValue = 0.09, DeltaColor = "#FF030303", FinalColor = "#FF030303", StartColor = "#FF030303" });
            Grades.Add(new ProductGrade() { Grade = 2, StartValue = 0.3, FinalValue = 2.3, DeltaValue = 2, DeltaColor = "#FF18E51B", FinalColor = "#FF18E51B", StartColor = "#FF18E51B" });
            Grades.Add(new ProductGrade() { Grade = 3, StartValue = 0.7, FinalValue = 3.1, DeltaValue = 2.4, DeltaColor = "#FFF7160B", FinalColor = "#FFF7160B", StartColor = "#FFF7160B" });
            Grades.Add(new ProductGrade() { Grade = 4, StartValue = 0.98, FinalValue = 4.1, DeltaValue = 3.12, DeltaColor = "#FFF3EF02", FinalColor = "#FFF3EF02", StartColor = "#FFF3EF02" });
            Grades.Add(new ProductGrade() { Grade = 5, StartValue = 1.6, FinalValue = 4.5, DeltaValue = 4, DeltaColor = "#FF08EFE4", FinalColor = "#FF08EFE4", StartColor = "#FF08EFE4" });
            Grades.Add(new ProductGrade() { Grade = 6, StartValue = 5, FinalValue = 5, DeltaValue = 5, DeltaColor = "#FF0A18E9", FinalColor = "#FF0A18E9", StartColor = "#FF0A18E9" });


            return Grades;
        }

        [XmlIgnore()]
        public List<ProductGrade> Grades
        {
            get
            {
                if (!GradesDictionary.ContainsKey(SensorGroupData.Resistance))
                {
                    GradesDictionary.Add(SensorGroupData.Resistance, new ResistanceConfig() { Grades = ProductModelConfig.AddDefaultGradeItem() });
                }
                return GradesDictionary[SensorGroupData.Resistance].Grades;
            }
        }
        [XmlIgnore()]
        public ResistanceConfig ResistanceConfig
        {
            get
            {
                if (!GradesDictionary.ContainsKey(SensorGroupData.Resistance))
                {
                    GradesDictionary.Add(SensorGroupData.Resistance, new ResistanceConfig() { Grades = ProductModelConfig.AddDefaultGradeItem() });
                }
                return GradesDictionary[SensorGroupData.Resistance];
            }
        }

        public ProductGrade FetchGrade(SensorItemEnum e, double value)
        {
            if (Grades == null) return null;
            ProductGrade pre = new ProductGrade() { DeltaValue = double.MinValue, FinalValue = double.MinValue, StartValue = double.MinValue };
            foreach (ProductGrade item in Grades)
            {
                if (e == SensorItemEnum.Start)
                {
                    if (value > pre.StartValue && value <= item.StartValue)
                    {
                        return item;
                    }
                }
                if (e == SensorItemEnum.Delta)
                {
                    if (value > pre.DeltaValue && value <= item.DeltaValue)
                    {
                        return item;
                    }
                }
                if (e == SensorItemEnum.Final)
                {
                    if (value > pre.FinalValue && value <= item.FinalValue)
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
            get;set;
        } 
        public double YMin { get; set; } = -0.2;
        public double YMax { get; set; } = 5.3;
        public double XMin { get; set; } = -2;
        public double XMax { get; set; } = 25;
        public double YMinDelta { get; set; } = -30;
        public double YMaxDelta { get; set; } = 1030;
        public double XMinDelta { get; set; } = -30;
        public double XMaxDelta { get; set; } = 830;

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
        public double FinalValue { get; set; }
        public double DeltaValue { get; set; }
        public double StartValue { get; set; }
         
        public string FinalColor { get; set; }
        public string DeltaColor { get; set; }
        public string StartColor { get; set; }


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
        ///  初始值
        /// </summary>
        Start,
        /// <summary>
        /// 反应值
        /// </summary>
        Final,
        /// <summary>
        /// 变化值
        /// </summary>
        Delta,
        /// <summary>
        /// 颜色图示
        /// </summary>
        ColorDiagram
    }
}
