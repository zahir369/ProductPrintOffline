using MaterialDesignThemes.Wpf;
using MKSS.Model;
using MKSS.Util.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace MKSS.APP.SemiTester
{

    public class UIZhuiSuC10Model {

        public static UIZhuiSuC10Model Instance = null; 
        public UIZhuiSuC10SettingModel SettingModel { get;   set; }
        public UIZhuiSuC10 View { get; set; }
        public UIZhuiSuData Data { get { return UIZhuiSuData.Instance; } }
        //PaletteHelper paletteHelper = new PaletteHelper();

        //private static void ApplyBase(bool isDark)
        //    => PaletteHelper.ModifyTheme(theme => theme.SetBaseTheme(isDark ? Theme.Dark : Theme.Light));
        //private bool _isDarkTheme;
        //public bool IsDarkTheme 
        //{
        //    get => _isDarkTheme;
        //    set
        //    {
        //        if (_isDarkTheme != value)
        //        {
        //            _isDarkTheme = value;
        //            ApplyBase(value);
        //        }
        //    }
        //}

        public MKSS.Service.SemiTester.SemiTesterService ECService { get; set; }

        public UIZhuiSuC10Model() {
            Instance = this;
            //ITheme theme = paletteHelper.GetTheme();
            //IsDarkTheme = theme.GetBaseTheme() == BaseTheme.Dark;
            //IsDarkTheme = true;

            ECService = new Service.SemiTester.SemiTesterService();
            ECService.OnNewData += SemiTesterService_OnNewData;
            ECService.OnFinish += ECService_OnFinish;
        }

        private void ECService_OnFinish(Batch _Batch, List<Sensor> sens, TimeSpan F_AddTime)
        {
            View.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                SettingModel.Started = false;
                View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.RefreshButtons();
                //设置反应值的点
                this.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.SetCurrentTime(SettingModel.TxtSpanPoint);
            });
        }

        /// <summary>
        ///  实时数据直接回传给界面
        /// </summary>
        /// <param name="_Batch"></param>
        /// <param name="datas_history"></param>
        [LogTagClass(Title = "刷新界面入口")]
        void SemiTesterService_OnNewData(Model.Batch _Batch, SensorGroupData datas_history, TimeSpan F_AddTime)
        {

            var testTask = new Task(() =>
            {

                if (_Batch == null) return;
                if (_Batch == null) return;
                if (datas_history == null || datas_history.Data.Count == 0) return;

                if (this.SettingModel == null) return;
                if (this.SettingModel.TxtSensorGrougAddress == null) return;

                List<int> addrs = this.SettingModel.Address();
                if (addrs == null || addrs.Count != 2)
                {
                    MessageBox.Show("地址不正确：" + this.SettingModel.TxtSensorGrougAddress);
                    return;
                }

                UIZhuiSuC10 con10 = this.View;
                con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                    RefrehPageInner(_Batch, datas_history, F_AddTime, con10);
                });
                 
            });
            testTask.Start();


        }

        [LogTagClass(Title = "刷新界面")]
        void RefrehPageInner(Model.Batch _Batch, SensorGroupData datas_history, TimeSpan F_AddTime,UIZhuiSuC10 con10) {
            Stopwatch watcher = new Stopwatch();
            watcher.Start();
            con10.UIZhuiSuC10Chart.SetDataRealTime(_Batch, datas_history);
            ULogger.Info("刷新Chart结束 :" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            con10.SetData(_Batch, datas_history, F_AddTime);
            UIZhuiSuC10Model.Instance.SettingModel.TxtCurrentSpan = datas_history.F_AddTime;//默认按照最新时间分级 
            UIZhuiSuC10Model.Instance.SettingModel.TxtCurrentSpanLatest = datas_history.F_AddTime;//默认按照最新时间分级 
            ULogger.Info("刷新其他结束 :" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            watcher.Stop();

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
                //DataTable tableSel = new DataTable("选择数据");

                tableAll.Columns.Add("时间", typeof(double));
                //tableSel.Columns.Add("时间", typeof(double));
                foreach (var item in ECService.BatchSensorDictionary.Values)
                {
                    tableAll.Columns.Add(item.PosString, typeof(double));
                }

                //foreach (var item in ECService.BatchSensorDictionary.Values)
                //{
                //    tableSel.Columns.Add(item.PosString, typeof(double));
                //}

                foreach (TimeSpan ts in ECService.BatchSensorDataDictionary.Keys)
                {
                    DataRow dr = tableAll.NewRow();
                    dr["时间"] = double.Parse(ts.TotalSeconds.ToString("f2"));
                    foreach (PosEnum sid in ECService.BatchSensorDataDictionary[ts].Data.Keys)
                    {
                        SensorDataItem sd = ECService.BatchSensorDataDictionary[ts].Data[sid];
                        Sensor s = ECService.BatchSensorDictionary[sd.F_SensorId];
                        string pos = s.PosString;
                        dr[pos] = sd.F_LoadDataValue;
                    }
                    tableAll.Rows.Add(dr);
                }

                //foreach (TimeSpan ts in ECService.BatchSensorDataDictionary.Keys)
                //{
                //    DataRow dr = tableSel.NewRow();
                //    dr["时间"] = double.Parse(ts.TotalSeconds.ToString("f2"));
                //    foreach (var sid in ECService.BatchSensorDataDictionary[ts].Keys)
                //    {
                //        Sensor s = ECService.BatchSensorDictionary[sid];
                //        SensorData sd = ECService.BatchSensorDataDictionary[ts][sid];
                //        string pos = s.PosString;
                //        dr[pos] = sd.F_LoadDataValue;
                //    }
                //    tableSel.Rows.Add(dr);
                //}

                DataTable tableKey = new DataTable("时间点");
                tableKey.Columns.Add("位置");
                tableKey.Columns.Add("最新数据", typeof(double));
                int[] Seconds = this.SettingModel.TxtTestTimePointsArr;
                foreach (var item in Seconds)
                {
                    tableKey.Columns.Add(string.Format("第{0}秒", item), typeof(double));
                }

                Dictionary<int, MKSS.Model.SensorGroupData> dss = UIZhuiSuC10Model.Instance.ECService.GetDataRange(Seconds);
                foreach (C10GridRow item in this.View.UIZhuiSuC10Grid.BasdeData)
                {
                    DataRow dr = tableKey.NewRow();
                    dr["位置"] = item.WEIZHI;
                    try
                    {
                        
                        if(!string.IsNullOrEmpty(item.XINHAO_AD) && item.XINHAO_AD!="-") dr["最新数据"] = string.IsNullOrEmpty(item.XINHAO_AD) ? 0 : double.Parse(item.XINHAO_AD);
                        
                        for (int x = 0; x < Seconds.Length; x++)
                        {
                            if (string.IsNullOrEmpty(item.SensorId)) continue;
                            if (!dss.ContainsKey(Seconds[x]) || !dss[Seconds[x]].Data.ContainsKey(item.PosEnum) || dss[Seconds[x]].Data[item.PosEnum] == null) continue;
                            dr[2 + x] = dss[Seconds[x]].Data[item.PosEnum].F_LoadDataValue;
                        }
                        tableKey.Rows.Add(dr);

                    }
                    catch (Exception ed)
                    {
                        throw ed;
                    }
                    
                }


                DataSet ds = new DataSet();
                //ds.Tables.Add(tableSel);
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
