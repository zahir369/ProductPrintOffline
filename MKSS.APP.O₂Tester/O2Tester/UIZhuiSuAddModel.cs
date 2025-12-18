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
using MKSS.APP.O2Tester;
using MKSS.Common;
using System.ComponentModel;
using SqlSugar;
using MKSS.Service.O2Tester;
using System.Diagnostics;

namespace MKSS.APP.O2Tester
{


	[LogTagClass(Title = "任务管理")]
	public class UIZhuiSuAddModel { 
    
        public string TxtProjectTitle { get; set; }
        public string TxtProductName { get; set; }
		public string TxtProductCode { get; set; }
		public double TxtTimeTotal { get; set; } = 120;
		public UIZhuiSuData Data { get { return UIZhuiSuData.Instance; } }

		public Batch SelectBatch { get; private set; }

		public List<Sensor> SelectSensors { get; private set; }


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
					F_AgingEndTime = (double)TxtTimeTotal,
					F_AgingLastUpdateTime = DateTime.Now,
					EnumAgingStatus = EnumAgingStatus.InAging,
					F_BatchId = Data.NextIdBatch,
					F_BatchName = TxtProjectTitle,
					F_SensorName = TxtProductName,
					F_SensorTypeName = TxtProductName,
					F_SensorTypeId = TxtProductCode,
				};
				if (string.IsNullOrEmpty(batch.F_BatchName)) batch.F_BatchName = batch.F_BatchId.ToString();
				int res = Data.BatchServices.Add(batch).Result;
				ULogger.Info("CREATE TABLE:" + watcher.Elapsed.TotalSeconds.ToString("f3"));

				//修改表名
				string create = @"
						CREATE TABLE ""pd_sensorgroupdata_" + batch.F_BatchId + @""" (
						  ""F_DataId"" text(50) NOT NULL,
						  ""F_AddTime"" text NOT NULL,
						  ""F_BatchId"" integer NOT NULL,
						  ""A1"" real(10),  ""A2"" real(10),  ""A3"" real(10),  ""A4"" real(10),  ""A5"" real(10),  ""A6"" real(10),  ""A7"" real(10),  ""A8"" real(10),  ""A9"" real(10),
						  ""A10"" real(10),  ""A11"" real(10),  ""A12"" real(10),  ""A13"" real(10),  ""A14"" real(10),  ""A15"" real(10),  ""A16"" real(10),   
						  ""B1"" real(10),  ""B2"" real(10),  ""B3"" real(10),  ""B4"" real(10),  ""B5"" real(10),  ""B6"" real(10),  ""B7"" real(10),  ""B8"" real(10),  ""B9"" real(10),
						  ""B10"" real(10),  ""B11"" real(10),  ""B12"" real(10),  ""B13"" real(10),  ""B14"" real(10),  ""B15"" real(10),  ""B16"" real(10),  
						  ""C1"" real(10),  ""C2"" real(10),  ""C3"" real(10),  ""C4"" real(10),  ""C5"" real(10),  ""C6"" real(10),  ""C7"" real(10),  ""C8"" real(10),  ""C9"" real(10),
						  ""C10"" real(10),  ""C11"" real(10),  ""C12"" real(10),  ""C13"" real(10),  ""C14"" real(10),  ""C15"" real(10),  ""C16"" real(10),   
						  ""D1"" real(10),  ""D2"" real(10),  ""D3"" real(10),  ""D4"" real(10),  ""D5"" real(10),  ""D6"" real(10),  ""D7"" real(10),  ""D8"" real(10),  ""D9"" real(10),
						  ""D10"" real(10),  ""D11"" real(10),  ""D12"" real(10),  ""D13"" real(10),  ""D14"" real(10),  ""D15"" real(10),  ""D16"" real(10),  
						  PRIMARY KEY (""F_DataId"")
						);
					";
				var rr =Data.BatchServices.ExecuteCommand(create).Result;
				ULogger.Info("Add batch:" + watcher.Elapsed.TotalSeconds.ToString("f3"));

				Data.DBListBatch.Add(batch);
				this.SelectBatch = batch;
				ULogger.Info("创建传感器:" + watcher.Elapsed.TotalSeconds.ToString("f3"));

				//创建传感器
				List<Sensor> sens = new List<Sensor>();
				string[] region = O2TesterService.Regions;
				foreach (var b in region)
				{
					for (int s = 1; s <= O2TesterService.RegionSize; s++)
					{ 
						string sensorId = Sensor.CreateCensorId(batch.F_BatchId, b, s);
						Sensor sen = new Sensor()
						{
							F_BatchId = batch.F_BatchId,
							F_BoardId = b,
							F_SensorId = sensorId,
							F_SensorName = this.TxtProductName,
							F_SensorTypeId = this.TxtProductCode,
							F_SensorTypeName = this.TxtProductName,
							F_SlotNO = s
						};
						sens.Add(sen);
					}
				}
				int xxas = Data.SensorServices.Add(sens).Result;
				SelectSensors = sens;
				ULogger.Info("复制数据文件:" + watcher.Elapsed.TotalSeconds.ToString("f3"));

				//复制数据文件，批次数据存储到这个文件中
				string data_file = O2TesterSaver.DbNameof(batch);
				if(!System.IO.File.Exists(data_file)) 
					System.IO.File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\MKSS.APP.O2Tester.sqlite", data_file);
				ULogger.Info("Copy Finish:" + watcher.Elapsed.TotalSeconds.ToString("f3"));


			}
			catch (Exception ex)
			{
				ULogger.Info((string.Format("添加检测任务出错 {0} ......", ex.Message)));
				throw ex;
			}
		 

		}

	}

}