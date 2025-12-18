using System.Collections.Generic;
using System.Linq;
using System;
using MKSS.Model;
using MKSS.Util.Log;
using System.Data;
using System.Threading;
using MKSS.Services;

namespace MKSS.APP.LaoHua
{

    [LogTagClass(Title = "定时统计老化数据")]
	public class LaoHuaGuanChaDataTimer
	{
		public static Dictionary<long, BatchStaData> Cache = new Dictionary<long, BatchStaData>();


		public Batch Batch;
		public BatchStaData Table;
		BatchServices _BatchServices = new BatchServices();
		System.Threading.Thread TimerStartInnerThread = null;
		#region 定时刷新统计表，plc 连接
		public void TimerStart(Batch _Batch,List<Sensor> sens)
		{
			Batch = _Batch;
			if (!Cache.ContainsKey(Batch.F_BatchId))
			{
				BatchWithXml _BatchXml = this._BatchServices.BaseDal.Db.Queryable<BatchWithXml>().Where(w => w.F_BatchId == _Batch.F_BatchId).First();
				Cache.Add(Batch.F_BatchId, BatchStaData.FromXml(_Batch, _BatchXml, sens));
			}

			Table = Cache[Batch.F_BatchId];
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

				

				StartBackGroundPatrolReportJobThreadInnerProcessing = true;
				 
				Table.status = sta_job_status.statistcing;
				ULogger.Log("开始统计"+ Batch.F_BatchId + "......" );
				if (this.Table!=null && this.Table.Ranges.Count>0 && this.Table.Ranges.FirstOrDefault().Value.Count>0) {


					ULogger.Log("开始统计" + Batch.F_BatchId + " 最新值#############################");
					List<BatchStaRange> rss = this.Table.Ranges.FirstOrDefault().Value;
                    foreach (var ran in this.Table.Ranges)
                    {
						foreach (var item in ran.Value)
						{
							item.Max = 0;
							item.Min = 0;
							item.Value.Value = 0;
							item.Value.ValueTime = DateTime.MinValue;
						}
					}
                    
					string sql1 = "SELECT distinct t.F_DataValue,t.F_SensorId,t.F_AddTime FROM (SELECT F_SensorId,max(F_AddTime) as F_AddTime FROM `pd_sensordata_" + Batch.F_BatchId + "` where F_DataValue>170 GROUP BY F_SensorId) a LEFT JOIN `pd_sensordata_" + Batch.F_BatchId + "` t ON t.F_SensorId=a.F_SensorId and t.F_AddTime=a.F_AddTime";
					DataTable tab1 = _BatchServices.QueryTable(sql1).Result;
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
					 
					foreach (var F_SensorId in this.Table.Ranges.Keys)
					{
						var x1 = this.Table.Ranges[F_SensorId];
						foreach (BatchStaRange item1 in x1)
						{
							if (!string.IsNullOrEmpty(F_SensorId) && dic1.ContainsKey(F_SensorId))
							{
								DataRow dr = dic1[F_SensorId];
								item1.Value.Value = Convert.ToInt32(dr["F_DataValue"] + "");
								item1.Value.ValueTime = Convert.ToDateTime(dr["F_AddTime"]); 
							}
						}
					}
					ULogger.Log("统计" + Batch.F_BatchId + " 最新值######========>" + this.Table.Ranges.Max(w => w.Value.Max(x => x.Value.Value)));

					var listr = rss;
                    for (int r = 0; r < listr.Count; r++)
                    {
						TimeSpan span = listr[r].Type;
						DateTime _now = DateTime.Now;
						ULogger.Log("开始统计" + Batch.F_BatchId + " ["+ span + "] 时间值#############################");
						if (Batch.EnumAgingStatus == EnumAgingStatus.Finished) _now = Batch.F_AgingEndTimeActual;
						if (_now.Year < 1900) _now = Batch.F_AgingEndTime;

						try
                        {
							string from = (_now - span).ToString("yyyy-MM-dd HH:mm:ss");
							string to = _now.ToString("yyyy-MM-dd HH:mm:ss");//F_DataValue<>0 and F_DataValue<>170 and 
							string sql = "SELECT  F_SensorId,max(F_DataValue) MIN,min(F_DataValue) MAX FROM `pd_sensordata_" + Batch.F_BatchId + "`"
								+ " where F_DataValue<>0 and F_DataValue<>170 and F_AddTime>='" + from + "' and F_AddTime<'" + to + "' group by F_SensorId";
							DataTable tab = _BatchServices.QueryTable(sql).Result;
							Dictionary<string, DataRow> dic = new Dictionary<string, DataRow>();
							for (int i = 0; i < tab.Rows.Count; i++)
							{
								DataRow dr = tab.Rows[i];
								string F_SensorId = dr["F_SensorId"] + "";
								dic.Add(F_SensorId, dr);
							}

							foreach (var F_SensorId in this.Table.Ranges.Keys)
							{
								var x1 = this.Table.Ranges[F_SensorId];
								foreach (BatchStaRange item1 in x1)
								{
									if (item1.Type == span && dic.ContainsKey(F_SensorId))
									{
										DataRow dr = dic[F_SensorId];
										item1.Min = Convert.ToInt32(dr["MIN"] + "");
										item1.Max = Convert.ToInt32(dr["MAX"] + "");
									}
								}
							}
							ULogger.Log("开始统计" + Batch.F_BatchId + " [" + span + "] 时间值====>" + this.Table.Ranges.Max(w => w.Value.Max(x => x.Max)));

						}
						catch (Exception ex)
                        {
							ULogger.Log("统计" + Batch.F_BatchId + " [" + span + "] 出错====>" + ex.Message);

						}
						
					}


				}
				//SELECT t.* FROM (SELECT address,max(create_time) as create_time FROM test GROUP BY address) a LEFT JOIN test t ON t.address=a.address and t.create_time=a.create_time
				
				ULogger.Log("统计完成" + Batch.F_BatchId + "......");
				Table.status = sta_job_status.finished;
				Batch = this._BatchServices.QueryById(Batch.F_BatchId).Result;
				Batch.F_STA_TIME = DateTime.Now;
				//Batch.F_STA_XML = Table.ToXml();
				Batch.EnumF_STA_AgingStatus = Batch.EnumAgingStatus;
				var s = this._BatchServices.Update(Batch).Result;
				BatchWithXml xml = new BatchWithXml()
				{
					F_BatchId = Batch.F_BatchId,
					F_STA_XML = Table.ToXml()
				};
				var resxx = this._BatchServices.BaseDal.Db.Updateable<BatchWithXml>(xml).ExecuteCommandHasChangeAsync().Result;


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

}
