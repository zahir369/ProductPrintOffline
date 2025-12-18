using MKSS.Model;
using MKSS.Util.Log;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using MKSS.APP.SemiTester;
using MKSS.Service.LaoHuaElectroChemical;
using MKSS.Service.LaoHua;
using System.Collections.Generic;

namespace MKSS.APP.LaoHuaService.MQTT
{

    [LogTagClass(Title = "串口数据接口")]
    public class LaoHuaDataProviderSerialPort : LaoHuaDataProvider
    {

        Batch Batch { get; set; }
        public bool Inited { get; private set; }
        public LaHuaTaskStatus TaskStatus { get { return Status.TaskStatus; } }
        public LaoHuaTaskOperate TaskOperate { get { return Status.TaskOperate; } }
        public StatusEntity Status { get; private set; } = new StatusEntity() { TaskStatus = LaHuaTaskStatus.Stoped, TaskOperate = LaoHuaTaskOperate.Stop };
        public BoardConnectionSreial Connection { get { return LaohuaService == null ? null : LaohuaService.Connection as BoardConnectionSreial; } }
        public ElectroChemicalService LaohuaService { get; private set; }

        static LaoHuaDataProviderSerialPort _Instance = null;
        public static LaoHuaDataProviderSerialPort Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = new LaoHuaDataProviderSerialPort();
                    ElectroChemicalService.OnNewData += _Instance.ElectroChemicalService_OnNewData;
                    ElectroChemicalService.OnFinish += _Instance.ElectroChemicalService_OnFinish;
                    ElectroChemicalService.CircleOnce += _Instance.ElectroChemicalService_CircleOnce;
                    ElectroChemicalService.StatusChanged += _Instance.ElectroChemicalService_StatusChanged;
                    ElectroChemicalService.OnError += _Instance.ElectroChemicalService_OnError;
                    ElectroChemicalService.OnSucess += _Instance.ElectroChemicalService_OnSucess;
                }
                return _Instance;
            }
        }

        public void InitConnection()
        {

            BoardConnectionSreial connection = new BoardConnectionSreial("COM1");
            LaohuaService = connection.CommFactory;
            if (LaoHuaDataProvider.UIMode == LaoHuaDataProviderUIMode.SingleBoard)
            {
                Dictionary<int, bool> FloorsVisible = connection.FloorsVisible;
                foreach (var floor in FloorsVisible.Keys)
                {
                    FloorsVisible[floor] = floor == 1;//只显示第一层的数据
                }
            }
            Inited = true;

        }


        private void ElectroChemicalService_OnSucess(Batch _Batch, int floor, string region)
        {
            MessageEntity status = new MessageEntity()
            {
                Floor = floor,
                RegionName = region
            }; 
            //ULogger.Info(string.Format("后台报错{0}层,{1}区,:{2}", status.Floor, status.RegionName, status.Message));
            UIZhuiSuC10 con10 = UIZhuiSuC10Model.Instance.View;
            con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.ClearErrorInfo(status);
            });

        }

        private void ElectroChemicalService_OnError(Batch _Batch, int floor, string region, string message)
        {
            ErrorEntity status = new ErrorEntity()
            {
                Floor = floor,
                RegionName = region,
                Message = message
            }; 
            UIZhuiSuC10 con10 = UIZhuiSuC10Model.Instance.View;
            con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.RefreshErrorInfo(status);
            });

            PublishStatus(_Batch);//通知界面其他状态，避免界面因错误长期不同步
        }

        private void ElectroChemicalService_StatusChanged(Batch _Batch, LaHuaTaskStatus time)
        {
            PublishStatus(_Batch);
        }
        private void ElectroChemicalService_CircleOnce(Batch _Batch, int time)
        {
            PublishStatus(_Batch);
        }
        private void ElectroChemicalService_OnFinish(Batch _Batch, TimeSpan F_AddTime)
        {
            PublishStatus(_Batch);
        }

        /// <summary>
        ///  实时数据直接回传给界面
        /// </summary>
        /// <param name="_Batch"></param>
        /// <param name="datas_history"></param>
        void ElectroChemicalService_OnNewData(Batch _Batch, SensorGroupData datas_history, TimeSpan F_AddTime)
        {
            if (datas_history == null || datas_history.Data().Count == 0) return;

            double span = _Batch.F_SpanTime;//从数据库中读出Span点时间
            double current_add_time = UIZhuiSuC10Model.Instance.SettingModel.TxtCurrentSpan;
            if (SensorGroupDataStander.SpanTimeLast == datas_history.F_AddTime)
            { 
                SensorGroupDataStander.SetSpan(datas_history);
            }

            UIZhuiSuC10 con10 = UIZhuiSuC10Model.Instance.View;
            con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                UIZhuiSuC10Model.Instance.RefrehPageInner(_Batch, datas_history, TimeSpan.FromSeconds(datas_history.F_AddTime), con10);
            });
            ULogger.Info(string.Format("收到数据:{0}/{1}", datas_history.F_AddTime, datas_history.F_FloorNo));

        }


        public void PublishStatus(Batch _Batch)
        {
            if (_Batch == null) return;
            Status.F_BatchId = _Batch.F_BatchId;
            Status.TaskOperate = LaohuaService.Operate;
            Status.TaskStatus = LaohuaService.Status;
            Status.FullQueryTimes = _Batch.FullQueryTimes;
            Status.FloorsVisible = LaohuaService.Connection.FloorsVisible;

            UIZhuiSuC10 con10 = UIZhuiSuC10Model.Instance.View;
            con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.RefreshButtons();
            });

        }

         

        public void Query(Batch _Batch, MqttTaskCommand comm = MqttTaskCommand.QueryRealtimeData, string param = null)
        {

            if (!Inited)
            {
                InitConnection();
            }

            if (Inited)
            {

                Batch = _Batch;
                if (_Batch == null) return;

                long batch_id_int = _Batch.F_BatchId;


                if (comm == MqttTaskCommand.QueryRealtimeData)
                {
                    //查询数据时会，返回实时数据
                    List<SensorGroupData> res = ElectroChemicalService.QueryLastData(batch_id_int).OrderBy(w => w.F_FloorNo).ToList();
                    if (res.Count > 0)
                    {
                        foreach (var item in res)
                        {
                            ElectroChemicalService_OnNewData(_Batch, item, TimeSpan.FromSeconds(item.F_AddTime));
                        }
                    }
                    else
                    {
                        ULogger.Info(string.Format("批次没有实时数据:{0}", batch_id_int));
                    }
                }

                if (comm== MqttTaskCommand.StartTask)
                {
                    //ULogger.Info(string.Format("任务启动命令:{0}", mqMessage.TopicName));
                    _Batch = ElectroChemicalService.OfBatch(batch_id_int);
                    LaohuaService.StartTask(_Batch).Start();
                    System.Threading.Thread.Sleep(50);
                }
                if (comm == MqttTaskCommand.StopTask)
                {
                    //ULogger.Info(string.Format("任务停止命令:{0}", mqMessage.TopicName));
                    _Batch = ElectroChemicalService.OfBatch(batch_id_int);
                    LaohuaService.StopTask();
                }
                if (comm == MqttTaskCommand.PauseTask)
                {
                    //ULogger.Info(string.Format("任务暂停命令:{0}", mqMessage.TopicName));
                    _Batch = ElectroChemicalService.OfBatch(batch_id_int);
                    LaohuaService.PauseTask();
                }
                if (comm == MqttTaskCommand.ResumeTask)
                {
                    //ULogger.Info(string.Format("任务继续命令:{0}", mqMessage.TopicName));
                    _Batch = ElectroChemicalService.OfBatch(batch_id_int);
                    LaohuaService.ResumeTask();
                }
                if (comm == MqttTaskCommand.SetVisible)
                {
                    //ULogger.Info(string.Format("任务继续命令:{0}", mqMessage.TopicName));
                    _Batch = ElectroChemicalService.OfBatch(batch_id_int);
                    LaohuaService.SetVisible(int.Parse(param), true);
                }
                if (comm == MqttTaskCommand.SetHidden)
                {
                    //ULogger.Info(string.Format("任务继续命令:{0}", mqMessage.TopicName));
                    _Batch = ElectroChemicalService.OfBatch(batch_id_int);
                    LaohuaService.SetVisible(int.Parse(param), false);
                }

                PublishStatus(_Batch);

            }
        }


    }


}
