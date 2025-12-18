using MKSS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace MKSS.Service.ElectroChemical
{
    public class ElectroChemicalConfgig
    {
        static string path
        {
            get {
                if (SensorGroupData.ShowV) {
                    return AppDomain.CurrentDomain.BaseDirectory + "\\ElectroChemicalConfgigShowV.xml";
                }
                return AppDomain.CurrentDomain.BaseDirectory + "\\ElectroChemicalConfgig.xml";
            }
        }  
        static ElectroChemicalConfgig _instance = null;
        public static ElectroChemicalConfgig Instance {
            get {
                if (_instance == null) {
                    if (File.Exists(path)) {
                        try
                        {
                            _instance = (ElectroChemicalConfgig)deserialize_from_xml(path, typeof(ElectroChemicalConfgig));
                            if (_instance.RefreshInterval <= 0) _instance.RefreshInterval = 200;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    if (_instance == null)
                    {
                        _instance = new ElectroChemicalConfgig()
                        {
                            SensorGrougAddress = "1-4", 
                            SerialPorts = "COM3", 
                            Product = new List<ProductConfig>()
                        }; 
                        _instance.Product.Add(new ProductConfig() { Name = "XYZ1", Code = "61200021",  TimeTotal = 300, GradingTimePoint = 50 });
                        _instance.Product.Add(new ProductConfig() { Name = "XYZ2", Code = "61200022", TimeTotal = 300, GradingTimePoint = 50 });
                        _instance.Product.Add(new ProductConfig() { Name = "XYZ3", Code = "61200023", TimeTotal = 300, GradingTimePoint = 50 });
                        foreach (var item in _instance.Product)
                        {
                            item.AddDefaultGrade();
                        }
                        serialize_to_xml(path, _instance);
                    }
                }
                return _instance;
            }
        }
        public string SerialPorts { get; set; }  
        public string ProductList { get; set; } 
        public string SensorGrougAddress { get; set; }
        public List<ProductConfig> Product { get; set; } 
        public int RefreshInterval { get;   set; } 

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
