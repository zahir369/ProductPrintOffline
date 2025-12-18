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
using MKSS.APP.SemiTester;
using MKSS.Common;
using System.ComponentModel;
using SqlSugar; 
using System.Diagnostics;
using MKSS.APP.LaoHuaService.MQTT;

namespace MKSS.APP.SemiTester
{


	[LogTagClass(Title = "任务管理")]
	public class UIZhuiSuAddModel { 
    
        public string TxtProjectTitle { get; set; }  
		public UIZhuiSuData Data { get { return UIZhuiSuData.Instance; } }
		 
		 
		public void AddBatch(Stopwatch watcher)
		{
			  
			try
			{

				ULogger.Info("DeleteBatch 50:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
				//添加之前判断，保留50条数据 
				while (true) {
					List<Batch> batches = Data.BatchServices.BaseDal.Db.Queryable<Batch>().OrderBy("F_BatchId").ToList();
					if (batches.Count < 50) {
						break;
					}
					UIZhuiSuData.Instance.DeleteBatch(batches[0]);
				}
				ULogger.Info("Add Batch:" + watcher.Elapsed.TotalSeconds.ToString("f3"));

				//根据选择开启批次任务
				Batch batch = new Batch()
				{
					 
					F_AgingStartTime = DateTime.Now,
					F_AgingEndTime = (double)0,
					F_AgingLastUpdateTime = DateTime.Now,
					EnumAgingStatus = EnumAgingStatus.InAging,
					F_BatchId = Data.NextIdBatch,
					F_BatchName = TxtProjectTitle,
					 
				};
				if (string.IsNullOrEmpty(batch.F_BatchName)) batch.F_BatchName = batch.F_BatchId.ToString();
				int res = Data.BatchServices.Add(batch).Result;
				ULogger.Info("CREATE TABLE:" + watcher.Elapsed.TotalSeconds.ToString("f3"));

				string create = @"
CREATE TABLE `pd_sensorgroupdata_" + batch.F_BatchId + @"`  (
  `F_DataId` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `F_AddTime` double NOT NULL,
  `F_BatchId` bigint NOT NULL,
  `F_BoardCaseNo` int NOT NULL,
  `F_FloorNo` int NOT NULL,
  `A1` double NULL DEFAULT NULL,
  `A2` double NULL DEFAULT NULL,
  `A3` double NULL DEFAULT NULL,
  `A4` double NULL DEFAULT NULL,
  `A5` double NULL DEFAULT NULL,
  `A6` double NULL DEFAULT NULL,
  `A7` double NULL DEFAULT NULL,
  `A8` double NULL DEFAULT NULL,
  `A9` double NULL DEFAULT NULL,
  `A10` double NULL DEFAULT NULL,
  `A11` double NULL DEFAULT NULL,
  `A12` double NULL DEFAULT NULL,
  `A13` double NULL DEFAULT NULL,
  `A14` double NULL DEFAULT NULL,
  `A15` double NULL DEFAULT NULL,
  `A16` double NULL DEFAULT NULL,
  `B1` double NULL DEFAULT NULL,
  `B2` double NULL DEFAULT NULL,
  `B3` double NULL DEFAULT NULL,
  `B4` double NULL DEFAULT NULL,
  `B5` double NULL DEFAULT NULL,
  `B6` double NULL DEFAULT NULL,
  `B7` double NULL DEFAULT NULL,
  `B8` double NULL DEFAULT NULL,
  `B9` double NULL DEFAULT NULL,
  `B10` double NULL DEFAULT NULL,
  `B11` double NULL DEFAULT NULL,
  `B12` double NULL DEFAULT NULL,
  `B13` double NULL DEFAULT NULL,
  `B14` double NULL DEFAULT NULL,
  `B15` double NULL DEFAULT NULL,
  `B16` double NULL DEFAULT NULL,
  `C1` double NULL DEFAULT NULL,
  `C2` double NULL DEFAULT NULL,
  `C3` double NULL DEFAULT NULL,
  `C4` double NULL DEFAULT NULL,
  `C5` double NULL DEFAULT NULL,
  `C6` double NULL DEFAULT NULL,
  `C7` double NULL DEFAULT NULL,
  `C8` double NULL DEFAULT NULL,
  `C9` double NULL DEFAULT NULL,
  `C10` double NULL DEFAULT NULL,
  `C11` double NULL DEFAULT NULL,
  `C12` double NULL DEFAULT NULL,
  `C13` double NULL DEFAULT NULL,
  `C14` double NULL DEFAULT NULL,
  `C15` double NULL DEFAULT NULL,
  `C16` double NULL DEFAULT NULL,
  `D1` double NULL DEFAULT NULL,
  `D2` double NULL DEFAULT NULL,
  `D3` double NULL DEFAULT NULL,
  `D4` double NULL DEFAULT NULL,
  `D5` double NULL DEFAULT NULL,
  `D6` double NULL DEFAULT NULL,
  `D7` double NULL DEFAULT NULL,
  `D8` double NULL DEFAULT NULL,
  `D9` double NULL DEFAULT NULL,
  `D10` double NULL DEFAULT NULL,
  `D11` double NULL DEFAULT NULL,
  `D12` double NULL DEFAULT NULL,
  `D13` double NULL DEFAULT NULL,
  `D14` double NULL DEFAULT NULL,
  `D15` double NULL DEFAULT NULL,
  `D16` double NULL DEFAULT NULL,
  PRIMARY KEY (`F_DataId`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = DYNAMIC;
					";

				if (LaoHuaDataProvider.UIMode == LaoHuaDataProviderUIMode.SerialPort)
				{
					create = @"
CREATE TABLE 'pd_sensorgroupdata_" + batch.F_BatchId + @"' (
  'F_DataId' text(255) NOT NULL,
  'F_AddTime' real NOT NULL,
  'F_BatchId' integer NOT NULL,
  'F_BoardCaseNo' integer NOT NULL,
  'F_FloorNo' integer NOT NULL,
  'A1' real,
  'A2' real,
  'A3' real,
  'A4' real,
  'A5' real,
  'A6' real,
  'A7' real,
  'A8' real,
  'A9' real,
  'A10' real,
  'A11' real,
  'A12' real,
  'A13' real,
  'A14' real,
  'A15' real,
  'A16' real,
  'B1' real,
  'B2' real,
  'B3' real,
  'B4' real,
  'B5' real,
  'B6' real,
  'B7' real,
  'B8' real,
  'B9' real,
  'B10' real,
  'B11' real,
  'B12' real,
  'B13' real,
  'B14' real,
  'B15' real,
  'B16' real,
  'C1' real,
  'C2' real,
  'C3' real,
  'C4' real,
  'C5' real,
  'C6' real,
  'C7' real,
  'C8' real,
  'C9' real,
  'C10' real,
  'C11' real,
  'C12' real,
  'C13' real,
  'C14' real,
  'C15' real,
  'C16' real,
  'D1' real,
  'D2' real,
  'D3' real,
  'D4' real,
  'D5' real,
  'D6' real,
  'D7' real,
  'D8' real,
  'D9' real,
  'D10' real,
  'D11' real,
  'D12' real,
  'D13' real,
  'D14' real,
  'D15' real,
  'D16' real,
  PRIMARY KEY ('F_DataId')
); 
					";
				}
				var rr =Data.BatchServices.ExecuteCommand(create).Result;
				ULogger.Info("Add batch:" + watcher.Elapsed.TotalSeconds.ToString("f3"));

				Data.DBListBatch.Add(batch);
				UIZhuiSuC10Model.Instance.BatchCurrent = batch;
				ULogger.Info("创建传感器:" + watcher.Elapsed.TotalSeconds.ToString("f3"));

			}
			catch (Exception ex)
			{
				ULogger.Info((string.Format("添加检测任务出错 {0} ......", ex.Message)));
				throw ex;
			}
		 

		}

	}

}