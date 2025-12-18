using MKSS.APP.UIBiaoDing.Config;
using MKSS.APP.UIBiaoDing.Util;
using Microsoft.Win32;
using MKSS.APP.UserControls;
using MKSS.Model;
using MKSS.Service.LaoHua;
using MKSS.Service.UIBiaoDing;
using OfficeOpenXml.Style;
using Stylet;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace MKSS.APP.UIBiaoDing
{
    public class UISheBeiBiaoDingViewModel : Screen 
	{
		 
		public static UISheBeiBiaoDingViewModel Intance { get; private set; }
		public static int MaxBoardCount = 99;
		public static int CONN_TYPE_NETWORK { get { return BiaoDingConfgig.CONN_TYPE_NETWORK; } }
		public static int CONN_TYPE_SERIALPORT { get { return BiaoDingConfgig.CONN_TYPE_SERIALPORT; } }
		public static int CONN_TYPE_BOARDCASE { get { return BiaoDingConfgig.CONN_TYPE_BOARDCASE; } }
		public AllConnection ConnectionPool { get; set; }
		public BiaoDingService DbService = new BiaoDingService();
		public SerialNoService SerialNoFactory = new SerialNoService();

		/// <summary>
		///  对应生产任务单，可能 null
		/// </summary>
		public SerialNoRuleEntity SelectRuleEntity { get; set; }
		/// <summary>
		///  选择的柜子
		/// </summary>
		public UIBoardCaseSelectEntity BoardCaseSelectEntity { get; set; }
		/// <summary>
		///  没选择任务单时候显示 提醒叹号
		/// </summary>
		public  string BtnScrwdTipBadge { get { return SelectRuleEntity == null || string.IsNullOrEmpty(SelectRuleEntity.ProductFullName) ? "!" : ""; } }
		public int TxtPortsModeSelect { get; set; } = 0;
		public string TxtTCPIP { get; set; } = "192.168.100.99:5000";
		public string TxtSerialPorts { get; set; }
		public string TxtSerialPortsDesc
		{
			get
			{
				string msg = "采集端口";
				if (!string.IsNullOrEmpty(TxtSerialPorts))
				{
					HardWareInfo info = HardWareInfo.MulGetHardwareInfo(HardwareEnum.Win32_SerialPort, TxtSerialPorts);
					if (info != null && !string.IsNullOrEmpty(info.DescName))
					{
						msg = string.Format("采集端口（{0}）", info.DescName);
					}
				}
				return msg;
			}
		}


		public string TxtProductList { get; set; }
		public int TxtValueBase { get; set; } = 10000;
		public int TxtValueAdd { get; set; } = 100;
		public int TxtValueMinus { get; set; } = 100;
		public int TxtVoltageValueBase { get; set; } = 20000;
		public int TxtVoltageValueAdd { get; set; } = 500;
		public int TxtVoltageValueMinus { get; set; } = 500;
		 
		public string TxtSensorGrougAddress { get; set; } = "1-3";
		public string TxtSensorGrougAddressDesc { get; set; } = "各通道地址(例如：1-3,16-35)";
		public int TxtTimeTotal { get; set; } = 60;
		public int TxtTimeInterval { get; set; } = 1000;
		public int TxtZreo { get; set; }
		public int TxtSpan { get; set; }

		public bool StaReadSerialNo { get; set; } = false;
		public bool StaFilterEmpSata { get { return SensorDataFilter.Enabled; } set { SensorDataFilter.Enabled =value; } }
		public bool StaZeroSpanTitleExchange { get; set; }
		
		public string LabelMessage { get; set; }

		public bool CanTxtPortsModeSelect { get { return !ConnIsOpen; } }
		public bool CanTxtSerialPorts { get { return !ConnIsOpen; } }
		public bool CanTxtTCPIP { get { return !ConnIsOpen; } }
		public bool CanTxtProductList { get { return !ConnIsOpen; } }
		public bool CanTxtValueBase { get { return true; } }
		public bool CanTxtValueAdd { get { return true; } }
		public bool CanTxtValueMinus { get { return true; } }
		public bool CanTxtSensorGrougAddress { get { return !ConnIsOpen; } }
		public bool CanTxtTimeTotal { get { return !ConnIsOpen; } }
		public bool CanTxtTimeInterval { get { return !ConnIsOpen; } }
		public bool CanTxtZreo { get { return !ConnIsOpen; } }
		public bool CanTxtSpan { get { return !ConnIsOpen; } }

		public bool CanBtnConnect { get { return !ConnIsOpen; } }
		public bool CanBtnDisConnect { get { return ConnIsOpen; } }
		public bool CanBtnSetZero { get { return ConnIsOpen; } }
		public bool CanBtnSetSpan { get { return ConnIsOpen; } }
		public bool CanBtnModeSelfReport { get { return ConnIsOpen; } }
		public bool CanBtnModeQuery { get { return ConnIsOpen; } }
		public bool CanBtnQuery { get { return ConnIsOpen && AddressAvaliable && !BtnQueryParameterIsRead; } }
		public bool CanBtnQueryParameter { get { return ConnIsOpen && AddressAvaliable && !BtnQueryIsRead; } }
		public bool CanBtnSetSerialNo { get { return ConnIsOpen && AddressAvaliable; } }
		public bool CanBtnSetTime { get { return ConnIsOpen && AddressAvaliable; } }
		public bool CanBtnScrwd { get { return ConnIsOpen; } }
		

		public DateTime Start { get; set; } = DateTime.MinValue;
		public string StaTimeElapsed { get; set; }  
		public int StaQuallified { get; set; }  
		public int StaUnqualified { get; set; }  
		public string StaRate { get; set; }  
		public double StaTotal { get; set; }  

		public bool IsWrite { get; set; }
		public bool ConnIsOpen { get; set; }

		public List<Address> AddressList { get; set; } = new List<Address>();
		public List<Address> CalcAddress()
		{

			List<byte> ret = new List<byte>();
			List<Address> retAddr = new List<Address>();
			if (TxtPortsModeSelect != (CONN_TYPE_BOARDCASE))
			{
				string src = TxtSensorGrougAddress+"";
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
					retAddr = ret.Select(w => new Address(w, 0)).ToList();
					foreach (var item in this.ConnectionPool.Values)
					{
						item.Address = retAddr;
					}
					for (int i = 0; i < ProductModelGroupTable.Count; i++)
					{
						if (ret.Count - 1 >= i) ProductModelGroupTable[i].Address = retAddr[i];
						else ProductModelGroupTable[i].Address = new Address(0, 0);
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
			else {


				StringBuilder sb = new StringBuilder();
				var sel_case = this.BoardCaseSelectEntity.SelectBoardCaseSelect.Where(w => w.Value);
				if (sel_case.Count() == 0)
				{
					TxtSensorGrougAddress = "";
					TxtSensorGrougAddressDesc = string.Format("各通道地址(例如：1-3,16-35)");
				}
				else {
					foreach (var item in sel_case)
					{
						if (sb.Length < 30) sb.Append(item.Key.ToString() + ",");
					}
					TxtSensorGrougAddress = sb.ToString();
					TxtSensorGrougAddressDesc = string.Format("选择老化柜({1}等{0}个)", sel_case.Count(), sb.ToString());
				}
				 
				foreach (var item in this.ConnectionPool.Values)
				{
					BoardConnectionBoardCase cas_conn = item as BoardConnectionBoardCase;
					if (cas_conn == null) continue;
					List<byte> ret1 = cas_conn.Addr;
					//改变要读取的地址范围 
					List<Address> retAddr1 = ret1.Select(w => new Address(w, cas_conn.BoardCase.F_BoardCaseAddress)).ToList();
					item.Address = retAddr1;
					ret.AddRange(ret1);
					retAddr.AddRange(retAddr1);
				}

				retAddr = retAddr.OrderBy(w => w.APP * 100000 +w.V).ToList();
				if (retAddr.Count > 0)
				{
					  
					for (int i = 0; i < ProductModelGroupTable.Count; i++)
					{
						if (ret.Count - 1 >= i) ProductModelGroupTable[i].Address = retAddr[i];
						else ProductModelGroupTable[i].Address = new Address(0, 0);
					}
					PageContext_Loaded(null, null); 
					TxtSensorGrougAddressDesc = string.Format("选择老化柜({1}等{0}层)", this.ConnectionPool.Count, sb.ToString());

				}
				else
				{
					
				}


			}
			AddressList = retAddr;
			return retAddr;

		}

		public void Closed()
        {
			this.BtnDisConnect();
        }

        public bool AddressAvaliable { get { return AddressList.Count > 0; } }

		public Dictionary<int, SensorGroupDataModel> ProductModelGroupTable { get; set; }

		public SensorGroupDataModel GroupDataOf(int index) {
			if (ProductModelGroupTable == null 
				|| !ProductModelGroupTable.ContainsKey(index)) {
				return new SensorGroupDataModel();
			}
			return ProductModelGroupTable[index];
		}

		public SensorGroupDataModel GroupDataOfAddress(Address index)
		{
            foreach (SensorGroupDataModel item in ProductModelGroupTable.Values)
            {
				if (item.Address.V == index.V) return item;

			}
			return null;
		}

		public int StaSelectedMode { get; set; }
		public int StaSelectedAddr { get; set; }
		public int StaSelectedCount { get; set; }
		public int StaSelectedAverage { get; set; }
		Dictionary<int, List<SensorDataX>> SelectedData { get; set; }
		public void ResetSelectedData(int addr, List<SensorDataX> dat,int mode) {
			if (SelectedData == null) SelectedData = new Dictionary<int, List<SensorDataX>>();
			if (!SelectedData.ContainsKey(addr)) SelectedData.Add(addr, new List<SensorDataX>());
			else SelectedData[addr] = dat;
			StaSelectedAddr = addr;
			StaSelectedMode = mode;
			ResetCalcSelectedData();
		}

		public void ResetCalcSelectedData()
		{
			if (StaSelectedAddr == 0) return;
			if (SelectedData == null) SelectedData = new Dictionary<int, List<SensorDataX>>();
			StaSelectedCount = SelectedData.Where(k=>k.Key== StaSelectedAddr).Sum(w => w.Value.Where(x => !x.IsEmpData && !x.IsEmp170).Count());
			long total = SelectedData.Where(k => k.Key == StaSelectedAddr).Sum(w => w.Value.Where(x => !x.IsEmpData && !x.IsEmp170).Sum(v => v.V07.Value));
			if (StaSelectedCount == 0 || total == 0)
			{
				StaSelectedAverage = 0;
				return;
			}
			StaSelectedAverage = (int)((double)total / (double)StaSelectedCount);
			
		}

		public StringBuilder MessageList = new StringBuilder();
		private readonly Stylet.IWindowManager _windowManager;

		public UISheBeiBiaoDingView PageContext { get; set; }
		public MainView MainView { get; set; }
		public UISheBeiBiaoDingViewModel()
		{
			Intance = this;
			BoardCaseSelectEntity = new UIBoardCaseSelectEntity();
			LoadConfig();
			ConnectionPool = new  AllConnection() {  };
			ProductModelGroupTable = new Dictionary<int, SensorGroupDataModel>();
			for (int i = 0; i < MaxBoardCount; i++)
			{
				ProductModelGroupTable.Add(i, new SensorGroupDataModel() { Address = new Address(0,0) }); 
			}

			List<Address> addrList = UISheBeiBiaoDingViewModel.Intance.CalcAddress();//必须调用一次，计算内存表
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


		public void PageContextLoadedByAddress(Address addr)
		{
			if (this.PageContext == null) return;

			this.PageContext.Dispatcher.Invoke(() =>
			{
				UISheBeiBiaoDingC05 con = this.PageContext.UISheBeiBiaoDingC05;
				if (con != null)
				{
					con.RefreshData(addr);
				}
				UISheBeiBiaoDingC10 con10 = this.PageContext.UISheBeiBiaoDingC10;
				if (con10 != null)
				{
					con10.RefreshData(addr);
				}
				UISheBeiBiaoDingCQT conqt = this.PageContext.UISheBeiBiaoDingCQT;
				if (con10 != null)
				{
					conqt.RefreshData(addr);
				}
			});

		}

		public void PageContext_Loaded(object sender, RoutedEventArgs e)
        {
			if (this.PageContext == null) return;

			this.PageContext.Dispatcher.Invoke(() =>
			{
				UISheBeiBiaoDingC05 con = this.PageContext.UISheBeiBiaoDingC05;

				SerialNoConfig config = this.SerialNoFactory.SerialNoBtnConfig;
				this.PageContext.BtnScrwd.Content = config.BtnTxt;
				this.PageContext.BtnScrwd.ToolTip = config.BtnTootip;

				if (con != null)
				{
					con.RefreshData();
				}
				UISheBeiBiaoDingC10 con10 = this.PageContext.UISheBeiBiaoDingC10;
				if (con10 != null)
				{
					con10.RefreshData();
				}
				UISheBeiBiaoDingCQT conqt = this.PageContext.UISheBeiBiaoDingCQT;
				if (con10 != null)
				{
					conqt.RefreshData();
				}
			});
			
		}

        private void SM_MessageEvent(object sender, CommFactory.MessageEventArgs e)
		{
			this.MessageList.Insert(0, e.Message);
			LabelMessage = e.Message;
			MessageLogCore(e.Message);
		}

		static object locked = new object();
		private const int LevelColumnSize = 15;
		private static readonly string BlankHeader = Environment.NewLine + new string(' ', LevelColumnSize);
		protected void MessageLogCore(string message)
		{
			message = message?.Replace(Environment.NewLine, BlankHeader);
			try
			{
				lock (locked)
				{
					string n = System.Threading.Thread.CurrentThread.Name;
					string dir = AppDomain.CurrentDomain.BaseDirectory + "\\Log\\" + DateTime.Now.ToString("yyyyMMdd") + "\\";
					if (!System.IO.Directory.Exists(dir)) System.IO.Directory.CreateDirectory(dir);
					string filename = dir + DateTime.Now.ToString("HH") + "_" + n + "_INFO.log";
					if (!System.IO.File.Exists(filename)) System.IO.File.Create(filename).Close();
					System.IO.FileStream fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Append, System.IO.FileAccess.Write);
					System.IO.StreamWriter sw = new System.IO.StreamWriter(fileStream, Encoding.Default);
					sw.Write(DateTime.Now.ToString("HH:mm:ss.fff") + ":" + message + System.Environment.NewLine);
					sw.Close();
					sw.Dispose();
					fileStream.Close();
					fileStream.Dispose();
				}
			}
			catch (Exception)
			{

			}
		}

		public void BtnConnect()
        {

			SaveConfig();
			this.ConnectionPool.Clear();
			if (TxtPortsModeSelect == (CONN_TYPE_SERIALPORT))
			{
				this.ConnectionPool.Push(new BoardConnectionSreial(this.TxtSerialPorts,this.ConnectionPool, BiaoDingConfgig.Instance.RefreshInterval));
			}

			if (TxtPortsModeSelect == (CONN_TYPE_NETWORK))
			{

				string str = this.TxtTCPIP;
				string[] arr = str.Split(':');
				if (arr.Length != 2)
				{
					SM_MessageEvent(null, new CommFactory.MessageEventArgs(string.Format("IP地址格式不正确，正确格式：192.168.100.99:5000")));
					return;
				}

				Ping pingSender = new Ping();
				PingReply reply = pingSender.Send(arr[0], 120);//第一个参数为ip地址，第二个参数为ping的时间
				if (reply.Status != IPStatus.Success)
				{
					SM_MessageEvent(null, new CommFactory.MessageEventArgs(string.Format("IP {0} 不通", arr[0])));
					return;
				}

				int port = 5000;
				if (!int.TryParse(arr[1], out port))
				{
					SM_MessageEvent(null, new CommFactory.MessageEventArgs(string.Format("端口 {0} 不是数字", arr[1])));
					return;
				}
				this.ConnectionPool.Push(new BoardConnectionNet(arr[0], port, this.ConnectionPool,BiaoDingConfgig.Instance.RefreshInterval));
				 
			}


			if (TxtPortsModeSelect == (CONN_TYPE_BOARDCASE))
			{

				List<MKSS.Model.Laohua.Board> BoardList = this.BoardCaseSelectEntity.SelectBoard.Where(w => w.Value).Select(w => w.Key).ToList();
				Dictionary<long, DataBusExt> DataBusList = this.BoardCaseSelectEntity.GetDataBusOf(BoardList);
                foreach (var item in DataBusList.Values)
                {
					if (item.DataBus == null) continue;
					string[] arr = new string[] { item.DataBus.F_SocketIP, item.DataBus.F_SocketPort + "" };

					Ping pingSender = new Ping();
					PingReply reply = pingSender.Send(arr[0], 120);//第一个参数为ip地址，第二个参数为ping的时间
					if (reply.Status != IPStatus.Success)
					{
						SM_MessageEvent(null, new CommFactory.MessageEventArgs(string.Format("IP {0} 不通", arr[0])));
						continue;
					}

					int port = 5000;
					if (!int.TryParse(arr[1], out port))
					{
						SM_MessageEvent(null, new CommFactory.MessageEventArgs(string.Format("端口 {0} 不是数字", arr[1])));
						continue;
					}
					this.ConnectionPool.Push(new BoardConnectionBoardCase(item.BoardCase, item.DataBus, item.Address, this.ConnectionPool, BiaoDingConfgig.Instance.RefreshInterval));
				}
				

			}


			List<Address> addrList = UISheBeiBiaoDingViewModel.Intance.CalcAddress();//必须调用一次，计算内存表

			foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.DataEvent -= this.SM_DataEvent;
			foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.MessageEvent -= this.SM_MessageEvent;
			foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.DataEvent += this.SM_DataEvent;
			foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.MessageEvent += this.SM_MessageEvent;

			foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.Open();

			this.ConnIsOpen = this.ConnectionPool.IsOpen;
			if (ConnIsOpen)
			{
				//foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.WriteSignalClearNoSleep();
				DbService.StartBatch();
			}

		}

        public void BtnDisConnect()
		{

			foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.DataEvent -= this.SM_DataEvent;
			foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.MessageEvent -= this.SM_MessageEvent;
			 
			foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadAutoAdjustStatusContinue = false;
			foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadLiangChengContinue = false;
			foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadOutPutVolageContinue = false;
			foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadVoltageRangeContinue = false;
			foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadSerialNoContinue = false;
			foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadAllContinue = false;

			this.BtnQueryParameterIsRead = false;
			this.BtnQueryIsRead = false;

			Start = DateTime.MinValue; 
			DbService.StopBatch();
			foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.StopRead();
			foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.Close();
			this.ConnIsOpen = this.ConnectionPool.IsOpen;

			//清空所有值
			for (int i = 0; i < this.ProductModelGroupTable.Count; i++)
			{
				SensorGroupDataModel mm = ProductModelGroupTable[i];
				for (int j = 0; j < mm.ProductTable.Count; j++)
				{
					mm.ProductTable[j].Reset();
				}
			}

		}

		public async Task BtnScrwd()
		{
			Window ww = this.SerialNoFactory.SerialNoWindow();
			ww.Owner = UISheBeiBiaoDingViewModel.Intance.MainView;
			ww.ShowInTaskbar = false;
			ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
			bool? ret = ww.ShowDialog();
		}


		public async Task BtnSetZero()
		{
			SaveConfig();
			WaitWindow.ShowWindow("正在设置零点", "正在设置零点......", PageContext);
			UISheBeiBiaoDingViewModel.Intance.ConnectionPool.SetZreo(TxtZreo);
		}

		public async Task BtnSetSpan()
		{
			SaveConfig();
			WaitWindow.ShowWindow("正在设置Span点", "正在设置Span点......", PageContext);
			UISheBeiBiaoDingViewModel.Intance.ConnectionPool.SetSpan(TxtSpan);
		}


		public async Task BtnJudge()
		{

			SaveConfig();
			int bbaseV7 = this.TxtValueBase;
			int bbaseV1 = this.TxtVoltageValueBase;
			if (bbaseV7 == 0 || bbaseV1 == 0) ConnectionPool.Qualified.CalcAvg(out bbaseV1, out bbaseV7);
			ConnectionPool.Qualified.SetQualifiedV1V7(
				bbaseV7, this.TxtValueMinus, this.TxtValueAdd,
				bbaseV1, this.TxtVoltageValueMinus, this.TxtVoltageValueAdd);

			foreach (var conn in this.ConnectionPool.Values.ToList()) {
				foreach (SensorGroupData src in this.ConnectionPool.Qualified.CurrentProductData.Values.ToArray())
				{
					SensorGroupDataModel groupTarget = ProductModelGroupTable.Values.FirstOrDefault(w => w.Address == src.Address);
					if (groupTarget == null) continue;
					List<SensorDataX> DataList = groupTarget.ProductTable;
					for (int j = 0; j < 15; j++)
					{
						DataList[j].IsQualifiedV1 = src.SingleAddressData[j].IsQualifiedV1;
						DataList[j].IsQualifiedV7 = src.SingleAddressData[j].IsQualifiedV7;
					}
				}
			}

			this.StaQuallified = ConnectionPool.Qualified.StaQuallified;
			this.StaRate = ConnectionPool.Qualified.StaRate == double.NaN ? "" : ConnectionPool.Qualified.StaRate.ToString("P1");
			this.StaTotal = ConnectionPool.Qualified.StaTotal; 
			this.StaUnqualified = ConnectionPool.Qualified.StaUnqualified;

			PageContext_Loaded(null, null);

		}

		public async Task BtnExportHistoryData() {
			SaveConfig();
			{
				string name = DateTime.Now.ToString("历史数据yyyyMMddHHmmss");
				var dlg = new SaveFileDialog()
				{
					Title = "标定成果-另存为",
					DefaultExt = "txt",
					Filter = "Text files (*.xlsx)|*.xlsx|All files|*.*",
					FileName = name
				};
				if (dlg.ShowDialog() == true)
				{

					if (this.PageContext.TabControlMain.SelectedItem == this.PageContext.TabItemVoltage10)
					{
						try
						{
							//ConnectionPool.Qualified.SaveCheckedDataToExcel(dlg.FileName);

							ExcuteToExcel ETE = new ExcuteToExcel("标定成果");
							ETE.SetCellValue<string>(1, 1, "位置");
							ETE.SetCellValue<string>(1, 2, "信号AD");
							ETE.SetCellValue<string>(1, 3, "零点AD");
							ETE.SetCellValue<string>(1, 4, "SPAN点AD");
							ETE.SetCellValue<string>(1, 5, "预留1");
							ETE.SetCellValue<string>(1, 6, "预留2");
							ETE.SetCellValue<string>(1, 7, "预留3");
							ETE.SetCellValue<string>(1, 8, "浓度");
							ETE.SetCellValue<string>(1, 9, "温度1");
							ETE.SetCellValue<string>(1, 10, "温度2");
							ETE.SetCellValue<string>(1, 11, "模拟电压");
							ETE.SetCellValue<string>(1, 12, "是否合格");

							int rowno = 1;
							for (int i = 0; i < this.ProductModelGroupTable.Count; i++)
							{
								SensorGroupDataModel mm = ProductModelGroupTable[i];
								if (mm.Address.V == 0) continue;
								if (mm.Empty) continue;
								for (int j = 0; j < mm.ProductTable.Count; j++)
								{
									rowno++;
									ETE.SetCellValue<string>(rowno, 1, string.Format("{0}-{1}", this.ProductModelGroupTable[i].Address, this.ProductModelGroupTable[i].ProductTable[j].Position));
									ETE.SetCellValue<int>(rowno, 2, this.ProductModelGroupTable[i].ProductTable[j].V01.Value);
									ETE.SetCellValue<int>(rowno, 3, this.ProductModelGroupTable[i].ProductTable[j].V02.Value);
									ETE.SetCellValue<int>(rowno, 4, this.ProductModelGroupTable[i].ProductTable[j].V04.Value);
									ETE.SetCellValue<int>(rowno, 5, this.ProductModelGroupTable[i].ProductTable[j].V03.Value);
									ETE.SetCellValue<int>(rowno, 6, this.ProductModelGroupTable[i].ProductTable[j].V05.Value);
									ETE.SetCellValue<int>(rowno, 7, this.ProductModelGroupTable[i].ProductTable[j].V06.Value);
									ETE.SetCellValue<int>(rowno, 8, this.ProductModelGroupTable[i].ProductTable[j].V07.Value);
									ETE.SetCellValue<int>(rowno, 9, this.ProductModelGroupTable[i].ProductTable[j].V08.Value);
									ETE.SetCellValue<int>(rowno, 10, this.ProductModelGroupTable[i].ProductTable[j].V09.Value);
									ETE.SetCellValue<int>(rowno, 11, this.ProductModelGroupTable[i].ProductTable[j].V10.Value);
									var Qualified = ConnectionPool.Qualified[this.ProductModelGroupTable[i].Address.APP, this.ProductModelGroupTable[i].Address.V, this.ProductModelGroupTable[i].ProductTable[j].Position];
									ETE.SetCellValue<string>(rowno, 12, Qualified == null || Qualified.Value ? "" : "X");
								}
							}

							ETE.AllCellsAlignment(ExcelHorizontalAlignment.Center);
							ETE.SetRowStyleOfFontToBold(1);
							string Year = DateTime.Now.Year.ToString();
							string Month = DateTime.Now.Month.ToString();
							string Day = DateTime.Now.Day.ToString();
							ETE.FilePath = dlg.FileName;
							ETE.SaveAsExcel();
							ETE.Dispose();

							MessageBox.Show("导出成功。");

						}
						catch (System.Exception ex)
						{
							MessageBox.Show(ex.Message, "导出出错");
						}
					}
					if (this.PageContext.TabControlMain.SelectedItem == this.PageContext.TabItemVoltage)
					{
						try
						{
							//ConnectionPool.Qualified.SaveCheckedDataToExcel(dlg.FileName);

							ExcuteToExcel ETE = new ExcuteToExcel("标定成果");
							ETE.SetCellValue<string>(1, 1, "位置");
							ETE.SetCellValue<string>(1, 2, "电压AD");
							ETE.SetCellValue<string>(1, 3, "零点AD");
							ETE.SetCellValue<string>(1, 4, "SPAN点AD");
							ETE.SetCellValue<string>(1, 5, "浓度");
							ETE.SetCellValue<string>(1, 6, "温度1");
							ETE.SetCellValue<string>(1, 7, "是否合格");

							int rowno = 1;
							for (int i = 0; i < this.ProductModelGroupTable.Count; i++)
							{
								SensorGroupDataModel mm = ProductModelGroupTable[i];
								if (mm.Address.V == 0) continue;
								if (mm.Empty) continue;
								for (int j = 0; j < mm.ProductTable.Count; j++)
								{
									rowno++;
									ETE.SetCellValue<string>(rowno, 1, string.Format("{0}-{1}", this.ProductModelGroupTable[i].Address, this.ProductModelGroupTable[i].ProductTable[j].Position));
									ETE.SetCellValue<int>(rowno, 2, this.ProductModelGroupTable[i].ProductTable[j].V01.Value);
									ETE.SetCellValue<int>(rowno, 3, this.ProductModelGroupTable[i].ProductTable[j].V02.Value);
									ETE.SetCellValue<int>(rowno, 4, this.ProductModelGroupTable[i].ProductTable[j].V04.Value);
									ETE.SetCellValue<int>(rowno, 5, this.ProductModelGroupTable[i].ProductTable[j].V07.Value);
									ETE.SetCellValue<int>(rowno, 6, this.ProductModelGroupTable[i].ProductTable[j].V08.Value);
									var Qualified = ConnectionPool.Qualified[this.ProductModelGroupTable[i].Address.APP,this.ProductModelGroupTable[i].Address.V, this.ProductModelGroupTable[i].ProductTable[j].Position];
									ETE.SetCellValue<string>(rowno, 7, Qualified == null || Qualified.Value ? "" : "X");
								}
							}
							ETE.AllCellsAlignment(ExcelHorizontalAlignment.Center);
							ETE.SetRowStyleOfFontToBold(1);
							string Year = DateTime.Now.Year.ToString();
							string Month = DateTime.Now.Month.ToString();
							string Day = DateTime.Now.Day.ToString();
							ETE.FilePath = dlg.FileName;
							ETE.SaveAsExcel();
							ETE.Dispose();

							MessageBox.Show("导出成功。");
						}
						catch (System.Exception ex)
						{
							MessageBox.Show(ex.Message, "导出出错");
						}
					}

				}
			}
		}

		public async Task BtnExportHistoryDataSelect()
		{

			//var v5 = (this.PageContext.TabControlMain.SelectedItem == this.PageContext.TabItemVoltage);
			//var v10 = (this.PageContext.TabControlMain.SelectedItem == this.PageContext.TabItemVoltage10);
			//ExcuteToExcel ETE = new ExcuteToExcel();
			//var CurrentProductData = ConnectionPool.Qualified.CurrentProductData;
   //         foreach (var index in this.ProductModelGroupTable.Keys)
   //         {
			//	var data = ProductModelGroupTable[index];
			//	if (data.Address == 0) continue;
			//	var addr = data.Address;
			//	//var addr = data.Empty;
			//	ETE.CreateSheet("地址"+ addr);
			//	if (v5) {

			//		ETE.SetCellValue<string>(1, 1, "产品位置");
			//		ETE.SetCellValue<string>(1, 2, "C1(84-23)");
			//		ETE.SetCellValue<string>(1, 3, "C2(84-45)");
			//		ETE.SetCellValue<string>(1, 4, "C4(85-23)");
			//		ETE.SetCellValue<string>(1, 5, "C7(86-23)");
			//		ETE.SetCellValue<string>(1, 6, "C8（86-5）");
			//		ETE.SetCellValue<string>(1, 7, "是否合格");

			//	}
				
			//}
   //         for (int x = 0; x < this.ProductModelGroupTable.Count; x++)
   //         {

   //         }
			//for (int i = 0; i < CurrentProductData.Count; i++)
			//{
			//	for (int j = 0; j < 15; j++)
			//	{
			//		ETE.SetCellValue<string>(i * 15 + j + 2, 1, string.Format("{0}->{1}", CurrentProductData[i].Address, CurrentProductData[i].SingleAddressData[j].Position));
			//		ETE.SetCellValue<int>(i * 15 + j + 2, 2, CurrentProductData[i].SingleAddressData[j].V01.Value);
			//		ETE.SetCellValue<int>(i * 15 + j + 2, 3, CurrentProductData[i].SingleAddressData[j].V02.Value);
			//		ETE.SetCellValue<int>(i * 15 + j + 2, 4, CurrentProductData[i].SingleAddressData[j].V04.Value);
			//		ETE.SetCellValue<int>(i * 15 + j + 2, 5, CurrentProductData[i].SingleAddressData[j].V07.Value);
			//		ETE.SetCellValue<int>(i * 15 + j + 2, 6, CurrentProductData[i].SingleAddressData[j].V08.Value);
			//		//ETE.SetCellValue<string>(i * 15 + j + 2, 7, CurrentProductData[i].SingleAddressData[j].IsQualified.ToString());
			//		//if (CurrentProductData[i].SingleAddressData[j].IsQualified != null
			//		//	&&
			//		//	!CurrentProductData[i].SingleAddressData[j].IsQualified.Value)
			//		//{
			//		//	ETE.SetCellBackColor(i * 15 + j + 2, 7, System.Drawing.Color.LightPink);
			//		//}
			//	}
			//}
			//ETE.AllCellsAlignment(ExcelHorizontalAlignment.Center);
			//ETE.SetRowStyleOfFontToBold(1);
			//string Year = DateTime.Now.Year.ToString();
			//string Month = DateTime.Now.Month.ToString();
			//string Day = DateTime.Now.Day.ToString();
			//bool flag2 = !Directory.Exists(string.Concat(new string[]
			//{
			//	Environment.GetFolderPath(Environment.SpecialFolder.Desktop) ,
			//	"\\判定结果\\",
			//	Year,
			//	"年\\",
			//	Month,
			//	"月\\",
			//	Day,
			//	"日"
			//}));
			//if (flag2)
			//{
			//	Directory.CreateDirectory(string.Concat(new string[]
			//	{
			//		Environment.GetFolderPath(Environment.SpecialFolder.Desktop) ,
			//		"\\判定结果\\",
			//		Year,
			//		"年\\",
			//		Month,
			//		"月\\",
			//		Day,
			//		"日"
			//	}));
			//}
			//ETE.FilePath = string.Concat(new string[]
			//{
			//	Environment.GetFolderPath(Environment.SpecialFolder.Desktop) ,
			//	"\\判定结果\\",
			//	Year,
			//	"年\\",
			//	Month,
			//	"月\\",
			//	Day,
			//	"日\\",
			//	DateTime.Now.ToString("yyyy年MM月dd日 HH时mm分ss秒"),
			//	".xlsx"
			//});
			//ETE.SaveAsExcel();
			//ETE.Dispose();
		}


		public string[] TitleFieldName = new string[] { "零点AD", "SPAN点AD" };
		public void SetStaZeroSpanTitleExchange(bool exchange)
		{
			TitleFieldName[1] = exchange ? "零点AD" : "SPAN点AD";
			TitleFieldName[0] = !exchange ? "零点AD" : "SPAN点AD";
		}

		public async Task BtnExport()
		{
			SaveConfig();
			{
				string name = DateTime.Now.ToString("标定yyyyMMddHHmmss");
				if (this.SelectRuleEntity != null) {
					name = string.Format("{0}[{1}]{2}", SelectRuleEntity.ProductFullName, SelectRuleEntity.OrderNumber, name);
				}
				var dlg = new SaveFileDialog()
				{
					Title =  "标定成果-另存为",
					DefaultExt = "txt",
					Filter = "Text files (*.xlsx)|*.xlsx|All files|*.*",
					FileName = name
				};
				if (dlg.ShowDialog() == true)
				{

					if (this.PageContext.TabControlMain.SelectedItem == this.PageContext.TabItemVoltage10) {
						try
						{
							//ConnectionPool.Qualified.SaveCheckedDataToExcel(dlg.FileName);

							ExcuteToExcel ETE = new ExcuteToExcel("标定成果");
							ETE.SetCellValue<string>(1, 1, "位置");
							ETE.SetCellValue<string>(1, 2, "实时AD");
							ETE.SetCellValue<string>(1, 3, TitleFieldName[0]);
							ETE.SetCellValue<string>(1, 4, TitleFieldName[1]);
							ETE.SetCellValue<string>(1, 5, "预留1");
							ETE.SetCellValue<string>(1, 6, "预留2");
							ETE.SetCellValue<string>(1, 7, "预留3");
							ETE.SetCellValue<string>(1, 8, "浓度");
							ETE.SetCellValue<string>(1, 9, "温度1");
							ETE.SetCellValue<string>(1, 10, "温度2"); 
							ETE.SetCellValue<string>(1, 11, "模拟电压");
							ETE.SetCellValue<string>(1, 12, "是否合格");
							ETE.SetCellValue<string>(1, 13, "设备串号");
							ETE.SetCellValue<string>(1, 14, "设备时间");

							int rowno = 1;
							for (int i = 0; i < this.ProductModelGroupTable.Count; i++)
							{
								SensorGroupDataModel mm = ProductModelGroupTable[i];
								if (mm.Address.V == 0) continue;
								if (mm.Empty) continue;
								for (int j = 0; j < mm.ProductTable.Count; j++)
								{
									rowno++;
                                    try
                                    {
										ETE.SetCellValue<string>(rowno, 1, string.Format("{0}-{1}", this.ProductModelGroupTable[i].Address, this.ProductModelGroupTable[i].ProductTable[j].Position));
										ETE.SetCellValue<int>(rowno, 2, this.ProductModelGroupTable[i].ProductTable[j].V01.Value);
										ETE.SetCellValue<int>(rowno, 3, this.ProductModelGroupTable[i].ProductTable[j].V02.Value);
										ETE.SetCellValue<int>(rowno, 4, this.ProductModelGroupTable[i].ProductTable[j].V04.Value);
										ETE.SetCellValue<int>(rowno, 5, this.ProductModelGroupTable[i].ProductTable[j].V03.Value);
										ETE.SetCellValue<int>(rowno, 6, this.ProductModelGroupTable[i].ProductTable[j].V05.Value);
										ETE.SetCellValue<int>(rowno, 7, this.ProductModelGroupTable[i].ProductTable[j].V06.Value);
										ETE.SetCellValue<int>(rowno, 8, this.ProductModelGroupTable[i].ProductTable[j].V07.Value);
										ETE.SetCellValue<int>(rowno, 9, this.ProductModelGroupTable[i].ProductTable[j].V08.Value);
										ETE.SetCellValue<int>(rowno, 10, this.ProductModelGroupTable[i].ProductTable[j].V09.Value);
										ETE.SetCellValue<int>(rowno, 11, this.ProductModelGroupTable[i].ProductTable[j].V10.Value);
										var Qualified = ConnectionPool.Qualified[this.ProductModelGroupTable[i].Address.APP, this.ProductModelGroupTable[i].Address.V, this.ProductModelGroupTable[i].ProductTable[j].Position];
										ETE.SetCellValue<string>(rowno, 12, Qualified == null || Qualified.Value ? "" : "X");
										ETE.SetCellValue<string>(rowno, 13, this.ProductModelGroupTable[i].ProductTable[j].Serial);
										ETE.SetCellValue<string>(rowno, 14, this.ProductModelGroupTable[i].ProductTable[j].DeviceDateTime);
									}
                                    catch (Exception xx)
                                    {

                                        //throw xx;
                                    }
									
								}
							}

							ETE.AllCellsAlignment(ExcelHorizontalAlignment.Center);
							ETE.SetRowStyleOfFontToBold(1);
							string Year = DateTime.Now.Year.ToString();
							string Month = DateTime.Now.Month.ToString();
							string Day = DateTime.Now.Day.ToString();
							ETE.FilePath = dlg.FileName;
							ETE.SaveAsExcel();
							ETE.Dispose();

							MessageBox.Show("导出成功。");

						}
						catch (System.Exception ex)
						{
							MessageBox.Show(ex.Message, "导出出错");
						}
					}
					if (this.PageContext.TabControlMain.SelectedItem == this.PageContext.TabItemVoltage)
					{
						try
						{
							//ConnectionPool.Qualified.SaveCheckedDataToExcel(dlg.FileName);

							ExcuteToExcel ETE = new ExcuteToExcel("标定成果");
							ETE.SetCellValue<string>(1, 1, "位置");
							ETE.SetCellValue<string>(1, 2, "实时AD");
							ETE.SetCellValue<string>(1, 3, TitleFieldName[0]);
							ETE.SetCellValue<string>(1, 4, TitleFieldName[1]);
							ETE.SetCellValue<string>(1, 5, "浓度");
							ETE.SetCellValue<string>(1, 6, "温度1");
							ETE.SetCellValue<string>(1, 7, "是否合格");
							ETE.SetCellValue<string>(1, 8, "设备串号");
							ETE.SetCellValue<string>(1, 9, "设备时间");

							int rowno = 1;
							for (int i = 0; i < this.ProductModelGroupTable.Count; i++)
							{
								SensorGroupDataModel mm = ProductModelGroupTable[i];
								if (mm.Address.V == 0) continue;
								if (mm.Empty) continue;
								for (int j = 0; j < mm.ProductTable.Count; j++)
								{
									rowno++;
                                    try
                                    {
										ETE.SetCellValue<string>(rowno, 1, string.Format("{0}-{1}", this.ProductModelGroupTable[i].Address, this.ProductModelGroupTable[i].ProductTable[j].Position));
										ETE.SetCellValue<int>(rowno, 2, this.ProductModelGroupTable[i].ProductTable[j].V01.Value);
										ETE.SetCellValue<int>(rowno, 3, this.ProductModelGroupTable[i].ProductTable[j].V02.Value);
										ETE.SetCellValue<int>(rowno, 4, this.ProductModelGroupTable[i].ProductTable[j].V04.Value);
										ETE.SetCellValue<int>(rowno, 5, this.ProductModelGroupTable[i].ProductTable[j].V07.Value);
										ETE.SetCellValue<int>(rowno, 6, this.ProductModelGroupTable[i].ProductTable[j].V08.Value);
										var Qualified = ConnectionPool.Qualified[this.ProductModelGroupTable[i].Address.APP, this.ProductModelGroupTable[i].Address.V, this.ProductModelGroupTable[i].ProductTable[j].Position];
										ETE.SetCellValue<string>(rowno, 7, Qualified == null || Qualified.Value ? "" : "X");
										ETE.SetCellValue<string>(rowno, 8, this.ProductModelGroupTable[i].ProductTable[j].Serial);
										ETE.SetCellValue<string>(rowno, 9, this.ProductModelGroupTable[i].ProductTable[j].DeviceDateTime);
									}
                                    catch (Exception xx)
                                    {
                                        //throw xx;
                                    }

								}
							}
							ETE.AllCellsAlignment(ExcelHorizontalAlignment.Center);
							ETE.SetRowStyleOfFontToBold(1);
							string Year = DateTime.Now.Year.ToString();
							string Month = DateTime.Now.Month.ToString();
							string Day = DateTime.Now.Day.ToString();
							ETE.FilePath = dlg.FileName;
							ETE.SaveAsExcel();
							ETE.Dispose();

							MessageBox.Show("导出成功。");
						}
						catch (System.Exception ex)
						{
							MessageBox.Show(ex.Message, "导出出错");
						}
					}


					if (this.PageContext.TabControlMain.SelectedItem == this.PageContext.TabItemParamenter)
					{
						try
						{
							//ConnectionPool.Qualified.SaveCheckedDataToExcel(dlg.FileName);

							ExcuteToExcel ETE = new ExcuteToExcel("参数导出");
							ETE.SetCellValue<string>(1, 1, "位置");
							ETE.SetCellValue<string>(1, 2, "序列号");
							ETE.SetCellValue<string>(1, 3, "时间");
							ETE.SetCellValue<string>(1, 4, "量程");
							ETE.SetCellValue<string>(1, 5, "输出电压");
							ETE.SetCellValue<string>(1, 6, "电压范围");
							ETE.SetCellValue<string>(1, 7, "自校准"); 

							int rowno = 1;
							for (int i = 0; i < this.ProductModelGroupTable.Count; i++)
							{
								SensorGroupDataModel mm = ProductModelGroupTable[i];
								if (mm.Address.V == 0) continue;
								    
								for (int j = 0; j < mm.ProductTable.Count; j++)
								{
									rowno++;
									ETE.SetCellValue<string>(rowno, 1, string.Format("{0}-{1}", this.ProductModelGroupTable[i].Address, this.ProductModelGroupTable[i].ProductTable[j].Position));
									ETE.SetCellValue<string>(rowno, 2, this.ProductModelGroupTable[i].ProductTable[j].Serial);
									ETE.SetCellValue<string>(rowno, 3, this.ProductModelGroupTable[i].ProductTable[j].DeviceDateTime);
									ETE.SetCellValue<string>(rowno, 4, this.ProductModelGroupTable[i].ProductTable[j].LiangCheng + "");
									ETE.SetCellValue<string>(rowno, 5, this.ProductModelGroupTable[i].ProductTable[j].OutPutVolage + "");
									ETE.SetCellValue<string>(rowno, 6, this.ProductModelGroupTable[i].ProductTable[j].DianYaRange);
									ETE.SetCellValue<string>(rowno, 7, this.ProductModelGroupTable[i].ProductTable[j].AutoAdjustState+"");
								}
							}
							ETE.AllCellsAlignment(ExcelHorizontalAlignment.Center);
							ETE.SetRowStyleOfFontToBold(1);
							string Year = DateTime.Now.Year.ToString();
							string Month = DateTime.Now.Month.ToString();
							string Day = DateTime.Now.Day.ToString();
							ETE.FilePath = dlg.FileName;
							ETE.SaveAsExcel();
							ETE.Dispose();

							MessageBox.Show("导出成功。");
						}
						catch (System.Exception ex)
						{
							MessageBox.Show(ex.Message, "导出出错");
						}
					}

				}
			}

		}

		public void BtnSetQualified()
		{

		}

		 


		public async Task BtnModeSelfReport()
		{
			SaveConfig();
			await Task.Run(delegate ()
			{
				foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.SetModeQuery(false);
			});
		}

		public async Task BtnModeQuery()
		{
			SaveConfig();
			await Task.Run(delegate ()
			{
				foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.SetModeQuery(true);
			});
		}


		public bool BtnQueryIsRead { get; set; }
		public string BtnQueryText { get { return !BtnQueryIsRead ? "读取数据" : "停止读取数据"; } }
		public Brush BtnQueryBackground { get { return !BtnQueryIsRead 
					? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF59B42E")) 
					: new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF33CCCC")); } }//#FF79B42E
		public async Task BtnQuery()
		{
			if (BtnQueryParameterIsRead)
			{
				foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadAutoAdjustStatusContinue = false;
				foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadLiangChengContinue = false;
				foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadOutPutVolageContinue = false;
				foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadVoltageRangeContinue = false;
				foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadSerialNoContinue = false;
				foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.ReadDeviceDateTimeContinue = false;
				BtnQueryParameterIsRead = false;
			}
			if (BtnQueryIsRead)
			{
				foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadAllContinue = false;
				this.BtnQueryIsRead = false;
				return;
			}
			this.PageContext.TabItemVoltage.IsSelected = true;
			SaveConfig();

			

			Start = DateTime.Now;
			this.BtnQueryIsRead = true;
			foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadAllContinue = true;
			
			if (StaReadSerialNo) {
				WaitWindow.ShowWindow("正在读取", "正在读取设备串号......", this.PageContext);
				  this.ConnectionPool.ReadSerialNo();
				WaitWindow.ShowWindow("正在读取", "正在读取设备时间......", this.PageContext);
				  this.ConnectionPool.ReadDeviceDateTime();
				WaitWindow.CloseWindow(this.PageContext);
			}
			DbService.StartSensorData(this.AddressList);

			this.ConnectionPool.ReadAll(); 


		}

		 


		public bool BtnQueryParameterIsRead { get; set; }
		public string BtnQueryParameterText { get { return !BtnQueryParameterIsRead ? "读取参数" : "停止读取参数"; } }
		public Brush BtnQueryParameterBackground
		{
			get
			{
				return !BtnQueryParameterIsRead
							? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF059F57"))
							: new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF33CC99"));//FF0B9F57
			}
		}//#FF79B42E
		public async Task BtnQueryParameter()
		{
			if (BtnQueryIsRead)
			{
				this.BtnQueryIsRead = false;
				foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadAllContinue = false;
			}
			if (BtnQueryParameterIsRead)
			{
				
				foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadAutoAdjustStatusContinue = false;
				foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadLiangChengContinue = false;
				foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadOutPutVolageContinue = false;
				foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadVoltageRangeContinue = false;
				foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadSerialNoContinue = false;
				foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.ReadDeviceDateTimeContinue = false;
				this.BtnQueryParameterIsRead = false;
				return;
			}
			this.PageContext.TabItemParamenter.IsSelected = true;
			SaveConfig();
			this.BtnQueryParameterIsRead = true;
			foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadAutoAdjustStatusContinue = true;
			foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadLiangChengContinue = true;
			foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadOutPutVolageContinue = true;
			foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadVoltageRangeContinue = true;
			foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadSerialNoContinue = true;
			foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.ReadDeviceDateTimeContinue = true;
			DbService.StartSensorData(this.AddressList);

			this.ConnectionPool.ReadParameterAll();

			//WaitWindow.ShowWindow("正在读取", "正在读取串号......", this.PageContext);
			//await this.ConnectionPool.ReadSerialNo();
			//WaitWindow.ShowWindow("正在读取", "正在读取设备时间......", this.PageContext);
			//await this.ConnectionPool.ReadDeviceDateTime();
			//WaitWindow.ShowWindow("正在读取", "正在读取量程......", this.PageContext);
			//await this.ConnectionPool.ReadLiangCheng();
			//WaitWindow.ShowWindow("正在读取", "正在读取输出电压......", this.PageContext);
			//await this.ConnectionPool.ReadOutPutVolage();
			//WaitWindow.ShowWindow("正在读取", "正在读取电压范围......", this.PageContext);
			//await this.ConnectionPool.ReadVoltageRange();
			//WaitWindow.ShowWindow("正在读取", "正在读取自动校准......", this.PageContext);
			//await this.ConnectionPool.ReadAutoAdjustStatus();
			//WaitWindow.CloseWindow(this.PageContext);

			//foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadAutoAdjustStatusContinue = false;
			//foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadLiangChengContinue = false;
			//foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadOutPutVolageContinue = false;
			//foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadVoltageRangeContinue = false;
			//foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadSerialNoContinue = false;
			//this.BtnQueryParameterIsRead = false;


		}


		public async Task BtnReadSerialNo()
		{
			this.PageContext.PopupBoxQueryParameter.IsPopupOpen = false;
			if (BtnQueryIsRead)
			{
				this.BtnQueryIsRead = false;
				foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.ReadAllContinue = false;
			} 
			this.PageContext.TabItemParamenter.IsSelected = true;
			SaveConfig();
			this.BtnQueryParameterIsRead = true; 
			foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.ReadSerialNoContinue = true; 
			WaitWindow.ShowWindow("正在读取", "正在读取串号......", this.PageContext);
			  this.ConnectionPool.ReadSerialNo(); 
			WaitWindow.CloseWindow(this.PageContext);
			 
			//foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadSerialNoContinue = false;
			//this.BtnQueryParameterIsRead = false;

		}

		public async Task BtnReadTime()
		{
			this.PageContext.PopupBoxQueryParameter.IsPopupOpen = false;
			if (BtnQueryIsRead)
			{
				this.BtnQueryIsRead = false;
				foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.ReadAllContinue = false;
			}
			 
			this.PageContext.TabItemParamenter.IsSelected = true; 
			this.BtnQueryParameterIsRead = true; 
			foreach (var conn in this.ConnectionPool.Values.ToList()) conn.CommFactory.ReadDeviceDateTimeContinue = true; 
			WaitWindow.ShowWindow("正在读取", "正在读取设备时间......", this.PageContext);
			  this.ConnectionPool.ReadDeviceDateTime(); 
			WaitWindow.CloseWindow(this.PageContext);

			//foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadAutoAdjustStatusContinue = false;
			//foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadLiangChengContinue = false;
			//foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadOutPutVolageContinue = false;
			//foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadVoltageRangeContinue = false;
			//foreach (var conn in this.ConnectionPool.Values.ToList())  conn.CommFactory.ReadSerialNoContinue = false;
			//this.BtnQueryParameterIsRead = false;

		}


		public async Task BtnSetSerialNo()
		{
			this.PageContext.PopupBoxQueryParameter.IsPopupOpen = false;
			UISerialNo.SetSerialNoAll( );
		}

		public async Task BtnSetTime()
		{
			this.PageContext.PopupBoxQueryParameter.IsPopupOpen = false;
			UISerialNo.SetSetTime();
		}

		void LoadConfig() { 
			 this.TxtTCPIP = BiaoDingConfgig.Instance.TCPIP;
			 this.TxtPortsModeSelect = BiaoDingConfgig.Instance.PortsModeSelect;
			this.StaFilterEmpSata = BiaoDingConfgig.Instance.StaFilterEmpSata;
			StaZeroSpanTitleExchange= BiaoDingConfgig.Instance.StaZeroSpanTitleExchange;
			this.TxtProductList= BiaoDingConfgig.Instance.ProductList;
			this.TxtSensorGrougAddress = BiaoDingConfgig.Instance.SensorGrougAddress;
			this.TxtSerialPorts= BiaoDingConfgig.Instance.SerialPorts;
			foreach (ProductConfig item in BiaoDingConfgig.Instance.Product)
			{
				if (item.Name == this.TxtProductList)
				{

					this.TxtSpan = item.Span;
					this.TxtZreo = item.Zero;

					this.TxtValueAdd = item.ValueAdd;
					this.TxtValueBase = item.ValueBase;
					this.TxtValueMinus = item.ValueMinus;

					this.TxtVoltageValueAdd = item.VoltageValueAdd;
					this.TxtVoltageValueBase = item.VoltageValueBase;
					this.TxtVoltageValueMinus = item.VoltageValueMinus;

				}
			}
		}
		void SaveConfig()
		{


			BiaoDingConfgig.Instance.TxtUser = UISheBeiBiaoDingViewModel.Intance.MainView.TxtUser.Text;
			BiaoDingConfgig.Instance.TxtUserAdmin = UISheBeiBiaoDingViewModel.Intance.MainView.TxtUserAdmin.Text;

			BiaoDingConfgig.Instance.StaFilterEmpSata = this.StaFilterEmpSata;
			BiaoDingConfgig.Instance.StaZeroSpanTitleExchange = this.StaZeroSpanTitleExchange; 
			BiaoDingConfgig.Instance.TCPIP = this.TxtTCPIP;
			BiaoDingConfgig.Instance.PortsModeSelect = this.TxtPortsModeSelect;
			BiaoDingConfgig.Instance.ProductList = this.TxtProductList;
			BiaoDingConfgig.Instance.SensorGrougAddress = this.TxtSensorGrougAddress;
			BiaoDingConfgig.Instance.SerialPorts = this.TxtSerialPorts;
            foreach (ProductConfig item in BiaoDingConfgig.Instance.Product)
            {
				if (item.Name == this.TxtProductList) {

					item.Span = this.TxtSpan;
					item.Zero = this.TxtZreo;

					item.ValueAdd = this.TxtValueAdd;
					item.ValueBase = this.TxtValueBase;
					item.ValueMinus = this.TxtValueMinus;

					item.VoltageValueAdd = this.TxtVoltageValueAdd;
					item.VoltageValueBase = this.TxtVoltageValueBase;
					item.VoltageValueMinus = this.TxtVoltageValueMinus;

				}
			}
			BiaoDingConfgig.Save(); 
		}


		DateTime SM_DataEventLastUIRefresh = DateTime.Now;
		private void SM_DataEvent(object sender, CommFactory.DataEventArgs e)
		{

			try
			{

				if (Start == DateTime.MinValue)
				{
					this.StaTimeElapsed = "00:00:00";
				}
				else
				{
					var t = (DateTime.Now - Start);
					this.StaTimeElapsed = string.Format("{0}:{1}:{2}", t.Hours.ToString("00"), t.Minutes.ToString("00"), t.Seconds.ToString("00"), t.Days < 1 ?"": t.Days.ToString()+"D ");
				}


				this.DbService.PushData(e.ProductData, e.ReadMode);//存储数据
				int bbaseV7 = this.TxtValueBase;
				int bbaseV1 = this.TxtVoltageValueBase;
				//if (bbaseV7 == 0|| bbaseV1==0) ConnectionPool.Qualified.CalcAvg(out bbaseV1,out bbaseV7 );
				ConnectionPool.Qualified.SetQualifiedV1V7(
					bbaseV7,this.TxtValueMinus, this.TxtValueAdd,
					bbaseV1, this.TxtVoltageValueMinus, this.TxtVoltageValueAdd);
				List<SensorGroupDataModel> groups = ProductModelGroupTable.Values.Where(w => w.Address == e.Address).ToList();
                foreach (SensorGroupDataModel group in groups)
				{
					group.QuertReceiveData();//记录收到数据
					List<SensorDataX> DataList = group.ProductTable;
					switch (e.ReadMode)
					{
						case ReadMode.WX_StartRead:
							for (int i = 0; i < 15; i++)
							{
                                try
                                {
									DataList[i].V01 = e.ProductData.SingleAddressData[i].V01;
									DataList[i].V02 = e.ProductData.SingleAddressData[i].V02;
									DataList[i].V03 = e.ProductData.SingleAddressData[i].V03;
									DataList[i].V04 = e.ProductData.SingleAddressData[i].V04;
									DataList[i].V05 = e.ProductData.SingleAddressData[i].V05;
									DataList[i].V06 = e.ProductData.SingleAddressData[i].V06;
									DataList[i].V07 = e.ProductData.SingleAddressData[i].V07;
									DataList[i].V08 = e.ProductData.SingleAddressData[i].V08;
									DataList[i].V09 = e.ProductData.SingleAddressData[i].V09;
									DataList[i].V10 = e.ProductData.SingleAddressData[i].V10;
									//CopyEntity(e.ProductData.SingleAddressData[i], DataList[i]);
								}
                                catch (System.Exception xx)
                                {
                                    throw xx;
                                }
								
							}
							break;
						case ReadMode.RX_ReadLiangCheng:
							for (int j = 0; j < 15; j++)
							{
								DataList[j].LiangCheng = e.ProductData.SingleAddressData[j].LiangCheng;
							}
							break;
						case ReadMode.RX_ReadVoltageRange:
							for (int k = 0; k < 15; k++)
							{
								DataList[k].DianYaRange = e.ProductData.SingleAddressData[k].DianYaRange;
							}
							break;
						case ReadMode.RX_ReadAutoAdjustStatus:
							for (int l = 0; l < 15; l++)
							{
								DataList[l].AutoAdjustState = e.ProductData.SingleAddressData[l].AutoAdjustState;
							}
							break;
						case ReadMode.RX_ReadSerialNo:
							for (int l = 0; l < 15; l++)
							{
								DataList[l].Serial = e.ProductData.SingleAddressData[l].Serial;
							}
							break;
						case ReadMode.RX_ReadDeviceDateTime:
							for (int l = 0; l < 15; l++)
							{
								DataList[l].DeviceDateTime = e.ProductData.SingleAddressData[l].DeviceDateTime;
							}
							break;
						case ReadMode.RX_ReadOutPutVolage:
							for (int m = 0; m < 15; m++)
							{
								DataList[m].OutPutVolage = e.ProductData.SingleAddressData[m].OutPutVolage;
							}
							break;
					}
				}
				this.StaQuallified = ConnectionPool.Qualified.StaQuallified;
				this.StaRate = ConnectionPool.Qualified.StaRate==double.NaN?"": ConnectionPool.Qualified.StaRate.ToString("P1");
				this.StaTotal = ConnectionPool.Qualified.StaTotal;
				this.StaUnqualified = ConnectionPool.Qualified.StaUnqualified;

			}
			catch (System.Exception ex)
			{
				throw ex;
			}
			finally {

				/// 最快半秒刷新一次，避免界面刷新太多卡了
				//if (DateTime.Now - SM_DataEventLastUIRefresh > new TimeSpan(0, 0, 0, 3, 0)) 
				{
					SM_DataEventLastUIRefresh = DateTime.Now;
					PageContextLoadedByAddress(e.Address);
				}

			}

			this.ResetCalcSelectedData();

			
		}

		void CopyEntity(object src,object target) {
            foreach (PropertyInfo item in src.GetType().GetProperties())
            {
				if(item.CanWrite) item.SetValue(target, item.GetValue(src, null), null);

			}
		}

		public static bool IsInDesignMode(System.Windows.Controls.Control control)
		{
			return System.ComponentModel.DesignerProperties.GetIsInDesignMode(control);
		}

	}



	public class HardWareInfo
	{
		static Dictionary<string, HardWareInfo> Cache = new Dictionary<string, HardWareInfo>();
		public static HardWareInfo Create(string name, string keyValue)
		{
			if (!Cache.ContainsKey(name))
			{
				Cache.Add(name, new HardWareInfo() { Name = name, DescName = keyValue });
			}
			Cache[name].DescName = keyValue;
			return Cache[name];
		}
		/// <summary>
		/// WMI取硬件信息
		/// </summary>
		/// <param name="hardType"></param>
		/// <param name="propKey"></param>
		/// <returns></returns>
		public static HardWareInfo MulGetHardwareInfo(HardwareEnum hardType, string keyValue)
		{
			HardWareInfo ret = HardWareInfo.Create(keyValue, keyValue);
			try
			{
				//COM查找/
				ObjectQuery query = new ObjectQuery("SELECT * FROM " + hardType); // Win32_USBControllerDevice
				//ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);
				using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
				{
					var hardInfos = searcher.Get();
					foreach (var hardInfo in hardInfos)
					{
						if (hardInfo.Properties["Name"].Value.ToString().Contains(keyValue))
						{
							ret.DescName = hardInfo.Properties["Name"].Value.ToString();
						}

					}
					searcher.Dispose();
				}
			}
			catch
			{

			}
			finally
			{
			}
			return ret;
		}
		public string Name { get; set; }
		public string DescName { get; set; }
		HardWareInfo() { }
		public override string ToString()
		{
			return DescName;
		}
	}


	/// <summary>
	/// 枚举win32 api
	/// </summary>
	public enum HardwareEnum
	{
		// 硬件
		Win32_Processor, // CPU 处理器
		Win32_PhysicalMemory, // 物理内存条
		Win32_Keyboard, // 键盘
		Win32_PointingDevice, // 点输入设备，包括鼠标。
		Win32_FloppyDrive, // 软盘驱动器
		Win32_DiskDrive, // 硬盘驱动器
		Win32_CDROMDrive, // 光盘驱动器
		Win32_BaseBoard, // 主板
		Win32_BIOS, // BIOS 芯片
		Win32_ParallelPort, // 并口
		Win32_SerialPort, // 串口
		Win32_SerialPortConfiguration, // 串口配置
		Win32_SoundDevice, // 多媒体设置，一般指声卡。
		Win32_SystemSlot, // 主板插槽 (ISA & PCI & AGP)
		Win32_USBController, // USB 控制器
		Win32_NetworkAdapter, // 网络适配器
		Win32_NetworkAdapterConfiguration, // 网络适配器设置
		Win32_Printer, // 打印机
		Win32_PrinterConfiguration, // 打印机设置
		Win32_PrintJob, // 打印机任务
		Win32_TCPIPPrinterPort, // 打印机端口
		Win32_POTSModem, // MODEM
		Win32_POTSModemToSerialPort, // MODEM 端口
		Win32_DesktopMonitor, // 显示器
		Win32_DisplayConfiguration, // 显卡
		Win32_DisplayControllerConfiguration, // 显卡设置
		Win32_VideoController, // 显卡细节。
		Win32_VideoSettings, // 显卡支持的显示模式。

		// 操作系统
		Win32_TimeZone, // 时区
		Win32_SystemDriver, // 驱动程序
		Win32_DiskPartition, // 磁盘分区
		Win32_LogicalDisk, // 逻辑磁盘
		Win32_LogicalDiskToPartition, // 逻辑磁盘所在分区及始末位置。
		Win32_LogicalMemoryConfiguration, // 逻辑内存配置
		Win32_PageFile, // 系统页文件信息
		Win32_PageFileSetting, // 页文件设置
		Win32_BootConfiguration, // 系统启动配置
		Win32_ComputerSystem, // 计算机信息简要
		Win32_OperatingSystem, // 操作系统信息
		Win32_StartupCommand, // 系统自动启动程序
		Win32_Service, // 系统安装的服务
		Win32_Group, // 系统管理组
		Win32_GroupUser, // 系统组帐号
		Win32_UserAccount, // 用户帐号
		Win32_Process, // 系统进程
		Win32_Thread, // 系统线程
		Win32_Share, // 共享
		Win32_NetworkClient, // 已安装的网络客户端
		Win32_NetworkProtocol, // 已安装的网络协议
		Win32_PnPEntity,//all device
	}


}
