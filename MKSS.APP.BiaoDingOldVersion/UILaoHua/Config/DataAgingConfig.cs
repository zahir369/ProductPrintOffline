using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DeviceDataMonitorWPF.UIBiaodingJiuJing.Config
{
    public class DataAgingConfig
    {
        static string path = AppDomain.CurrentDomain.BaseDirectory + "\\DataAgingConfig.xml";
        static DataAgingConfig _instance = null;
        public static DataAgingConfig Instance {
            get {
                if (_instance == null) {
                    if (File.Exists(path)) {
                        try
                        {
                            _instance = (DataAgingConfig)deserialize_from_xml(path, typeof(DataAgingConfig));
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (_instance == null)
                    {
                        _instance = new DataAgingConfig()
                        {
                            SensorGrougAddress = "1-16",
                            TCPIP = "192.168.100.99:5000",
                            SerialPorts = "COM3", 
                            Product = new List<DataAgingProductConfig>()
                        };
                        _instance.Product.Add(new DataAgingProductConfig());
                        _instance.Product.Add(new DataAgingProductConfig() { Name = "MIX1004", VoltageValueBase = 20000 });
                        _instance.Product.Add(new DataAgingProductConfig() { Name = "MIX1004B", VoltageValueBase = 20001 });
                        _instance.Product.Add(new DataAgingProductConfig() { Name = "MIX2004",  VoltageValueBase = 20002 });
                        _instance.Product.Add(new DataAgingProductConfig() { Name = "MIX8030",  VoltageValueBase = 20003 });
                        _instance.Product.Add(new DataAgingProductConfig() { Name = "MIX8011",  VoltageValueBase = 20004 });
                        _instance.Product.Add(new DataAgingProductConfig() { Name = "MIX8016",  VoltageValueBase = 20005 });
                        _instance.Product.Add(new DataAgingProductConfig() { Name = "MIX8410-O₂", VoltageValueBase = 20006 });
                        _instance.Product.Add(new DataAgingProductConfig() { Name = "MIX8412-O₃",  VoltageValueBase = 20007 });
                        _instance.Product.Add(new DataAgingProductConfig() { Name = "MIX8416-H2S", VoltageValueBase = 20008 });
                        _instance.Product.Add(new DataAgingProductConfig() { Name = "MIX2811",   VoltageValueBase = 20009 });
                        _instance.Product.Add(new DataAgingProductConfig() { Name = "MIX2801",   VoltageValueBase = 20010 });
                        _instance.Product.Add(new DataAgingProductConfig() { Name = "MIX2806",   VoltageValueBase = 20011 });
                        _instance.Product.Add(new DataAgingProductConfig() { Name = "MIX2112",  VoltageValueBase = 20012 });
                        serialize_to_xml(path, _instance);
                    }
                }
                return _instance;
            }
        }
        public string SerialPorts { get; set; }
        public string TCPIP { get; set; }
        public int PortsModeSelect { get; set; }
        public string ProductList { get; set; }
        
        public string SensorGrougAddress { get; set; }
        public List<DataAgingProductConfig> Product { get; set; }

        public static void Save()
        {
            serialize_to_xml(path, _instance);
        }

        /// <summary>
        /// serialize object to xml file.
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
