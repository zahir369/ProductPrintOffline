using MKSS.APP.ZhuiSu.UserControls;
using MKSS.APP.ZhuiSu.Util;
using MKSS.APP.ZhuiSuService.MQTT;
using MKSS.Model;
using MKSS.Util.Log;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MKSS.APP.ZhuiSu
{

    public class UIZhuiSuC64Model {

        public static UIZhuiSuC64Model Instance = null;
        /// <summary>
        ///   默认批次，存放开发模式数据
        /// </summary>
        public PageBoardTable BoardCaseTable { get; set; }
        public UIZhuiSuC64 View { get; set; }
        public UIZhuiSuData Data { get { return UIZhuiSuData.Instance; } }
        public UIZhuiSuDataTimer DataTimer = new UIZhuiSuDataTimer();
        ZhuiSuMqttClientConsumer QueryMQTT = ZhuiSuMqttClientConsumer.Instance;
        public UIZhuiSuC64Model() {
            Instance = this;
            BoardCaseTable = new PageBoardTable();
            QueryMQTT.Start();
        }

        public class BoardLayer { 
            public Board Base { get; set; }  
        }

        /// <summary>
        ///  重新计算地址
        /// </summary>
        [LogTagClass(Title = "重新计算表格")]
        public void RefreshAddress(Batch _Batch)
        {

            if (_Batch == null) return;

            DataTable tab1 = UIZhuiSuData.Instance.SensorServices.BaseDal.QueryTable("SELECT F_SerialNO,F_SensorId FROM pd_sensor WHERE F_BatchId=" + _Batch.F_BatchId + "").Result;
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            for (int i = 0; i < tab1.Rows.Count; i++)
            {
                DataRow dr = tab1.Rows[i];
                string F_SensorId = dr["F_SensorId"] + "";
                dic1.Add(F_SensorId, dr["F_SerialNO"] == DBNull.Value ? "" : dr["F_SerialNO"].ToString());
            }
             

            Dictionary<BoardLayer, BoardCase> ret_case_no = new Dictionary<BoardLayer, BoardCase>();
            if (!Data.DBBatchDictionary.ContainsKey(_Batch))
            {
                ULogger.Info(string.Format("已完结批次({0})", _Batch.F_BatchName));
                foreach (Board _Board in Data.GetBoard(_Batch))
                { 
                    BoardCase _BoardCase = Data.DBBoardCaseDictionary.FirstOrDefault(w => w.Value.Contains(_Board)).Key;
                    ret_case_no.Add(new BoardLayer() { Base = _Board }, _BoardCase);
                }
            }
            else {
                foreach (Board _Board in Data.DBBatchDictionary[_Batch])
                { 
                    BoardCase _BoardCase = Data.DBBoardCaseDictionary.FirstOrDefault(w => w.Value.Contains(_Board)).Key;
                    ret_case_no.Add(new BoardLayer() { Base = _Board }, _BoardCase);
                }
            }

            if (ret_case_no.Count > 0)
            {

                Dictionary<BoardLayer, BoardCase> ret_case_no_new = new Dictionary<BoardLayer, BoardCase>();
                foreach (var item in ret_case_no.Keys)
                {
                    if (ret_case_no_new.Count > UIZhuiSuModel.MaxBoardCount) break;
                    ret_case_no_new.Add(item, ret_case_no[item]);
                }
                List<BoardLayer> ret = ret_case_no_new.Keys.ToList();

                for (int i = 0; i < this.BoardCaseTable.ProductModelGroupTable.Count; i++)
                {
                    if (ret_case_no_new.Count - 1 >= i)
                    {
                        this.BoardCaseTable.ProductModelGroupTable[i].Address = ret[i].Base.AddrByte;
                        this.BoardCaseTable.ProductModelGroupTable[i].CaseNo = ret_case_no_new[ret[i]].F_BoardCaseAddress;

                        int r = 0;
                        foreach (PageSensorModel sen in this.BoardCaseTable.ProductModelGroupTable[i].ProductTable)
                        {
                            r++;

                            sen.Parent.SensorId = null;
                            sen.Parent.Reset();
                            sen.Reset();
                            sen.Parent.Value = null;

                            sen.Parent.SensorId = Sensor.CreateCensorId(
                                _Batch.F_BatchId,
                                ret_case_no_new[ret[i]].F_BoardCaseAddress,
                                ret[i].Base.F_FloorNO, r);

                            if (dic1.ContainsKey(sen.Parent.SensorId)) sen.Parent.SerialNO = dic1[sen.Parent.SensorId];

                        }
                    }
                    else
                    {
                        this.BoardCaseTable.ProductModelGroupTable[i].Address = 0;
                        this.BoardCaseTable.ProductModelGroupTable[i].CaseNo = 0;
                        foreach (PageSensorModel sen in this.BoardCaseTable.ProductModelGroupTable[i].ProductTable)
                        {
                            sen.Parent.SensorId = null;
                            sen.Parent.Reset();
                            sen.Reset();
                            sen.Parent.Value = null;
                        }
                    }
                }

                StringBuilder sb = new StringBuilder();
                foreach (var item in ret)
                {
                    if (sb.Length < 6) sb.Append(item + ",");
                }
                ULogger.Info(string.Format("已找到检测柜，通道地址({1}等{0}个)", ret.Count, sb.ToString()));
            }
            else
            {

            }

            QueryMQTT.Query(_Batch);


            UIZhuiSuC64 con10 = this.View;
            if (con10 != null)
            {
                con10.SetData(this.BoardCaseTable.ProductModelGroupTable.Values.ToList(), _Batch);
            }


        }

        public void DataTimerRefresh(Batch _Batch)
        {

            if (_Batch == null) return;

            UIZhuiSuC64 con10 = this.View;
            if (DataTimer.Batch == null || (
                DataTimer.Batch != null )
            )
            {
                DataTimer.TimerStop();
                DataTimer.TimerStart(Data, _Batch, BoardCaseTable, con10);
            }

        }

        internal void InitPage(UIZhuiSuC64 uIZhuiSuGuanChaC10)
        {
            this.View = uIZhuiSuGuanChaC10;
        }


        public void ExportExcel(string path, Batch Batch)
        {

            try
            {

                string sql_s = " F_BatchId='" + Batch.F_BatchId + "' order by F_SensorId ";
                List<Sensor> tabs = UIZhuiSuData.Instance.SensorServices.Query(sql_s).Result;
                Dictionary<string, Sensor> dic_sens = tabs.ToDictionary(w => w.F_SensorId, w => w);

                DataRow dr = null;
                double F_AddTime_pre = -1;


                WaitWindow.ShowWindow("正在导出", "正在读取"+ Batch.F_BatchName + "，请稍候......", this.View);
                string sql1 = "SELECT t.F_DataValue,t.F_AddTime,t.F_SensorId FROM `pd_sensordata_" + Batch.F_BatchId + "` t order by t.F_AddTime ";
                DataTable tab1 = UIZhuiSuData.Instance.BatchServices.QueryTable(sql1).Result;

                DateTime dtTh = Batch.F_AgingStartTime;
                DateTime dtThEnd = Batch.F_AgingEndTime;
                List<Env> listEnv = UIZhuiSuData.Instance.SensorDataServices.BaseDal.Db.Queryable<Env>().WhereIF(true, w => w.F_DateTime >= dtTh && w.F_DateTime <= dtThEnd && w.F_Humidity > 0).OrderBy("F_DateTime").ToListAsync().Result;
                 

                WaitWindow.ShowWindow("正在导出", "正在生成A板详细数据，请稍候......", this.View);
                DataTable tableAllA = new DataTable("A板详细数据");
                tableAllA.Columns.Add("时间", typeof(double));
                tableAllA.Columns.Add("温度", typeof(double));
                tableAllA.Columns.Add("湿度", typeof(double));
                foreach (Sensor sen in tabs)
                {
                    if (!sen.IsA) continue;
                    tableAllA.Columns.Add(sen.PosString, typeof(double));
                }
                for (int i = 0; i < tab1.Rows.Count; i++)
                {
                    DataRow dre = tab1.Rows[i];
                    string F_SensorId = dre["F_SensorId"] + "";
                    double F_AddTime = Convert.ToDouble(dre["F_AddTime"]);
                    double F_DataValue = Convert.ToDouble(dre["F_DataValue"]);
                    
                    if (!dic_sens[F_SensorId].IsA) continue;
                    Sensor _Sensor = dic_sens[F_SensorId];
                    if (F_AddTime_pre != F_AddTime)
                    {
                        if (dr != null)
                        {
                            tableAllA.Rows.Add(dr);
                        }
                        dr = tableAllA.NewRow();
                        dr["时间"] = F_AddTime;
                        F_AddTime_pre = F_AddTime;
                    }
                    dr[_Sensor.PosString] = F_DataValue;
                    Env env = listEnv.FirstOrDefault(env => (env.F_DateTime - Batch.F_AgingStartTime).TotalSeconds >= F_AddTime);
                    if (env != null)
                    {
                        dr["温度"] = env.F_Temperature;
                        dr["湿度"] = env.F_Humidity;
                    }
                }
                if (dr != null)
                {
                    tableAllA.Rows.Add(dr);
                }
                dr = null;
                F_AddTime_pre = -1;



                WaitWindow.ShowWindow("正在导出", "正在生成B板详细数据，请稍候......", this.View);
                DataTable tableAllB = new DataTable("B板详细数据");
                tableAllB.Columns.Add("时间", typeof(double));
                tableAllB.Columns.Add("温度", typeof(double));
                tableAllB.Columns.Add("湿度", typeof(double));
                foreach (Sensor sen in tabs)
                {
                    if (sen.IsA) continue;
                    tableAllB.Columns.Add(sen.PosString, typeof(double));
                }
                for (int i = 0; i < tab1.Rows.Count; i++)
                {
                    DataRow dre = tab1.Rows[i];
                    string F_SensorId = dre["F_SensorId"] + "";
                    double F_AddTime = Convert.ToDouble(dre["F_AddTime"]);
                    double F_DataValue = Convert.ToDouble(dre["F_DataValue"]);
                    if (dic_sens[F_SensorId].IsA) continue;
                    Sensor _Sensor = dic_sens[F_SensorId];
                    if (F_AddTime_pre != F_AddTime)
                    {
                        if (dr != null)
                        {
                            tableAllB.Rows.Add(dr);
                        }
                        dr = tableAllB.NewRow();
                        dr["时间"] = F_AddTime;
                        F_AddTime_pre = F_AddTime;
                    }
                    dr[_Sensor.PosString] = F_DataValue;
                    Env env = listEnv.FirstOrDefault(env => (env.F_DateTime - Batch.F_AgingStartTime).TotalSeconds >= F_AddTime);
                    if (env != null)
                    {
                        dr["温度"] = env.F_Temperature;
                        dr["湿度"] = env.F_Humidity;
                    }
                }
                if (dr != null)
                {
                    tableAllB.Rows.Add(dr);
                }
                dr = null;
                F_AddTime_pre = -1;



                //DataTable tableSel = new DataTable("选择数据");
                //tableSel.Columns.Add("时间", typeof(double));
                //List<string> sel = this.View.UIZhuiSuC64Grid.SelectSensorIds;
                //if (sel != null && sel.Count > 0) {
                //    foreach (var item in sel)
                //    {
                //        tableSel.Columns.Add(dic_sens[item].PosString, typeof(double));
                //    }
                //    for (int i = 0; i < tab1.Rows.Count; i++)
                //    {
                //        DataRow dre = tab1.Rows[i];
                //        string F_SensorId = dre["F_SensorId"] + "";
                //        if (!dic_sens.ContainsKey(F_SensorId) || !sel.Contains(F_SensorId)) continue;
                //        double F_AddTime = Convert.ToDouble(dre["F_AddTime"]);
                //        double F_DataValue = Convert.ToDouble(dre["F_DataValue"]);
                //        Sensor _Sensor = dic_sens[F_SensorId];
                //        if (F_AddTime_pre != F_AddTime)
                //        {
                //            if (dr != null)
                //            {
                //                tableSel.Rows.Add(dr);
                //                F_AddTime_pre = F_AddTime;
                //                dr["时间"] = F_AddTime;
                //            }
                //            dr = tableSel.NewRow();
                //        }
                //        dr[_Sensor.PosString] = F_DataValue;
                //    }
                //    if (dr != null)
                //    {
                //        tableSel.Rows.Add(dr);
                //    }
                //    dr = null;
                //    F_AddTime_pre = -1;
                //}



                WaitWindow.ShowWindow("正在导出", "正在生成取样点数据，请稍候......", this.View);
                DataTable tableKey = new DataTable("取样点数据");
                tableKey.Columns.Add("位置");
                tableKey.Columns.Add("最新数据", typeof(double));
                int[] Seconds = this.View.UIZhuiSuC64Setting1.TxtTestTimePointsArr;
                foreach (var item in Seconds)
                {
                    tableKey.Columns.Add(UIZhuiSuAdd.ToMyFormat(TimeSpan.FromSeconds(item)) , typeof(double));
                }

                foreach (C10GridRow item in this.View.UIZhuiSuC64Grid.BasdeData)
                {
                    DataRow drx = tableKey.NewRow();
                    drx["位置"] = item.WEIZHI;

                    double valXINHAO_AD = 0;
                    if (double.TryParse(item.XINHAO_AD, out valXINHAO_AD))
                    {
                        drx["最新数据"] = valXINHAO_AD;
                    }
                    for (int x = 0; x < Seconds.Length; x++)
                    {
                        if (string.IsNullOrEmpty(item.SensorId)) continue;

                        PropertyInfo pro = item.GetType().GetProperty("BD_" + (x + 1));
                        if (pro != null)
                        {
                            object val = pro.GetValue(item, null);
                            if (val != null && !string.IsNullOrEmpty(val.ToString())) {
                                double valxx = 0;
                                if (double.TryParse(pro.GetValue(item, null).ToString(), out valxx)) { 
                                    drx[2 + x] = valxx;
                                }
                            }
                        }
                    }
                    tableKey.Rows.Add(drx);
                }


                WaitWindow.ShowWindow("正在导出", "正在写入EXCEL文件，请稍候......", this.View);
                DataSet ds = new DataSet();
                ds.Tables.Add(tableAllA);
                ds.Tables.Add(tableAllB);
                ds.Tables.Add(tableKey);
                //ds.Tables.Add(tableSel);
                EPPlusHelper.ImportExcel(path, ds);

            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

    }

}
