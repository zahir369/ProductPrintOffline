using System.Collections.Generic;
using System;
using System.Linq;
using System.ComponentModel;
using MKSS.Model;
using MKSS.Util.Log;
using MKSS.Util.Loger;
using DeviceDataMonitorWPF.UIBiaoDing.Util;
using System.Net.Sockets;
using NModbus;
using NModbus.Serial;

namespace MKSS.Service.LaoHua
{

	[LogTagClass(Title = "老化柜")]
	public class BoardConnectionBoardCase : BoardConnection
	{

		/// <summary>
		///  唯一标记
		/// </summary>
		public override string ID { get { return this.DataBus.F_SocketIP+":"+this.DataBus.F_SocketPort; } }
		/// <summary>
		///  主类别
		/// </summary>
		public override string Main { get { return "老化柜"; } }
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
		public override bool IsOpen { get { return this._tcpClient != null && this._tcpClient.Connected; } }
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

			this.Pool.Qualified.Clear();
			if (_tcpClient == null) _tcpClient = new TcpClient();
			bool connected = this._tcpClient.Connected;
			if (connected)
			{
				CommFactory.Message = "error:tcp已经连接";
			}
			else
			{
				if (IP == "")
				{
					CommFactory.Message = "error:请输入远程服务器地址";
				}
				else
				{
					try
					{
						this._tcpClient.Connect(this.IP, this.PORT);
						CommFactory.Message = "已连接 " + IP + ":" + PORT + " ！";
					}
					catch (Exception e)
					{
						CommFactory.Message = e.Message;
						return;
					}
					var adapter = new NModbus.IO.TcpClientAdapter(_tcpClient);
					CommFactory.Master = modbusFactory.CreateRtuMaster(adapter);
					CommFactory.Master.Transport.ReadTimeout = 1000;
					CommFactory.Master.Transport.WriteTimeout = 1000;
					CommFactory.Master.Transport.Retries = 0;
					CommFactory.Master.Transport.WaitToRetryMilliseconds = 250;
				}
			}
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
				bool isOpen = this._tcpClient != null && this._tcpClient.Connected;
				if (isOpen)
				{
					try
					{
						this._tcpClient.Close();
						bool flag = !this._tcpClient.Connected;
						if (flag)
						{
							CommFactory.Message = "已关闭网络连接！";
						}
						_tcpClient = null;
					}
					catch
					{
						CommFactory.Message = "关闭网络连接失败！";
					}
				}
			}
		}

		private TcpClient _tcpClient;
		private string IP { get; set; }
		private int PORT { get; set; }
		public List<byte> Addr { get; set; }
		public MKSS.Model.Laohua.DataBus DataBus { get; set; }
		public MKSS.Model.Laohua.BoardCase BoardCase { get; set; }


		public event PropertyChangedEventHandler PropertyChanged;

		public BoardConnectionBoardCase(MKSS.Model.Laohua.BoardCase boardCase, MKSS.Model.Laohua.DataBus databus, List<byte> addr, AllConnection pool,int readspeed =200) :base(readspeed) {
			BoardCase = boardCase; DataBus = databus; Addr = addr;
			IP = databus.F_SocketIP;
			PORT = databus.F_SocketPort;
			this._tcpClient = new TcpClient();
			this.Pool = pool;
		}


		public override string ToString()
		{
			return string.Format("{0}_{1}_{2}", this.IP, this.PORT, this.IsOpen ? "T" : "F");
		}

	}

}
