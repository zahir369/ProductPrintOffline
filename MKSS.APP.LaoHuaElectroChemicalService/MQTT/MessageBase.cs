using Newtonsoft.Json;
using System;

namespace MKSS.APP.LaoHuaService.MQTT
{
    /// <summary>
    /// 消息基础类
    /// </summary>
    public abstract class MessageBase
    {
        /// <summary>
        /// 扩展数据内容
        /// </summary>
        public object ExtendObjectData { get; set; } = DateTime.Now.ToString();

        /// <summary>
        /// 转换为JSON数据
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
