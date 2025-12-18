using MKSS.APP.LaoHua.Config;
using Stylet;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;
using MKSS.APP.LaoHua.Util;
using MKSS.Services;
using MKSS.Model;
using System.Windows.Controls;
using MKSS.Util.Log;
using MKSS.APP.LaoHua;
using MKSS.Common;
using System.ComponentModel;
using SqlSugar;
using DeviceDataMonitorWPF.UIBiaoDing;

namespace MKSS.APP.LaoHua.UILaoHua
{


    public class UILaoHuaGuanChaAddModel { 
    
        public string TxtProjectTitle { get; set; }
        public string TxtProductName { get; set; }
		public string TxtProductCode { get; set; }
		public double TxtTimeTotal { get; set; }
		public UILaoHuaGuanChaAdd PageContext { get; set; }
		public UILaoHuaGuanChaData Data { get { return UILaoHuaGuanChaData.Instance; } }

		public Batch SelectBatch { get; private set; }
		public List<BoardCase> TxtBoardCaseSelectObject { get; set; }
		public Dictionary<Board, bool> SelectBatchBoard
		{
			get; private set;
		}

		public List<Board> SelectBatchBoardTrue
		{
			get
			{
				if (SelectBatchBoard == null)
					return new List<Board>();
				return SelectBatchBoard.Where(wx => wx.Value).Select(w => w.Key).ToList();
			}
		}
		public Dictionary<Board, List<Sensor>> BoardSensorDictionary { get; set; }

		public void InitPage(UILaoHuaGuanChaAdd uILaoHuaGuanChaAdd)
		{
			PageContext = uILaoHuaGuanChaAdd;
		}




		/// <summary>
		///  选择批次时候，加载数据库数据
		/// </summary>
		public void SelectBatchEventByBatches(Batch b)
		{
			SelectBatch = b;

			List<Board> xxList = null;
			if (SelectBatch == null || SelectBatch.F_BatchId == 0) xxList = new List<Board>();
			xxList = Data.DBBatchDictionary.ContainsKey(SelectBatch) ? Data.DBBatchDictionary[SelectBatch] : new List<Board>();
			SelectBatchBoard = new Dictionary<Board, bool>();
			foreach (var item in xxList)
			{
				SelectBatchBoard.Add(item, true);
			}

		}

		/// <summary>
		///  选择柜子时候，根据柜子构造批次
		/// </summary>
		public void SelectBatchEventByAddBoardCase(List<BoardCase> ccc)
		{
			SelectBatch = null;
			if (SelectBatchBoard == null) SelectBatchBoard = new Dictionary<Board, bool>();

			List<Board> all = new List<Board>();
			foreach (BoardCase cas in ccc)
			{
				foreach (var ba in Data.DBListBoard)
				{
					if (cas.F_BoardCaseId == ba.F_BoarCaseId)
					{
						all.Add(ba);
						if (!SelectBatchBoard.ContainsKey(ba))
						{
							SelectBatchBoard.Add(ba, false);
						}
					}
				}
			}
			foreach (var item in SelectBatchBoard.Keys.ToArray())
			{
				if (!all.Contains(item)) SelectBatchBoard.Remove(item);
			}


		}

		/// <summary>
		///  选择层
		/// </summary>
		/// <param name="_BoardCase"></param>
		/// <param name="bbs"></param>
		public void SelectBatchEventByAddBoard( List<Board> bbs)
		{

			foreach (Board item in this.SelectBatchBoard.Keys.ToList())
			{
				foreach (BoardCase casee in Data.DBListBoardCase)
				{
					if (casee.F_BoardCaseId == item.F_BoarCaseId )
					{
						if (bbs.Contains(item))
						{
							this.SelectBatchBoard[item] = true;
						}
						else
						{
							this.SelectBatchBoard[item] = false;
						}

					}
				}
			}

		}



		public void AddBatch( )
		{
			  
			try
			{

				BoardSensorDictionary = new Dictionary<Board, List<Sensor>>();
				//根据选择开启批次任务
				Batch batch = new Batch()
				{
					F_ADAdd = 0,
					F_ADBase = 0,
					F_ADMinus = 0,
					F_AgingStartTime = DateTime.Now,
					F_AgingEndTime = DateTime.Now.AddHours(TxtTimeTotal),
					F_AgingLastUpdateTime = DateTime.Now,
					EnumAgingStatus = EnumAgingStatus.InAging,
					F_BatchId = Data.NextIdBatch,
					F_BatchName = TxtProjectTitle,
					F_SensorName = TxtProductName,
					F_SensorTypeName = TxtProductName,
					F_SensorTypeId = TxtProductCode,
				};

				batch.F_SCRWD_Date = DateTime.Parse(SelectScrwEntity.Date);
				batch.F_SCRWD_Department = (SelectScrwEntity.Department);
				batch.F_SCRWD_OrderID = (SelectScrwEntity.OrderID).ToString();
				batch.F_SCRWD_OrderNumber = (SelectScrwEntity.OrderNumber);
				batch.F_SCRWD_ProductCode = (SelectScrwEntity.ProductCode);
				batch.F_SCRWD_ProductFullName = (SelectScrwEntity.ProductFullName);
				batch.F_SCRWD_ProductType = (SelectScrwEntity.ProductType);
				batch.F_SCRWD_Qty = (SelectScrwEntity.Qty);
                int res = Data.BatchServices.Add(batch).Result;

				//修改表名
				string create = @"
					CREATE TABLE `pd_sensordata_" + batch.F_BatchId+ @"`  (
					  `F_DataId` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
					  `F_SensorId` bigint(0) NOT NULL,
					  `F_DataValue` decimal(10, 0) NOT NULL,
					  `F_AddTime` datetime(0) NOT NULL,
					  PRIMARY KEY (`F_DataId`) USING BTREE
					) ENGINE = InnoDB AUTO_INCREMENT = 319411 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;
					";
				var rr =Data.BatchServices.QueryTable(create).Result;
				//Util.MainDb.GetDbClient().AddQueue(create);
				//SugarTable ca = TypeDescriptor.GetAttributes(typeof(SensorData)).OfType<SugarTable>().FirstOrDefault();
				//TypeDescriptor.AddAttributes(typeof(SensorData), new SugarTable("pd_sensordata_"+ batch.F_BatchId));
				//创建表 执行完数据库就有这个表了 /*设置varchar默认长度为50*/
				//Util.MainDb.GetDbClientAttribute().CodeFirst.SetStringDefaultLength(50).InitTables(typeof(SensorData)); 

				Data.DBListBatch.Add(batch);
				this.SelectBatch = batch;
				Data.DBBatchDictionary.Add(batch, new List<Board>());

				foreach (BoardCase item in TxtBoardCaseSelectObject)
				{
					foreach (Board b in Data.DBBoardCaseDictionary[item])
					{
						bool v = this.SelectBatchBoard.ContainsKey(b) && SelectBatchBoard[b];
						if (v)
						{
							BatchRel rel = new BatchRel()
							{
								F_LastUseStart = DateTime.Now,
								F_LastUseUpdate = DateTime.Now,
								F_UseStatus = 1,
								F_BatchId = batch.F_BatchId,
								F_BoardId = b.F_BoardId
							};
							res = Data.BatchRelServices.Add(rel).Result;
							b.EnumUseInFree = EnumUseInFree.InUse;
							b.EnumUseType = EnumUseType.LaoHua;
							b.F_LastUseSensorCount = 15;
							b.F_LastUseStart = DateTime.Now;
							b.F_LastUseUpdate = DateTime.Now;
							bool resx = Data.BoardServices.Update(b).Result;
							Data.DBBatchDictionary[batch].Add(b);
							//创建传感器
							BoardSensorDictionary.Add(b, new List<Sensor>());
							for (int i = 0; i < b.F_SensorCount; i++)
							{
								string sensorId = Sensor.CreateCensorId(batch.F_BatchId,item.F_BoardCaseAddress,b.F_FloorNO,i+1);
								Sensor sen = new Sensor()
								{
									F_BatchId = batch.F_BatchId,
									F_BoardCaseId = item.F_BoardCaseId,
									F_BoardId = b.F_BoardId,
									F_SensorId = sensorId,
									F_SensorName = this.TxtProductName,
									F_SensorTypeId = this.TxtProductCode,
									F_SensorTypeName = this.TxtProductName,
									F_SlotNO = i + 1
								};
								BoardSensorDictionary[b].Add(sen);
							}
							List<Sensor> sens = BoardSensorDictionary[b].ToList();
							int xxas = Data.SensorServices.Add(sens).Result;
						}
					}
					item.EnumUseInFree = EnumUseInFree.InUse;
					item.F_LastUseUpdate = DateTime.Now;
					bool resx1 = Data.BoardCaseServices.Update(item).Result;
				}

			}
			catch (Exception ex)
			{
				ULogger.Info((string.Format("添加老化任务出错 {0} ......", ex.Message)));
				throw ex;
			}
		 

		}


		public ScrwEntity SelectScrwEntity { get; set; }
		public async Task BtnScrwd()
		{
			UIScrwSelect ww = new UIScrwSelect(this, PageContext);
			ww.Owner = PageContext;
			ww.ShowInTaskbar = false;
			ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			bool? ret = ww.ShowDialog();
			if (ret != null && ret.Value)
			{
				this.TxtProjectTitle = string.Format("{0}[{1}]", SelectScrwEntity.ProductFullName, SelectScrwEntity.OrderNumber);
				this.TxtProductName = SelectScrwEntity.ProductFullName;
				this.TxtProductCode = SelectScrwEntity.ProductCode;
				this.PageContext.TxtProjectTitle.Text = this.TxtProjectTitle; 
			}
			 
		}

	}

}