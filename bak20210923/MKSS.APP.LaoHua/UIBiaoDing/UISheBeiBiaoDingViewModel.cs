using MKSS.APP.UIBiaoDing.Util;
using Stylet;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;
using System.ComponentModel;
using MKSS.APP.UIBiaoDing.Config;
using MKSS.Model;
using MKSS.Services;
using MKSS.Util.Log;
using System.Windows.Threading;
using System.Threading;

namespace MKSS.APP.UIBiaoDing
{
	public class UISheBeiBiaoDingViewModel : Screen 
	{
		 
		public static UISheBeiBiaoDingViewModel Intance { get; private set; }
		public static int MaxBoardCount = 99;
		public static int READ_SPEED = 10;//200毫秒钟读一次数据
		
		public int TxtValueBase { get; set; } = 10000;
		public int TxtValueAdd { get; set; } = 100;
		public int TxtValueMinus { get; set; } = 100;
		public int TxtVoltageValueBase { get; set; } = 20000;
		public int TxtVoltageValueAdd { get; set; } = 500;
		public int TxtVoltageValueMinus { get; set; } = 500;
		public int TxtTimeTotal { get; set; } = 60;
		public int TxtTimeInterval { get; set; } = 1000;
		public int TxtZreo { get; set; }
		public int TxtSpan { get; set; }

		public string LabelMessage { get; set; }

		public bool CanTxtValueBase { get { return true; } }
		public bool CanTxtValueAdd { get { return true; } }
		public bool CanTxtValueMinus { get { return true; } }
		public bool CanTxtSensorGrougAddress { get { return !SerialComIsOpen; } }
		public bool CanTxtTimeTotal { get { return !SerialComIsOpen; } }
		public bool CanTxtTimeInterval { get { return !SerialComIsOpen; } }
		public bool CanTxtZreo { get { return !SerialComIsOpen; } }
		public bool CanTxtSpan { get { return !SerialComIsOpen; } }

		public bool CanBtnConnect { get { return !SerialComIsOpen; } }
		public bool CanBtnDisConnect { get { return SerialComIsOpen; } }
		public bool CanBtnSetZero { get { return SerialComIsOpen; } }
		public bool CanBtnSetSpan { get { return SerialComIsOpen; } }
		public bool CanBtnModeSelfReport { get { return SerialComIsOpen; } }
		public bool CanBtnModeQuery { get { return SerialComIsOpen; } }
		public bool CanBtnQuery { get { return SerialComIsOpen && AddressAvaliable ; } }
		public bool CanBtnQueryParameter { get { return SerialComIsOpen && AddressAvaliable; } }

		public DateTime Start { get; set; } = DateTime.MinValue;
		public string StaTimeElapsed { get { if (Start == DateTime.MinValue) return "00:00:00"; return (DateTime.Now - Start).ToString("HH:mm:ss"); } }  
		public int StaQuallified { get; set; }  
		public int StaUnqualified { get; set; }  
		public string StaRate { get; set; }  
		public double StaTotal { get; set; }  

		public bool IsWrite { get; set; }
		public bool SerialComIsOpen { get; set; }


		~UISheBeiBiaoDingViewModel() {
			this.BtnDisConnect();
		}

		public void Closed()
        {
			this.BtnDisConnect();
        }

        public bool AddressAvaliable { get { return this.ConnectionPool.Sum(w=>w.Value.Address.Count) > 0; } }

		public Dictionary<int, SensorGroupDataModel> ProductModelGroupTable { get; set; }

		public StringBuilder MessageList = new StringBuilder();
		private readonly Stylet.IWindowManager _windowManager;

		public UISheBeiBiaoDingView PageContext { get; set; }

		public List<Board> DBListBoard { get; set; }
		public List<BoardCase> DBListBoardCase { get; internal set; }
		public Dictionary<BoardCase, List<Board>> DBBoardCaseDictionary { get; set; }
		public List<BoardCase> TxtBoardCaseSelectObject { get; set; }
		DataBusServices _DataBusServices = new DataBusServices();
		BoardServices _BoardServices = new BoardServices();
		BoardCaseServices _BoardCaseServices = new BoardCaseServices();
		public Dictionary<Board, bool> SelectBoard
		{
			get; private set;
		}
		/// <summary>
		///   连接池
		/// </summary>
		Dictionary<long, ModBusBoardConnection> ConnectionPool = new Dictionary<long, ModBusBoardConnection>();


		public List<Board> SelectBatchBoardTrue
		{
			get
			{
				if (SelectBoard == null)
					return new List<Board>();
				return SelectBoard.Where(wx => wx.Value).Select(w => w.Key).ToList();
			}
		}
		 

		/// <summary>
		///  选择柜子时候，根据柜子构造批次
		/// </summary>
		public void SelectAddBoardCase(List<BoardCase> ccc)
		{
			if (SelectBoard == null) SelectBoard = new Dictionary<Board, bool>();

			List<Board> all = new List<Board>();
			foreach (BoardCase cas in ccc)
			{
				foreach (var ba in DBListBoard)
				{
					if (cas.F_BoardCaseId == ba.F_BoarCaseId)
					{
						all.Add(ba);
						if (!SelectBoard.ContainsKey(ba))
						{
							SelectBoard.Add(ba, false);
						}
					}
				}
			}
			foreach (var item in SelectBoard.Keys.ToArray())
			{
				if (!all.Contains(item)) SelectBoard.Remove(item);
			}
			RefreshPageAddr();

		}

		/// <summary>
		///  选择层
		/// </summary>
		/// <param name="_BoardCase"></param>
		/// <param name="bbs"></param>
		public void SelectAddBoard(List<Board> bbs)
		{

			foreach (Board item in this.SelectBoard.Keys.ToList())
			{
				foreach (BoardCase casee in DBListBoardCase)
				{
					if (casee.F_BoardCaseId == item.F_BoarCaseId)
					{
						if (bbs.Contains(item))
						{
							this.SelectBoard[item] = true;
						}
						else
						{
							this.SelectBoard[item] = false;
						}

					}
				}
			}
			RefreshPageAddr();

		}


		void RefreshPageAddr(){
			int addr = -1;
            foreach (var item in this.SelectBoard.Keys)
            {
				if (this.SelectBoard[item])
				{
					var ad = item.AddrList;
					addr++;
					this.ProductModelGroupTable[addr].Board = item;
					this.ProductModelGroupTable[addr].Address = ad[0];
					addr++;
					this.ProductModelGroupTable[addr].Board = item;
					this.ProductModelGroupTable[addr].Address = ad[1];
				}
			}
		} 


		public UISheBeiBiaoDingViewModel()
		{

			DBListBoardCase = _BoardCaseServices.Query<BoardCase>(null).Result;
			DBListBoard = _BoardServices.Query<Board>(null).Result; 
			DBBoardCaseDictionary = new Dictionary<BoardCase, List<Board>>();
			foreach (BoardCase b in DBListBoardCase)
			{
				DBBoardCaseDictionary.Add(b, DBListBoard.Where(w => w.F_BoarCaseId == b.F_BoardCaseId).OrderBy(w => w.F_BoardId).ToList());
			}


			ConnectionPool = new Dictionary<long, ModBusBoardConnection>();
			//初始连接池
			Dictionary<long, DataBus> _DataBusDic =
				_DataBusServices.QuerySql(string.Format("select * from pd_databus "))
				.Result.ToDictionary(w => w.F_DataBusId, w => w);
			foreach (var item in _DataBusDic.Values)
			{
				ModBusBoardConnection x = new ModBusBoardConnection(item);
				this.ConnectionPool.Add(item.F_DataBusId, x);
			}


			Intance = this;
			LoadConfig();
			ProductModelGroupTable = new Dictionary<int, SensorGroupDataModel>();
			for (int i = 0; i < MaxBoardCount; i++)
			{
				ProductModelGroupTable.Add(i, new SensorGroupDataModel() { Address = 0 }); 
			}
			ULogger.OnLog += this.SM_MessageEvent;
		}

		public void InitPage(UISheBeiBiaoDingView page)
		{
			PageContext = page;
			PageContext.Loaded += PageContext_Loaded;
		}

		public new UIElement View
		{
			get
			{
				return this.PageContext
	;
			}
		}

		public void PageContext_Loaded(object sender, RoutedEventArgs e)
        {
			if (this.PageContext == null) return;


			//刷新通信链路状态
			Dictionary<long, DataBus> _DataBusDic =
				_DataBusServices.QuerySql(string.Format("select * from pd_databus "))
				.Result.ToDictionary(w => w.F_DataBusId, w => w);
			foreach (var item in _DataBusDic.Values)
			{
				if (this.ConnectionPool.ContainsKey(item.F_DataBusId))
				{
					ModBusBoardConnection conn = this.ConnectionPool[item.F_DataBusId];
					conn.DataBus.F_UseStatus = item.F_UseStatus;
					conn.DataBus.F_UseStartTime = item.F_UseStartTime;
					conn.DataBus.F_UseEndTime = item.F_UseEndTime;
				}
			}


			UISheBeiBiaoDingC05 con = this.PageContext.UISheBeiBiaoDingC05;
			if (con != null)
			{
				con.SetData(ProductModelGroupTable.Values.ToList());
			}
			UISheBeiBiaoDingC10 con10 = this.PageContext.UISheBeiBiaoDingC10;
			if (con10 != null)
			{
				con10.SetData(ProductModelGroupTable.Values.ToList());
			}
			UISheBeiBiaoDingCQT conqt = this.PageContext.UISheBeiBiaoDingCQT;
			if (con10 != null)
			{
				conqt.SetData(ProductModelGroupTable.Values.ToList());
			}

		}

        private void SM_MessageEvent(object messsage)
		{
			this.MessageList.Insert(0, messsage.ToString());
			LabelMessage = messsage.ToString();
		}

		public void BtnConnect()
        {

			SaveConfig();
			try
			{

				RefreshPageAddr();
				this.BtnDisConnect();
				foreach (ModBusBoardConnection Connection in ConnectionPool.Values)
				{
					Connection.BoardList.Clear();
                    foreach (Board item in this.SelectBoard.Keys)
                    {
						if (this.SelectBoard[item] && item.DataBusList.Contains(Connection.DataBus.F_DataBusId)) {
							Connection.BoardList.Add(item,null);
							
						}
                    }
				}


				int count = 0; 
				foreach (ModBusBoardConnection Connection in ConnectionPool.Values)
				{
					{
						Task.Run(delegate ()
						{

							if (Connection.BoardList.Count == 0) return;

							if (Connection.IsOpen) return;
							Ping pingSender = new Ping();
							PingReply reply = pingSender.Send(Connection.IP, 120);//第一个参数为ip地址，第二个参数为ping的时间
							if (reply.Status != IPStatus.Success)
							{
								ULogger.Log(string.Format("IP {0} 不通", Connection.IP));
								return;
							}
							int port = Connection.PORT;


							DataBus ds = Connection.DataBus;
							if (Connection.DataBus.EnumUseStatus != DataBusUseStatus.None)
							{
								ULogger.Log(string.Format("通道 {0}:{1} 锁定占用中，{2} {3}", Connection.IP, Connection.PORT, ds.EnumUseStatus, ds.F_UseStartTime));
								return;
							}

							//ULogger.Log(string.Format("正在连接 {0}:{1} ......", Connection.IP, Connection.PORT));
							try
							{

								//占用通道
								ds.EnumUseStatus = DataBusUseStatus.BiaoDing;
								ds.F_UseStartTime = DateTime.Now;
								_DataBusServices.Update(ds);
								Connection.ModBusBoard.OpenTcp(Connection.IP, port);
								if (Connection.ModBusBoard.IsOpen)
								{
									Connection.ModBusBoard.DataEvent -= this.SM_DataEvent;
									Connection.ModBusBoard.DataEvent += this.SM_DataEvent;
								}
								//ULogger.Log(string.Format("连接 {0}:{1} {2}......", Connection.IP, Connection.PORT, Connection.ModBusBoard.IsOpen ? "成功" : "失败"));
							}
							catch (Exception ex)
							{
								ULogger.Log(string.Format("端口 {0}:{1} 不通，{2}", Connection.IP, Connection.PORT, ex.Message));
							}

							count++;
							this.View.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
								this.SerialComIsOpen = ConnectionPool.Values.Count(w => w.IsOpen) > 0;
							});

						});

					}
				}
				ULogger.Log(string.Format("正在连接......", count));


			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("连接出错 {0} ......", ex.Message));
			}

			this.SerialComIsOpen = ConnectionPool.Values.Count(w => w.IsOpen) > 0;

		}

        public void BtnDisConnect()
		{

			try
			{

				int count = 0;
				Start = DateTime.MinValue;
				foreach (ModBusBoardConnection Connection in ConnectionPool.Values)
				{
					Task.Run(delegate ()
					{

						if (!Connection.IsOpen) Connection.ModBusBoard.CloseTCPIP();
						Connection.ModBusBoard.DataEvent -= this.SM_DataEvent;
						this.View.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
							this.SerialComIsOpen = ConnectionPool.Values.Count(w => w.IsOpen) > 0;
						});

					});
				}

			}
			catch (Exception ex)
			{
				ULogger.Log(string.Format("关闭连接出错 {0} ......", ex.Message));
			}
			finally {
				foreach (ModBusBoardConnection Connection in ConnectionPool.Values)
				{
					Task.Run(delegate ()
					{
						 
						DataBus ds = Connection.DataBus;
						//释放通道
						ds.EnumUseStatus = DataBusUseStatus.None;
						ds.F_UseStartTime = DateTime.Now;
						_DataBusServices.Update(ds);

					});
				}
			}
			 
		}

        public async Task BtnSetZero()
		{
			SaveConfig();
			await Task.Run(delegate ()
			{
                foreach (var item in this.ConnectionPool.Values)
				{
					if (!item.ModBusBoard.IsOpen) continue;
					var SerialModBus = item.ModBusBoard;
					SerialModBus.SetZero(TxtZreo);
				}
			});
		}

		public async Task BtnSetSpan()
		{
			SaveConfig();
			await Task.Run(delegate ()
			{
				foreach (var item in this.ConnectionPool.Values)
				{
					if (!item.ModBusBoard.IsOpen) continue;
					var SerialModBus = item.ModBusBoard;
					SerialModBus.SetSpan(TxtSpan);
				}
			});
		}
		 

		public async Task BtnJudge()
		{

			SaveConfig();

			this.ReCalcQulify();

			foreach (var item in this.ConnectionPool.Values)
			{
				if (!item.ModBusBoard.IsOpen) continue;
				var SerialModBus = item.ModBusBoard;
				for (int i = 0; i < SerialModBus.Qualified.CurrentProductData.Count; i++)
				{
					SensorGroupData src = SerialModBus.Qualified.CurrentProductData[i];
					SensorGroupDataModel groupTarget = ProductModelGroupTable.Values.FirstOrDefault(w => w.Address == src.Address);
					if (groupTarget == null) continue;
					List<Util.SensormData> DataList = groupTarget.ProductTable;
					for (int j = 0; j < 15; j++)
					{
						DataList[j].IsQualifiedV1 = src.SingleAddressData[j].IsQualifiedV1;
						DataList[j].IsQualifiedV7 = src.SingleAddressData[j].IsQualifiedV7;
					}
				}
			}
			
			 
			this.ShowQulify();

			PageContext_Loaded(null, null);

		}

		public async Task BtnExport()
		{
			SaveConfig();
			//SerialModBus.Qualified.SaveCheckedDataToExcel();
		}

		public void BtnSetQualified()
		{

		}


		public async Task BtnModeSelfReport()
		{
			SaveConfig();
			await Task.Run(delegate ()
			{
				foreach (var item in this.ConnectionPool.Values)
				{
					if (!item.ModBusBoard.IsOpen) continue;
					var SerialModBus = item.ModBusBoard;
					SerialModBus.SetModeQuery(false);
				} 
			});
		}

		public async Task BtnModeQuery()
		{
			SaveConfig();
			await Task.Run(delegate ()
			{
				foreach (var item in this.ConnectionPool.Values)
				{
					if (!item.ModBusBoard.IsOpen) continue;
					var SerialModBus = item.ModBusBoard;
					SerialModBus.SetModeQuery(true);
				} 
			});
		}


		public bool BtnQueryIsRead { get; set; }
		public string BtnQueryText { get { return !BtnQueryIsRead ? "读取数据" : "停止读取"; } }
		public async Task BtnQuery()
		{

			foreach (var item in this.ConnectionPool.Values)
			{
				if (!item.ModBusBoard.IsOpen) continue;
				var SerialModBus = item.ModBusBoard;
				if (BtnQueryParameterIsRead)
				{
					SerialModBus.ReadAutoAdjustStatusContinue = false;
					SerialModBus.ReadLiangChengContinue = false;
					SerialModBus.ReadOutPutVolageContinue = false;
					SerialModBus.ReadVoltageRangeContinue = false;
					SerialModBus.ReadSerialNoContinue = false;
				}
				if (BtnQueryIsRead)
				{
					SerialModBus.ReadAllContinue = false;
					return;
				}
				this.PageContext.TabItemVoltage.IsSelected = true;
				SaveConfig();
				Start = DateTime.Now;
				this.BtnQueryIsRead = true;
				SerialModBus.ReadAllContinue = true;
				await SerialModBus.ReadAll();
				this.BtnQueryIsRead = SerialModBus.IsRead;
			}
			
		}

		public bool BtnQueryParameterIsRead { get; set; }
		public string BtnQueryParameterText { get { return !BtnQueryParameterIsRead ? "读取参数" : "停止读取"; } }


        public async Task BtnQueryParameter()
		{

			foreach (var item in this.ConnectionPool.Values)
			{
				if (!item.ModBusBoard.IsOpen) continue;
				var SerialModBus = item.ModBusBoard;
				if (BtnQueryIsRead)
				{
					SerialModBus.ReadAllContinue = false;
				}
				if (BtnQueryParameterIsRead)
				{
					SerialModBus.ReadAutoAdjustStatusContinue = false;
					SerialModBus.ReadLiangChengContinue = false;
					SerialModBus.ReadOutPutVolageContinue = false;
					SerialModBus.ReadVoltageRangeContinue = false;
					SerialModBus.ReadSerialNoContinue = false;
					return;
				}
				this.PageContext.TabItemParamenter.IsSelected = true;
				SaveConfig();
				this.BtnQueryParameterIsRead = true;
				SerialModBus.ReadAutoAdjustStatusContinue = true;
				SerialModBus.ReadLiangChengContinue = true;
				SerialModBus.ReadOutPutVolageContinue = true;
				SerialModBus.ReadVoltageRangeContinue = true;
				SerialModBus.ReadSerialNoContinue = true;
				await SerialModBus.ReadAutoAdjustStatus();
				await SerialModBus.ReadLiangCheng();
				await SerialModBus.ReadOutPutVolage();
				await SerialModBus.ReadVoltageRange();
				await SerialModBus.ReadSerialNo();
				this.BtnQueryParameterIsRead = SerialModBus.IsRead;
			}

			
		}

		void LoadConfig() {
			MKSS.APP.LaoHua.Config.ProductConfig item = MKSS.APP.LaoHua.Config.LaoHuaConfgig.Instance.Product.FirstOrDefault();
			if (item == null) return;
			this.TxtSpan = item.Span;
			this.TxtZreo = item.Zero;

			this.TxtValueAdd = item.ValueAdd;
			this.TxtValueBase = item.ValueBase;
			this.TxtValueMinus = item.ValueMinus;

			this.TxtVoltageValueAdd = item.VoltageValueAdd;
			this.TxtVoltageValueBase = item.VoltageValueBase;
			this.TxtVoltageValueMinus = item.VoltageValueMinus;
		}
		void SaveConfig() {

			MKSS.APP.LaoHua.Config.ProductConfig item = MKSS.APP.LaoHua.Config.LaoHuaConfgig.Instance.Product.FirstOrDefault();
			if (item == null) return;
			item.Span = this.TxtSpan;
			item.Zero = this.TxtZreo;

			item.ValueAdd = this.TxtValueAdd;
			item.ValueBase = this.TxtValueBase;
			item.ValueMinus = this.TxtValueMinus;

			item.VoltageValueAdd = this.TxtVoltageValueAdd;
			item.VoltageValueBase = this.TxtVoltageValueBase;
			item.VoltageValueMinus = this.TxtVoltageValueMinus;
			BiaoDingConfgig.Save(); 
		}


		private void SM_DataEvent(object sender, ModBusBoard.DataEventArgs e)
		{

			try
			{

				ReCalcQulify();//如有必要重算平均值

				List<SensorGroupDataModel> groups = ProductModelGroupTable.Values.Where(w => w.Address == e.Address).ToList();
                foreach (SensorGroupDataModel group in groups)
				{ 
					List<Util.SensormData> DataList = group.ProductTable;
					switch (e.ReadMode)
					{
						case ReadMode.RX01:
							for (int i = 0; i < 15; i++)
							{
                                try
                                {
									CopyEntity(e.ProductData.SingleAddressData[i], DataList[i]);
								}
                                catch (System.Exception xx)
                                {
                                    throw xx;
                                }
								
							}
							break;
						case ReadMode.RX9B_ReadLiangCheng:
							for (int j = 0; j < 15; j++)
							{
								DataList[j].LiangCheng = e.ProductData.SingleAddressData[j].LiangCheng;
							}
							break;
						case ReadMode.RXA5_ReadVoltageRange:
							for (int k = 0; k < 15; k++)
							{
								DataList[k].DianYaRange = e.ProductData.SingleAddressData[k].DianYaRange;
							}
							break;
						case ReadMode.RX7D_ReadAutoAdjustStatus:
							for (int l = 0; l < 15; l++)
							{
								DataList[l].AutoAdjustState = e.ProductData.SingleAddressData[l].AutoAdjustState;
							}
							break;
						case ReadMode.RX90_ReadSerialNo:
							for (int l = 0; l < 15; l++)
							{
								DataList[l].Serial = e.ProductData.SingleAddressData[l].Serial;
							}
							break;
						case ReadMode.RXA9_ReadOutPutVolage:
							for (int m = 0; m < 15; m++)
							{
								DataList[m].OutPutVolage = e.ProductData.SingleAddressData[m].OutPutVolage;
							}
							break;
					}
				}

				//获取最新统计结果
				ShowQulify();


			}
			catch (System.Exception ex)
			{
				throw ex;
			}
			finally {
				PageContext_Loaded(null, null);
			}

			

			
		}

		/// <summary>
		///   如有必要重算平均值
		/// </summary>
		void ReCalcQulify() {

			int bbaseV7_total = this.TxtValueBase;
			int bbaseV1_total = this.TxtVoltageValueBase;
			if (bbaseV7_total == 0 || bbaseV1_total == 0) 
				DataQualified.CalcAvg(
					this.ConnectionPool.Values.Select(w=>w.ModBusBoard.Qualified).ToList(), 
						out bbaseV1_total, out bbaseV7_total);

			foreach (var item in this.ConnectionPool.Values)
			{
				var SerialModBus = item.ModBusBoard;
				SerialModBus.Qualified.SetQualifiedV1V7(
						bbaseV7_total, this.TxtValueMinus, this.TxtValueAdd,
						bbaseV1_total, this.TxtVoltageValueMinus, this.TxtVoltageValueAdd);
			}

		}


		void ShowQulify()
		{

			//获取最新统计结果
			this.StaQuallified = this.ConnectionPool.Values.Sum(w=>w.ModBusBoard.Qualified.StaQuallified);
			this.StaTotal = this.ConnectionPool.Values.Sum(w => w.ModBusBoard.Qualified.StaTotal);
			double _HeGeRatio = (double)this.StaQuallified / (double)StaTotal;
			this.StaUnqualified = this.ConnectionPool.Values.Sum(w => w.ModBusBoard.Qualified.StaUnqualified);
			this.StaRate = _HeGeRatio == double.NaN ? "" : _HeGeRatio.ToString("P1");

		}

		void CopyEntity(object src,object target) {
            foreach (PropertyInfo item in src.GetType().GetProperties())
            {
				if(item.CanWrite) item.SetValue(target, item.GetValue(src, null), null);

			}
		}

	}
}
