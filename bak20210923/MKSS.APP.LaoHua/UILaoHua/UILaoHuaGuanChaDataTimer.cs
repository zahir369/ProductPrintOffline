using System.Collections.Generic;
using System.Linq;
using System;
using MKSS.APP.LaoHua.Util;
using MKSS.Model;
using MKSS.Util.Log;
using System.Data;
using System.Windows.Threading;
using System.Threading;

namespace MKSS.APP.LaoHua
{

    [LogTagClass(Title = "定时刷新老化数据")]
	public class UILaoHuaGuanChaDataTimer {
		public Batch Batch;
		PageBoardTable Table ;
		UILaoHuaGuanChaData Data;
		UILaoHuaGuanChaC10 C10; 
		public static Dictionary<long, BatchStaData> Cache = new Dictionary<long, BatchStaData>();

		#region 定时刷新统计表，plc 连接
		public void TimerStart(UILaoHuaGuanChaData data, Batch _Batch, PageBoardTable table, UILaoHuaGuanChaC10 con10)
		{
			Data = data;
			Batch = _Batch;
			Table = table;
			C10 = con10;

			if (_Batch.EnumAgingStatus == EnumAgingStatus.InAging)
			{
				ULogger.Log("启动统计表刷新统计任务......");
				TimerStop();
				TimerStartInner();//手动执行一次
				ULogger.Log("启动统计表刷新统计任务 Sucess......");
			}
			else {

				

			}
			

		}

		bool ReadFromXmlIng = false;
		public void ReadFromXml(UILaoHuaGuanChaData data, Batch _Batch, PageBoardTable table, UILaoHuaGuanChaC10 con10)
		{
			if (ReadFromXmlIng) return;

			try
			{
				ReadFromXmlIng = true;
				Data = data;
				Batch = _Batch;
				C10 = con10;
				Table = table;
				//直接读取结果
				List<Sensor> _SensorAll = this.Data.SensorServices.QuerySql(string.Format("select * from pd_sensor where F_BatchId =" + _Batch.F_BatchId)).Result;
				BatchWithXml _BatchXml = this.Data.SensorServices.BaseDal.Db.Queryable<BatchWithXml>().Where(w => w.F_BatchId == _Batch.F_BatchId).First();
				BatchStaData sta = BatchStaData.FromXml(_Batch, _BatchXml, _SensorAll);
				foreach (var x1 in this.Table.ProductModelGroupTable.Values)
				{
					foreach (var item1 in x1.ProductTable)
					{
						if (!string.IsNullOrEmpty(item1.SensorId) && sta.Ranges.ContainsKey(item1.SensorId))
						{
							if (sta.Ranges.ContainsKey(item1.SensorId))
							{
								List<BatchStaRange> dr = sta.Ranges[item1.SensorId];
								foreach (var item in dr)
								{
									item1.Ranges[item.Type].Min = item.Min;
									item1.Ranges[item.Type].Max = item.Max;
									item1.Parent.Value = item.Value.Value;
								}
							}

						}
					}
				}


				//con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
				//	con10.SetData(this.Table.ProductModelGroupTable.Values.ToList(), Batch);
				//});
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally {

				ReadFromXmlIng = false;
			}
			

		}

		public void TimerStop()
		{
			ULogger.Log("停止刷新统计任务......");
			{

				if (TimerStartInnerThread != null)
				{
					try
					{
						TimerStartInnerThread.Abort();
					}
					catch (Exception)
					{

					}
					try
					{
						TimerStartInnerThread = null;
					}
					catch (Exception)
					{
					}
				}
				 
			}

			ULogger.Log("停止刷新统计任务 Sucess......");
		}

		System.Threading.Thread TimerStartInnerThread = null;
		private void TimerStartInner( )
		{

			ULogger.Log("定时统计 ......");

			if (StartBackGroundPatrolReportJobThreadInnerProcessing)
			{
				ULogger.Log("正忙，定时刷新退出......");
				return;
			}

			ULogger.Log("创建后台统计任务......");
			try
			{

				if (TimerStartInnerThread != null)
				{

					try
					{
						TimerStartInnerThread.Abort();
					}
					catch (Exception)
					{

					}

					try
					{
						TimerStartInnerThread = null;
					}
					catch (Exception)
					{

					}

				}

				TimerStartInnerThread = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(StartBackGroundPatrolReportJobThreadInner));//创建线程
				TimerStartInnerThread.Start( );
				ULogger.Log("后台统计任务已创建......");

			}
			catch (Exception ex)
			{
				ULogger.Error(ex);
			}
		}

		bool StartBackGroundPatrolReportJobThreadInnerProcessing { get; set; }
		 
		/// <summary>
		///   后台生成报表
		/// </summary>
		/// <param name="target"></param>
		void StartBackGroundPatrolReportJobThreadInner(object target)
		{ 

			if (StartBackGroundPatrolReportJobThreadInnerProcessing)
			{
				Table.status = sta_job_status.statistcing;
				ULogger.Log("正忙，任务退出......");
				return;
			}
			try
			{

				UILaoHuaGuanChaC10 con10 = this.C10;
				con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
					con10.ProgressNow.Visibility = System.Windows.Visibility.Visible; 
				});
				

				StartBackGroundPatrolReportJobThreadInnerProcessing = true;
				 
				Table.status = sta_job_status.statistcing;
				ULogger.Log("开始统计"+ Batch.F_BatchId + "......" );
				if (this.Table!=null && this.Table.ProductModelGroupTable.Count>0 && this.Table.ProductModelGroupTable[0].ProductTable.Count>0) {
					
					Dictionary<TimeSpan, PageSensorRange> range = this.Table.ProductModelGroupTable[0].ProductTable[0].Parent.Ranges;

					string sql1 = "SELECT distinct t.F_DataValue,t.F_SensorId,t.F_AddTime FROM (SELECT F_SensorId,max(F_AddTime) as F_AddTime FROM `pd_sensordata_" + Batch.F_BatchId + "` where F_DataValue>170 GROUP BY F_SensorId) a LEFT JOIN `pd_sensordata_" + Batch.F_BatchId + "` t ON t.F_SensorId=a.F_SensorId and t.F_AddTime=a.F_AddTime";
					DataTable tab1 = this.Data.BatchServices.QueryTable(sql1).Result;
					Dictionary<string, DataRow> dic1 = new Dictionary<string, DataRow>();
					for (int i = 0; i < tab1.Rows.Count; i++)
					{
						DataRow dr = tab1.Rows[i];
						string F_SensorId = dr["F_SensorId"] + "";
                        try
                        {
							dic1.Add(F_SensorId, dr);
						}
                        catch (Exception ex )
                        {
							throw ex;
                        } 
					}
					int addd = 0;
					foreach (var x1 in this.Table.ProductModelGroupTable.Values)
					{
						foreach (var item1 in x1.ProductTable)
						{
							if (!string.IsNullOrEmpty(item1.SensorId) && dic1.ContainsKey(item1.SensorId))
							{
								DataRow dr = dic1[item1.SensorId];
								item1.Parent.Value = Convert.ToInt32(dr["F_DataValue"]);
								item1.Parent.RefreshTime = Convert.ToDateTime(dr["F_AddTime"]);
								foreach (var item in item1.Parent.Ranges.Keys)
								{
									PageSensorRange xxs = item1.Parent.Ranges[item];
									addd++;
									//if (xxs.Min>0 && xxs.Min > item1.Parent.Value) xxs.Min = item1.Parent.Value.Value;
									//if (xxs.Max < item1.Parent.Value) xxs.Max = item1.Parent.Value.Value;
								}
							}
						}
					}


					var listr = range.Keys.ToList();
                    for (int r = 0; r < listr.Count; r++)
                    {
						TimeSpan span = listr[r];

						DateTime _now = DateTime.Now;
						if (Batch.EnumAgingStatus == EnumAgingStatus.Finished) _now = Batch.F_AgingEndTimeActual;
						string from = (_now - span).ToString("yyyy-MM-dd HH:mm:ss");
						string to = _now.ToString("yyyy-MM-dd HH:mm:ss");//F_DataValue<>0 and F_DataValue<>170 and 
						string sql = "SELECT  F_SensorId,max(F_DataValue) MIN,min(F_DataValue) MAX FROM `pd_sensordata_" + Batch.F_BatchId + "`"
							+ " where F_DataValue<>0 and F_DataValue<>170 and F_AddTime>='" + from + "' and F_AddTime<'" + to + "' group by F_SensorId";
						DataTable tab = this.Data.BatchServices.QueryTable(sql).Result;
						Dictionary<string, DataRow> dic = new Dictionary<string, DataRow>();
                        for (int i = 0; i < tab.Rows.Count; i++)
                        {
							DataRow dr = tab.Rows[i];
							string F_SensorId = dr["F_SensorId"] + "";
							dic.Add(F_SensorId, dr); 
						}

                        foreach (var x1 in this.Table.ProductModelGroupTable.Values)
                        {
							foreach (var item1 in x1.ProductTable)
							{
								if (!string.IsNullOrEmpty(item1.SensorId)  && dic.ContainsKey(item1.SensorId))
								{
									DataRow dr = dic[item1.SensorId];
									item1.Ranges[span].Min = Convert.ToInt32(dr["MIN"]);
									item1.Ranges[span].Max = Convert.ToInt32(dr["MAX"]);
								}
							}
						}

						con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
							con10.SetData(this.Table.ProductModelGroupTable.Values.ToList(), Batch);
						});

					}

					addd++;

				}
				//SELECT t.* FROM (SELECT address,max(create_time) as create_time FROM test GROUP BY address) a LEFT JOIN test t ON t.address=a.address and t.create_time=a.create_time
				
				ULogger.Log("统计完成......" );
				Table.status = sta_job_status.finished;

				con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
					con10.SetData(this.Table.ProductModelGroupTable.Values.ToList(), Batch);
					con10.ProgressNow.Visibility = System.Windows.Visibility.Hidden;
				});

			}
			catch (Exception ex)
			{
				ULogger.Error(ex);
				Table.status = sta_job_status.finished;
			}
			finally
			{

				StartBackGroundPatrolReportJobThreadInnerProcessing = false;

			}

		}

		 

		#endregion
	}


	/// <summary>
	///  统计状态
	/// </summary>
	public enum sta_job_status
	{
		/// <summary>
		///  初始化
		/// </summary>
		init = 0,
		/// <summary>
		///  添加站点
		/// </summary>
		adding_stations = 1,
		/// <summary>
		///  解析对应传感器
		/// </summary>
		parsing_configs = 2,
		/// <summary>
		///  查询数据库表
		/// </summary>
		statistcing = 3,
		/// <summary>
		///  完成
		/// </summary>
		finished = 4
	}
}
