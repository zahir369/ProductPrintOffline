using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace MKSS.Core.Common.Helper
{
    public class JsonHelper
    {

        /// <summary>
        /// json序列化
        /// </summary>
        /// <param name="obj">序列化对象</param>
        /// <returns></returns>
        public static string ToJson(object obj)
        {
            if (obj == null) return "";
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter();
            timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd' 'HH':'mm':'ss";
            string jsonstr = JsonConvert.SerializeObject(obj, timeConverter);
            return jsonstr;
        }
        /// <summary>
        /// json反序列化
        /// </summary>
        /// <typeparam name="T">反序列化对象类型</typeparam>
        /// <param name="jsonStr">JSON字符串</param>
        /// <returns></returns>
        public static T ToObject<T>(string jsonStr)
        {
            try
            {
                T obj = JsonConvert.DeserializeObject<T>(jsonStr);
                return obj;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return default(T);
            }

        }
        /// <summary>
        /// json反序列化
        /// </summary>
        /// <typeparam name="T">反序列化对象类型</typeparam>
        /// <param name="jsonStr">JSON字符串</param>
        /// <param name="depthLength">反序列化深度</param>
        /// <returns></returns>
        public static T ToObject<T>(string jsonStr, int depthLength)
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings() { MaxDepth = depthLength };
                T obj = JsonConvert.DeserializeObject<T>(jsonStr, settings);
                return obj;
            }
            catch
            {
                return default(T);
            }
        }
        /// <summary>
        /// Json反序列化
        /// </summary>
        /// <param name="jsonStr">JSON字符串</param>
        /// <returns></returns>
        public static dynamic DeserializeObject(string jsonStr)
        {
            try
            {
                dynamic obj = JsonConvert.DeserializeObject(jsonStr);
                return obj;
            }
            catch (Exception)
            {

                throw;
            }
        }
        /// <summary>
        /// 查找特定的值
        /// </summary>
        /// <param name="jsonStr">JSON字符串</param>
        /// <param name="key">指定的Key</param>
        /// <returns></returns>
        public static string ReadJsonString(string jsonStr, string key)
        {
            try
            {
                JObject jsonObj = JObject.Parse(jsonStr);
                return jsonObj[key].ToString();
            }
            catch
            {
                return "";
            }
        }
        /// <summary>
        /// 指定Key的字符串反序列化
        /// </summary>
        /// <typeparam name="T">反序列化的类型</typeparam>
        /// <param name="jsonStr">JSON字符串</param>
        /// <param name="key">指定的Key</param>
        /// <returns></returns>
        public static T ReadJsonObject<T>(string jsonStr, string key)
        {
            try
            {
                JObject jsonObj = JObject.Parse(jsonStr);
                return jsonObj[key].ToObject<T>();
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        /// <summary>
        /// 读取字符串反序列化
        /// </summary>
        /// <typeparam name="T">反序列化的类型</typeparam>
        /// <param name="jsonStr">JSON字符串</param>
        /// <returns></returns>
        public static T ReadJsonObject<T>(string jsonStr)
        {
            try
            {
                JObject jsonObj = JObject.Parse(jsonStr);
                return jsonObj.ToObject<T>();
            }
            catch
            {
                return default(T);
            }
        }

    }
}
