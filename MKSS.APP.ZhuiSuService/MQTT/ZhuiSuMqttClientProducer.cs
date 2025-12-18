using MKSS.Util.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MKSS.APP.ZhuiSuService.MQTT
{


    [LogTagClass(Title = "后端查询接口")]
    public class ZhuiSuMqttClientProducer : ZhuiSuMqttClient {
        static ZhuiSuMqttClientProducer _Instance = null;
        public static ZhuiSuMqttClientProducer Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ZhuiSuMqttClientProducer();
                    _Instance.ClientId = "Producer" + GetLocalIp();
                    _Instance.OnGetMQData += _Instance.Instance_OnGetMQData;
                    WindwsService.DeviceMonitor.Program.ZhuiSuService.OnNewData += _Instance.ZhuiSuService_OnNewData;
                }
                return _Instance;
            }
        }

        /// <summary>
        ///  实时数据直接回传给界面
        /// </summary>
        /// <param name="_Batch"></param>
        /// <param name="datas_history"></param>
        void ZhuiSuService_OnNewData(Model.Batch _Batch, List<Model.SensorData> datas_history)
        {
            List<dynamic> list = new List<dynamic>();
            foreach (Model.SensorData s in datas_history)
            {
                if (s.F_DataValue == 0) continue;
                list.Add(
                    new
                    {
                        D = s.F_SensorId,
                        V = s.F_DataValue,
                        T = s.Temperature,
                        H = s.Humidity,
                        DT = s.HTDateTime
                    }
                );
            }
            long batch_id_int = _Batch.F_BatchId;
            MqttMessage mes = new MqttMessage();
            mes.TopicName = "ZhuiSu/QueryRealtimeDataResponse/" + batch_id_int;
            mes.PayLoad = JsonConvert.SerializeObject(list);
            ULogger.Info(string.Format("返回数据:{0} Bytes", Length(mes.PayLoad)));
            bool b = base.MQ_Pub(mes).Result;
        }

        private Task Instance_OnGetMQData(MqttMessage mqMessage)
        {
            Task t = new Task(()=>{

                if (mqMessage.TopicName.StartsWith("ZhuiSu/QueryRealtimeData")) {
                    ULogger.Info(string.Format("已收到查询:{0}", mqMessage.TopicName));
                    //查询数据时会，返回实时数据
                    string batch_id = mqMessage.PayLoad;
                    long batch_id_int = 0;
                    if (long.TryParse(batch_id, out batch_id_int))
                    {
                        Dictionary<long, Dictionary<string, MKSS.Model.SensorData>> data
                            = WindwsService.DeviceMonitor.Program.ZhuiSuService.BatchSensorDataDictionary;
                        if (data.ContainsKey(batch_id_int))
                        {
                            MqttMessage mes = new MqttMessage();
                            mes.TopicName = "ZhuiSu/QueryRealtimeDataResponse/" + batch_id_int;
                            List<dynamic> list = new List<dynamic>();
                            foreach (var item in data[batch_id_int].Values)
                            {
                                list.Add(
                                    new {
                                        D = item.F_SensorId, V = item.F_DataValue
                                    }
                                );
                            }
                            mes.PayLoad = JsonConvert.SerializeObject(list);
                            ULogger.Info(string.Format("返回数据:{0} Bytes", Length(mes.PayLoad)));
                            bool b = base.MQ_Pub(mes).Result;
                        }
                        else
                        {
                            ULogger.Info(string.Format("批次没有实时数据:{0}", batch_id));
                        }
                    }
                    else {
                        ULogger.Info(string.Format("批次格式无效:{0}", batch_id));
                    }
               
                }

            });
            t.Start();
            return t;
        }

        public void Start()
        {
            bool b1 = base.MQ_Init().Result;
            bool b2 = base.MQ_Sub(new MqttMessage() { TopicName = "ZhuiSu/QueryRealtimeData" }).Result;
        }


    }
}
