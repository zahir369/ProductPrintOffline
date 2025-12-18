using System.Collections.Generic;
using System;
using System.Linq;
using System.ComponentModel;
using MKSS.Model;
using MKSS.Util.Log;
using MKSS.Util.Loger;
using MKSS.APP.UIBiaoDing.Util;
using System.IO.Ports;
using NModbus;
using NModbus.Serial;

namespace MKSS.Service.LaoHua
{

	[LogTagClass(Title = "硬件连接")]
	public class BoardConnectionSreial : BoardConnection
	{

		/// <summary>
		///  唯一标记
		/// </summary>
		public override string ID { get { return "Sreial"; } }
		/// <summary>
		///  主类别
		/// </summary>
		public virtual string Main { get { return "硬件连接"; } }
		/// <summary>
		///  分类名称
		/// </summary>
		public virtual string DataTitle { get { return "某地址"; } }
		/// <summary>
		///  用户对象
		/// </summary>
		public virtual object Tag { get { return null; } set { } }
		/// <summary>
		/// 是否已经打开
		/// </summary>
		public override bool IsOpen { get { return PooledConnection.Pool.Of(COM).IsOpen; } }
		/// <summary>
		///  检查连接 
		/// </summary>
		/// <returns></returns>
		public override bool CheckConnection()
		{

			if (this.IsOpen) return true;

			int tryTimes = 0;
			while (!this.IsOpen)
			{
				if (tryTimes >= 3) return false;
				tryTimes++;
				Open();
			}
			return this.IsOpen;

		}


		public override void Open( )
		{ 
			Pool.Qualified.Clear();
			SerialPort _serial = PooledConnection.Pool.Of(COM);
			bool isOpen = _serial.IsOpen;
			if (isOpen)
			{
				CommFactory.Message = "重用已有连接";
			}
			else
			{
				if (COM == "")
				{
					CommFactory.Message = "error:请选择你的设备端口！";
				}
				else
				{
					_serial.PortName = COM;
					_serial.BaudRate = 19200;
					_serial.DataBits = 8;
					_serial.Parity = Parity.None;
					_serial.StopBits = StopBits.One;
					bool flag2 = !_serial.IsOpen;
					if (flag2)
					{
						try
						{
							_serial.Open();
							CommFactory.Message = "串口打开成功";
						}
						catch
						{
							CommFactory.Message = "error:串口打开失败，请检查系统串口是否可用！";
							return;
						}
					}
				}
			}
			CommFactory.Master = modbusFactory.CreateRtuMaster(_serial);
			CommFactory.Master.Transport.ReadTimeout = 1000;
			CommFactory.Master.Transport.WriteTimeout = 1000;
			CommFactory.Master.Transport.Retries = 0;
			CommFactory.Master.Transport.WaitToRetryMilliseconds = 250;
		}



		public override void Close()
		{
			bool isRead = CommFactory.IsRead;
			if (isRead)
			{
				CommFactory.Message = "数据正在读取，请先停止读取数据后再关闭串口！";
			}
			else
			{

				if (CommFactory.Master != null)
				{
					CommFactory.Master.Dispose();
					CommFactory.Master = null;
				}

				SerialPort _serial = PooledConnection.Pool.Of(COM);
				try
				{
					PooledConnection.Pool.Close(COM);
					bool flag = !_serial.IsOpen;
					if (flag)
					{
						CommFactory.Message = "已关闭串口！";
					}
				}
				catch
				{
					CommFactory.Message = "关闭串口失败！";
				}

				if (CommFactory.Master != null)
				{
					CommFactory.Master.Dispose();
					CommFactory.Master = null;
				}

			}
		}
		 
		public string COM = "";


		public BoardConnectionSreial(string com, AllConnection pool, int readspeed = 200) : base(readspeed)
		{
			COM = com; 
			this.Pool = pool;
		}

		public override string ToString()
		{
			return string.Format("{0}_{1}", this.COM, this.IsOpen ? "T" : "F");
		}

	}


}
