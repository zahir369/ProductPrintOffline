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
using System.ComponentModel;
using System.Data;
using Microsoft.Win32;
using System.Windows.Input;
using OfficeOpenXml.Style;
using System.IO;
using MKSS.Service.UIBiaoDing;
using MKSS.Model;
using System.Windows.Media;
using MKSS.APP.UserControls;

namespace DeviceDataMonitorWPF.UIBiaoDing
{
	public class UISheBeiBiaoDingViewModel : Screen 
	{
		 
		public static UISheBeiBiaoDingViewModel Intance { get; private set; }
		public static int MaxBoardCount = 99;
		public static int TCPIP_TYPE_NETWORK = 0;
		public ModBusBoard SerialModBus { get; set; }
		public BiaoDingService DbService = new BiaoDingService();
		public ScrwdService ScrwdService = new ScrwdService();

		/// <summary>
		///  对应生产任务单，可能 null
		/// </summary>
		public ScrwEntity ScrwEntity { get; set; }

		/// <summary>
		///  没选择任务单时候显示 提醒叹号
		/// </summary>
		public  string BtnScrwdTipBadge { get { return ScrwEntity == null || string.IsNullOrEmpty(ScrwEntity.ProductFullName) ? "!" : ""; } }
		public int TxtPortsModeSelect { get; set; } = 0;
		public string TxtTCPIP { get; set; } = "192.168.100.99:5000";
		public string TxtSerialPorts { get; set; }
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

		public bool StaReadSerialNo { get; set; } = true;
		public bool StaFilterEmpSata { get { return SensorDataFilter.Enabled; } set { SensorDataFilter.Enabled =value; } }
		public bool StaZeroSpanTitleExchange { get; set; }
		
		public string LabelMessage { get; set; }

		public bool CanTxtPortsModeSelect { get { return !SerialComIsOpen; } }
		public bool CanTxtSerialPorts { get { return !SerialComIsOpen; } }
		public bool CanTxtTCPIP { get { return !SerialComIsOpen; } }
		public bool CanTxtProductList { get { return !SerialComIsOpen; } }
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
		public bool CanBtnSetSerialNo { get { return SerialComIsOpen && AddressAvaliable; } }
		public bool CanBtnSetTime { get { return SerialComIsOpen && AddressAvaliable; } }
		public bool CanBtnScrwd { get { return SerialComIsOpen; } }
		

		public DateTime Start { get; set; } = DateTime.MinValue;
		public string StaTimeElapsed { get; set; }  
		public int StaQuallified { get; set; }  
		public int StaUnqualified { get; set; }  
		public string StaRate { get; set; }  
		public double StaTotal { get; set; }  

		public bool IsWrite { get; set; }
		public bool SerialComIsOpen { get; set; }

		public List<Address> Address
		{
			get
			{
				List<byte> ret = new List<byte>();
				List<Address> retAddr = new List<Address>();
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
						if (ret.Count > MaxBoardCount) {
							ret = ret.GetRange(0, MaxBoardCount);
						}
						//改变要读取的地址范围 
						retAddr = ret.Select(w => new Address(w)).ToList();
						if (this.SerialModBus != null) SerialModBus.Address = retAddr;
						for (int i = 0; i < ProductModelGroupTable.Count; i++)
						{
							if(ret.Count-1>= i) ProductModelGroupTable[i].Address = retAddr[i];  
							else ProductModelGroupTable[i].Address = new Address(0);
						}
						PageContext_Loaded(null, null);

						StringBuilder sb = new StringBuilder();
						foreach (var item in ret)
						{
							if (sb.Length < 6) sb.Append(item + ",");
						}
						TxtSensorGrougAddressDesc = string.Format("通道地址({1}等{0}个)", ret.Count, sb.ToString());
					}
					else {
						TxtSensorGrougAddressDesc = string.Format("各通道地址(例如：1-3,16-35)");
					}
					
				}
				return retAddr;
			}
		}

		public void Closed()
        {
			this.BtnDisConnect();
        }

        public bool AddressAvaliable { get { return Address.Count > 0; } }

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
			LoadConfig();
			SerialModBus = new ModBusBoard() { ReadSpeed= BiaoDingConfgig.Instance.RefreshInterval };
			ProductModelGroupTable = new Dictionary<int, SensorGroupDataModel>();
			for (int i = 0; i < MaxBoardCount; i++)
			{
				ProductModelGroupTable.Add(i, new SensorGroupDataModel() { Address = new Address(0) }); 
			}
			this.SerialModBus.DataEvent += this.SM_DataEvent;
			this.SerialModBus.MessageEvent += this.SM_MessageEvent;
			List<Address> addrList = UISheBeiBiaoDingViewModel.Intance.Address;//必须调用一次，计算内存表
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

			this.PageContext.Dispatcher.Invoke(() =>
			{
				UISheBeiBiaoDingC05 con = this.PageContext.UISheBeiBiaoDingC05;
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

        private void SM_MessageEvent(object sender, ModBusBoard.MessageEventArgs e)
		{
			this.MessageList.Insert(0, e.Message);
			LabelMessage = e.Message;
		}

		public void BtnConnect()
        {

			SaveConfig();
			bool flag =  TxtPortsModeSelect!=(TCPIP_TYPE_NETWORK);
			if (flag)
			{
				this.SerialModBus.Open(this.TxtSerialPorts);
			}
			else
			{
				string str = this.TxtTCPIP;
				string[] arr = str.Split(':');
				if (arr.Length != 2) {
					SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("IP地址格式不正确，正确格式：192.168.100.99:5000"))  );
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
				if (!int.TryParse(arr[1], out port)) {
					SM_MessageEvent(null, new ModBusBoard.MessageEventArgs(string.Format("端口 {0} 不是数字", arr[1])));
					return;
				}
				this.SerialModBus.OpenTcp(arr[0], port);

			}
			this.SerialComIsOpen = this.SerialModBus.IsOpen;
			if (SerialComIsOpen) {
				SerialModBus.WriteSignalClearNoSleep();
				DbService.StartBatch();
			}
		}

        public void BtnDisConnect()
		{

			this.SerialModBus.ReadAutoAdjustStatusContinue = false;
			this.SerialModBus.ReadLiangChengContinue = false;
			this.SerialModBus.ReadOutPutVolageContinue = false;
			this.SerialModBus.ReadVoltageRangeContinue = false;
			this.SerialModBus.ReadSerialNoContinue = false;
			this.SerialModBus.ReadAllContinue = false;

			bool flag = TxtPortsModeSelect != (TCPIP_TYPE_NETWORK);
			Start = DateTime.MinValue; 
			DbService.StopBatch();
			if (flag)
			{ 
				this.SerialModBus.StopRead();
				this.SerialModBus.Close();
				this.SerialComIsOpen = this.SerialModBus.IsOpen;
			}
			else {
				this.SerialModBus.StopRead();
				this.SerialModBus.CloseTCPIP();
				this.SerialComIsOpen = this.SerialModBus.IsOpen;
			}

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
			UIScrwSelect.ShowScrwSelect();
		}
		

		public async Task BtnSetZero()
		{
			SaveConfig();
			await Task.Run(delegate ()
			{
				this.SerialModBus.SetZero(TxtZreo);
			});
		}

		public async Task BtnSetSpan()
		{
			SaveConfig();
			await Task.Run(delegate ()
			{
				this.SerialModBus.SetSpan(TxtSpan);
			});
		}
		 

		public async Task BtnJudge()
		{

			SaveConfig();
			int bbaseV7 = this.TxtValueBase;
			int bbaseV1 = this.TxtVoltageValueBase;
			if (bbaseV7 == 0 || bbaseV1 == 0) this.SerialModBus.Qualified.CalcAvg(out bbaseV1, out bbaseV7);
			this.SerialModBus.Qualified.SetQualifiedV1V7(
				bbaseV7, this.TxtValueMinus, this.TxtValueAdd,
				bbaseV1, this.TxtVoltageValueMinus, this.TxtVoltageValueAdd);

            
			foreach (SensorGroupData src in this.SerialModBus.Qualified.CurrentProductData.Values.ToArray())
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

			this.StaQuallified = this.SerialModBus.Qualified.StaQuallified;
			this.StaRate = this.SerialModBus.Qualified.StaRate == double.NaN ? "" : this.SerialModBus.Qualified.StaRate.ToString("P1");
			this.StaTotal = this.SerialModBus.Qualified.StaTotal; 
			this.StaUnqualified = this.SerialModBus.Qualified.StaUnqualified;

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
							//this.SerialModBus.Qualified.SaveCheckedDataToExcel(dlg.FileName);

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
									var Qualified = SerialModBus.Qualified[this.ProductModelGroupTable[i].Address.V, this.ProductModelGroupTable[i].ProductTable[j].Position];
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
							//this.SerialModBus.Qualified.SaveCheckedDataToExcel(dlg.FileName);

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
									var Qualified = SerialModBus.Qualified[this.ProductModelGroupTable[i].Address.V, this.ProductModelGroupTable[i].ProductTable[j].Position];
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
			//var CurrentProductData = this.SerialModBus.Qualified.CurrentProductData;
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
				if (this.ScrwEntity != null) {
					name = string.Format("{0}[{1}]{2}", ScrwEntity.ProductFullName, ScrwEntity.OrderNumber, name);
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
							//this.SerialModBus.Qualified.SaveCheckedDataToExcel(dlg.FileName);

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
									var Qualified = SerialModBus.Qualified[this.ProductModelGroupTable[i].Address.V, this.ProductModelGroupTable[i].ProductTable[j].Position];
									ETE.SetCellValue<string>(rowno, 12, Qualified == null || Qualified.Value ? "" : "X");
									ETE.SetCellValue<string>(rowno, 13, this.ProductModelGroupTable[i].ProductTable[j].Serial);
									ETE.SetCellValue<string>(rowno, 14, this.ProductModelGroupTable[i].ProductTable[j].DeviceDateTime);
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
							//this.SerialModBus.Qualified.SaveCheckedDataToExcel(dlg.FileName);

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
									ETE.SetCellValue<string>(rowno, 1, string.Format("{0}-{1}", this.ProductModelGroupTable[i].Address, this.ProductModelGroupTable[i].ProductTable[j].Position));
									ETE.SetCellValue<int>(rowno, 2, this.ProductModelGroupTable[i].ProductTable[j].V01.Value);
									ETE.SetCellValue<int>(rowno, 3, this.ProductModelGroupTable[i].ProductTable[j].V02.Value);
									ETE.SetCellValue<int>(rowno, 4, this.ProductModelGroupTable[i].ProductTable[j].V04.Value);
									ETE.SetCellValue<int>(rowno, 5, this.ProductModelGroupTable[i].ProductTable[j].V07.Value);
									ETE.SetCellValue<int>(rowno, 6, this.ProductModelGroupTable[i].ProductTable[j].V08.Value);
									var Qualified = SerialModBus.Qualified[this.ProductModelGroupTable[i].Address.V, this.ProductModelGroupTable[i].ProductTable[j].Position];
									ETE.SetCellValue<string>(rowno, 7, Qualified == null || Qualified.Value ? "" : "X");
									ETE.SetCellValue<string>(rowno, 8, this.ProductModelGroupTable[i].ProductTable[j].Serial);
									ETE.SetCellValue<string>(rowno, 9, this.ProductModelGroupTable[i].ProductTable[j].DeviceDateTime);
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
				this.SerialModBus.SetModeQuery(false);
			});
		}

		public async Task BtnModeQuery()
		{
			SaveConfig();
			await Task.Run(delegate ()
			{
				this.SerialModBus.SetModeQuery(true);
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
				this.SerialModBus.ReadAutoAdjustStatusContinue = false;
				this.SerialModBus.ReadLiangChengContinue = false;
				this.SerialModBus.ReadOutPutVolageContinue = false;
				this.SerialModBus.ReadVoltageRangeContinue = false;
				this.SerialModBus.ReadSerialNoContinue = false;
				BtnQueryParameterIsRead = false;
			}
			if (BtnQueryIsRead)
			{
				this.SerialModBus.ReadAllContinue = false;
				this.BtnQueryIsRead = false;
				return;
			}
			this.PageContext.TabItemVoltage.IsSelected = true;
			SaveConfig();

			

			Start = DateTime.Now;
			this.BtnQueryIsRead = true;
			this.SerialModBus.ReadAllContinue = true;
			
			if (StaReadSerialNo) {
				WaitWindow.ShowWindow("正在读取", "正在读取设备串号......", this.PageContext);
				await this.SerialModBus.ReadSerialNo();
				WaitWindow.ShowWindow("正在读取", "正在读取设备时间......", this.PageContext);
				await this.SerialModBus.ReadDeviceDateTime();
				WaitWindow.CloseWindow(this.PageContext);
			}
			DbService.StartSensorData(this.Address);

			await this.SerialModBus.ReadAll(); 


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
				this.SerialModBus.ReadAllContinue = false;
			}
			if (BtnQueryParameterIsRead)
			{
				this.SerialModBus.ReadAutoAdjustStatusContinue = false;
				this.SerialModBus.ReadLiangChengContinue = false;
				this.SerialModBus.ReadOutPutVolageContinue = false;
				this.SerialModBus.ReadVoltageRangeContinue = false;
				this.SerialModBus.ReadSerialNoContinue = false;
				this.BtnQueryParameterIsRead = false;
				return;
			}
			this.PageContext.TabItemParamenter.IsSelected = true;
			SaveConfig();
			this.BtnQueryParameterIsRead = true;
			this.SerialModBus.ReadAutoAdjustStatusContinue = true;
			this.SerialModBus.ReadLiangChengContinue = true;
			this.SerialModBus.ReadOutPutVolageContinue = true;
			this.SerialModBus.ReadVoltageRangeContinue = true;
			this.SerialModBus.ReadSerialNoContinue = true;
			DbService.StartSensorData(this.Address);
			await this.SerialModBus.ReadSerialNo();
			await this.SerialModBus.ReadDeviceDateTime();
			await this.SerialModBus.ReadLiangCheng();
			await this.SerialModBus.ReadOutPutVolage();
			await this.SerialModBus.ReadVoltageRange();
			await this.SerialModBus.ReadAutoAdjustStatus();

			this.SerialModBus.ReadAutoAdjustStatusContinue = false;
			this.SerialModBus.ReadLiangChengContinue = false;
			this.SerialModBus.ReadOutPutVolageContinue = false;
			this.SerialModBus.ReadVoltageRangeContinue = false;
			this.SerialModBus.ReadSerialNoContinue = false;
			this.BtnQueryParameterIsRead = false;


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


		private void SM_DataEvent(object sender, ModBusBoard.DataEventArgs e)
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
				//if (bbaseV7 == 0|| bbaseV1==0) this.SerialModBus.Qualified.CalcAvg(out bbaseV1,out bbaseV7 );
				this.SerialModBus.Qualified.SetQualifiedV1V7(
					bbaseV7,this.TxtValueMinus, this.TxtValueAdd,
					bbaseV1, this.TxtVoltageValueMinus, this.TxtVoltageValueAdd);
				List<SensorGroupDataModel> groups = ProductModelGroupTable.Values.Where(w => w.Address == e.Address).ToList();
                foreach (SensorGroupDataModel group in groups)
				{ 
					List<SensorDataX> DataList = group.ProductTable;
					switch (e.ReadMode)
					{
						case ReadMode.RX01:
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
						case ReadMode.RXAB_ReadDeviceDateTime:
							for (int l = 0; l < 15; l++)
							{
								DataList[l].DeviceDateTime = e.ProductData.SingleAddressData[l].DeviceDateTime;
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
				this.StaQuallified = this.SerialModBus.Qualified.StaQuallified;
				this.StaRate = this.SerialModBus.Qualified.StaRate==double.NaN?"": this.SerialModBus.Qualified.StaRate.ToString("P1");
				this.StaTotal = this.SerialModBus.Qualified.StaTotal;
				this.StaUnqualified = this.SerialModBus.Qualified.StaUnqualified;

			}
			catch (System.Exception ex)
			{
				throw ex;
			}
			finally {
				PageContext_Loaded(null, null);
			}

			this.ResetCalcSelectedData();

			
		}

		void CopyEntity(object src,object target) {
            foreach (PropertyInfo item in src.GetType().GetProperties())
            {
				if(item.CanWrite) item.SetValue(target, item.GetValue(src, null), null);

			}
		}

	}
}
