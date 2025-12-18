using MKSS.APP.ZhuiSu;
using MKSS.APP.ZhuiSu.Util;
using MKSS.Model;
using MKSS.Util.Log;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MKSS.APP.ZhuiSuService.MQTT
{
    [LogTagClass(Title = "查询接口")]
    public class ZhuiSuMqttClientConsumer : ZhuiSuMqttClient
    {

        static ZhuiSuMqttClientConsumer _Instance = null;
        public static ZhuiSuMqttClientConsumer Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new ZhuiSuMqttClientConsumer();
                    _Instance.ClientId = "Consumer" + GetLocalIp();
                    _Instance.OnGetMQData += _Instance.Instance_OnGetMQData;
                }
                return _Instance;
            }
        }

        DateTime Instance_OnGetMQDataRefreshUI = DateTime.MinValue;
        private Task Instance_OnGetMQData(MqttMessage mqMessage)
        {
            Task t = new Task(() => {

                Batch _Batch = Batch;
                if (_Batch == null) return;
                if (mqMessage.TopicName.StartsWith("ZhuiSu/QueryRealtimeDataResponse/"+ _Batch.F_BatchId))
                {

                    //查询数据时会，返回实时数据
                    ULogger.Info(string.Format("收到查询结果:{0}", mqMessage.TopicName));
                    string batch_id = mqMessage.TopicName.Substring(mqMessage.TopicName.LastIndexOf("/")+1);
                    int batch_id_int = 0;
                    if (int.TryParse(batch_id, out batch_id_int))
                    {

                        if (_Batch == null || _Batch.F_BatchId!= batch_id_int) return;

                        List<dynamic> list = JsonConvert.DeserializeObject<List<dynamic>>(mqMessage.PayLoad);
                        Dictionary<string, dynamic> vs = new Dictionary<string, dynamic>();
                        foreach (dynamic d in list)
                        {
                            if(!vs.ContainsKey(d.D.Value + "")) vs.Add(d.D.Value + "", d);//Convert.ToInt32
                        }

                        int fitted = 0;
                        int min = int.MaxValue;
                        int max = int.MinValue;
                        foreach (var m in UIZhuiSuC64Model.Instance.BoardCaseTable.ProductModelGroupTable.Values)
                        {
                            foreach (PageSensorModel a in m.ProductTable)
                            {
                                if (string.IsNullOrEmpty(a.SensorId)) continue;
                                //设置值，应用到界面
                                if (vs.ContainsKey(a.SensorId)) {
                                    fitted++;

                                    a.Humidity = Convert.ToDouble(vs[a.SensorId].H.Value);
                                    a.Temperature = Convert.ToDouble(vs[a.SensorId].T.Value);
                                    a.HTDateTime = Convert.ToDateTime(vs[a.SensorId].DT.Value);

                                    a.Parent.Value = Convert.ToInt32(vs[a.SensorId].V.Value);
                                    a.Parent.Humidity = Convert.ToDouble(vs[a.SensorId].H.Value);
                                    a.Parent.Temperature = Convert.ToDouble(vs[a.SensorId].T.Value);
                                    a.Parent.HTDateTime = Convert.ToDateTime(vs[a.SensorId].DT.Value);


                                    if (a.Parent.Value < min) min = a.Parent.Value.Value;
                                    if (a.Parent.Value > max) max = a.Parent.Value.Value;

                                    //根据经过的时间赋值
                                    double total_sec = (DateTime.Now - UIZhuiSuC64Model.Instance.View.Batch.F_AgingStartTime).TotalSeconds;
                                    var tt = a.Ranges.Keys.FirstOrDefault(w => w.TotalSeconds > total_sec);
                                    if (tt.TotalSeconds > 0) {
                                        a.Ranges[tt].Value = Convert.ToInt32(vs[a.SensorId].V.Value);
                                        a.Ranges[tt].Humidity = Convert.ToDouble(vs[a.SensorId].H.Value);
                                        a.Ranges[tt].Temperature = Convert.ToDouble(vs[a.SensorId].T.Value);
                                        a.Ranges[tt].HTDateTime = Convert.ToDateTime(vs[a.SensorId].DT.Value);
                                    }
                                }
                            }
                        }
                        ULogger.Info(string.Format("收到数据:{0}/{1} 个,{2}-{3} ", list.Count,fitted,min,max));

                        if (Instance_OnGetMQDataRefreshUI == DateTime.MinValue || (DateTime.Now - Instance_OnGetMQDataRefreshUI).TotalSeconds > 1) {
                            Instance_OnGetMQDataRefreshUI = DateTime.Now;//1 秒刷一次界面
                            UIZhuiSuC64 con10 = UIZhuiSuC64Model.Instance.View;
                            con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                                //con10.UIZhuiSuC10Chart.SetDataRealTime(UIZhuiSuC10Model.Instance.BoardCaseTable.ProductModelGroupTable.Values.ToList());
                                con10.SetData(UIZhuiSuC64Model.Instance.BoardCaseTable.ProductModelGroupTable.Values.ToList(), Batch);
                            });
                        }

                    }
                    else
                    {
                        ULogger.Info(string.Format("批次格式无效:{0}", batch_id));
                    }

                }

            });
            t.Start();
            return t;
        }
        Batch  Batch { get; set; }
        public bool Inited { get; set; }
        public void Start()
        {
            Inited = base.MQ_Init().Result;
            
        }

        public void Query(Batch _Batch) {
            if (!Inited)
            {
                Inited = base.MQ_Init().Result;
            }
            if (Inited)
            {
                Batch = _Batch;
                if (_Batch == null) return; 
                bool b3 = base.MQ_Sub(new MqttMessage() { TopicName = "ZhuiSu/QueryRealtimeDataResponse/" + _Batch.F_BatchId, PayLoad = _Batch.F_BatchId + "" }).Result;
                bool b2 = base.MQ_Pub(new MqttMessage() { TopicName = "ZhuiSu/QueryRealtimeData", PayLoad = _Batch.F_BatchId+"" }).Result;
                ULogger.Info(string.Format("已发送查询:{0},{1},{2}", _Batch.F_BatchId,b2,b3));
            }
        }

    }
}
