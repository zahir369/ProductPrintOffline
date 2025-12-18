using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DeviceDataMonitorWPF.UIBiaoDing.Config
{
    public class BiaoDingConfgig
    {
        static string path = AppDomain.CurrentDomain.BaseDirectory + "\\BiaoDingConfgig.xml";
        static BiaoDingConfgig _instance = null;
        public static BiaoDingConfgig Instance {
            get {
                if (_instance == null) {
                    if (File.Exists(path)) {
                        try
                        {
                            _instance = (BiaoDingConfgig)deserialize_from_xml(path, typeof(BiaoDingConfgig));
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
                        _instance.Product.Add(new ProductConfig());
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2111A-家用燃气", Code = 61200020, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2111DS", Code = 61200271, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2111D-SMOKE", Code = 61200258, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2111D-数字输出-空气污染", Code = 61200019, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2111S-D", Code = 61200272, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2111S-D", Code = 61200318, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2111S-空气污染", Code = 61200274, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2111S-派斯", Code = 61200296, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2111-空气污染", Code = 61200016, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2112-空气污染", Code = 61200017, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2113-车载易燃易爆", Code = 61200021, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2217", Code = 61200207, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-03-200PPM", Code = 61200257, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-CL2-10PPM-工业", Code = 61200042, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-CO-1000PPM-工业", Code = 61200023, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-CO-500PPM-工业", Code = 61200024, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-ETO", Code = 61200348, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-H2S-100PPM-工业", Code = 61200027, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-H2S-10PPM-工业", Code = 61200025, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-H2S-20PPM-工业", Code = 61200026, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-NH3-1000PPM-工业", Code = 61200037, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-NH3-100PPM-工业", Code = 61200036, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-NH3-10PPM-工业", Code = 61200033, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-NH3-20PPM-工业", Code = 61200034, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-NH3-50PPM-工业", Code = 61200035, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-NH3-D-10PPM-智慧公厕", Code = 61200038, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-NH3-D-20PPM-智慧公厕", Code = 61200039, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-NH3-D-50PPM-智慧公厕", Code = 61200040, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-O2-25%-工业", Code = 61200030, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-O3-20PPM-工业", Code = 61200157, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-PH3-10PPM-工业", Code = 61200045, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2801-SO2-20PPM-工业", Code = 61200041, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2802-CO-20PPM-大气监测", Code = 61200047, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2802-NO2-2PPM-大气监测", Code = 61200049, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2802-O3-2PPM-大气监测", Code = 61200050, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2802-SO2-2PPM-大气监测", Code = 61200048, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2803-CO2", Code = 61200305, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2807-O2", Code = 61200246, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2808-CH3OH-SK", Code = 61200310, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2808-CO-SK", Code = 61200300, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2808-H2-SK", Code = 61200321, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2808-H2S-SK", Code = 61200250, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2808-NH3", Code = 61200240, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2808-NH3-SK", Code = 61200253, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2808-O3-SK", Code = 61200260, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2810A-CO-故障判断", Code = 61200053, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2810-CO", Code = 61200051, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2811A-CO", Code = 61200188, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2811-CO", Code = 61200052, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2812", Code = 61200306, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2815A-CO", Code = 61200190, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2815-CO", Code = 61200054, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2816-电气火灾", Code = 61200055, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2818A-CO", Code = 61200170, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2820BN", Code = 61200273, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2820B-甲醛-双插针", Code = 61200058, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2820C-端子", Code = 61200339, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2820N-甲醛", Code = 61200267, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2820-甲醛-端子", Code = 61200057, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2825-VOC", Code = 61200335, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2860-H2", Code = 61200301, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2870-O3", Code = 61200171, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX2871", Code = 61200346, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX9601A-NH3+H2S", Code = 61200297, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX9601-单气体-H2S", Code = 61200252, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX9601-单气体-NH3", Code = 61200251, ValueBase = 1001, VoltageValueBase = 20000 });
                        _instance.Product.Add(new ProductConfig() { Name = "模组-MIX9603-CO", Code = 61200189, ValueBase = 1001, VoltageValueBase = 20000 });

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
        public List<ProductConfig> Product { get; set; }

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
