using System.Collections.Generic;
using System;
using System.Linq;
using System.ComponentModel;
using MKSS.Model;
using MKSS.Util.Log;
using MKSS.Util.Loger; 
using System.Net.Sockets;
using NModbus;
using NModbus.Serial;

namespace MKSS.Service.LaoHua
{

	[LogTagClass(Title = "网络连接")]
	public class BoardConnectionNet : BoardConnection
	{

		/// <summary>
		///  唯一标记
		/// </summary>
		public override string ID { get { return "TCPIP"; } }
		/// <summary>
		///  主类别
		/// </summary>
		public override string Main { get { return "网络连接"; } }
		/// <summary>
		///  分类名称
		/// </summary>
		public override string DataTitle { get { return string.Format("{0}:{1}", IP, PORT); } }
		/// <summary>
		///  用户对象
		/// </summary>
		public override object Tag { get { return null; } set { } }
		/// <summary>
		/// 是否已经打开
		/// </summary>
		public override bool IsOpen { get { return PooledConnection.Pool.Of(IPAddr).Connected; } }


		public IPAddr IPAddr { get { return new IPAddr(IP, PORT); } }
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

		/// <summary>
		///  打开
		/// </summary>
		public override void Open()
		{
			 
			TcpClient _tcpClient = PooledConnection.Pool.Of(IPAddr);
			if (_tcpClient.Connected)
			{
				CommFactory.Message = "重用已有连接";
			}
			else
			{
				try
				{
					PooledConnection.Pool.Open(IPAddr);
				}
				catch (Exception e)
				{
					CommFactory.MessageLog = e.Message;
					return;
				}
			}

			var adapter = new NModbus.IO.TcpClientAdapter(_tcpClient);
			CommFactory.Master = modbusFactory.CreateRtuMaster(adapter);
			CommFactory.Master.Transport.ReadTimeout = 2000;
			CommFactory.Master.Transport.WriteTimeout = 2000;
			CommFactory.Master.Transport.Retries = 0;
			CommFactory.Master.Transport.WaitToRetryMilliseconds = 250;

		}


		/// <summary>
		///  关闭
		/// </summary>
		public override void Close()
		{
			bool isRead = CommFactory.IsRead;
			if (isRead)
			{
				CommFactory.Message = "数据正在读取，请先停止读取数据后再关闭串口！";
			}
			else
			{ 
				//必须先关闭 TcpClient,Master.Dispose 会连带释放 TcpClient
				TcpClient _tcpClient = PooledConnection.Pool.Of(IPAddr);
				PooledConnection.Pool.Close(IPAddr);

				if (CommFactory.Master != null)
				{
					CommFactory.Master.Dispose();
					CommFactory.Master = null;
				}

			}
		}
		 
		private string IP { get; set; }
		private int PORT { get; set; }

		 
		public event PropertyChangedEventHandler PropertyChanged;

		public BoardConnectionNet(string ip,int port,   int readspeed = 200) : base(readspeed)
		{
			IP = ip;
			PORT = port;  
		}


		public override string ToString()
		{
			return string.Format("{0}_{1}_{2}", this.IP, this.PORT, this.IsOpen ? "T" : "F");
		}

	}

}
