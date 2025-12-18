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
using MKSS.Common;
using System.ComponentModel;
using SqlSugar;
using MKSS.Service.SemiCatalysis;

namespace MKSS.APP.SemiCatalysis
{


    public class UIZhuiSuAddModel { 
    
        public string TxtProjectTitle { get; set; }
        public string TxtProductName { get; set; }
		public string TxtProductCode { get; set; }
		public double TxtTimeTotal { get; set; } = 120;
		public UIZhuiSuData Data { get { return UIZhuiSuData.Instance; } }

		public Batch SelectBatch { get; private set; }

		public List<Sensor> SelectSensors { get; private set; }


		public void AddBatch( )
		{
			  
			try
			{

				//添加之前判断，保留50条数据
				while (true) {
					List<Batch> batches = Data.BatchServices.BaseDal.Db.Queryable<Batch>().OrderBy("F_BatchId").ToList();
					if (batches.Count < 50) {
						break;
					}
					UIZhuiSuData.Instance.DeleteBatch(batches[0]);
				}

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

				//修改表名
				string create = @"
						CREATE TABLE ""pd_sensordata_" + batch.F_BatchId + @""" (
						  ""F_DataId"" text(50) NOT NULL,
						  ""F_SensorId"" text NOT NULL,
						  ""F_DataValue"" real(10) NOT NULL,
						  ""F_AddTime"" text NOT NULL,
						  PRIMARY KEY(""F_DataId"")
						);
					";
				var rr =Data.BatchServices.ExecuteCommand(create).Result;
				//Util.MainDb.GetDbClient().AddQueue(create);
				//SugarTable ca = TypeDescriptor.GetAttributes(typeof(SensorData)).OfType<SugarTable>().FirstOrDefault();
				//TypeDescriptor.AddAttributes(typeof(SensorData), new SugarTable("pd_sensordata_"+ batch.F_BatchId));
				//创建表 执行完数据库就有这个表了 /*设置varchar默认长度为50*/
				//Util.MainDb.GetDbClientAttribute().CodeFirst.SetStringDefaultLength(50).InitTables(typeof(SensorData)); 

				Data.DBListBatch.Add(batch);
				this.SelectBatch = batch;

				//创建传感器
				List<Sensor> sens = new List<Sensor>();
				string[] region = SemiCatalysisService.Regions;
				foreach (var b in region)
				{
					for (int s = 1; s <= 16; s++)
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
				SelectSensors = sens.OrderBy(w=>w.VisibleXh).ToList();

				//复制数据文件，批次数据存储到这个文件中
				string data_file = SemiCatalysisSaver.DbNameof(batch);
				if(!System.IO.File.Exists(data_file)) 
					System.IO.File.Copy(AppDomain.CurrentDomain.BaseDirectory + "\\MKSS.APP.SemiCatalysis.sqlite", data_file);


			}
			catch (Exception ex)
			{
				ULogger.Info((string.Format("添加检测任务出错 {0} ......", ex.Message)));
			}
		 

		}

	}

}