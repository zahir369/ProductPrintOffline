using MKSS.Model;
using MKSS.Util.Log;
using Newtonsoft.Json;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using MKSS.APP.SemiTester;

namespace MKSS.APP.LaoHuaService.MQTT
{

    [LogTagClass(Title = "网络数据接口")]
    public class LaoHuaDataProviderNetwork : LaoHuaMqttClient , LaoHuaDataProvider
    {

        static LaoHuaDataProviderNetwork _Instance = null;
        public static LaoHuaDataProviderNetwork Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new LaoHuaDataProviderNetwork();
                    _Instance.ClientId = "Consumer" + GetLocalIp();
                    _Instance.OnGetMQData += _Instance.Instance_OnGetMQData;
                }
                return _Instance;
            }
        }
        public bool Inited { get; private set; }
        public LaHuaTaskStatus TaskStatus { get { return Status.TaskStatus; } }
        public LaoHuaTaskOperate TaskOperate { get { return Status.TaskOperate; } }
        public StatusEntity Status { get; private set; } = new StatusEntity() { TaskStatus = LaHuaTaskStatus.Stoped, TaskOperate = LaoHuaTaskOperate.Stop };

        private Task Instance_OnGetMQData(MqttMessage mqMessage)
        {
            Task t = new Task(() => {

                Batch _Batch = Batch;
                if (_Batch == null) return;


                if (mqMessage.TopicName.StartsWith("LaoHua/StatusTaskResponse/" + _Batch.F_BatchId))
                {
                    //查询数据时会，返回实时数据
                    ULogger.Info(string.Format("收到状态结果:{0}", mqMessage.TopicName));
                    string batch_id = mqMessage.TopicName.Substring(mqMessage.TopicName.LastIndexOf("/") + 1);
                    long batch_id_int = 0;
                    if (long.TryParse(batch_id, out batch_id_int))
                    {
                        StatusEntity status = JsonConvert.DeserializeObject<StatusEntity>(mqMessage.PayLoad);
                        if (status == null) return;
                        Status.F_BatchId = status.F_BatchId;
                        Status.Message = status.Message;
                        Status.TaskOperate = status.TaskOperate;
                        Status.TaskStatus = status.TaskStatus;
                        Status.FullQueryTimes = status.FullQueryTimes;
                        Status.FloorsVisible = status.FloorsVisible;

                        UIZhuiSuC10 con10 = UIZhuiSuC10Model.Instance.View;
                        con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.RefreshButtons();
                        });
                    }
                    else
                    {
                        ULogger.Info(string.Format("批次格式无效:{0}", batch_id));
                    }

                    return;
                }


                if (mqMessage.TopicName.StartsWith("LaoHua/QueryRealtimeDataResponse/" + _Batch.F_BatchId))
                {

                    //查询数据时会，返回实时数据
                    ULogger.Info(string.Format("收到查询结果:{0}", mqMessage.TopicName));
                    string batch_id = mqMessage.TopicName.Substring(mqMessage.TopicName.LastIndexOf("/") + 1);
                    long batch_id_int = 0;
                    if (long.TryParse(batch_id, out batch_id_int))
                    {
                        SensorGroupData datas_history = JsonConvert.DeserializeObject<SensorGroupData>(mqMessage.PayLoad);
                        if (datas_history == null || datas_history.Data().Count == 0) return;

                        double span = _Batch.F_SpanTime;//从数据库中读出Span点时间
                        double spanValue = _Batch.F_SpanValue;//从数据库中读出Span点时间
                        if (SensorGroupDataStander.SpanTimeLast == datas_history.F_AddTime)
                        { 
                            SensorGroupDataStander.SetSpan(datas_history);
                        }

                        UIZhuiSuC10 con10 = UIZhuiSuC10Model.Instance.View;
                        con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                            UIZhuiSuC10Model.Instance.RefrehPageInner(_Batch, datas_history, TimeSpan.FromSeconds(datas_history.F_AddTime), con10);
                        });
                        ULogger.Info(string.Format("收到数据:{0}/{1}", datas_history.F_AddTime, datas_history.F_FloorNo));
                        UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.ClearErrorInfo();
                    }
                    else
                    {
                        ULogger.Info(string.Format("批次格式无效:{0}", batch_id));
                    }

                }

                if (mqMessage.TopicName.StartsWith("LaoHua/OnErrorResponse/" + _Batch.F_BatchId))
                {
                    //查询数据时会，返回实时数据
                    string batch_id = mqMessage.TopicName.Substring(mqMessage.TopicName.LastIndexOf("/") + 1);
                    long batch_id_int = 0;
                    if (long.TryParse(batch_id, out batch_id_int))
                    {
                        ErrorEntity status = JsonConvert.DeserializeObject<ErrorEntity>(mqMessage.PayLoad);
                        if (status == null) return; 
                        //ULogger.Info(string.Format("后台报错{0}层,{1}区,:{2}", status.Floor, status.RegionName, status.Message));
                        UIZhuiSuC10 con10 = UIZhuiSuC10Model.Instance.View;
                        con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.RefreshErrorInfo(status);
                        });
                    }
                    else
                    {
                        ULogger.Info(string.Format("批次格式无效:{0}", batch_id));
                    }

                    return;
                }


                if (mqMessage.TopicName.StartsWith("LaoHua/OnSucessResponse/" + _Batch.F_BatchId))
                {
                    //查询数据时会，返回实时数据
                    string batch_id = mqMessage.TopicName.Substring(mqMessage.TopicName.LastIndexOf("/") + 1);
                    long batch_id_int = 0;
                    if (long.TryParse(batch_id, out batch_id_int))
                    {
                        MessageEntity status = JsonConvert.DeserializeObject<MessageEntity>(mqMessage.PayLoad);
                        if (status == null) return;
                        //ULogger.Info(string.Format("后台报错{0}层,{1}区,:{2}", status.Floor, status.RegionName, status.Message));
                        UIZhuiSuC10 con10 = UIZhuiSuC10Model.Instance.View;
                        con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.ClearErrorInfo(status);
                        });
                    }
                    else
                    {
                        ULogger.Info(string.Format("批次格式无效:{0}", batch_id));
                    }

                    return;
                }

            });
            t.Start();
            return t;
        }
        public Batch Batch { get { return UIZhuiSuC10Model.Instance.BatchCurrent; } }

        public void InitConnection()
        {
            Inited = base.MQ_Init().Result;
        }

        public void Query(Batch _Batch, MqttTaskCommand comm = MqttTaskCommand.QueryRealtimeData, string param = null)
        {
            if (!Inited)
            {
                Inited = base.MQ_Init().Result;
            }

            if (Inited)
            { 
                if (_Batch == null) return;
                bool b0 = base.MQ_Sub(new MqttMessage() { TopicName = "LaoHua/OnErrorResponse/" + _Batch.F_BatchId }).Result;
                bool b1 = base.MQ_Sub(new MqttMessage() { TopicName = "LaoHua/StatusTaskResponse/" + _Batch.F_BatchId }).Result;
                bool b31 = base.MQ_Sub(new MqttMessage() { TopicName = "LaoHua/QueryRealtimeDataResponse/" + _Batch.F_BatchId }).Result;
                ULogger.Info(string.Format("已订阅:{0},{1} ", b1, b31));
                bool b2 = base.MQ_Pub(new MqttMessage() { TopicName = "LaoHua/" + comm, PayLoad = _Batch.F_BatchId + (string.IsNullOrEmpty(param)?";0":";"+ param) }).Result;
                ULogger.Info(string.Format("已发送命令:{0},{1} ", _Batch.F_BatchId, b2));
            }
        }
         

    }


}
