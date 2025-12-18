using DeviceDataMonitorWPF.UIBiaoDing.Util;
using Stylet;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using DeviceDataMonitorWPF.UIBiaoDing.Config;
using System;
using DeviceDataMonitorWPF.UIBiaoDing;
using DeviceDataMonitorWPF.UIBiaodingJiuJing.Config;
using DeviceDataMonitorWPF.UIBiaodingJiuJing.Util;
using MKSS.Services;
using MKSS.Model;
using System.Windows.Controls;

namespace DeviceDataMonitorWPF.UIBiaodingJiuJing
{


    public partial class UILaoHuaGuanChaModel : Screen {

		public static UILaoHuaGuanChaModel Intance { get; private set; }
		public static int MaxBoardCount = 99;
		public static int BATCH_TYPE_NETWORK = 0;
		public static int TCPIP_TYPE_NETWORK = 1;
		public static int SERIPORTS_TYPE_NETWORK = 2;
		public static int READ_SPEED = 200;//10 秒钟读一次数据

		public int TxtPortsModeSelect { get; set; } = 0;
		public bool IS_BATCH_TYPE { get { return TxtPortsModeSelect == BATCH_TYPE_NETWORK; } }
		public string TxtTCPIP { get; set; } = "192.168.100.99:5000";
		public string TxtSerialPorts { get; set; }
		public string TxtProductList { get; set; }
		public ProductConfig TxtProductListObject { get; set; }
		public string TxtBoardCaseSelect { get; set; }
		public List<BoardCase> TxtBoardCaseSelectObject { get; set; }

		public int TxtVoltageValueBase { get; set; } = 20000;
		public int TxtVoltageValueAdd { get; set; } = 500;
		public int TxtVoltageValueMinus { get; set; } = 500;
		public string TxtSensorGrougAddress { get; set; } = "1-3";
		public string TxtSensorGrougAddressDesc { get; set; } = "各通道地址(例如：1-3,16-35)";
		string _TxtProjectTitle = null;
		public string TxtProjectTitle {
			get {
				return string.IsNullOrEmpty(_TxtProjectTitle) ? string.Format("{0}_{1}_{2}H_{3}",
					TxtProductList, TxtSensorGrougAddress, TxtTimeTotal, DateTime.Now.ToString("yyMMddHHmm")) : _TxtProjectTitle;
			}
			set { _TxtProjectTitle = value; }
		}

		public DateTime TxtTimeTotalStart { get; set; }
		public double TxtTimeTotal { get; set; } = 3.5;
		public string LabelMessage { get; set; }

		public bool CanTxtPortsModeSelect { get { return !SerialComIsOpen; } }
		public bool CanTxtSerialPorts { get { return !SerialComIsOpen; } }
		public bool CanTxtTCPIP { get { return !SerialComIsOpen; } }
		public bool CanTxtProductList { get { return !SerialComIsOpen; } }
		public bool CanTxtValueBase { get { return true; } }
		public bool CanTxtValueAdd { get { return true; } }
		public bool CanTxtValueMinus { get { return true; } }
		public bool CanTxtSensorGrougAddress { get { return !SerialComIsOpen && TxtPortsModeSelect != BATCH_TYPE_NETWORK; } }
		public bool CanTxtProjectTitle { get { return !SerialComIsOpen; } }
		public bool CanTxtTimeTotal { get { return !SerialComIsOpen; } }

		public bool CanBtnQuery { get { 
				return 
					AddressAvaliable 
					&& this.TxtProductListObject!=null;  } }

		public DateTime Start { get; set; } = DateTime.MinValue;
		public string StaTimeElapsed { get { if (Start == DateTime.MinValue) return "00:00:00"; return (DateTime.Now - Start).ToString("HH:mm:ss"); } }
		public int StaQuallified { get; set; }
		public int StaUnqualified { get; set; }
		public string StaRate { get; set; }
		public double StaTotal { get; set; }

		bool _SerialComIsOpen = false;
		public bool SerialComIsOpen {
			get {
				if (!this.IS_BATCH_TYPE)
				{
					return _SerialComIsOpen;
				}
				else {
					return  this.SerialModBusBoardCase[this.SelectBatchBoardTrue].Count(w => w.SerialComIsOpen) > 0;
				}
			}
			set {
				_SerialComIsOpen = value;
			}
		}

		public List<byte> Address
		{
			get;set;
		}

		/// <summary>
		///  重新计算地址
		/// </summary>
		public void CalcAddress() {
			List<byte> ret = new List<byte>();

			if (TxtPortsModeSelect != BATCH_TYPE_NETWORK)
			{

				#region 开发模式，非老化柜模式

				if (!string.IsNullOrEmpty(TxtSensorGrougAddress))
				{
					string src = TxtSensorGrougAddress;
					src = src.Replace("-", "-");
					string[] linge = src.Split(",".ToCharArray());
					foreach (var item in linge)
					{
						int itemInt = -1;
						if (int.TryParse(item, out itemInt))
						{
							if (itemInt <= 255)
							{
								if (!ret.Contains((byte)itemInt)) ret.Add((byte)itemInt);
							}
						}
						else
						{
							string[] from_to = item.Split("-".ToCharArray());
							if (from_to.Length == 2)
							{
								int itemIntFrom = -1;
								int itemIntTo = -1;
								if (int.TryParse(from_to[0], out itemIntFrom) && int.TryParse(from_to[1], out itemIntTo))
								{
									if (itemIntFrom <= 255 && itemIntTo <= 255)
									{
										for (int i = itemIntFrom; i <= itemIntTo; i++)
										{
											if (!ret.Contains((byte)i)) ret.Add((byte)i);
										}
									}
								}
							}
						}
					}
					ret = ret.OrderBy(w => w).ToList();

					if (ret.Count > 0)
					{
						if (ret.Count > MaxBoardCount)
						{
							ret = ret.GetRange(0, MaxBoardCount);
						}
						//改变要读取的地址范围
						if (this.SerialModBusDefault != null) SerialModBusDefault.Address = ret;
						for (int i = 0; i < this.BoardCaseTable.ProductModelGroupTable.Count; i++)
						{
							if (ret.Count - 1 >= i) this.BoardCaseTable.ProductModelGroupTable[i].Address = ret[i];
							else this.BoardCaseTable.ProductModelGroupTable[i].Address = 0;
							this.BoardCaseTable.ProductModelGroupTable[i].CaseNo = 0;
						}
						PageContext_Loaded(null, null);

						StringBuilder sb = new StringBuilder();
						foreach (var item in ret)
						{
							if (sb.Length < 6) sb.Append(item + ",");
						}
						TxtSensorGrougAddressDesc = string.Format("通道地址({1}等{0}个)", ret.Count, sb.ToString());
					}
					else
					{
						TxtSensorGrougAddressDesc = string.Format("各通道地址(例如：1-3,16-35)");
					}

				}
				#endregion

			}

			if (TxtPortsModeSelect == BATCH_TYPE_NETWORK)
			{

				#region 老化柜模式

				Dictionary<ModBusBoard, List<byte>> addr_ret = new Dictionary<ModBusBoard, List<byte>>();
				if (this.SelectBatchBoard != null)
				{
					foreach (Board item in this.SelectBatchBoardTrue.OrderBy(w => w.F_BoarCaseId + "" + w.F_FloorNO))
					{
						foreach (BoardCase casee in this.DBListBoardCase)
						{
							if (casee.F_BoardCaseId == item.F_BoarCaseId)
							{
								ModBusBoard _ModBusBoard = this.SerialModBusPool[casee.F_BoardCaseAddress, item.TunnelNO];
								if (!addr_ret.ContainsKey(_ModBusBoard)) addr_ret.Add(_ModBusBoard, new List<byte>());
								addr_ret[_ModBusBoard].AddRange(item.AddrList);
							}
						}
					}
				}

				Dictionary<byte, int> ret_case_no = new Dictionary<byte, int>();
				foreach (ModBusBoard modbus in addr_ret.Keys)
				{
					//改变要读取的地址范围
					modbus.Address = addr_ret[modbus];
					ret.AddRange(addr_ret[modbus]);
                    foreach (var item in addr_ret[modbus])
                    {
						if (!ret_case_no.ContainsKey(item)) ret_case_no.Add(item, modbus.BoardCaseNo);
					}
				}

				if (ret.Count > 0)
				{
					if (ret.Count > MaxBoardCount)
					{
						ret = ret.GetRange(0, MaxBoardCount);
					}

					for (int i = 0; i < this.BoardCaseTable.ProductModelGroupTable.Count; i++)
					{
						if (ret.Count - 1 >= i) { 
							this.BoardCaseTable.ProductModelGroupTable[i].Address = ret[i]; 
							this.BoardCaseTable.ProductModelGroupTable[i].CaseNo = ret_case_no[ret[i]];
						}
						else {
							this.BoardCaseTable.ProductModelGroupTable[i].Address = 0;
							this.BoardCaseTable.ProductModelGroupTable[i].CaseNo = 0;
						}
					}
					PageContext_Loaded(null, null);

					StringBuilder sb = new StringBuilder();
					foreach (var item in ret)
					{
						if (sb.Length < 6) sb.Append(item + ",");
					}
					TxtSensorGrougAddressDesc = string.Format("已选择老化柜，通道地址({1}等{0}个)", ret.Count, sb.ToString());
				}
				else
				{
					TxtSensorGrougAddressDesc = string.Format("已选择老化柜，各通道地址(例如：1-3,16-35)");
				}

				#endregion

			}
			Address = ret;
		}

		public void Closed()
		{
			this.BtnDisConnect();
		}

		public bool AddressAvaliable { get { return Address.Count > 0; } }


		public StringBuilder MessageList = new StringBuilder();
		private readonly Stylet.IWindowManager _windowManager;

		public UILaoHuaGuanCha PageContext { get; set; }
		public BoardServices BoardServices { get; set; } 
		public BoardCaseServices BoardCaseServices { get; set; }
		public BatchServices BatchServices { get; set; }
		public BatchRelServices BatchRelServices { get; set; }
		public SensorServices SensorServices { get; set; }
		public SensorDataServices SensorDataServices { get; set; }

		public List<Board> DBListBoard { get; set; }
		public List<Batch> DBListBatch { get; set; } 
		public List<BoardCase> DBListBoardCase { get; set; }
		public Dictionary<Batch, List<Board>> DBBatchDictionary { get; set; }
		public Dictionary<BoardCase, List<Board>> DBBoardCaseDictionary { get; set; }
		public Dictionary<Board, List<Sensor>> BoardSensorDictionary { get; set; }

		/// <summary>
		///   默认批次，存放开发模式数据
		/// </summary>
		public BoardCaseGroupTable BoardCaseTable { get; set; }
		/// <summary>
		///   连接池
		/// </summary>
		public ModBusBoardTable SerialModBusPool { get; set; }

		public ModBusBoard SerialModBusDefault
		{
			get
			{
				if (SerialModBusPool == null) return null;
				return SerialModBusPool[0, 0];
			}
		}



		public UILaoHuaGuanChaModel()
		{

			Intance = this;
			LoadConfig();
			SerialModBusPool = new ModBusBoardTable();
			BoardCaseTable = new BoardCaseGroupTable(); 


			BatchServices = new BatchServices();
			BoardServices = new BoardServices();
			BatchRelServices = new BatchRelServices();
			BoardCaseServices = new BoardCaseServices();
			SensorServices = new SensorServices();
			SensorDataServices = new SensorDataServices();

			DBListBoardCase = BoardCaseServices.Query<BoardCase>(null).Result;
			DBListBoard = BoardServices.Query<Board>(null).Result;
			DBListBatch = BatchServices.Query(wx=>wx.F_AgingStatus==1).Result;
			DBBoardCaseDictionary = new Dictionary<BoardCase, List<Board>>();
			foreach (BoardCase b in DBListBoardCase)
			{
				DBBoardCaseDictionary.Add(b, DBListBoard.Where(w => w.F_BoarCaseId == b.F_BoardCaseId).OrderBy(w => w.F_BoardId).ToList());
			}
			DBBatchDictionary = new Dictionary<Batch, List<Board>>();
            foreach (Batch item in DBListBatch)
            {
				DBBatchDictionary.Add(item, new List<Board>());
				List<BatchRel> rel = BatchRelServices.Query((w => w.F_BatchId == item.F_BatchId && w.F_UseStatus==1)).Result;
                foreach (var r in rel)
                {
					var b = DBListBoard.FirstOrDefault(wx => wx.F_BoardId == r.F_BoardId);
					if (b != null) DBBatchDictionary[item].Add(b);
				}
			}
			

		}

		/// <summary>
		///  初始化模式
		/// </summary>
		public void EnsureMode(int mode) {

			bool TCPIP_TYPE = mode == (UILaoHuaGuanChaModel.TCPIP_TYPE_NETWORK);
			bool BATCH_TYPE = mode == (UILaoHuaGuanChaModel.BATCH_TYPE_NETWORK);
			bool SERIPORTS_TYPE = mode == (UILaoHuaGuanChaModel.SERIPORTS_TYPE_NETWORK);
			this.TxtPortsModeSelect = mode;
			if (BATCH_TYPE)
			{
				if (this.PageContext != null)
				{
					UILaoHuaGuanChaC10 con10 = this.PageContext.UILaoHuaGuanChaC10;
					if (con10 != null)
					{
						con10.SetData(this.BoardCaseTable.ProductModelGroupTable.Values.ToList());
					}
				}

			}
			else {

				if (this.PageContext != null) {
					UILaoHuaGuanChaC10 con10 = this.PageContext.UILaoHuaGuanChaC10;
					if (con10 != null)
					{
						con10.SetData(this.BoardCaseTable.ProductModelGroupTable.Values.ToList());
					}
				}
				

			}

		}

		public Batch SelectBatch { get; private set; }
		public Dictionary<Board,bool> SelectBatchBoard {
			get;private set;
		}
		public List<Board> SelectBatchBoardTrue
		{
			get { 
				if (SelectBatchBoard == null) 
					return new List<Board>();
				return SelectBatchBoard.Where(wx => wx.Value).Select(w => w.Key).ToList(); 
			}
		}

		ModBusBoardListExt _SerialModBusBoardCase = null;
		public ModBusBoardListExt SerialModBusBoardCase { 
			get
			{
				if (_SerialModBusBoardCase == null) _SerialModBusBoardCase = new ModBusBoardListExt();
				return _SerialModBusBoardCase;
			}
		}

		/// <summary>
		///  选择批次时候，加载数据库数据
		/// </summary>
		public void SelectBatchEventByBatches(Batch b)
		{
			SelectBatch = b;

			List<Board> xxList = null;
			if (SelectBatch == null || SelectBatch.F_BatchId == 0) xxList= new List<Board>();
			xxList = DBBatchDictionary.ContainsKey(SelectBatch) ? DBBatchDictionary[SelectBatch] : new List<Board>();
			SelectBatchBoard = new Dictionary<Board, bool>();
            foreach (var item in xxList)
            {
				SelectBatchBoard.Add(item,true);

			}

			SerialModBusBoardCase.Clear();
			ModBusBoardListExt mxx = SerialModBusBoardCase;
			if ( this.SelectBatch != null
				&& this.SelectBatchBoard != null) {
				foreach (Board item in this.SelectBatchBoardTrue)
				{
					foreach (BoardCase casee in this.DBListBoardCase)
					{
						if(casee.F_BoardCaseId==item.F_BoarCaseId) mxx.Push(item, casee,this.SerialModBusPool);
					}
				}
			}
		}

		/// <summary>
		///  选择柜子时候，根据柜子构造批次
		/// </summary>
		public void SelectBatchEventByAddBoardCase(List<BoardCase> ccc)
		{
			SelectBatch = null;
			if(SelectBatchBoard==null) SelectBatchBoard = new Dictionary<Board, bool>();

			List<Board> all = new List<Board>();
			foreach (BoardCase cas in ccc)
			{
				foreach (var ba in DBListBoard)
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

			SerialModBusBoardCase.Clear();
			ModBusBoardListExt mxx = SerialModBusBoardCase;
			foreach (Board item in this.SelectBatchBoardTrue)
			{
				foreach (BoardCase casee in this.DBListBoardCase)
				{
					if (casee.F_BoardCaseId == item.F_BoarCaseId) mxx.Push(item, casee, this.SerialModBusPool);
				}
			} 

		}

		/// <summary>
		///  选择层
		/// </summary>
		/// <param name="_BoardCase"></param>
		/// <param name="bbs"></param>
		public void SelectBatchEventByAddBoard(BoardCase _BoardCase,List<Board> bbs) {

			foreach (Board item in this.SelectBatchBoard.Keys.ToList())
			{
				foreach (BoardCase casee in this.DBListBoardCase)
				{
					if (casee.F_BoardCaseId == item.F_BoarCaseId && _BoardCase== casee) {
						if (bbs.Contains(item)) { 
							this.SelectBatchBoard[item] = true; 
						}
						else { 
							this.SelectBatchBoard[item] = false;
						}
						
					}
				}
			}

			SerialModBusBoardCase.Clear();
			ModBusBoardListExt mxx = SerialModBusBoardCase;
			foreach (Board item in this.SelectBatchBoardTrue)
			{
				foreach (BoardCase casee in this.DBListBoardCase)
				{
					if (casee.F_BoardCaseId == item.F_BoarCaseId) mxx.Push(item, casee, this.SerialModBusPool);
				}
			}

		}



		public void InitPage(UILaoHuaGuanCha page)
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

		private void PageContext_Loaded(object sender, RoutedEventArgs e)
        {
			if (this.PageContext == null) return;
			 
			
			 
		}

        private void SM_MessageEvent(object sender, ModBusBoard.MessageEventArgs e)
		{
			this.MessageList.Insert(0, e.Message);
			LabelMessage = e.Message;

            try
            {
				string dir = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
				if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
				string filename = dir + DateTime.Now.ToString("yyyyMMddHH") + "_COMMAND.log";
				if (!System.IO.File.Exists(filename)) System.IO.File.Create(filename).Close();
				System.IO.File.AppendAllText(filename, DateTime.Now.ToString("HH:mm:ss") + ":" + e.Message + System.Environment.NewLine);
			}
            catch (Exception)
            {
				 
            }
			

		}

		public async void BtnConnect()
        {

			if (!IS_BATCH_TYPE)
			{

				SaveConfig();
				bool flag = TxtPortsModeSelect != (TCPIP_TYPE_NETWORK);
				if (flag)
				{
					this.SerialModBusDefault.Open(this.TxtSerialPorts);

					this.SerialModBusDefault.DataEventAging += this.SM_DataEvent;
					this.SerialModBusDefault.MessageEvent += this.SM_MessageEvent;

				}
				else
				{
					string str = this.TxtTCPIP;
					string[] arr = str.Split(":");
					if (arr.Length != 2)
					{
						SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("IP地址格式不正确，正确格式：192.168.100.99:5000")));
						return;
					}

					Ping pingSender = new Ping();
					PingReply reply = pingSender.Send(arr[0], 120);//第一个参数为ip地址，第二个参数为ping的时间
					if (reply.Status != IPStatus.Success)
					{
						SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("IP {0} 不通", arr[0])));
						return;
					}

					int port = 5000;
					if (!int.TryParse(arr[1], out port))
					{
						SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("端口 {0} 不是数字", arr[1])));
						return;
					}
					this.SerialModBusDefault.OpenTcp(arr[0], port);
					this.SerialModBusDefault.DataEventAging += this.SM_DataEvent;
					this.SerialModBusDefault.MessageEvent += this.SM_MessageEvent;

				}
				this.SerialComIsOpen = this.SerialModBusDefault.IsOpen;

			}
			else {

				SaveConfig();

				List<ModBusBoardConfig> conns = this.SerialModBusBoardCase[this.SelectBatchBoardTrue];
				 
                foreach (ModBusBoardConfig conn in conns)
                {

					Ping pingSender = new Ping();
					PingReply reply = pingSender.Send(conn.IP, 120);//第一个参数为ip地址，第二个参数为ping的时间
					if (reply.Status != IPStatus.Success)
					{
						SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("IP {0} 不通", conn.IP)));
						continue;
					}
					int port = conn.PORT;

			 
					await Task.Run(delegate ()
					{
						SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("正在连接 {0}:{1} ......", conn.IP, conn.PORT)));
						try
						{
							conn.ModBusBoard.OpenTcp(conn.IP, port);
							if (conn.ModBusBoard.IsOpen)
							{
								conn.ModBusBoard.DataEventAging += this.SM_DataEvent;
								conn.ModBusBoard.MessageEvent += this.SM_MessageEvent;
							}
							SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("连接 {0}:{1} {2}......", conn.IP, conn.PORT, conn.ModBusBoard.IsOpen ? "成功" : "失败")));
						}
						catch (Exception ex)
						{
							SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("端口 {0}:{1} 不通，{2}", conn.IP, conn.PORT, ex.Message)));
						}
					});




				}

				this.SerialComIsOpen = true;

			}
			this.PageContext.BtnQuery.IsEnabled = this.SerialComIsOpen;
			 

		}

        public void BtnDisConnect()
		{

			if (!IS_BATCH_TYPE)
			{

				this.SerialModBusDefault.DataEventAging -= this.SM_DataEvent;
				this.SerialModBusDefault.MessageEvent -= this.SM_MessageEvent;

				bool flag = TxtPortsModeSelect != (TCPIP_TYPE_NETWORK);
				Start = DateTime.MinValue;
				if (flag)
				{
					this.SerialModBusDefault.StopRead();
					this.SerialModBusDefault.Close();
					this.SerialComIsOpen = this.SerialModBusDefault.IsOpen;
				}
				else
				{
					this.SerialModBusDefault.StopRead();
					this.SerialModBusDefault.CloseTCPIP();
					this.SerialComIsOpen = this.SerialModBusDefault.IsOpen;
				}

			}
			else {

				List<ModBusBoardConfig> conns = this.SerialModBusBoardCase[this.SelectBatchBoardTrue];

				foreach (ModBusBoardConfig conn in conns) {

					Task t = new Task(() =>
					{
						SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("正在关闭 {0}:{1} ......", conn.IP, conn.PORT)));
						try
						{

							conn.ModBusBoard.DataEventAging -= this.SM_DataEvent;
							conn.ModBusBoard.MessageEvent -= this.SM_MessageEvent;

							bool flag = TxtPortsModeSelect != (TCPIP_TYPE_NETWORK);
							Start = DateTime.MinValue;
							conn.ModBusBoard.StopRead();
							conn.ModBusBoard.CloseTCPIP();

							SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("关闭 {0}:{1} 成功......", conn.IP, conn.PORT)));
						}
						catch (Exception ex)
						{
							SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("端口 {0}:{1} 关闭出错，{2}", conn.IP, conn.PORT, ex.Message)));
						}
					});
					t.Start();


				}
				this.SerialComIsOpen = false;


			}
			
		}
		  

		public async Task BtnExport()
		{
			SaveConfig(); 
		}
		public Batch OfBatch(DataAgingSensorGroupDataModel DataSrc)
		{
			if (DataSrc.Address <= 0 && DataSrc.CaseNo <= 0) return null;
			BoardCase _BoardCase = this.DBListBoardCase.FirstOrDefault(w => w.F_BoardCaseAddress == DataSrc.CaseNo);
			if (_BoardCase == null) return null;
			List<Board> list = DBBoardCaseDictionary[_BoardCase];
			Board _Board = list.FirstOrDefault(w => w.AddrList.Contains((byte)DataSrc.Address));
			if (_Board == null) return null;
			Batch _Batch = this.DBBatchDictionary.Where(wx => wx.Value.Contains(_Board)).Select(wx => wx.Key).FirstOrDefault();
			if (_Batch != null) return _Batch;
			return null;
		}
		public bool BtnFinishBoardExeEnabled(DataAgingSensorGroupDataModel DataSrc)
		{
			if (DataSrc.Address <= 0 && DataSrc.CaseNo <= 0) return false;
			BoardCase _BoardCase = this.DBListBoardCase.FirstOrDefault(w => w.F_BoardCaseAddress == DataSrc.CaseNo);
			if (_BoardCase == null) return false;
			List<Board> list = DBBoardCaseDictionary[_BoardCase];
			Board _Board = list.FirstOrDefault(w => w.AddrList.Contains((byte)DataSrc.Address));
			if (_Board == null) return false;
			Batch _Batch = this.DBBatchDictionary.Where(wx => wx.Value.Contains(_Board)).Select(wx => wx.Key).FirstOrDefault();
			if (_Batch == null) return false;
			bool ret = DataSrc.Address > 0 && DataSrc.CaseNo > 0 && _Batch!=null;
			return ret;
		}
		public void BtnFinishBoardExe(DataAgingSensorGroupDataModel DataSrc) 
		{
            try
            {
				BoardCase _BoardCase = this.DBListBoardCase.FirstOrDefault(w => w.F_BoardCaseAddress == DataSrc.CaseNo);
				if (_BoardCase != null && DBBoardCaseDictionary.ContainsKey(_BoardCase))
				{
					List<Board> list = DBBoardCaseDictionary[_BoardCase];
					Board _Board = list.FirstOrDefault(w => w.AddrList.Contains((byte)DataSrc.Address));
					Batch _Batch = this.DBBatchDictionary.Where(wx => wx.Value.Contains(_Board)).Select(wx=>wx.Key).FirstOrDefault();
					if (_Batch !=null && _Board != null)
					{

						//结束他
						_Board.F_LastUseUpdate = DateTime.Now;
						_Board.EnumUseType = EnumUseType.LaoHua;
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
                            //更新柜子可选状态
                            foreach (ListBoxItem item in this.PageContext.TxtBoardCaseList01.Items)
                            {
								BoardCase x = item.Content as BoardCase;
								if (x.F_BoardCaseId == _BoardCase.F_BoardCaseId) item.IsEnabled = true;
							}
							foreach (ListBoxItem item in this.PageContext.TxtBoardCaseList02.Items)
							{
								BoardCase x = item.Content as BoardCase;
								if (x.F_BoardCaseId == _BoardCase.F_BoardCaseId) item.IsEnabled = true;
							}
						}
						
						//判断批次是否需要结束
						rels = BatchRelServices.Query((w => w.F_BatchId == SelectBatch.F_BatchId)).Result;
						count = BoardServices.Query(wx => wx.F_UseStatus == 1).Result.Count;
						if (count == 0)
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
						SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("找不到老化柜{0}地址{1}", DataSrc.CaseNo, DataSrc.Address)));
					}
				}
				else
				{
					SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("找不到老化柜{0}", DataSrc.CaseNo)));
				}
			}
            catch (Exception ex)
            {
				SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("结束老化出错：{0}", ex.Message)));
			}
			
		}

		DateTime NextIdDate = DateTime.MinValue;
		int CurrentId = 0;
		public string NextId
        {
			get {
				if (DateTime.Now.DayOfYear != NextIdDate.DayOfYear) {
					NextIdDate = DateTime.Now;
					CurrentId = 0;
				}
				CurrentId++;
				return CurrentId.ToString("000000");
			}
		}
		public async Task BtnQuery()
		{

			this.BtnConnect();

			SaveConfig();
			if (!IS_BATCH_TYPE)
			{
				Start = DateTime.Now;
				TxtTimeTotalStart = DateTime.Now;
				this.TxtProjectTitle = TxtProjectTitle;
				await this.SerialModBusDefault.ReadVoltage();

				
			}
			else
			{


				Task t = new Task(() =>
				{
					try
					{

						 
						BoardSensorDictionary = new Dictionary<Board, List<Sensor>>();
						//根据选择开启批次任务
						Batch batch = new Batch()
						{
							F_ADAdd = this.TxtVoltageValueBase,
							F_ADBase = this.TxtVoltageValueAdd,
							F_ADMinus = this.TxtVoltageValueMinus,
							F_AgingStartTime = DateTime.Now,
							F_AgingEndTime = DateTime.Now.AddHours(this.TxtTimeTotal),
							F_AgingLastUpdateTime = DateTime.Now,
							EnumAgingStatus = EnumAgingStatus.InAging,
							F_BatchId = long.Parse(DateTime.Now.ToString("yyyyMMddHHmm") + NextId),
							F_BatchName = this.TxtProjectTitle,
							F_SensorName = TxtProductListObject.Name,
							F_SensorTypeName = TxtProductListObject.Name,
							F_SensorTypeId = (TxtProductListObject.Code)+"",
						};
						int res = BatchServices.Add(batch).Result;
						this.DBListBatch.Add(batch);
						this.SelectBatch = batch;
						this.DBBatchDictionary.Add(batch,new List<Board>());
						foreach (BoardCase item in TxtBoardCaseSelectObject)
						{
							foreach (Board b in this.DBBoardCaseDictionary[item])
							{
								bool v = this.SelectBatchBoard.ContainsKey(b) && SelectBatchBoard[b];
								if (v) {
									BatchRel rel = new BatchRel()
									{
										F_LastUseStart = DateTime.Now,
										F_LastUseUpdate = DateTime.Now,
										F_UseStatus = 1,
										F_BatchId = batch.F_BatchId,
										F_BoardId = b.F_BoardId
									};
									res = BatchRelServices.Add(rel).Result;
									b.EnumUseInFree = EnumUseInFree.InUse;
									b.EnumUseType = EnumUseType.LaoHua;
									b.F_LastUseSensorCount = 15;
									b.F_LastUseStart = DateTime.Now;
									b.F_LastUseUpdate = DateTime.Now;
									bool resx = BoardServices.Update(b).Result;
									this.DBBatchDictionary[batch].Add(b);
									//创建传感器
									BoardSensorDictionary.Add(b, new List<Sensor>());
                                    for (int i = 0; i < b.F_SensorCount; i++)
                                    {
										Sensor sen = new Sensor() {
											F_BatchId = batch.F_BatchId,
											F_BoardCaseId = item.F_BoardCaseId,
											F_BoardId = b.F_BoardId,
											F_SensorId = (DateTime.Now.ToString("yyMMddHHmm") + NextId),
											F_SensorName = TxtProductListObject.Name,
											F_SensorTypeId =  (TxtProductListObject.Code)+"",
											F_SensorTypeName = TxtProductListObject.Name, F_SlotNO= i+1
										};
										BoardSensorDictionary[b].Add(sen);
									}
									List<Sensor>  sens = BoardSensorDictionary[b].ToList();
									int xxas = this.SensorServices.Add(sens).Result;
								}
							}
							item.EnumUseInFree = EnumUseInFree.InUse;
							item.F_LastUseUpdate = DateTime.Now;
							bool resx1 = BoardCaseServices.Update(item).Result;
						}

						List<ModBusBoardConfig> conns = this.SerialModBusBoardCase[this.SelectBatchBoardTrue];
						foreach (ModBusBoardConfig conn in conns)
						{
							if(conn.SerialComIsOpen) 
								conn.ModBusBoard.ReadVoltage();
						}

					}
					catch (Exception ex)
					{
						SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("开始老化出错 {0} ......", ex.Message)));
					}
				});
				t.Start();
				
				
			}

			
		}

		void LoadConfig() { 
			this.TxtTCPIP = DataAgingConfig.Instance.TCPIP;
			this.TxtPortsModeSelect = DataAgingConfig.Instance.PortsModeSelect;
			this.TxtProductList= DataAgingConfig.Instance.ProductList;
			this.TxtSensorGrougAddress = DataAgingConfig.Instance.SensorGrougAddress;
			this.TxtSerialPorts= DataAgingConfig.Instance.SerialPorts;
			foreach (DataAgingProductConfig item in DataAgingConfig.Instance.Product)
			{
				if (item.Name == this.TxtProductList)
				{
					this.TxtVoltageValueAdd = item.VoltageValueAdd;
					this.TxtVoltageValueBase = item.VoltageValueBase;
					this.TxtVoltageValueMinus = item.VoltageValueMinus;
				}
			}
		}


		void SaveConfig() {
			DataAgingConfig.Instance.TCPIP = this.TxtTCPIP;
			DataAgingConfig.Instance.PortsModeSelect = this.TxtPortsModeSelect;
			DataAgingConfig.Instance.ProductList = this.TxtProductList;
			DataAgingConfig.Instance.SensorGrougAddress = this.TxtSensorGrougAddress;
			DataAgingConfig.Instance.SerialPorts = this.TxtSerialPorts;
            foreach (DataAgingProductConfig item in DataAgingConfig.Instance.Product)
            {
				if (item.Name == this.TxtProductList) {

					item.VoltageValueAdd = this.TxtVoltageValueAdd;
					item.VoltageValueBase = this.TxtVoltageValueBase;
					item.VoltageValueMinus = this.TxtVoltageValueMinus;

				}
			}
			DataAgingConfig.Save(); 
		}

        //Dictionary<long, Dictionary<int, Dictionary<int, Sensor>>> SaveSensorDataCache = new Dictionary<long, Dictionary<int, Dictionary<int, Sensor>>>();
        //bool SaveSensorData(DataAgingSensorGroupDataModel DataSrc)
        //{

        //    if (DataSrc.Address <= 0 && DataSrc.CaseNo <= 0) return false;
        //    BoardCase _BoardCase = this.DBListBoardCase.FirstOrDefault(w => w.F_BoardCaseAddress == DataSrc.CaseNo);
        //    if (_BoardCase == null) return false;
        //    List<Board> list = DBBoardCaseDictionary[_BoardCase];
        //    Board _Board = list.FirstOrDefault(w => w.AddrList.Contains((byte)DataSrc.Address));
        //    if (_Board == null) return false;
        //    Batch _Batch = this.DBBatchDictionary.Where(wx => wx.Value.Contains(_Board)).Select(wx => wx.Key).FirstOrDefault();
        //    if (_Batch == null) return false;

        //    if (!SaveSensorDataCache.ContainsKey(_Batch.F_BatchId)) SaveSensorDataCache.Add(_Batch.F_BatchId, new Dictionary<int, Dictionary<int, Sensor>>());
        //    if (!SaveSensorDataCache[_Batch.F_BatchId].ContainsKey(DataSrc.CaseNo)) SaveSensorDataCache[_Batch.F_BatchId].Add(DataSrc.CaseNo, new Dictionary<int, Sensor>());
        //    if (!SaveSensorDataCache[_Batch.F_BatchId][DataSrc.CaseNo].ContainsKey(DataSrc.Address))
        //    {
        //        Sensor _Sensor = this.se

        //        SaveSensorDataCache[_Batch.F_BatchId][DataSrc.CaseNo].Add(DataSrc.Address, xx);
        //    }
        //    List<MKSS.Model.SensorData> dbList = new List<MKSS.Model.SensorData>();
        //    MKSS.Model.SensorData ssd = new MKSS.Model.SensorData()
        //    {
        //        F_AddTime = DateTime.Now,
        //        F_SensorId =

        //            };
        //    dbList.Add(ssd);

        //}
        private void SM_DataEvent(object sender, ModBusBoard.DataEventAgingArgs e)
		{

			int BoardCaseNo = e.ProductData.Qualified.BoardCaseNo;
			int TunnelNo = e.ProductData.Qualified.TunnelNo;
			int bbaseV1 = this.TxtVoltageValueBase;
			ModBusBoardConfig config = ModBusBoardConfig.Of(BoardCaseNo, TunnelNo);
			ModBusBoard _SerialModBus = config == null ? this.SerialModBusDefault : config.ModBusBoard;
			if (bbaseV1 == 0) _SerialModBus.DataAging.CalcAvg(out bbaseV1);
			_SerialModBus.DataAging.SetQualifiedV1V7(bbaseV1, this.TxtVoltageValueMinus, this.TxtVoltageValueAdd);


			List<DataAgingSensorGroupDataModel> groups = BoardCaseTable.ProductModelGroupTable.Values.Where(w => w.Address == e.Address && w.CaseNo == BoardCaseNo).ToList();
			foreach (DataAgingSensorGroupDataModel group in groups)
			{
				group.LastUpdate = e.ProductData.Time;
				List<DataAgingSensorModel> DataList = group.ProductTable;
				for (int i = 0; i < 15; i++)
				{
					DataList[i].Fill(e.ProductData.SingleAddressData[i]);
				}

				if (group.Address > 0 && group.CaseNo > 0) {
					BoardCase _BoardCase = this.DBListBoardCase.FirstOrDefault(w => w.F_BoardCaseAddress == group.CaseNo);
					if (_BoardCase != null) {
						List<Board> list = DBBoardCaseDictionary[_BoardCase];
						Board _Board = list.FirstOrDefault(w => w.AddrList.Contains((byte)group.Address));
						if (_Board != null)
						{
							int start =  15 * _Board.AddrList.IndexOf((byte)group.Address);
							//插入数据库
							if (this.BoardSensorDictionary.ContainsKey(_Board))
							{
								List<Sensor> sens = this.BoardSensorDictionary[_Board];
								List<MKSS.Model.SensorData> datas = new List<MKSS.Model.SensorData>();
                                for (int i = 0; i < group.ProductTable.Count; i++)
                                {
									if (group.ProductTable[i].Value != null) {
										datas.Add(new MKSS.Model.SensorData()
										{
											F_AddTime = e.ProductData.Time,
											F_DataValue = group.ProductTable[i].Value.Value, F_SensorId = sens[start+i].F_SensorId
										});
									}
								}
								this.SensorDataServices.Add(datas);
							}
						}
					}
				}
       
			}
			this.StaQuallified = _SerialModBus.Qualified.StaQuallified;
			this.StaRate = _SerialModBus.Qualified.StaRate == double.NaN ? "" : _SerialModBus.Qualified.StaRate.ToString("P1");
			this.StaTotal = _SerialModBus.Qualified.StaTotal;
			this.StaUnqualified = _SerialModBus.Qualified.StaUnqualified;
			

            this.PageContext.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
					this.EnsureMode(TxtPortsModeSelect);
				}
                catch (System.Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    PageContext_Loaded(null, null);
                }

            }));


        }
		 

	}

}
