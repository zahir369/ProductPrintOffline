using System.Collections.Generic;
using System.Linq;
using System;
using MKSS.APP.ZhuiSu.Util;
using MKSS.Model;
using MKSS.Util.Log;
using System.Data;
using System.Windows.Threading;
using System.Threading;
using MKSS.APP.ZhuiSu.UserControls;

namespace MKSS.APP.ZhuiSu
{
    [LogTagClass(Title = "定时刷新检测数据")]
	public class UIZhuiSuDataTimer {
		public Batch Batch;
		PageBoardTable Table ;
		UIZhuiSuData Data;
		UIZhuiSuC64 C10;
		#region 定时刷新统计表， 
		public void TimerStart(UIZhuiSuData data, Batch _Batch, PageBoardTable table, UIZhuiSuC64 con10)
		{
			Data = data;
			Batch = _Batch;
			Table = table;
			C10 = con10;
			ULogger.Log("启动统计表刷新统计任务......");
			TimerStop();
			TimerStartInner( );//手动执行一次
			ULogger.Log("启动统计表刷新统计任务 Sucess......");

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


			UIZhuiSuC64 con10 = this.C10;
			try
			{


				WaitWindow.ShowWindow("正在统计", "正在获取数据，请稍候......", con10);


				StartBackGroundPatrolReportJobThreadInnerProcessing = true;
				 
				Table.status = sta_job_status.statistcing;
				ULogger.Log("开始统计"+ Batch.F_BatchId + "......" );
				WaitWindow.ShowWindow("正在统计", "开始统计" + Batch.F_BatchId + "......", con10);
				if (this.Table!=null && this.Table.ProductModelGroupTable.Count>0 && this.Table.ProductModelGroupTable[0].ProductTable.Count>0) {
					
					Dictionary<TimeSpan, PageSensorRange> range = this.Table.ProductModelGroupTable[0].ProductTable[0].Parent.Ranges;

					WaitWindow.ShowWindow("正在统计", "开始获取最新数据......", con10);
					string sql1 = "SELECT t.F_DataValue,t.F_SensorId FROM (SELECT F_SensorId,max(F_AddTime) as F_AddTime FROM `pd_sensordata_" + Batch.F_BatchId + "` where F_DataValue>170 GROUP BY F_SensorId) a LEFT JOIN `pd_sensordata_" + Batch.F_BatchId + "` t ON t.F_SensorId=a.F_SensorId and t.F_AddTime=a.F_AddTime";
					DataTable tab1 = this.Data.BatchServices.QueryTable(sql1).Result;
					Dictionary<string, DataRow> dic1 = new Dictionary<string, DataRow>();
					for (int i = 0; i < tab1.Rows.Count; i++)
					{
						DataRow dr = tab1.Rows[i];
						string F_SensorId = dr["F_SensorId"] + "";
						dic1.Add(F_SensorId, dr);
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

					con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
						con10.SetData(this.Table.ProductModelGroupTable.Values.ToList(), Batch);
					});

					var listr = range.Keys.ToList();
                    for (int r = 0; r < listr.Count; r++)
                    {
						TimeSpan span = listr[r];

						WaitWindow.ShowWindow("正在统计", "开始获取" + span.TotalSeconds + "秒数据......", con10);

						DateTime _start = Batch.F_AgingStartTime;
						double from = span.TotalSeconds - Batch.F_Intraval * 1.3;
						double to = span.TotalSeconds+ Batch.F_Intraval * 1.3;//F_DataValue<>0 and F_DataValue<>170 and 
						string sql = "SELECT  F_SensorId,avg(F_DataValue) F_DataValue FROM `pd_sensordata_" + Batch.F_BatchId + "`"
							+ " where F_DataValue<>0 and F_DataValue<>170 and F_AddTime>=" + from + " and F_AddTime<" + to + " group by F_SensorId";
						DataTable tab = this.Data.BatchServices.QueryTable(sql).Result;
						Dictionary<string, DataRow> dic = new Dictionary<string, DataRow>();
                        for (int i = 0; i < tab.Rows.Count; i++)
                        {
							DataRow dr = tab.Rows[i];
							string F_SensorId = dr["F_SensorId"] + "";
							dic.Add(F_SensorId, dr); 
						}

						DateTime dtTh = (Batch.F_AgingStartTime + span);//.ToString("yyyy-MM-dd HH:mm:ss");
						string sqlTh = "SELECT  * FROM `pd_env`"
							+ " where F_DateTime<='" + dtTh.ToString("yyyy-MM-dd HH:mm:ss") + "' and F_EndDateTime>'" + dtTh.ToString("yyyy-MM-dd HH:mm:ss") + "' ";
						Env tabTh = this.Data.BatchServices.BaseDal.Db.Queryable<Env>().WhereIF(true,w => w.F_DateTime>= dtTh && w.F_EndDateTime <= dtTh).FirstAsync().Result;

						foreach (var x1 in this.Table.ProductModelGroupTable.Values)
                        {
							foreach (var item1 in x1.ProductTable)
							{
								if (!string.IsNullOrEmpty(item1.SensorId)  && dic.ContainsKey(item1.SensorId))
								{
									DataRow dr = dic[item1.SensorId];
									item1.Ranges[span].Value = Convert.ToInt32(dr["F_DataValue"]);
									if (tabTh != null) { 
										item1.Ranges[span].Temperature = tabTh.F_Temperature;
										item1.Ranges[span].Humidity = tabTh.F_Humidity;
										item1.Ranges[span].HTDateTime = tabTh.F_DateTime;
									}
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
					//con10.ProgressNow.Visibility = System.Windows.Visibility.Hidden;
					WaitWindow.CloseWindow(con10);
				});

			}
			catch (Exception ex)
			{
				ULogger.Error(ex);
				Table.status = sta_job_status.finished;
			}
			finally
			{

				WaitWindow.CloseWindow(con10);
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
