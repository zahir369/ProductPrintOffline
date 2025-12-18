using Stylet;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System; 
using MKSS.Services;
using MKSS.Model;
using System.Windows.Controls;
using MKSS.Util.Log;
using MKSS.APP.SemiCatalysis;
using System.Data;
using MKSS.Service.SemiCatalysis;

namespace MKSS.APP.SemiCatalysis
{
    [LogTagClass(Title = "检测数据")]
	public class UIZhuiSuData
    {

		static UIZhuiSuData _instance = null;
		public static UIZhuiSuData Instance
        {
			get {
				if (_instance == null) {
					_instance = new UIZhuiSuData();
					_instance.RefreshData();

				}
				return _instance;
			}
		}
		 
		public BatchServices BatchServices { get; set; } 
		public SensorServices SensorServices { get; set; }
		public SensorDataServices SensorDataServices { get; set; }

		public UIZhuiSuData() {
			BatchServices = new BatchServices(); 
			SensorServices = new SensorServices();
			SensorDataServices = new SensorDataServices();
		}

		 
		public List<Batch> DBListBatch { get; set; }
		public List<Batch> DBListBatchFinished { get; set; }
		public List<Batch> DBListAll
		{
			get
			{
				List<Batch> rett = new List<Batch>();
				rett.AddRange(DBListBatch);
				rett.AddRange(DBListBatchFinished);
				return rett.OrderByDescending(w=>w.F_AgingStartTime).ToList();
			}
		}  


		[LogTagClass(Title = "刷新检测数据列表")]
		public void RefreshData()
		{

			DBListBatch = BatchServices.Query(wx => wx.F_AgingStatus == 1).Result;
			DBListBatchFinished= BatchServices.Query(wx => wx.F_AgingStatus != 1).Result;
			 
			foreach (Batch item in DBListBatch)
			{
				 
			}

		} 
		  

		public void BtnFinishBoardExe(Batch _Batch)
		{
			try
			{
				if (_Batch != null )
				{
					 
					Batch SelectBatch = _Batch;
					_Batch.F_AgingEndTimeActual = DateTime.Now;
					_Batch.F_AgingLastUpdateTime = DateTime.Now;
					_Batch.EnumAgingStatus = EnumAgingStatus.Finished;
					var ress = this.BatchServices.Update(_Batch).Result;
					this.DBListBatch.Remove(_Batch);
				}  
			}
			catch (Exception ex)
			{
				ULogger.Info((string.Format("结束检测出错：{0}", ex.Message)));
			}

		}

		public void FinishBatch(Batch _Batch,double F_AgingEndTime)
		{

			try
			{

				_Batch.F_AgingEndTimeActual = DateTime.Now;
				_Batch.F_AgingLastUpdateTime = DateTime.Now;
				_Batch.EnumAgingStatus = EnumAgingStatus.Finished;
				if(F_AgingEndTime>0) _Batch.F_AgingEndTime = F_AgingEndTime;
				bool resccs = BatchServices.Update(_Batch).Result;
				this.DBListBatch.Remove(_Batch); 

			}
			catch (Exception ex)
			{
				ULogger.Error(string.Format("结束检测出错：{0}", ex.Message));
				ULogger.Error(ex);
			}



		}

		public void RestartBatch(Batch _Batch)
		{

			_Batch.F_AgingEndTimeActual = DateTime.Now;
			_Batch.F_AgingLastUpdateTime = DateTime.Now;
			_Batch.EnumAgingStatus = EnumAgingStatus.InAging;
			var ress = BatchServices.Update(_Batch).Result;

		}

		public void DeleteBatch(Batch batch)
		{

			try
			{
				 
				bool b1 = BatchServices.DeleteById(batch.F_BatchId).Result;

				//修改表名
				string create = @"
					delete from pd_sensordata where F_SensorId in(select F_SensorId from pd_sensor where F_BatchId={0});
					DROP TABLE IF EXISTS pd_sensordata_{0};
					delete from pd_sensor where F_BatchId={0};
					delete from pd_batch where F_BatchId={0};"; 
				var rr = BatchServices.ExecuteCommand(string.Format(create, batch.F_BatchId)).Result;

				//删除数据文件
				string data_file = SemiCatalysisSaver.DbNameof(batch);
				if(System.IO.File.Exists(data_file)) System.IO.File.Delete(data_file);

			}
			catch (Exception ex)
			{
				ULogger.Info((string.Format("删除检测任务出错 {0} ......", ex.Message)));
				throw ex;
			}


		}


		void LogUI(string msg)
		{
			ULogger.Info(msg);
		}

		public long NextIdBatch
		{
			get
			{
				long year = long.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));
				long year_from = year * 100 + 0;
				long year_to = (year + 1) * 100 + 1;
				DataTable dt = this.BatchServices.QueryTable(
					string.Format("select max(F_BatchId) from pd_batch where F_BatchId >{0} and F_BatchId<{1}", year_from, year_to))
					.Result;
				long CurrentIdBatch =
					(dt == null || dt.Rows.Count == 0 || dt.Rows[0][0] == DBNull.Value)
					? year_from : (Convert.ToInt64(dt.Rows[0][0]));
				CurrentIdBatch++;
				return CurrentIdBatch;
			}
		}

	}

}
