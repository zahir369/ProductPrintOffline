using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;
using System.Runtime.Serialization;

namespace MKSS.Model
{
    public class BatchStaData
    {

        [XmlIgnore()]
        public sta_job_status status { get; set; }

        [XmlIgnore()]
        public Batch Batch { get; set; }

        /// <summary>
        /// 老化批次号
        /// </summary>		
        public long F_BatchId { get { return Batch.F_BatchId; } }


        /// <summary>
        /// 统计时间
        /// </summary>		
        public DateTime F_STA_TIME { get; set; }

         
        public XmlDictionary<string, List<BatchStaRange>> Ranges { get; set; }


        public string ToXml() {
            return serialize_to_xml(this);
        }

        public static BatchStaData FromXml(Batch b, BatchWithXml bXml, List<Sensor> sens)
        {
            
            try
            {

                if (string.IsNullOrEmpty(bXml.F_STA_XML))
                {
                    BatchStaData ss = new BatchStaData() { Batch = b };
                    ss.Ranges = new XmlDictionary<string, List<BatchStaRange>>();
                    foreach (Sensor s in sens)
                    {
                        List<BatchStaRange> all = new List<BatchStaRange>();
                        all.Add(new BatchStaRange(b, s) { Type = new TimeSpan(0, 0, 10) });
                        all.Add(new BatchStaRange(b, s) { Type = new TimeSpan(0, 1, 0) });
                        all.Add(new BatchStaRange(b, s) { Type = new TimeSpan(0, 5, 0) });
                        all.Add(new BatchStaRange(b, s) { Type = new TimeSpan(0, 15, 0) });
                        all.Add(new BatchStaRange(b, s) { Type = new TimeSpan(0, 30, 0) });
                        all.Add(new BatchStaRange(b, s) { Type = new TimeSpan(1, 0, 0) });
                        all.Add(new BatchStaRange(b, s) { Type = new TimeSpan(3, 0, 0) });
                        all.Add(new BatchStaRange(b, s) { Type = new TimeSpan(6, 0, 0) });
                        all.Add(new BatchStaRange(b, s) { Type = new TimeSpan(12, 0, 0) });
                        all.Add(new BatchStaRange(b, s) { Type = new TimeSpan(24, 0, 0) });
                        all.Add(new BatchStaRange(b, s) { Type = new TimeSpan(72, 0, 0) });
                        ss.Ranges.Add(s.F_SensorId, all);
                    }

                    return ss;
                }


                BatchStaData ret = (BatchStaData)BatchStaData.deserialize_from_xml(bXml.F_STA_XML, typeof(BatchStaData));
                ret.Batch = b;
                Dictionary<string, Sensor> send_dic =  sens.ToDictionary(w => w.F_SensorId, w => w);
                foreach (var F_SensorId in ret.Ranges.Keys)
                {
                    var rs = ret.Ranges[F_SensorId];
                    var sen = send_dic[F_SensorId];
                    foreach (var item in rs)
                    {
                        item.Batch = b;
                        item.Sensor = sen;
                    }
                }
                return ret;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// serialize object to xml file.
        /// </summary>
        /// <param name="obj">the object you want to serialize</param>
        public static string serialize_to_xml(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            string content = string.Empty;
            //serialize
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                content = writer.ToString();
            }
            return content;
        }

        /// <summary>
        /// deserialize xml str to object
        /// </summary>
        /// <param name="str">str</param>
        /// <param name="object_type">the object type you want to deserialize</param>
        public static object deserialize_from_xml(string str, Type object_type)
        {
            XmlSerializer serializer = new XmlSerializer(object_type);
            using (StringReader reader = new StringReader(str))
            {
                return serializer.Deserialize(reader);
            }
        }


    }

    public class BatchSensorData
    {
        public int Value { get; set; }
        public DateTime ValueTime { get; set; }
    }

    /// <summary>
    ///  统计状态
    /// </summary>
    public enum sta_job_status
    {
        /// <summary>
        ///  初始化
        /// </summary>
        init = 0,
        /// <summary>
        ///  添加站点
        /// </summary>
        adding_stations = 1,
        /// <summary>
        ///  解析对应传感器
        /// </summary>
        parsing_configs = 2,
        /// <summary>
        ///  查询数据库表
        /// </summary>
        statistcing = 3,
        /// <summary>
        ///  完成
        /// </summary>
        finished = 4
    }

    /// <summary>
    /// Dictionary(支持XML序列化)
    /// </summary>
    /// <typeparam name="TKey">键类型</typeparam>
    /// <typeparam name="TValue">值类型</typeparam>
    [XmlRoot("XmlDictionary")]
    [Serializable]
    public class XmlDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public XmlDictionary() { }
        public XmlDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
        public XmlDictionary(IEqualityComparer<TKey> comparer) : base(comparer) { }
        public XmlDictionary(int capacity) : base(capacity) { }
        public XmlDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) { }
        protected XmlDictionary(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public void Serialize(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            {
                XmlSerializer formatter = new XmlSerializer(GetType());
                formatter.Serialize(fs, this);
            }
        }

        public XmlDictionary<TKey, TValue> Deserialize(string fileName)
        {
            if (File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    XmlSerializer formatter = new XmlSerializer(GetType());
                    return formatter.Deserialize(fs) as XmlDictionary<TKey, TValue>;
                }
            }
            return null;
        }

        #region IXmlSerializable
        public XmlSchema GetSchema() => null;
        /// <summary>
        /// 从对象的XML表示形式生成该对象(反序列化)
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }
            var ks = new XmlSerializer(typeof(TKey));
            var vs = new XmlSerializer(typeof(TValue));
            reader.Read();
            while (reader.NodeType != XmlNodeType.EndElement)
            {
                reader.ReadStartElement("Item");
                reader.ReadStartElement("Key");
                var key = (TKey)ks.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("Value");
                var value = (TValue)vs.Deserialize(reader);
                reader.ReadEndElement();
                Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }
        /// <summary>
        /// 将对象转换为其XML表示形式(序列化)
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(XmlWriter writer)
        {
            var ks = new XmlSerializer(typeof(TKey));
            var vs = new XmlSerializer(typeof(TValue));
            foreach (var key in Keys)
            {
                writer.WriteStartElement("Item");
                writer.WriteStartElement("Key");
                ks.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("Value");
                vs.Serialize(writer, this[key]);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
        #endregion
    }

}