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

namespace DeviceDataMonitorWPF.UIBiaoDing
{
	public class UISheBeiBiaoDingModel : Screen {

		public static UISheBeiBiaoDingModel Intance { get; private set; }
		public static int MaxBoardCount = 99;
		public static int TCPIP_TYPE_NETWORK = 0;
		public ModBusBoard SerialModBus { get; set; }
		
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
		public bool CanBtnQuery { get { return SerialComIsOpen && AddressAvaliable; } }
		public bool CanBtnQueryParameter { get { return SerialComIsOpen && AddressAvaliable; } }

		public DateTime Start { get; set; } = DateTime.MinValue;
		public string StaTimeElapsed { get { if (Start == DateTime.MinValue) return "00:00:00"; return (DateTime.Now - Start).ToString("HH:mm:ss"); } }  
		public int StaQuallified { get; set; }  
		public int StaUnqualified { get; set; }  
		public string StaRate { get; set; }  
		public double StaTotal { get; set; }  

		public bool IsRead { get; set; }
		public bool IsWrite { get; set; }
		public bool SerialComIsOpen { get; set; }

		public List<byte> Address
		{
			get
			{
				List<byte> ret = new List<byte>();
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
						if (this.SerialModBus != null) SerialModBus.Address = ret;
						for (int i = 0; i < ProductModelGroupTable.Count; i++)
						{
							if(ret.Count-1>= i) ProductModelGroupTable[i].Address = ret[i];  
							else ProductModelGroupTable[i].Address = 0;
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
				return ret;
			}
		}

		public void Closed()
        {
			this.BtnDisConnect();
        }

        public bool AddressAvaliable { get { return Address.Count > 0; } }

		public Dictionary<int, SensorGroupDataModel> ProductModelGroupTable { get; set; }

		public StringBuilder MessageList = new StringBuilder();
		private readonly Stylet.IWindowManager _windowManager;

		public UISheBeiBiaoDing PageContext { get; set; }
		public UISheBeiBiaoDingModel()
		{
			Intance = this;
			LoadConfig();
			SerialModBus = new ModBusBoard() { ReadSpeed=50 };
			ProductModelGroupTable = new Dictionary<int, SensorGroupDataModel>();
			for (int i = 0; i < MaxBoardCount; i++)
			{
				ProductModelGroupTable.Add(i, new SensorGroupDataModel() { Address = 0 }); 
			}
			this.SerialModBus.DataEvent += this.SM_DataEvent;
			this.SerialModBus.MessageEvent += this.SM_MessageEvent;
		}

		public void InitPage(UISheBeiBiaoDing page)
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
				string[] arr = str.Split(":");
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
		}

        public void BtnDisConnect()
		{
			bool flag = TxtPortsModeSelect != (TCPIP_TYPE_NETWORK);
			Start = DateTime.MinValue;
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
			for (int i = 0; i < this.SerialModBus.Qualified.CurrentProductData.Count; i++)
			{
				SensorGroupData src = this.SerialModBus.Qualified.CurrentProductData[i];
				SensorGroupDataModel groupTarget = ProductModelGroupTable.Values.FirstOrDefault(w => w.Address == src.Address);
				if (groupTarget == null) continue;
				List<SensorData> DataList = groupTarget.ProductTable;
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

		public async Task BtnExport()
		{
			SaveConfig();
			this.SerialModBus.Qualified.SaveCheckedDataToExcel();
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

        public async Task BtnQuery()
		{
			SaveConfig();
			Start = DateTime.Now;
			this.IsRead = true;
			await this.SerialModBus.ReadAll();
			this.IsRead = this.SerialModBus.IsRead;
		}

		public async Task BtnQueryParameter()
		{
			SaveConfig();
			this.IsRead = true;
			await this.SerialModBus.ReadAutoAdjustStatus();
			await this.SerialModBus.ReadLiangCheng();
			await this.SerialModBus.ReadOutPutVolage();
			await this.SerialModBus.ReadVoltageRange();
			await this.SerialModBus.ReadSerialNo();
			this.IsRead = this.SerialModBus.IsRead;
		}

		void LoadConfig() { 
			 this.TxtTCPIP = BiaoDingConfgig.Instance.TCPIP;
			 this.TxtPortsModeSelect = BiaoDingConfgig.Instance.PortsModeSelect;
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
		void SaveConfig() {
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
				int bbaseV7 = this.TxtValueBase;
				int bbaseV1 = this.TxtVoltageValueBase;
				if (bbaseV7 == 0|| bbaseV1==0) this.SerialModBus.Qualified.CalcAvg(out bbaseV1,out bbaseV7 );
				this.SerialModBus.Qualified.SetQualifiedV1V7(
					bbaseV7,this.TxtValueMinus, this.TxtValueAdd,
					bbaseV1, this.TxtVoltageValueMinus, this.TxtVoltageValueAdd);
				List<SensorGroupDataModel> groups = ProductModelGroupTable.Values.Where(w => w.Address == e.Address).ToList();
                foreach (SensorGroupDataModel group in groups)
				{ 
					List<SensorData> DataList = group.ProductTable;
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

			

			
		}

		void CopyEntity(object src,object target) {
            foreach (PropertyInfo item in src.GetType().GetProperties())
            {
				if(item.CanWrite) item.SetValue(target, item.GetValue(src, null), null);

			}
		}

	}
}
