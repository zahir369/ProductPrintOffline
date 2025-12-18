
using MKSS.APP.LaoHuaService.MQTT;
using MKSS.Util.Log;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Publishing;
using MQTTnet.Client.Receiving;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MKSS.APP.LaoHuaService.MQTT
{


    public class LaoHuaMqttClient  
    {
   

        IMqttClient Client;
        IMqttClientOptions clientOptions;
        /// <summary>
        /// 返回订阅数据的事件
        /// </summary>
        public event GetMQDataHandler OnGetMQData;
        /// <summary>
        /// 客户端断开重连时重新订阅数据事件
        /// </summary>
        public event ReSubDataHandler OnReSubData;
        public string ClientId { get; set; }= Guid.NewGuid().ToString("D");


        public static string GetLocalIp()
        {
            IPAddress localIp = null;

            try
            {
                var task = Task.Run(async () => { return await System.Net.Dns.GetHostAddressesAsync(Dns.GetHostName()); });
                IPAddress[] ipArray = task.Result;
                localIp = ipArray.First(ip => ip.AddressFamily == AddressFamily.InterNetwork);
            }
            catch (Exception ex)
            {
            }
            if (localIp == null)
            {
                localIp = IPAddress.Parse("127.0.0.1");
            }
            return localIp.ToString();
        }

        /// <summary>
        /// 存储重新订阅主题信息
        /// </summary>
        private MqttMessage reSubMqttMessage;

        public int Length(string str) {
            int charNum = 0; //统计字节位数
            char[] _charArray = str.ToCharArray();
            for (int i = 0; i < _charArray.Length; i++)
            {
                char _eachChar = _charArray[i];
                if (_eachChar >= 0x4e00 && _eachChar <= 0x9fa5) //判断中文字符
                    charNum += 2;
                else
                    charNum += 1;
            }
            return charNum;
        }

        /// <summary>
        /// 初始化消息队列
        /// </summary>
        /// <param name="cfg"></param>
        /// <returns></returns>
        public async Task<bool> MQ_Init()
        {

            var mqttModel = LaoHuaMqttServerConfig.Instance;

            clientOptions = new MqttClientOptionsBuilder()
                              .WithClientId(ClientId)
                              .WithTcpServer(mqttModel.ServerIP,
                              mqttModel.ServerPort)
                              .WithCredentials(mqttModel.ServerLoginName, mqttModel.ServerLoginPwd)
                              .WithCleanSession()                             
                              //.WithKeepAlivePeriod(TimeSpan.FromSeconds(5))
                              //.WithKeepAliveSendInterval(TimeSpan.FromSeconds(5)) //该方法过期
                              .Build();
            try
            {
                if (Client == null || !Client.IsConnected)
                {
                    var factory = new MqttFactory();

                    Client = factory.CreateMqttClient();


                    Client.ConnectedHandler = new MqttClientConnectedHandlerDelegate(e =>
                   {
                       ULogger.Info(string.Format("### CONNECTED WITH SERVER ###:{0},{1} ", mqttModel.ServerIP, mqttModel.ServerPort));
                       //Console.WriteLine("### CONNECTED WITH SERVER ###");
                       //默认匹配所有主题
                       //await Client.SubscribeAsync(new TopicFilterBuilder().WithTopic("#").Build());

                       //Console.WriteLine("### SUBSCRIBED ###");
                   });

                    

                    Client.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
                    {
                        //Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                        //Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                        //Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        //Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                        //Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                        //Console.WriteLine();
                        // e.ClientId
                        //ULogger.Info($"TopicName:{e.ApplicationMessage.Topic} Payload:{Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                        //Console.WriteLine("当前ClientId:" + e.ClientId);
                        //Console.WriteLine($"TopicName:{e.ApplicationMessage.Topic}");

                        var mqMsg = new MqttMessage()
                        {
                            TopicName = e.ApplicationMessage.Topic,
                            PayLoad = Encoding.UTF8.GetString(e.ApplicationMessage.Payload)
                        };

                        if (OnGetMQData != null)
                        {
                            OnGetMQData.Invoke(mqMsg);
                        }

                    });




                    Client.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(async e =>
                    {

                        //断开后启动重连机制
                        await Task.Delay(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

                        try
                        {
                            //重新连接
                            await Client.ConnectAsync(clientOptions).ConfigureAwait(false);
                            //重新订阅
                            if (reSubMqttMessage != null)
                                await MQ_Sub(reSubMqttMessage).ConfigureAwait(false);

                            //OnReSubData?.Invoke(reSubMqttMessage);

                        }
                        catch (Exception ex)
                        {
                            //重连失败
                            //Console.WriteLine("### RECONNECTING FAILED ###");
                        }
                    });

                    await Client.ConnectAsync(clientOptions).ConfigureAwait(false);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<bool> MQ_Pub(MqttMessage msg)
        {

            if (!Client.IsConnected)
                return false;

            try
            {
                var appMsg = new MqttApplicationMessageBuilder()
                           .WithTopic(msg?.TopicName)
                           .WithPayload(msg.PayLoad)
                           .WithAtLeastOnceQoS() // 
                           .WithRetainFlag(false) //保持标志                          
                           .Build();

                await Client.PublishAsync(appMsg).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public MqttClientPublishResult MQ_PubWaiting(MqttMessage msg)
        {

            if (!Client.IsConnected) {
                return new MqttClientPublishResult()
                {
                    ReasonCode = MqttClientPublishReasonCode.UnspecifiedError,
                    ReasonString = "未连接"
                };
            }
                 

            try
            {
                var appMsg = new MqttApplicationMessageBuilder()
                           .WithTopic(msg?.TopicName)
                           .WithPayload(msg.PayLoad)
                           .WithAtLeastOnceQoS() // 
                           .WithRetainFlag(false) //保持标志                          
                           .Build();

                return Client.PublishAsync(appMsg).Result;
            }
            catch (Exception ex)
            {
                return new MqttClientPublishResult() { 
                    ReasonCode= MqttClientPublishReasonCode.UnspecifiedError, ReasonString= ex.Message
                };
            }
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<bool> MQ_Sub(MqttMessage msg)
        {
            if (!Client.IsConnected)
                return false;
            try
            {
                reSubMqttMessage = msg;

                //await Client.SubscribeAsync(new TopicFilterBuilder().WithTopic(msg.TopicName).Build());
                //await Client.SubscribeAsync(new List<TopicFilter> {
                //new TopicFilter(msg.TopicName, MqttQualityOfServiceLevel.AtMostOnce)
                //});
                //await client.SubscribeAsync(new TopicFilter(msg.TopicName, MqttQualityOfServiceLevel.AtMostOnce));
                //await Client.SubscribeAsync(new TopicFilterBuilder().WithTopic(msg.TopicName).Build());

                

                await Client.SubscribeAsync(new MqttTopicFilterBuilder()
                  .WithTopic(msg?.TopicName)
                  .WithAtLeastOnceQoS()
                  .Build()
                  ).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        /// <summary>
        /// 取消订阅主题
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<bool> MQ_UnSub(MqttMessage msg)
        {
            if (!Client.IsConnected)
                return false;

            await Client.UnsubscribeAsync(msg?.TopicName).ConfigureAwait(false);

            return true;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <returns></returns>
        public async Task MQ_Close()
        {

            await Client.DisconnectAsync().ConfigureAwait(false);

            Client.Dispose();
        }

    }

    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="mqMessage"></param>
    /// <returns></returns>
    public delegate Task GetMQDataHandler(MqttMessage mqMessage);
    /// <summary>
    /// 重新订阅
    /// </summary>
    /// <param name="mqMessage"></param>
    /// <returns></returns>
    public delegate Task ReSubDataHandler(MqttMessage mqMessage);
}
