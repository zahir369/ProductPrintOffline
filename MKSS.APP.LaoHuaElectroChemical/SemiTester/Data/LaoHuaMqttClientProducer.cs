//using MKSS.Util.Log;
//using Newtonsoft.Json;
//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using MKSS.Service.LaoHuaElectroChemical;
//using MKSS.Model;
//using MQTTnet.Client.Publishing;

//namespace MKSS.APP.LaoHuaService.MQTT
//{
//    [LogTagClass(Title = "后端查询接口")]
//    public class LaoHuaMqttClientProducer : LaoHuaMqttClient {
//        static LaoHuaMqttClientProducer _Instance = null;
//        public static LaoHuaMqttClientProducer Instance
//        {
//            get
//            {
//                if (_Instance == null)
//                {
//                    _Instance = new LaoHuaMqttClientProducer();
//                    _Instance.ClientId = "Producer" + GetLocalIp();
//                    _Instance.OnGetMQData += _Instance.Instance_OnGetMQData;
//                }
//                return _Instance;
//            }
//        }

//        private void ElectroChemicalService_StatusChanged(Batch _Batch, LaHuaTaskStatus time)
//        {
//            PublishStatus(_Batch);
//        }
//        private void ElectroChemicalService_CircleOnce(Batch _Batch,int time)
//        {
//            PublishStatus(_Batch);
//        }
//        private void ElectroChemicalService_OnFinish(Batch _Batch, TimeSpan F_AddTime)
//        {
//            PublishStatus(_Batch);
//        }


//        / <summary>
//        /  实时数据直接回传给界面
//        / </summary>
//        / <param name="_Batch"></param>
//        / <param name="datas_history"></param>
//        void LaohuaService_OnNewData(Batch _Batch, SensorGroupData datas_history, TimeSpan F_AddTime)
//        {
//            long batch_id_int = _Batch.F_BatchId;
//            MqttMessage mes = new MqttMessage();
//            mes.TopicName = "LaoHua/QueryRealtimeDataResponse/" + batch_id_int;
//            mes.PayLoad = JsonConvert.SerializeObject(datas_history);
//            bool b = base.MQ_Pub(mes).Result;
//            ULogger.Info(b?"T":"F", null, false);
//        }

//        private Task Instance_OnGetMQData(MqttMessage mqMessage)
//        {
//            Task t = new Task(()=>{

//                string batch_id = mqMessage.PayLoad;
//                long batch_id_int = 0;
//                Batch _Batch = null;
//                if (!long.TryParse(batch_id, out batch_id_int)) {
                    
//                }

//                if (mqMessage.TopicName.StartsWith("LaoHua/"+ MqttTaskCommand.QueryRealtimeData)) {

//                    _Batch = ElectroChemicalService.OfBatch(batch_id_int);
//                    查询数据时会，返回实时数据
//                    List<SensorGroupData> res = ElectroChemicalService.QueryLastData(batch_id_int).OrderBy(w => w.F_FloorNo).ToList();
//                    if (res.Count > 0)
//                    {
//                        foreach (var item in res)
//                        {
//                            MqttMessage mes = new MqttMessage();
//                            mes.TopicName = "LaoHua/QueryRealtimeDataResponse/" + batch_id_int;
//                            mes.PayLoad = JsonConvert.SerializeObject(item);
//                            bool b = base.MQ_Pub(mes).Result;
//                        }
//                    }
//                    else
//                    {
//                        ULogger.Info(string.Format("批次没有实时数据:{0}", batch_id));
//                    }
                    
//                }
//                else {

//                    if (mqMessage.TopicName.StartsWith("LaoHua/" + MqttTaskCommand.StartTask))
//                    {
//                        ULogger.Info(string.Format("任务启动命令:{0}", mqMessage.TopicName));
//                        _Batch = ElectroChemicalService.OfBatch(batch_id_int);
//                        Program.LaohuaService.StartTask(_Batch).Start();
//                        System.Threading.Thread.Sleep(50);
//                    }
//                    if (mqMessage.TopicName.StartsWith("LaoHua/" + MqttTaskCommand.StopTask))
//                    {
//                        ULogger.Info(string.Format("任务停止命令:{0}", mqMessage.TopicName));
//                        _Batch = ElectroChemicalService.OfBatch(batch_id_int);
//                        Program.LaohuaService.StopTask();
//                    }
//                    if (mqMessage.TopicName.StartsWith("LaoHua/" + MqttTaskCommand.PauseTask))
//                    {
//                        ULogger.Info(string.Format("任务暂停命令:{0}", mqMessage.TopicName));
//                        _Batch = ElectroChemicalService.OfBatch(batch_id_int);
//                        Program.LaohuaService.PauseTask();
//                    }
//                    if (mqMessage.TopicName.StartsWith("LaoHua/" + MqttTaskCommand.ResumeTask))
//                    {
//                        ULogger.Info(string.Format("任务继续命令:{0}", mqMessage.TopicName));
//                        _Batch = ElectroChemicalService.OfBatch(batch_id_int);
//                        Program.LaohuaService.ResumeTask();
//                    }

//                    PublishStatus(_Batch); 

//                }

                

//            });
//            t.Start();
//            return t;
//        }


//        public void PublishStatus(Batch _Batch)
//        {

//            if (_Batch == null) return;
//            StatusEntity status = new StatusEntity()
//            {
//                F_BatchId = _Batch.F_BatchId,
//                TaskOperate = Program.LaohuaService.Operate,
//                TaskStatus = Program.LaohuaService.Status,
//                FullQueryTimes = Program.LaohuaService.FullQueryTimes,
//            };
//            MqttMessage mes = new MqttMessage();
//            mes.TopicName = "LaoHua/StatusTaskResponse/" + _Batch.F_BatchId;
//            mes.PayLoad = JsonConvert.SerializeObject(status);
//            MqttClientPublishResult bxx = base.MQ_PubWaiting(mes);
//            ULogger.Info(string.Format("已发送状态:{0},{1},{2}", _Batch.F_BatchId, status,
//                bxx.ReasonCode == MqttClientPublishReasonCode.Success
//                ? "成功"
//                : string.Format("失败，" + bxx.ReasonCode + ":" + bxx.ReasonString)
//                ));
//        }


//        public void Start() {
//            bool b1 = base.MQ_Init().Result;
//            foreach (var item in Enum.GetValues<MqttTaskCommand>())
//            {
//                bool b2 = base.MQ_Sub(new MqttMessage() { TopicName = "LaoHua/"+ item }).Result;
//            }
//        }


//    }
//}
