using MKSS.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace MKSS.APP.ZhuiSuService.MQTT
{
    public class ZhuiSuMqttServerConfig
    {

        public static ZhuiSuMqttServerConfig Instance
        {
            get {
                try
                {
                    ZhuiSuMqttServerConfig ret = new ZhuiSuMqttServerConfig()
                    {
                        ServerIP = Appsettings.App(new string[] { "MQTT_ServerIP" }),
                        ServerPort = int.Parse(Appsettings.App(new string[] { "MQTT_ServerPORT" })),
                        ServerLoginName = "ZhuiSuMQTT",
                        ServerLoginPwd = "ZhuiSuMQTT_MKSS2021!",
                    };
                    return ret;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        //    = new ZhuiSuMqttServerConfig()
        //{
        //    ServerIP = "127.0.0.1",
        //    ServerPort = 8966,
        //    ServerLoginName = "ZhuiSuMQTT",
        //    ServerLoginPwd = "ZhuiSuMQTT_MKSS2021!",
        //};

        public string ClientId { get; set; } = Guid.NewGuid().ToString("D");
        public string ServerIP { get; set; } = "127.0.0.1";
        public int ServerPort { get; set; } = 8967;
        public string ServerLoginName { get; set; } = "guest";
        public string ServerLoginPwd { get; set; } = "guest";
    }
}
