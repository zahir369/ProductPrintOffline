using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MKSS.APP.ConfigTool
{



    [XmlInclude(typeof(AddrConfigItem))]
    [XmlInclude(typeof(AddrValue))]
    [XmlInclude(typeof(AddrConfig))]
    [Serializable]
    public class MainConfig
	{

        static string path = AppDomain.CurrentDomain.BaseDirectory + "\\ConfigTool.xml";
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
                            SoftName = "四合一传感器配置工具",
                            DefaultCOM = "",
                            DefaultAddress = 1,
                            DefaultBaudRate = 9600,
                            BaudRateList = new int[] { 2400, 4800, 9600 },
                            AutoRefresh = true,
                            AutoRefreshInterval = 1000,
                            Addrs = new List<AddrValue>(),
                        };

                        _instance.Addrs.Add(new AddrValue()
                        {
                            Xh = 100,
                            Name = "设备型号",
                            Type = "设备型号",
                            Address = 0x1007,
                            ReadOnly = true,
                            DefaultValue = 0,
                            DataUnit = "",
                            Range = "1 ~5",
                            Fomula = "",
                            DropdownList = new AddrConfigItem[] {
                                new AddrConfigItem() { Name= "全气体", Value=0 },
                                new AddrConfigItem() { Name= "温湿度", Value=1 },
                                new AddrConfigItem() { Name= "CO2二氧化碳", Value=2 },
                                new AddrConfigItem() { Name= "PM2.5", Value=3 },
                                new AddrConfigItem() { Name= "NH3氨气", Value=4 },
                                new AddrConfigItem() { Name= "H2S硫化氢", Value=5 }, },
                            Memo = ""
                        });

                        _instance.Addrs.Add(new AddrValue() { Xh = 200, Name = "温度", Type = "实时数据", Address = 0x01, ReadOnly = true, DefaultValue = 0, DataUnit = "°C", Range = "0 ~ 400", Fomula = "x*0.1", Memo = "如读出245，表示24.5摄氏度" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 201, Name = "湿度", Type = "实时数据", Address = 0x02, ReadOnly = true, DefaultValue = 0, DataUnit = "%RH", Range = "0 ~ 1000", Fomula = "x*0.1", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 202, Name = "CO2", Type = "实时数据", Address = 0x03, ReadOnly = true, DefaultValue = 0, DataUnit = "ppm", Range = "0 ~ 5000", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 203, Name = "PM2.5", Type = "实时数据", Address = 0x04, ReadOnly = true, DefaultValue = 0, DataUnit = "ppm", Range = "0 ~ ", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 204, Name = "电化学传感器通道1", Type = "实时数据", Address = 0x05, ReadOnly = true, DefaultValue = 0, DataUnit = "", Range = "", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 205, Name = "电化学传感器通道2", Type = "实时数据", Address = 0x06, ReadOnly = true, DefaultValue = 0, DataUnit = "", Range = "", Fomula = "", Memo = "" });

                        _instance.Addrs.Add(new AddrValue() { Xh = 301, Name = "设备地址", Type = "设备参数", Address = 0x100, ReadOnly = false, DefaultValue = 1, DataUnit = "", Range = "1 ~ 255", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue()
                        {
                            Xh = 302,
                            Name = "波特率",
                            Type = "设备参数",
                            Address = 0x101,
                            ReadOnly = false,
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

                        _instance.Addrs.Add(new AddrValue()
                        {
                            Xh = 310,
                            Name = "设备型号",
                            Type = "设备参数",
                            Address = 0x1007,
                            ReadOnly = false,
                            DefaultValue = 1,
                            DataUnit = "",
                            Range = "",
                            Fomula = "",
                            DropdownList = new AddrConfigItem[] {
                                new AddrConfigItem() { Name= "全气体", Value=0 },
                                new AddrConfigItem() { Name= "温湿度", Value=1 },
                                new AddrConfigItem() { Name= "CO2二氧化碳", Value=2 },
                                new AddrConfigItem() { Name= "PM2.5", Value=3 },
                                new AddrConfigItem() { Name= "NH3氨气", Value=4 },
                                new AddrConfigItem() { Name= "H2S硫化氢", Value=5 }, },
                            Memo = "该值对应0x00寄存器里的值"
                        });

                        _instance.Addrs.Add(new AddrValue() { Xh = 303, Name = "实时AD值", Type = "设备参数", Address = 0x1000, ReadOnly = false, DefaultValue = 0, DataUnit = "", Range = "", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 304, Name = "标定0点AD值", Type = "设备参数", Address = 0x1001, ReadOnly = false, DefaultValue = 372, DataUnit = "", Range = "", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 30, Name = "标定SPAN点AD值", Type = "设备参数", Address = 0x1002, ReadOnly = false, DefaultValue = 2000, DataUnit = "", Range = "", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 306, Name = "0点标定浓度值", Type = "设备参数", Address = 0x1003, ReadOnly = false, DefaultValue = 0, DataUnit = "", Range = "", Fomula = "", Memo = "写入0标定零点" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 307, Name = "SPAN点标定浓度值", Type = "设备参数", Address = 0x1004, ReadOnly = false, DefaultValue = 0, DataUnit = "", Range = "", Fomula = "", Memo = "写入SPAN浓度值标定SPAN点" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 308, Name = "量程", Type = "设备参数", Address = 0x1005, ReadOnly = false, DefaultValue = 500, DataUnit = "", Range = "", Fomula = "", Memo = "" });
                        _instance.Addrs.Add(new AddrValue() { Xh = 309, Name = "屏蔽值", Type = "设备参数", Address = 0x1006, ReadOnly = false, DefaultValue = 10, DataUnit = "", Range = "", Fomula = "", Memo = "" });


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

        public int DefaultAddress { get; set; }
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
