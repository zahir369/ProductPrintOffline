using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MKSS.APP.UIBiaoDing.Config
{
    public class BiaoDingConfgig
    {
        public static int CONN_TYPE_NETWORK = 0;
        public static int CONN_TYPE_SERIALPORT = 1;
        public static int CONN_TYPE_BOARDCASE = 2;
        static string path = AppDomain.CurrentDomain.BaseDirectory + "\\BiaoDingConfgig.xml";
        static BiaoDingConfgig _instance = null;
        public static BiaoDingConfgig Instance {
            get {
                if (_instance == null) {
                    if (File.Exists(path)) {
                        try
                        {
                            _instance = (BiaoDingConfgig)deserialize_from_xml(path, typeof(BiaoDingConfgig));
                            if (_instance.RefreshInterval <= 0) _instance.RefreshInterval = 200;
                            if (_instance.ReadSerialNoWait1 <= 0) _instance.ReadSerialNoWait1 = 2000;
                            if (_instance.ReadSerialNoWait2 <= 0) _instance.ReadSerialNoWait2 = 2000;
                            if (_instance.PortsModeSelect== CONN_TYPE_BOARDCASE) _instance.PortsModeSelect = CONN_TYPE_SERIALPORT;//避免初始状态是老化柜，老化柜需要联网，不联网报错
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (_instance == null)
                    {
                        _instance = new BiaoDingConfgig()
                        {
                            SensorGrougAddress = "1-16",
                            TCPIP = "192.168.100.99:5000",
                            SerialPorts = "COM3", 
                            Product = new List<ProductConfig>()
                        };

                        _instance.Product.Add(new ProductConfig() { Name = "MK413", Code = 10413, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "MK412", Code = 10412, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "MK411", Code = 10411, ValueBase = 1001, VoltageValueBase = 20000 });
                         
                        serialize_to_xml(path, _instance);
                    }
                }
                return _instance;
            }
        }
        public string TxtUser { get; set; } = "*****";
        public string TxtUserAdmin { get; set; } = "*****";
        public string SerialPorts { get; set; }
        public string TCPIP { get; set; }
        public int PortsModeSelect { get; set; }
        public string ProductList { get; set; }
        public int ReadSerialNoWait1 { get; set; } = 3000;
        public int ReadSerialNoWait2 { get; set; } = 5000;

        public string SensorGrougAddress { get; set; }
        public List<ProductConfig> Product { get; set; }
        public bool StaFilterEmpSata { get;   set; }
        public int RefreshInterval { get;   set; }
        public bool StaZeroSpanTitleExchange { get;   set; }
        public double YMin { get;   set; } = -100;
        public double YMax { get; set; } = 10000;
        public double XMax { get;   set; } = 620;
        public double XMin { get; set; } = -20;

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
