using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Xml.Serialization;

namespace MKSS.APP.ConfigToolMulti
{



    [XmlInclude(typeof(AddrConfigItem))]
    [XmlInclude(typeof(AddrValue))]
    [XmlInclude(typeof(AddrConfig))]
    [Serializable]
    public class MainConfig
	{

        static string path = AppDomain.CurrentDomain.BaseDirectory + "\\ConfigToolMulti.xml";
        static MainConfig _instance = null;
        public static MainConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (File.Exists(path))
                    {
                        try
                        {
                            _instance = (MainConfig)deserialize_from_xml(path, typeof(MainConfig));
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    if (_instance == null)
                    {

                        _instance = new MainConfig()
                        {
                            Parity = Parity.Odd,
                            SoftName = "氨气传感器配置工具V2.0",
                            DefaultCOM = "",
                            DefaultAddress = "5-13",
                            DefaultBaudRate = 9600,
                            BaudRateList = new int[] { 2400, 4800, 9600 },
                            AutoRefresh = true,
                            AutoRefreshInterval = 1000,
                            Addrs = new List<AddrValue>(),
                        };

                        _instance.Addrs.Add(new AddrValue()
                        {
                            Xh = 2,
                            Name = "波特率",
                            Type = "设备参数",
                            Address = 0x02,
                            ReadOnly = true,
                            DefaultValue = 2,
                            DataUnit = "",
                            Range = "0 ~2",
                            Fomula = "",
                            DropdownList = new AddrConfigItem[] {
                                new AddrConfigItem() { Name= "2400", Value=0 },
                                new AddrConfigItem() { Name= "4800", Value=1 },
                                new AddrConfigItem() { Name= "9600", Value=2 }
                            },
                            Memo = ""
                        });

                        _instance.Addrs.Add(new AddrValue() { Xh = 8, Name = "量程", Type = "量程", Address = 8, ReadOnly = true, DefaultValue = 0, DataUnit = "", Range = "", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 9, Name = "屏蔽值", Type = "屏蔽值", Address = 9, ReadOnly = true, DefaultValue = 0, DataUnit = "", Range = "", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 10, Name = "实时AD", Type = "实时AD", Address = 10, ReadOnly = true, DefaultValue = 0, DataUnit = "", Range = "", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 11, Name = "零点AD", Type = "零点AD", Address = 11, ReadOnly = true, DefaultValue = 0, DataUnit = "", Range = "", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 12, Name = "SPAN点AD", Type = "SPAN点AD", Address = 12, ReadOnly = true, DefaultValue = 0, DataUnit = "", Range = "", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 13, Name = "零点浓度值", Type = "零点浓度值", Address = 13, ReadOnly = false, DefaultValue = 0, DataUnit = "", Range = "", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 14, Name = "SPAN点浓度值", Type = "SPAN点浓度值", Address = 14, ReadOnly = false, DefaultValue = 0, DataUnit = "", Range = "", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 15, Name = "当前浓度", Type = "当前浓度", Address = 15, ReadOnly = true, DefaultValue = 0, DataUnit = "", Range = "", Fomula = "", Memo = "" });

                        serialize_to_xml(path, _instance);
                    }
                }
                return _instance;
            }
        }


        public string SoftName { get; set; }
        public string DefaultCOM { get; set; }
		public int DefaultBaudRate { get; set; }
        public int[] BaudRateList { get; set; }
        public Parity Parity { get; set; }

        public string DefaultAddress { get; set; }
		public bool AutoRefresh { get; set; }
        public bool ShowDebugComm { get; set; }
        public int AutoRefreshInterval { get; set; }
		public List<AddrValue> Addrs { get; set; }

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
