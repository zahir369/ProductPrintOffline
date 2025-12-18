using MKSS.APP.LaoHua.Util;
using MKSS.APP.LaoHuaService.MQTT;
using MKSS.APP.UserControls;
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

namespace MKSS.APP.LaoHua
{
    public class UILaoHuaGuanChaC10Model {
        public static UILaoHuaGuanChaC10Model Instance = null;
        /// <summary>
        ///   默认批次，存放开发模式数据
        /// </summary>
        public PageBoardTable BoardCaseTable { get; set; }
        public UILaoHuaGuanChaC10 View { get; set; }
        public UILaoHuaGuanChaData Data { get { return UILaoHuaGuanChaData.Instance; } }
        public UILaoHuaGuanChaDataTimer DataTimer = new UILaoHuaGuanChaDataTimer();
        public LaoHuaMqttClientConsumer QueryMQTT = LaoHuaMqttClientConsumer.Instance;
        public UILaoHuaGuanChaC10Model() {
            Instance = this;
            BoardCaseTable = new PageBoardTable();
            Task.Run(delegate ()
            {
                try
                {
                    QueryMQTT.Start();
                }
                catch (Exception)
                {
                }
                finally
                {
                }
            });
        }

        public class BoardLayer { 
            public Board Base { get; set; }
            public int Part { get; set; }
        }

        /// <summary>
        ///  重新计算地址
        /// </summary>
        [LogTagClass(Title = "重新计算表格")]
        public void RefreshAddress(Batch _Batch)
        {


            Task.Run(delegate ()
            {
                try
                {
                    if (_Batch == null) return;
                    Dictionary<BoardLayer, BoardCase> ret_case_no = new Dictionary<BoardLayer, BoardCase>();
                    if (!Data.DBBatchDictionary.ContainsKey(_Batch))
                    {
                        ULogger.Info(string.Format("已完结批次({0})", _Batch.F_BatchName));
                        foreach (Board _Board in Data.GetBoard(_Batch))
                        {
                            List<byte> addrs = _Board.AddrList;
                            BoardCase _BoardCase = Data.DBBoardCaseDictionary.FirstOrDefault(w => w.Value.Contains(_Board)).Key;
                            //地址码，左右对调
                            ret_case_no.Add(new BoardLayer() { Base = _Board, Part = 1 }, _BoardCase);
                            ret_case_no.Add(new BoardLayer() { Base = _Board, Part = 0 }, _BoardCase);
                        }
                    }
                    else
                    {
                        foreach (Board _Board in Data.DBBatchDictionary[_Batch])
                        {
                            List<byte> addrs = _Board.AddrList;
                            BoardCase _BoardCase = Data.DBBoardCaseDictionary.FirstOrDefault(w => w.Value.Contains(_Board)).Key;
                            //地址码，左右对调
                            ret_case_no.Add(new BoardLayer() { Base = _Board, Part = 1 }, _BoardCase);
                            ret_case_no.Add(new BoardLayer() { Base = _Board, Part = 0 }, _BoardCase);
                        }
                    }

                    if (ret_case_no.Count > 0)
                    {

                        Dictionary<BoardLayer, BoardCase> ret_case_no_new = new Dictionary<BoardLayer, BoardCase>();
                        foreach (var item in ret_case_no.Keys)
                        {
                            if (ret_case_no_new.Count > UILaoHuaGuanChaModel.MaxBoardCount) break;
                            ret_case_no_new.Add(item, ret_case_no[item]);
                        }
                        List<BoardLayer> ret = ret_case_no_new.Keys.ToList();

                        for (int i = 0; i < this.BoardCaseTable.ProductModelGroupTable.Count; i++)
                        {
                            if (ret_case_no_new.Count - 1 >= i)
                            {
                                this.BoardCaseTable.ProductModelGroupTable[i].Address = ret[i].Base.AddrList[ret[i].Part];
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
                                        ret[i].Base.F_FloorNO, (ret[i].Part) * 15 + r);
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
                        ULogger.Info(string.Format("已找到老化柜，通道地址({1}等{0}个)", ret.Count, sb.ToString()));
                    }
                    else
                    {

                    }

                    QueryMQTT.Query(_Batch);
                }
                catch (Exception)
                {
                }
                finally
                {
                    //直接刷新的数据，不需要长时间统计的直接显示
                    this.View.Dispatcher.Invoke(new Action(() =>
                    {
                        UILaoHuaGuanChaC10 con10 = this.View;
                        if (con10 != null)
                        {
                            con10.SetData(this.BoardCaseTable.ProductModelGroupTable.Values.ToList(), _Batch);
                        }
                    })); 
                    
                }
            });

        }

        public void DataTimerRefresh(Batch _Batch)
        {

            if (_Batch == null) return;

            UILaoHuaGuanChaC10 con10 = this.View;

            //直接读取统计结果，定时统计放到服务端了 
            DataTimer.ReadFromXml(Data, _Batch, BoardCaseTable, con10);

            /**
            if (DataTimer.Batch == null || (
                DataTimer.Batch != null && DataTimer.Batch.F_BatchId != _Batch.F_BatchId)
            )
            {

            }
            DataTimer.TimerStop();
            DataTimer.TimerStart(Data, _Batch, BoardCaseTable, con10);
            **/

        }

        internal void InitPage(UILaoHuaGuanChaC10 uILaoHuaGuanChaC10)
        {
            this.View = uILaoHuaGuanChaC10;
        }


        public void ExportExcel(string path, Batch _Batch)
        {
            WaitWindow.ShowWindow("正在导出", "正在查询批次" + _Batch.F_BatchName + "数据......", this.View);
            // c.F_BoardCaseAddress 老化柜, b.F_FloorNO 托盘, s.F_SlotNO 位置,
            //DataTable rels = this.Data.BatchServices.QueryTable(
            //    string.Format(
            //    @"
            //        select concat(c.F_BoardCaseAddress,'号柜',b.F_FloorNO,'号盘位置',s.F_SlotNO) 位置,s.F_SerialNO 串号,s.F_DeviceHistoryClock 校时,v.F_DataValue 电压,v.F_AddTime 时间
            //        from pd_sensordata_{0} v 
            //        LEFT JOIN pd_sensor s on s.F_SensorId=v.F_SensorId
            //        LEFT JOIN pd_board b on b.F_BoardId=s.F_BoardId
            //        LEFT JOIN pd_boardcase c on c.F_BoardCaseId=s.F_BoardCaseId
            //        where v.F_DataValue>170
            //        order by v.F_SensorId,v.F_AddTime
            //        ", _Batch.F_BatchId)
            //    ).Result;

            //DataSet ds = rels.DataSet;
            //rels.TableName = "详细数据";

            DataSet ds = new DataSet(); 
            WaitWindow.ShowWindow("正在导出", "正在查询批次" + _Batch.F_BatchName + "统计报告......", this.View);
            DataTable report = new DataTable("统计报告");
            //report.Columns.Add("老化柜",typeof(int));
            //report.Columns.Add("板地址", typeof(int));
            //report.Columns.Add("托盘", typeof(int));
            //report.Columns.Add("位置", typeof(int));
            report.Columns.Add("位置", typeof(string));
            report.Columns.Add("串号", typeof(string));
            report.Columns.Add("校时", typeof(string));
            report.Columns.Add("电压", typeof(int));
            //report.Columns.Add("近10秒", typeof(string));
            //report.Columns.Add("近1分钟", typeof(string));
            report.Columns.Add("近5分钟", typeof(string));
            report.Columns.Add("近15分钟", typeof(string));
            report.Columns.Add("近30分钟", typeof(string));
            report.Columns.Add("近1小时", typeof(string));
            report.Columns.Add("近3小时", typeof(string));
            report.Columns.Add("近6小时", typeof(string));
            report.Columns.Add("近12小时", typeof(string));
            report.Columns.Add("近24小时", typeof(string));
            report.Columns.Add("近72小时", typeof(string));
            for (int b = 0; b < this.BoardCaseTable.ProductModelGroupTable.Count; b++) {
                var tab = this.BoardCaseTable.ProductModelGroupTable[b];
                BoardCase _BoardCase = null;
                Board _Board = null;
                List<Sensor> senList = new List<Sensor>();
                Batch _Batch2 = UILaoHuaGuanChaData.Instance.OfBatch(tab, ref _BoardCase, ref _Board,ref senList);
                if (_BoardCase == null || _Board == null) continue;
                List<PageSensorModel> tables = tab.ProductTable;
                for (int i = 0; i < tables.Count; i++)
                {

                    Sensor _Sensor = senList.FirstOrDefault(w => w.F_SensorId == tables[i].SensorId);
                    DataRow dr = report.NewRow();
                    //dr["老化柜"] = tab.CaseNo;
                    //dr["板地址"] = tab.Address;
                    //dr["托盘"] = _Board.F_FloorNO;
                    //dr["位置"] = tab.Address % 2 == 1 ? (i +1) : (i + 16);
                    dr["位置"] = string.Format("{0}号柜{1}号盘位置{2}", tab.CaseNo, _Board.F_FloorNO, tab.Address % 2 == 1 ? "左侧" + (i + 1) : "右侧" + (i + 1));
                    dr["串号"] = _Sensor == null ? "" : _Sensor.F_SerialNO;
                    dr["校时"] = _Sensor == null ? "" : _Sensor.F_DeviceHistoryClock;

                    dr["电压"] = tables[i].IsEmpSensor ? DBNull.Value : tables[i].Value;
                    //dr["近10秒"] = C10GridRow.DAR(tables[i], new TimeSpan(0, 0, 10));
                    //dr["近1分钟"] = C10GridRow.DAR(tables[i], new TimeSpan(0, 1, 0));
                    dr["近5分钟"] = C10GridRow.DAR(tables[i], new TimeSpan(0, 5, 0));
                    dr["近15分钟"] = C10GridRow.DAR(tables[i], new TimeSpan(0, 15, 0));
                    dr["近30分钟"] = C10GridRow.DAR(tables[i], new TimeSpan(0, 30, 0));
                    dr["近1小时"] = C10GridRow.DAR(tables[i], new TimeSpan(1, 0, 0));
                    dr["近3小时"] = C10GridRow.DAR(tables[i], new TimeSpan(3, 0, 0));
                    dr["近6小时"] = C10GridRow.DAR(tables[i], new TimeSpan(6, 0, 0));
                    dr["近12小时"] = C10GridRow.DAR(tables[i], new TimeSpan(12, 0, 0));
                    dr["近24小时"] = C10GridRow.DAR(tables[i], new TimeSpan(24, 0, 0));
                    dr["近72小时"] = C10GridRow.DAR(tables[i], new TimeSpan(72, 0, 0));
                    report.Rows.Add(dr);

                }
            }
            ds.Tables.Add(report);
            WaitWindow.ShowWindow("正在导出", "正在导出批次" + _Batch.F_BatchName + "EXCEL文件......", this.View);
            EPPlusHelper.ImportExcel(path, ds);
        }



        public void ExportBoardExcel(string path, Batch _Batch, BoardCase _BoardCase,Board _board)
        {
            WaitWindow.ShowWindow("正在导出", "正在查询批次" + _Batch.F_BatchName + "数据......", this.View);
            string prefix = Sensor.CreateCensorPrefix(_Batch.F_BatchId, _BoardCase.F_BoardCaseAddress, _board.F_FloorNO);
            //c.F_BoardCaseAddress 老化柜, b.F_FloorNO 托盘, s.F_SlotNO 位置,
            DataTable rels = this.Data.BatchServices.QueryTable(
               string.Format(
               @"
                    select concat(c.F_BoardCaseAddress,'号柜',b.F_FloorNO,'号盘位置',s.F_SlotNO) 位置,s.F_SerialNO 串号,s.F_DeviceHistoryClock 校时,v.F_DataValue 电压,v.F_AddTime 时间
                    from pd_sensordata_{0} v 
                    LEFT JOIN pd_sensor s on s.F_SensorId=v.F_SensorId
                    LEFT JOIN pd_board b on b.F_BoardId=s.F_BoardId
                    LEFT JOIN pd_boardcase c on c.F_BoardCaseId=s.F_BoardCaseId
                    where v.F_DataValue>170 and v.F_SensorId like '" + prefix + @"%'
                    order by v.F_SensorId,v.F_AddTime
                    ", _Batch.F_BatchId)
               ).Result;

            DataSet ds = rels.DataSet;
            rels.TableName = "详细数据";
             
            WaitWindow.ShowWindow("正在导出", "正在导出批次" + _Batch.F_BatchName + "EXCEL文件......", this.View);
            EPPlusHelper.ImportExcel(path, ds);
        }


        public void ExportBoardExcelV2(string path, Batch _Batch, BoardCase _BoardCase, Board _board, PageBoardItem src)
        {
            bool large16 = src.Address % 2 == 0;
            WaitWindow.ShowWindow("正在导出", "正在查询批次" + _Batch.F_BatchName + "数据......", this.View);
            string prefix = Sensor.CreateCensorPrefix(_Batch.F_BatchId, _BoardCase.F_BoardCaseAddress, _board.F_FloorNO);
            //c.F_BoardCaseAddress 老化柜, b.F_FloorNO 托盘, s.F_SlotNO 位置,
            string large16Str = large16 ? "":"";
            DataTable rels = this.Data.BatchServices.QueryTable(
               string.Format(
               @"
                    select s.F_SlotNO,v.F_DataValue,v.F_AddTime
                    from pd_sensordata_{0} v 
                    LEFT JOIN pd_sensor s on s.F_SensorId=v.F_SensorId
                    LEFT JOIN pd_board b on b.F_BoardId=s.F_BoardId
                    LEFT JOIN pd_boardcase c on c.F_BoardCaseId=s.F_BoardCaseId
                    where v.F_SensorId like '" + prefix + @"%' 
                    order by v.F_SensorId,v.F_AddTime
                    ", _Batch.F_BatchId)
               ).Result;
 

            DataTable dt = new DataTable("详细数据");
            DataSet ret = new DataSet();
            ret.Tables.Add(dt);
            Dictionary<DateTime, DataRow> Cache = new Dictionary<DateTime, DataRow>();
            dt.Columns.Add("Time", typeof(DateTime));
            for (int i = 0; i < rels.Rows.Count; i++)
            {
                DataRow dr = rels.Rows[i];
                int F_SlotNO = Convert.ToInt32(dr["F_SlotNO"]);
                int F_DataValue = Convert.ToInt32(dr["F_DataValue"]);
                DateTime F_AddTime = Convert.ToDateTime(dr["F_AddTime"]);
                if (large16 && F_SlotNO < 16) continue;
                if (!large16 && F_SlotNO >= 16) continue;
                if (!dt.Columns.Contains(F_SlotNO + "")) {
                    dt.Columns.Add(F_SlotNO + "",typeof(int));
                }
                if (!Cache.ContainsKey(F_AddTime)) {
                    DataRow drn = dt.NewRow();
                    dt.Rows.Add(drn);
                    Cache.Add(F_AddTime, drn);
                    drn["Time"] = F_AddTime;
                }
                Cache[F_AddTime][F_SlotNO + ""] = F_DataValue;
            }

            WaitWindow.ShowWindow("正在导出", "正在导出批次" + _Batch.F_BatchName + "EXCEL文件......", this.View);
            EPPlusHelper.ImportExcel(path, ret);
        }


        public void ExportBoardExcelV2ALL(string path, Batch _Batch)
        {

            try
            {
                DataSet ret = new DataSet();
                Dictionary<string, DataTable> dicDataTable = new Dictionary<string, DataTable>();
                foreach (var src in UILaoHuaGuanChaC10Model.Instance.BoardCaseTable.ProductModelGroupTable.Values)
                {
                    BoardCase _BoardCase = null;
                    Board _board = null;
                    Batch _BatchXX = UILaoHuaGuanChaData.Instance.OfBatch(src, ref _BoardCase, ref _board);

                    if (_board == null) continue; 

                    bool large16 = src.Address % 2 == 0;
                    string xname = String.Format("{0}#{1}{2}", _BoardCase.F_BoardCaseAddress, _board.F_FloorNO, large16? "右侧":"左侧");
                    WaitWindow.ShowWindow("正在查询", "正在查询" + xname + "的老化数据数据......", this.View);
                    string prefix = Sensor.CreateCensorPrefix(_Batch.F_BatchId, _BoardCase.F_BoardCaseAddress, _board.F_FloorNO);

                    DataTable rels = null;
                    if (!dicDataTable.ContainsKey(prefix))
                    {
                        //c.F_BoardCaseAddress 老化柜, b.F_FloorNO 托盘, s.F_SlotNO 位置,
                        rels = this.Data.BatchServices.QueryTable(
                           string.Format(
                           @"
                                select s.F_SlotNO,v.F_DataValue,v.F_AddTime
                                from pd_sensordata_{0} v 
                                LEFT JOIN pd_sensor s on s.F_SensorId=v.F_SensorId
                                LEFT JOIN pd_board b on b.F_BoardId=s.F_BoardId
                                LEFT JOIN pd_boardcase c on c.F_BoardCaseId=s.F_BoardCaseId
                                where v.F_SensorId like '" + prefix + @"%' 
                                order by v.F_SensorId,v.F_AddTime
                            ", _Batch.F_BatchId)
                                    ).Result;
                        dicDataTable.Add(prefix, rels);
                    }
                    else {
                        rels=dicDataTable[prefix];
                    }
                    


                    DataTable dt = new DataTable(xname);
                    ret.Tables.Add(dt);
                    Dictionary<DateTime, DataRow> Cache = new Dictionary<DateTime, DataRow>();
                    dt.Columns.Add("Time", typeof(DateTime));
                    for (int i = 0; i < rels.Rows.Count; i++)
                    {
                        DataRow dr = rels.Rows[i];
                        int F_SlotNO = Convert.ToInt32(dr["F_SlotNO"]);
                        int F_DataValue = Convert.ToInt32(dr["F_DataValue"]);
                        DateTime F_AddTime = Convert.ToDateTime(dr["F_AddTime"]);
                        if (large16 && F_SlotNO < 16) continue;
                        if (!large16 && F_SlotNO >= 16) continue;
                        if (!dt.Columns.Contains(F_SlotNO + ""))
                        {
                            dt.Columns.Add(F_SlotNO + "", typeof(int));
                        }
                        if (!Cache.ContainsKey(F_AddTime))
                        {
                            DataRow drn = dt.NewRow();
                            dt.Rows.Add(drn);
                            Cache.Add(F_AddTime, drn);
                            drn["Time"] = F_AddTime;
                        }
                        Cache[F_AddTime][F_SlotNO + ""] = F_DataValue;
                    }
                     
                }

                WaitWindow.ShowWindow("正在存储", "正在存储批次" + _Batch.F_BatchName + "EXCEL文件......", this.View);
                EPPlusHelper.ImportExcel(path, ret);
            }
            catch (Exception ex)
            {
                WaitWindow.CloseWindow( this.View);
                MessageBox.Show(ex.Message);
            }
            
        }


    }

}
