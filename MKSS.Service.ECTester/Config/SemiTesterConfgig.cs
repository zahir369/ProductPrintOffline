using MKSS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MKSS.Service.ECTester
{
    public class ECTesterConfgig
    {

        static string path = AppDomain.CurrentDomain.BaseDirectory + "\\ECTesterConfgig.xml";
        static ECTesterConfgig _instance = null;
        public static ECTesterConfgig Instance {
            get {
                if (_instance == null) {
                    if (File.Exists(path)) {
                        try
                        {
                            _instance = (ECTesterConfgig)deserialize_from_xml(path, typeof(ECTesterConfgig));
                            if (_instance.RefreshInterval <= 0) _instance.RefreshInterval = 800;
                            if (_instance.Product == null || _instance.Product[0].Models == null || _instance.Product[0].Models[0] == null) {
                                _instance = null;
                            }
                            if (string.IsNullOrEmpty(_instance.PauseColor)) _instance.PauseColor = "#FFC3C300";
                            if (string.IsNullOrEmpty(_instance.PauseExcepColor)) _instance.PauseExcepColor = "#FF02719B";
                            if (_instance.PauseValueExtend == null) _instance.PauseValueExtend = new double[] { -0.03, 0.03 };


                        }
                        catch (Exception)
                        {
                            _instance = null;
                        }
                    }
                    if (_instance == null)
                    {
                        _instance = new ECTesterConfgig()
                        {
                            SensorGrougAddress = "1-4", 
                            SerialPorts = "COM3", 
                            Product = new List<ProductConfig>()
                        };
                        string[] ps = new string[] { "H₂S", "CO", "NH₃", "O₃", "SO₂", "Cl₂", "HCl", "ETO", "NO₂", "O₂" };

                        foreach (var item in ps)
                        {
                            _instance.Product.Add(
                                new ProductConfig()
                                {
                                    Name = item,
                                    Code = item,
                                    TimeTotal = 30,
                                    GradingTimePoint = 29 ,ZeroPoint =3,SpanPoint=17,
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
        public static int ContainerPPM { get; set; } = 100;
        public static bool AutoPrintPPMSensibility { get; set; } = false;

        /// <summary>
        ///  大标签还是小标签
        /// </summary>
        public bool IndustryModeLabelSmall { get; set; }
        public double IndustryModeLabelXmm { get; set; } = 23.3;
        public double IndustryModeLabelYmm { get; set; } = 2.6;
        public int PrintLabelCount { get; set; } = 64;

        public string IndustryModeLabelLargeContent { get; set; } = "Ethylene Oxide\nMIX8418-ETO-100\n{灵敏度}\nM1H12S00H24";
        public string ShowChart { get; set; } = "";
        public string SerialPorts { get; set; } 
        public string ProductList { get; set; }
        public string ModelList { get; set; }
        public string SensorGrougAddress { get; set; }
        public List<ProductConfig> Product { get; set; } 
        public int RefreshInterval { get;   set; }


        /// <summary>
        ///  暂停筛选色
        /// </summary>
        public string PauseColor { get; set; } = "#FFC3C300";
        public string PauseExcepColor { get; set; } = "#FFFF0000";
        public bool PauseEnabled { get; set; } = true;
        /// <summary>
        ///  暂停筛选色范围
        /// </summary>
        public double[] PauseValueExtend { get; set; } = new double[] { -0.03, 0.03 };

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
