using Stylet;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;
using MKSS.APP.ZhuiSu.Util;
using MKSS.Services;
using MKSS.Model;
using System.Windows.Controls;
using MKSS.Util.Log;
using MKSS.APP.ZhuiSu;
using System.Data;

namespace MKSS.APP.ZhuiSu
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

		public BoardServices BoardServices { get; set; }
		public BoardCaseServices BoardCaseServices { get; set; }
		public BatchServices BatchServices { get; set; }
		public BatchRelServices BatchRelServices { get; set; }
		public SensorServices SensorServices { get; set; }
		public SensorDataServices SensorDataServices { get; set; }

		public UIZhuiSuData() {
			BatchServices = new BatchServices();
			BoardServices = new BoardServices();
			BatchRelServices = new BatchRelServices();
			BoardCaseServices = new BoardCaseServices();
			SensorServices = new SensorServices();
			SensorDataServices = new SensorDataServices();
		}


		public List<Board> DBListBoard { get; set; }
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
		public List<BoardCase> DBListBoardCase { get; set; }
		public Dictionary<Batch, List<Board>> DBBatchDictionary { get; set; }
		public Dictionary<BoardCase, List<Board>> DBBoardCaseDictionary { get; set; }
		


		[LogTagClass(Title = "刷新检测数据列表")]
		public void RefreshData()
		{

			DBListBoardCase = BoardCaseServices.Query<BoardCase>(null).Result;
			DBListBoard = BoardServices.Query<Board>(null).Result;
			DBListBatch = BatchServices.Query(wx => wx.F_AgingStatus == 1).Result;
			DBListBatchFinished= BatchServices.Query(wx => wx.F_AgingStatus != 1).Result;
			DBBoardCaseDictionary = new Dictionary<BoardCase, List<Board>>();
			foreach (BoardCase b in DBListBoardCase)
			{
				DBBoardCaseDictionary.Add(b, DBListBoard.Where(w => w.F_BoarCaseId == b.F_BoardCaseId).OrderBy(w => w.F_BoardId).ToList());
			}
			DBBatchDictionary = new Dictionary<Batch, List<Board>>();
			foreach (Batch item in DBListBatch)
			{
				DBBatchDictionary.Add(item, new List<Board>());
				List<BatchRel> rel = BatchRelServices.Query((w => w.F_BatchId == item.F_BatchId && w.F_UseStatus == 1)).Result;
				foreach (var r in rel)
				{
					var b = DBListBoard.FirstOrDefault(wx => wx.F_BoardId == r.F_BoardId);
					if (b != null) DBBatchDictionary[item].Add(b);
				}
			}

		}

		public List<Board> GetBoard(Batch item) {
			List<Board> ret = new List<Board>();
			if (item != null) {
				List<BatchRel> rel = BatchRelServices.Query((w => w.F_BatchId == item.F_BatchId)).Result;
				foreach (var r in rel)
				{
					var b = DBListBoard.FirstOrDefault(wx => wx.F_BoardId == r.F_BoardId);
					if (b != null) ret.Add(b);
				}
			}
			return ret;
		}

		public Batch OfBatch(PageBoardItem DataSrc,
			ref BoardCase _BoardCase,ref Board _Board)
		{
			if (DataSrc.Address <= 0 && DataSrc.CaseNo <= 0) return null;
			_BoardCase = this.DBListBoardCase.FirstOrDefault(w => w.F_BoardCaseAddress == DataSrc.CaseNo);
			if (_BoardCase == null) return null;
			List<Board> listBoard = DBBoardCaseDictionary[_BoardCase];
			_Board = listBoard.FirstOrDefault(w => w.AddrByte ==((byte)DataSrc.Address));
			if (_Board == null) return null;
			Board _xx = _Board;
			Batch _Batch = this.DBBatchDictionary.Where(wx => wx.Value.Contains(_xx)).Select(wx => wx.Key).FirstOrDefault();
			if (_Batch != null) return _Batch;
			return null;
		}

		public bool BtnFinishBoardExeEnabled(PageBoardItem DataSrc)
		{
			if (DataSrc.Address <= 0 && DataSrc.CaseNo <= 0) return false;
			BoardCase _BoardCase = this.DBListBoardCase.FirstOrDefault(w => w.F_BoardCaseAddress == DataSrc.CaseNo);
			if (_BoardCase == null) return false;
			List<Board> list = DBBoardCaseDictionary[_BoardCase];
			Board _Board = list.FirstOrDefault(w => w.AddrByte == ((byte)DataSrc.Address));
			if (_Board == null) return false;
			Batch _Batch = this.DBBatchDictionary.Where(wx => wx.Value.Contains(_Board)).Select(wx => wx.Key).FirstOrDefault();
			if (_Batch == null) return false;
			bool ret = DataSrc.Address > 0 && DataSrc.CaseNo > 0 && _Batch != null;
			return ret;
		}


		public void BtnFinishBoardExe(PageBoardItem DataSrc)
		{
			try
			{
				BoardCase _BoardCase = this.DBListBoardCase.FirstOrDefault(w => w.F_BoardCaseAddress == DataSrc.CaseNo);
				if (_BoardCase != null && DBBoardCaseDictionary.ContainsKey(_BoardCase))
				{
					List<Board> list = DBBoardCaseDictionary[_BoardCase];
					Board _Board = list.FirstOrDefault(w => w.AddrByte == ((byte)DataSrc.Address));
					Batch _Batch = this.DBBatchDictionary.Where(wx => wx.Value.Contains(_Board)).Select(wx => wx.Key).FirstOrDefault();
					Batch SelectBatch = _Batch;
					if (_Batch != null && _Board != null)
					{

						//结束他
						_Board.F_LastUseUpdate = DateTime.Now;
						_Board.EnumUseType = EnumUseType.ZhuiSu;
						_Board.EnumUseInFree = EnumUseInFree.Free;
						bool ress = BoardServices.Update(_Board).Result;

						//更新关联关系状态
						List<BatchRel> rels = BatchRelServices.Query((w => w.F_BatchId == SelectBatch.F_BatchId && w.F_BoardId == _Board.F_BoardId)).Result;
						foreach (var r in rels)
						{
							r.F_LastUseUpdate = DateTime.Now;
							r.EnumUseInFree = EnumUseInFree.Free;
							ress = this.BatchRelServices.Update(r).Result;
						}
						this.DBBatchDictionary[_Batch].Remove(_Board);

						//判断柜子是否有使用中板子
						int count = BoardServices.Query(wx => wx.F_BoarCaseId == _BoardCase.F_BoardCaseId && wx.F_UseStatus == 1).Result.Count;
						if (count == 0)
						{
							_BoardCase.EnumUseInFree = EnumUseInFree.Free;
							_BoardCase.F_LastUseUpdate = DateTime.Now;
							ress = this.BoardCaseServices.Update(_BoardCase).Result;
						}

						//判断批次是否需要结束
						rels = BatchRelServices.Query((w => w.F_BatchId == _Batch.F_BatchId && w.F_UseStatus == 1)).Result;
						if (rels.Count == 0)
						{
							_Batch.F_AgingEndTimeActual = DateTime.Now;
							_Batch.F_AgingLastUpdateTime = DateTime.Now;
							_Batch.EnumAgingStatus = EnumAgingStatus.Finished;
							ress = this.BatchServices.Update(_Batch).Result;
							this.DBListBatch.Remove(_Batch);
							this.DBBatchDictionary.Remove(_Batch);
						}

					}
					else
					{
						ULogger.Info((string.Format("找不到检测柜{0}地址{1}", DataSrc.CaseNo, DataSrc.Address)));
					}
				}
				else
				{
					ULogger.Info((string.Format("找不到检测柜{0}", DataSrc.CaseNo)));
				}
			}
			catch (Exception ex)
			{
				ULogger.Info((string.Format("结束检测出错：{0}", ex.Message)));
			}

		}

		public void FinishBatch(Batch _Batch)
		{

			try
			{

				foreach (Board _Board in this.DBBatchDictionary[_Batch])
				{

					//结束板子
					_Board.F_LastUseUpdate = DateTime.Now;
					_Board.EnumUseType = EnumUseType.ZhuiSu;
					_Board.EnumUseInFree = EnumUseInFree.Free;
					bool ress = BoardServices.Update(_Board).Result;

					//更新关联关系状态
					List<BatchRel> rels = BatchRelServices.Query((w => w.F_BatchId == _Batch.F_BatchId && w.F_BoardId == _Board.F_BoardId)).Result;
					foreach (var r in rels)
					{
						r.F_LastUseUpdate = DateTime.Now;
						r.EnumUseInFree = EnumUseInFree.Free;
						ress = BatchRelServices.Update(r).Result;
					}

					BoardCase _BoardCase = this.DBListBoardCase.FirstOrDefault(w => w.F_BoardCaseId == _Board.F_BoarCaseId);
					_BoardCase.EnumUseInFree = EnumUseInFree.Free;
					_BoardCase.F_LastUseUpdate = DateTime.Now;
					ress = BoardCaseServices.Update(_BoardCase).Result;
				
				}

				_Batch.F_AgingEndTimeActual = DateTime.Now;
				_Batch.F_AgingLastUpdateTime = DateTime.Now;
				_Batch.EnumAgingStatus = EnumAgingStatus.Finished;
				bool resccs = BatchServices.Update(_Batch).Result;
				this.DBListBatch.Remove(_Batch);
				this.DBBatchDictionary.Remove(_Batch);

			}
			catch (Exception ex)
			{
				ULogger.Error(string.Format("结束检测出错：{0}", ex.Message));
				ULogger.Error(ex);
			}



		}

		public void RestartBatch(Batch _Batch)
		{

			List<BatchRel> rel = BatchRelServices.Query((w => w.F_BatchId == _Batch.F_BatchId )).Result;
			foreach (var rr in rel)
			{
				var _Board = DBListBoard.FirstOrDefault(wx => wx.F_BoardId == rr.F_BoardId);
				BoardCase _BoardCase = this.DBListBoardCase.FirstOrDefault(w => w.F_BoardCaseId == _Board.F_BoarCaseId);
				try
				{
					{

						//判断 是否需要结束
						{

							//结束板子
							_Board.F_LastUseUpdate = DateTime.Now;
							_Board.EnumUseType = EnumUseType.ZhuiSu;
							_Board.EnumUseInFree = EnumUseInFree.InUse;
							bool ress = BoardServices.Update(_Board).Result;

							//更新关联关系状态
							List<BatchRel> rels = BatchRelServices.Query((w => w.F_BatchId == _Batch.F_BatchId && w.F_BoardId == _Board.F_BoardId)).Result;
							foreach (var r in rels)
							{
								r.F_LastUseUpdate = DateTime.Now;
								r.EnumUseInFree = EnumUseInFree.InUse;
								ress = BatchRelServices.Update(r).Result;
							}

							//判断柜子是否有使用中板子
							{
								_BoardCase.EnumUseInFree = EnumUseInFree.InUse;
								_BoardCase.F_LastUseUpdate = DateTime.Now;
								ress = BoardCaseServices.Update(_BoardCase).Result;
							}

							//判断是否所有板子已经结束
							{
								_Batch.F_AgingEndTimeActual = DateTime.Now;
								_Batch.F_AgingLastUpdateTime = DateTime.Now;
								_Batch.EnumAgingStatus = EnumAgingStatus.InAging;
								ress = BatchServices.Update(_Batch).Result;
							}

						}

					}

				}
				catch (Exception ex)
				{
					ULogger.Error(string.Format("重启检测出错：{0}", ex.Message));
					ULogger.Error(ex);
				}
			}


		}

		public void DeleteBatch(Batch batch)
		{

			try
			{
				 
				bool b1 = BatchServices.DeleteById(batch.F_BatchId).Result;

				//修改表名
				string create = @"
					update pd_board set F_UseStatus=2 where F_BoardId in(select F_BoardId from pd_batch_rel where F_BatchId={0});
					update pd_boardcase set F_UseStatus=2 where F_BoardCaseId in(select F_BoarCaseId from pd_board where F_BoardId in(select F_BoardId from pd_batch_rel where F_BatchId={0}));
					delete from pd_sensordata where F_SensorId in(select F_SensorId from pd_sensor where F_BatchId={0});
					DROP TABLE IF EXISTS pd_sensordata_{0};
					delete from pd_sensor where F_BatchId={0};
					delete from pd_batch_rel where F_BatchId={0};
					delete from pd_batch where F_BatchId={0};"; 
				var rr = BatchServices.QueryTable(string.Format(create, batch.F_BatchId)).Result;
				 

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
				int year = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
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
