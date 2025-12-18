using MKSS.Util;
using MKSS.Util.Log;
using MQTTnet;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using MQTTnet.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MKSS.APP.ZhuiSuService.MQTT
{
    [LogTagClass(Title = "MQTT服务端")]
    public class ZhuiSuMqttServer 
    {
        static ZhuiSuMqttServer _Instance = null;
        public static ZhuiSuMqttServer Instance
        {
            get {
                if (_Instance == null) {
                    _Instance = new ZhuiSuMqttServer();
                }
                return _Instance;
            }
        }

        public MqttServer mqttServer = null;
        MqttServerOptionsBuilder optionsBuilder = null;
          

        /// <summary>
        ///  判断是否启动，未启动尝试启动
        /// </summary>
        public async void StartMqttServerAssert()
        {
            try
            {
                if (mqttServer == null)
                {
                    ZhuiSuMqttServerConfig ZhuiSuMqttServrConfig = ZhuiSuMqttServerConfig.Instance;


                    optionsBuilder = new MqttServerOptionsBuilder()
                    .WithDefaultEndpoint().WithDefaultEndpointPort(ZhuiSuMqttServrConfig.ServerPort).WithConnectionValidator(
                        c =>
                        {
                            var currentUser = ZhuiSuMqttServrConfig.ServerLoginName;
                            var currentPWD = ZhuiSuMqttServrConfig.ServerLoginPwd;

                            if (currentUser == null || currentPWD == null)
                            {
                                c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                return;
                            }

                            if (c.Username != currentUser)
                            {
                                c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                return;
                            }

                            if (c.Password != currentPWD)
                            {
                                c.ReasonCode = MqttConnectReasonCode.BadUserNameOrPassword;
                                return;
                            }

                            c.ReasonCode = MqttConnectReasonCode.Success;
                        }).WithSubscriptionInterceptor(
                        c =>
                        {
                            c.AcceptSubscription = true;
                        }).WithApplicationMessageInterceptor(
                        c =>
                        {
                            c.AcceptPublish = true;
                        });

                    mqttServer = new MqttFactory().CreateMqttServer() as MqttServer;
                    mqttServer.StartedHandler = new MqttServerStartedHandlerDelegate(OnMqttServerStarted);
                    mqttServer.StoppedHandler = new MqttServerStoppedHandlerDelegate(OnMqttServerStopped);

                    mqttServer.ClientConnectedHandler = new MqttServerClientConnectedHandlerDelegate(OnMqttServerClientConnected);
                    mqttServer.ClientDisconnectedHandler = new MqttServerClientDisconnectedHandlerDelegate(OnMqttServerClientDisconnected);
                    mqttServer.ClientSubscribedTopicHandler = new MqttServerClientSubscribedHandlerDelegate(OnMqttServerClientSubscribedTopic);
                    mqttServer.ClientUnsubscribedTopicHandler = new MqttServerClientUnsubscribedTopicHandlerDelegate(OnMqttServerClientUnsubscribedTopic);
                    mqttServer.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(OnMqttServer_ApplicationMessageReceived);

                }

                if (!mqttServer.IsStarted)
                {
                    ULogger.Info("MQTT Server Starting...");
                    await mqttServer.StartAsync(optionsBuilder.Build());
                    ULogger.Info("MQTT Server is started.");
                }


            }
            catch (Exception ex)
            {
                ULogger.Error($"MQTT Server start fail.>{ex.Message}");
                ULogger.Error(ex);
            }
        }

        /// <summary>
        ///  发布主题
        /// </summary>
        /// <param name="topic"></param>
        /// <param name="payload"></param>
        public async void ServerPublishMqttTopic(string topic, string payload)
        {
            var message = new MqttApplicationMessage()
            {
                Topic = topic,
                Payload = Encoding.UTF8.GetBytes(payload)
            };
            await mqttServer.PublishAsync(message);
            ULogger.Info( string.Format("MQTT Broker发布主题[{0}]成功！", topic));
        } 


        private void OnMqttServer_ApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs e)
        {
            //ULogger.Info($"客户端[{e.ClientId}]>> 主题：{e.ApplicationMessage.Topic} 负荷大小：{e.ApplicationMessage.Payload.Length} Qos：{e.ApplicationMessage.QualityOfServiceLevel} 保留：{e.ApplicationMessage.Retain}");
        }

        private void OnMqttServerClientUnsubscribedTopic(MqttServerClientUnsubscribedTopicEventArgs e)
        {
            ULogger.Info($"客户端[{e.ClientId}]>> 取消订阅主题：{e.TopicFilter} ");

        }

        private void OnMqttServerClientSubscribedTopic(MqttServerClientSubscribedTopicEventArgs e)
        {
            ULogger.Info($"客户端[{e.ClientId}]>> 订阅主题：{e.TopicFilter} ");
        }

        private void OnMqttServerClientDisconnected(MqttServerClientDisconnectedEventArgs e)
        {
            ULogger.Info($"客户端[{e.ClientId}]已断开连接");
        }

        private void OnMqttServerClientConnected(MqttServerClientConnectedEventArgs e)
        {
            ULogger.Info($"客户端[{e.ClientId}]已连接");
        }

        private void OnMqttServerStopped(EventArgs obj)
        {
            ULogger.Info($"服务器已停止");
        }

        private void OnMqttServerStarted(EventArgs obj)
        {
            ULogger.Info($"服务器已开启");
        }

        public async void StopMqttServer()
        {
            if (mqttServer == null) return;
            try
            {
                await mqttServer?.StopAsync();
                mqttServer = null;
                ULogger.Info("MQTT Server is stopped."); 
            }
            catch (Exception ex)
            {
                ULogger.Error($"MQTT Server stop fail.>{ex.Message}");
                ULogger.Error(ex);
            }
        }
         
    }
}
