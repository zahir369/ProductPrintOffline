namespace MKSS.APP.LaoHuaService.MQTT
{
    /// <summary>
    /// MQTT消息类
    /// </summary>
    public class MqttMessage : MessageBase
    {
        /// <summary>
        /// 主题名称
        /// </summary>
        public string TopicName { get; set; } = "";

        /// <summary>
        /// 消息载体
        /// </summary>
        public string PayLoad { get; set; } = "";
    }
}
