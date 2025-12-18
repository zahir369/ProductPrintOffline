using MKSS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MKSS.Service.SemiTester
{
    public class SemiTesterConfgig
    {
        static string path = AppDomain.CurrentDomain.BaseDirectory + "\\SemiTesterConfgig.xml";
        static SemiTesterConfgig _instance = null;
        public static SemiTesterConfgig Instance {
            get {
                if (_instance == null) {
                    if (File.Exists(path)) {
                        try
                        {
                            _instance = (SemiTesterConfgig)deserialize_from_xml(path, typeof(SemiTesterConfgig));
                            if (_instance.RefreshInterval <= 0) _instance.RefreshInterval = 800;
                            if (_instance.Product == null || _instance.Product[0].Models == null || _instance.Product[0].Models[0] == null) {
                                _instance = null;
                            }
                            foreach (ProductConfig p in _instance.Product)
                            {
                                foreach (ProductModelConfig m in p.Models)
                                {
                                    foreach (ResistanceEnum item in Enum.GetValues(typeof(ResistanceEnum)))
                                    {
                                        List<ProductGrade> Grades = ProductModelConfig.AddDefaultGradeItem();
                                        if (!m.GradesDictionary.ContainsKey(item)) {
                                            m.GradesDictionary.Add(item, new ResistanceConfig() { Grades = Grades });
                                        }
                                    }
                                }
                            }
                            
                        }
                        catch (Exception)
                        {
                            _instance = null;
                        }
                    }
                    if (_instance == null)
                    {
                        _instance = new SemiTesterConfgig()
                        {
                            SensorGrougAddress = "1-2", 
                            SerialPorts = "COM3", 
                            Product = new List<ProductConfig>()
                        };
                        string[] ps = new string[] {
                            "MIX1001", "MIX1002", "MIX1002B", "MIX1003", "MIX1004", "MIX1004B",
                            "MIX1004N", "MIX1005", "MIX1005B", "MIX1006", "MIX1006B", "MIX1007B",
                            "MIX1012","MIX1014","MIX1013","MIX1014P","MIX1015","MIX1037",
                            "MIX2001", "MIX2002", "MIX2004",  "MIX2004K", "MIX2005", "MIX2007", "MIX2011", "MIX2011A",
                           "MIX2017", "MIX2021", "MIX2018","MIX3003", "MIX5007", "MIX5005", "MIX7001" };

                        foreach (var item in ps)
                        {
                            _instance.Product.Add(
                                new ProductConfig()
                                {
                                    Name = item,
                                    Code = item,
                                    TimeTotal = 30,
                                    GradingTimePoint = 25 ,ZeroPoint =3,SpanPoint=25,
                                    TestTimePoints = ProductConfig.AutoTestTimePoints(20,3,25),
                                    Models = ProductModelConfig.AddDefaultModelList(), 
                                }
                            );
                        }
                         
                        serialize_to_xml(path, _instance);
                    }
                }
                return _instance;
            }
        }
        public string SerialPorts { get; set; }  
        public string ProductList { get; set; }
        public string ModelList { get; set; }
        public string SensorGrougAddress { get; set; }
        public List<ProductConfig> Product { get; set; }
        public string ShowChart { get; set; } = "";
        public int RefreshInterval { get;   set; } 

        public static void Save()
        {
            serialize_to_xml(path, _instance);
        }

        /// <summary>
        ///  serialize object to xml file.
        /// </summary>
        /// <param name="path">the path to save the xml file</param>
        /// <param name="obj">the object you want to serialize</param>
        public static void serialize_to_xml(string path, object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            string content = string.Empty;
            //serialize
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                content = writer.ToString();
            }
            //save to file
            using (StreamWriter stream_writer = new StreamWriter(path))
            {
                stream_writer.Write(content);
            }
        }

        /// <summary>
        /// deserialize xml file to object
        /// </summary>
        /// <param name="path">the path of the xml file</param>
        /// <param name="object_type">the object type you want to deserialize</param>
        public static object deserialize_from_xml(string path, Type object_type)
        {
            XmlSerializer serializer = new XmlSerializer(object_type);
            using (StreamReader reader = new StreamReader(path))
            {
                return serializer.Deserialize(reader);
            }
        }

    }

}
