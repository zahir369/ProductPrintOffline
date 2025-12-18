using MKSS.Model;
using MKSS.Util.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MKSS.APP.SemiCatalysis
{

    public class UIZhuiSuC10Model {

        public static UIZhuiSuC10Model Instance = null; 
        public UIZhuiSuC10SettingModel SettingModel { get;   set; }
        public UIZhuiSuC10 View { get; set; }
        public UIZhuiSuData Data { get { return UIZhuiSuData.Instance; } }


        DateTime Instance_OnGetMQDataRefreshUI = DateTime.MinValue;
        public MKSS.Service.SemiCatalysis.SemiCatalysisService ECService { get; set; }

        public UIZhuiSuC10Model() {
            Instance = this;
            ECService = new Service.SemiCatalysis.SemiCatalysisService();
            ECService.OnNewData += SemiCatalysisService_OnNewData;
            ECService.OnFinish += ECService_OnFinish;
        }

        private void ECService_OnFinish(Batch _Batch, List<Sensor> sens, TimeSpan F_AddTime)
        {
            SettingModel.Started = false;
            View.UIZhuiSuC10Setting.RefreshButtons();
        }

        /// <summary>
        ///  实时数据直接回传给界面
        /// </summary>
        /// <param name="_Batch"></param>
        /// <param name="datas_history"></param>
        [LogTagClass(Title = "刷新界面")]
        void SemiCatalysisService_OnNewData(Model.Batch _Batch, Dictionary<string, MKSS.Model.SensorData> datas_history,TimeSpan F_AddTime)
        {
            if (_Batch == null) return;
            if (_Batch == null) return;
            if (datas_history == null || datas_history.Count == 0) return;

            if (this.SettingModel == null) return;
            if (this.SettingModel.TxtSensorGrougAddress == null) return;
            List<int> addrs = this.SettingModel.Address();
             

            if (Instance_OnGetMQDataRefreshUI == DateTime.MinValue || (DateTime.Now - Instance_OnGetMQDataRefreshUI).TotalSeconds > 0.2)
            {
                Instance_OnGetMQDataRefreshUI = DateTime.Now;//1 秒刷一次界面
                UIZhuiSuC10 con10 = this.View;
                con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                    con10.UIZhuiSuC10Chart.SetDataRealTime(_Batch, datas_history,   F_AddTime);
                    con10.SetData(_Batch, datas_history,   F_AddTime);
                });
            } 


        }

        /// <summary>
        ///   绑定批次数据到界面
        /// </summary>
        /// <param name="batch"></param>
        public void RefreshAddress(Batch batch)
        {
            
        }


        public void DataTimerRefresh(Batch _Batch)
        {

            if (_Batch == null) return;

            UIZhuiSuC10 con10 = this.View;
             
        }

        internal void InitPage(UIZhuiSuC10 uIZhuiSuGuanChaC10)
        {
            this.View = uIZhuiSuGuanChaC10;
        }


        public void ExportExcel(string path, Batch _Batch)
        {


            try
            {
                DataTable tableAll = new DataTable("详细数据");
                DataTable tableSel = new DataTable("选择数据");

                tableAll.Columns.Add("时间", typeof(double));
                tableSel.Columns.Add("时间", typeof(double));
                foreach (var item in ECService.BatchSensorDictionary.Values)
                {
                    tableAll.Columns.Add(item.PosString, typeof(double));
                }

                List<string> sel = this.View.UIZhuiSuC10Sensors.SelectSensorPositions;
                foreach (var item in this.View.UIZhuiSuC10Sensors.SelectSensorPositions)
                {
                    tableSel.Columns.Add(item, typeof(double));
                }

                foreach (TimeSpan ts in ECService.BatchSensorDataDictionary.Keys)
                {
                    DataRow dr = tableAll.NewRow();
                    dr["时间"] = double.Parse(ts.TotalSeconds.ToString("f2"));
                    foreach (var sid in ECService.BatchSensorDataDictionary[ts].Keys)
                    {
                        Sensor s = ECService.BatchSensorDictionary[sid];
                        SensorData sd = ECService.BatchSensorDataDictionary[ts][sid];
                        string pos = s.PosString;
                        dr[pos] = sd.F_ShowValue;
                    }
                    tableAll.Rows.Add(dr);
                }

                foreach (TimeSpan ts in ECService.BatchSensorDataDictionary.Keys)
                {
                    DataRow dr = tableSel.NewRow();
                    dr["时间"] = double.Parse(ts.TotalSeconds.ToString("f2"));
                    foreach (var sid in ECService.BatchSensorDataDictionary[ts].Keys)
                    {
                        Sensor s = ECService.BatchSensorDictionary[sid];
                        if (sel.Contains(s.PosString))
                        {
                            SensorData sd = ECService.BatchSensorDataDictionary[ts][sid];
                            string pos = s.PosString;
                            dr[pos] = sd.F_ShowValue;
                        }
                    }
                    tableSel.Rows.Add(dr);
                }

                DataTable tableKey = new DataTable("关键数据");
                tableKey.Columns.Add("位置");
                tableKey.Columns.Add("最新数据", typeof(double));
                int[] Seconds = this.SettingModel.TxtTestTimePointsArr;
                foreach (var item in Seconds)
                {
                    tableKey.Columns.Add(string.Format("第{0}秒", item), typeof(double));
                }
                tableKey.Columns.Add("差值", typeof(double));

                Dictionary<int, Dictionary<string, MKSS.Model.SensorData>> dss = UIZhuiSuC10Model.Instance.ECService.GetDataRange(Seconds);
                foreach (C10GridRow item in this.View.UIZhuiSuC10Grid.BasdeData)
                {
                    DataRow dr = tableKey.NewRow();
                    dr["位置"] = item.WEIZHI;
                    dr["最新数据"] = string.IsNullOrEmpty(item.XINHAO_AD) ? 0 : double.Parse(item.XINHAO_AD);
                    for (int x = 0; x < Seconds.Length; x++)
                    {
                        if (string.IsNullOrEmpty(item.SensorId)) continue;
                        if (!dss.ContainsKey(Seconds[x]) || !dss[Seconds[x]].ContainsKey(item.SensorId) || dss[Seconds[x]][item.SensorId] == null) continue;
                        dr[2 + x] = dss[Seconds[x]][item.SensorId].F_ShowValue;
                    }
                    dr["差值"] = item.BD_CZ;
                    tableKey.Rows.Add(dr);
                }


                DataSet ds = new DataSet();
                ds.Tables.Add(tableSel);
                ds.Tables.Add(tableKey);
                ds.Tables.Add(tableAll);
                EPPlusHelper.ImportExcel(path, ds);
            }
            catch (Exception ex)
            {
                throw ex;
            }
             
            

        }

    }

}
