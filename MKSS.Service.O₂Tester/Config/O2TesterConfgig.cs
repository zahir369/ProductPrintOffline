using MKSS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MKSS.Service.O2Tester
{
    public class O2TesterConfgig
    {
        static string path = AppDomain.CurrentDomain.BaseDirectory + "\\O2TesterConfgig.xml";
        static O2TesterConfgig _instance = null;
        public static O2TesterConfgig Instance {
            get {
                if (_instance == null) {
                    if (File.Exists(path)) {
                        try
                        {
                            _instance = (O2TesterConfgig)deserialize_from_xml(path, typeof(O2TesterConfgig));
                            if (_instance.RefreshInterval <= 0) _instance.RefreshInterval = 800;
                            if (_instance.Product == null || _instance.Product[0].Models == null || _instance.Product[0].Models[0] == null) {
                                _instance = null;
                            }
                             
                        }
                        catch (Exception)
                        {
                            _instance = null;
                        }
                    }
                    if (_instance == null)
                    {
                        _instance = new O2TesterConfgig()
                        {
                            SensorGrougAddress = "1-4", 
                            SerialPorts = "COM3", 
                            Product = new List<ProductConfig>()
                        };
                        string[] ps = new string[] { "O₂" };

                        foreach (var item in ps)
                        {
                            _instance.Product.Add(
                                new ProductConfig()
                                {
                                    Name = item,
                                    Code = item,
                                    TimeTotal = 120,
                                    GradingTimePoint = 35 ,ZeroPoint =3,SpanPoint=17,
                                    TestTimePoints = ProductConfig.AutoTestTimePoints(20,3,17),
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
        public string ShowChart { get; set; } = "";
        public string SerialPorts { get; set; }  
        public string ProductList { get; set; }
        public string ModelList { get; set; }
        public string SensorGrougAddress { get; set; }
        public List<ProductConfig> Product { get; set; } 
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
