//using Modbus.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Linq;
using System.Windows.Forms;
using NModbus;
using NModbus.Serial;
using System.Threading.Tasks;

namespace MKSS.APP.ConfigToolMulti
{
    public partial class FormMain : Form
    {
		//写入数据时禁止读取 
		public static bool WritingValues = false;
		SerialPort serialPort1 = new SerialPort();
		BindingSource mbs = new BindingSource();
		private ModbusFactory modbusFactory;
		public IModbusMaster _master;
		MainConfig config = null;
		List<AddrConfigExtend> queryExtend = new List<AddrConfigExtend>();
		public FormMain()
        {

            InitializeComponent();
			this.dataGridView1.AutoGenerateColumns = true;
			this.Text = "出错了！！！";
            CbxSerialPorts_MouseDoubleClick(null, null);
			config = MainConfig.Instance;

			this.Text = "美克盛世 "+config.SoftName;
			this.checkBox1.Text = config.AutoRefreshInterval + "ms定时刷新";
			this.checkBox1.Checked = config.AutoRefresh;
			string defaultCOMM = config.DefaultCOM;
			string defaultAddr = config.DefaultAddress.ToString();

			foreach (var item in config.BaudRateList)
			{
				CbxBandRate.Items.Add(item.ToString());
			}
			string defaultBaudRate = config.DefaultBaudRate.ToString();

			if (!string.IsNullOrEmpty(defaultBaudRate)) {
				CbxBandRate.SelectedItem = defaultBaudRate;
			}

			comboBoxParity.Items.Add(Parity.None);
			comboBoxParity.Items.Add(Parity.Odd);
			comboBoxParity.Items.Add(Parity.Even);
			comboBoxParity.SelectedItem = config.Parity;


			if (!string.IsNullOrEmpty(defaultCOMM))
			{
				CbxSerialPorts.SelectedItem = defaultCOMM;
			}
			if (!string.IsNullOrEmpty(defaultAddr))
			{
				this.TxtAddrList.Text = defaultAddr;
			}

			this.timer1.Interval = config.AutoRefreshInterval;
            foreach (AddrValue item in config.Addrs.OrderBy(w=>w.Address).ToList())
			{
				while (!AddrConfigExtend.TryAdd(item, queryExtend))
				{
					//递归产生任务链
				}
			}

            foreach (var item in queryExtend)
            {
				this.Message("分组查询:"+ item);
			}

			modbusFactory = new ModbusFactory(new List<IModbusFunctionService>(), true, new ModBusBoardLogger(LoggingLevel.Trace,this.textBoxLogger, config));
            this.dataGridView1.DataError += DataGridView1_DataError;

		}

        private void DataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
			this.Message("列表错误:" + e.Exception.Message);
		}

        private void TxtAddrList_TextChanged(object sender, EventArgs e)
		{
            try
            {
				this.DataArray = CalcAddress();
				this.dataGridView1.Columns.Clear();
				mbs.DataSource = this.DataArray.Table;
				this.dataGridView1.DataSource = mbs;
			}
            catch (Exception ex)
            {
				MessageBox.Show(ex.Message);
            }

		}
		int MaxBoardCount = 1000;
		public AddrValueListTable DataArray { get; set; }
		public AddrValueListTable CalcAddress()
		{
			List<byte> ret = new List<byte>();
			string src = this.TxtAddrList.Text + "";
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
				AddrValueListTable xxret = new AddrValueListTable();
                foreach (var item in ret)
                {
					AddrValueList vItem = new AddrValueList((int)item);
                    foreach (var x in config.Addrs)
                    {
						vItem.Add(x.CreateCopy());
					}
					xxret.Add(vItem);
				}
				return xxret;
			}
			else
			{ 
			}
			return new AddrValueListTable();

		}

		private void FormMain_Load(object sender, EventArgs e)
        {
			
		}

		void RefreshControls() {
			bool IsOpen = (this.serialPort1.IsOpen);
			this.BtnConnect.Text = IsOpen ? "关闭" : "打开";
			this.BtnRefresh.Text = this.timer1.Enabled ? "停止读取" : "开始读取";
			this.CbxBandRate.Enabled = !IsOpen;
			this.CbxSerialPorts.Enabled = !IsOpen;
			this.comboBoxParity.Enabled = !IsOpen;
			this.TxtAddrList.Enabled = !IsOpen;
		}


        private void CbxSerialPorts_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.CbxSerialPorts.Items.Clear();
            foreach (var item in SerialPort.GetPortNames())
            {
                this.CbxSerialPorts.Items.Add(item);
            }
            if (this.CbxSerialPorts.Items.Count > 0)
            {
                this.CbxSerialPorts.SelectedItem = this.CbxSerialPorts.Items[0];
            }
        }

		private void BtnConnect_Click(object sender, EventArgs e)
        {
			TxtAddrList_TextChanged(null, null);
			if (this.serialPort1.IsOpen)
			{
				this.Message("关闭串口");
				_master.Dispose();
				_master = null;
				this.serialPort1.Close();
			}
			else
			{
				string COM = this.CbxSerialPorts.SelectedItem + "";
				if (COM == "")
				{
					this.Message ("请选择你的设备端口！");
				}
				else
				{

					if (this.CbxBandRate.SelectedItem!=null )
					{

						int BaudRate = int.Parse(this.CbxBandRate.SelectedItem.ToString());
						this.serialPort1.PortName = COM;
						this.serialPort1.BaudRate = BaudRate;
						this.serialPort1.DataBits = 8;
						this.serialPort1.Parity = (Parity)this.comboBoxParity.SelectedItem;
						this.serialPort1.StopBits = StopBits.One;
						try
						{
							this.serialPort1.Open();
							this.Message("串口打开成功");
							this._master = modbusFactory.CreateRtuMaster(this.serialPort1);
							this._master.Transport.ReadTimeout = 2000;
							this._master.Transport.WriteTimeout = 2000;
							this._master.Transport.Retries = 0;
							this._master.Transport.WaitToRetryMilliseconds = 250;

						}
						catch (Exception ex)
						{
							this.Message(ex.Message);
							this.Message("串口打开失败，请检查系统串口是否可用！");
							try
							{
								if (serialPort1.IsOpen) serialPort1.Close();
							}
							catch (Exception)
							{

							}
							return;
						}
					}
					else {
						this.Message("请指定波特率");
					}
					
				}
			}
			this.RefreshControls();
		}

		private void BtnRefresh_Click(object sender, EventArgs e)
		{

			if (this.timer1.Enabled)
			{
				this.timer1.Enabled = false;
				this.RefreshControls();
				return;
			} 

			if (this.serialPort1.IsOpen)
			{
				
				if (this.checkBox1.Checked)
				{
					this.timer1.Enabled = true;//来时定时读取
				}
				else
				{
					TimerRefresh_Tick(null,null);//只执行一次
				}

			}
			else
			{
				this.Message("请打开串口");
				this.timer1.Enabled = false;
			}

			this.RefreshControls();

		}

		public bool WriteValue(AddrValue addr,ushort value,bool allAddr,int device_addr) {
			try
			{
				if (allAddr)
				{
                    for (int i = 0; i < this.DataArray.Count; i++)
                    {
						var item = this.DataArray[i];
                        try
                        {
							_master.WriteSingleRegister((byte)item.DeviceAddr, addr.Address, value);
							addr.Value = value;
							this.Message(string.Format("地址{3}写入：{0} {1} 成功 ", addr.Name, value, 1, (byte)item.DeviceAddr));
							Task.Delay(20);
						}
                        catch (Exception ex)
                        {
							this.Message(string.Format("地址{3}写入：{0} {1}出错重试，{2}", addr.Name, value, ex.Message, (byte)item.DeviceAddr));
							i--;
						}
					}
					return true;
				}
				else {
					try
					{
						_master.WriteSingleRegister((byte)device_addr, addr.Address, value);
						addr.Value = value;
					}
					catch (Exception ex)
					{
						this.Message(string.Format("地址{3}写入：{0} {1}出错，{2}", addr.Name, value, ex.Message, device_addr));
					}
					
					return true;
				}
				 
			}
			catch (Exception ex)
			{
				this.Message(string.Format("写入：{0} {1}出错", addr.Name, value));
				this.Message(ex.Message);
				this.Message(ex.StackTrace);
				return false;
			}
		}

		private void TimerRefresh_Tick(object sender, EventArgs e)
		{
			if (this.serialPort1.IsOpen)
			{
				if (_master != null) {

					Task task = new Task(() =>
					{
						try
						{

							foreach (var device in this.DataArray)
							{
								if (WritingValues) continue;
								foreach (AddrConfigExtend ext in this.queryExtend)
								{
									ushort[] ret = null;
									try
									{
										if (WritingValues) continue;
										ret = _master.ReadHoldingRegisters((byte)device.DeviceAddr, (ushort)ext.Min, (ushort)(ext.Max - ext.Min + 1));//测试读取数据
										Task.Delay(20);
									}
									catch (Exception ex)
									{
										this.Message(string.Format("地址 {1} 读取：{0} 出错 ,{2}", ext, (byte)device.DeviceAddr, ex.Message));
										//this.Message(ex.Message);
										//this.Message(ex.StackTrace);
										continue;
									}
									if (WritingValues) continue;
									foreach (var itemx in ext.address_dic)
									{
										try
										{  
											var item = device.FirstOrDefault(w => w.Address == itemx.Address);
											if (item != null)
											{
												ushort val = ret[(item.Address - ext.Min)];
												item.Value = val;
												item.RefreshTime = DateTime.Now;
											}
										}
										catch (Exception ex)
										{
											this.Message(string.Format("提取：{0} {1}出错", ext, itemx));
											this.Message(ex.Message);
											this.Message(ex.StackTrace);
										}
									}

								}
								this.DataArray.RefreshData(device);
							}

							this.BeginInvoke(new EventHandler(delegate {
								dataGridView1.Invalidate();
								this.dataGridView1.Update();
								Application.DoEvents();
							}));

						}
						catch (Exception ex)
						{
							this.Message(ex.Message);
							this.Message(ex.StackTrace);
						}
					});
					//启动任务,并安排到当前任务队列线程中执行任务(System.Threading.Tasks.TaskScheduler)
					task.Start();
					
				}
			}
			else {
				this.Message("请打开串口");
				this.timer1.Enabled = false;
			}
			
		}

		 


		void Message(string str) {
			textBoxLogger.AppendText(System.Environment.NewLine);
			textBoxLogger.AppendText(str);
		}

        private void dataGridView1_SizeChanged(object sender, EventArgs e)
        {
			
		}

        private void FormMain_SizeChanged(object sender, EventArgs e)
        {
			int total_pre = 0;
			foreach (DataGridViewColumn item in this.dataGridView1.Columns)
			{
				total_pre += item.Width;
			}
			foreach (DataGridViewColumn item in this.dataGridView1.Columns)
			{
				item.Width = (int)((double)this.Width * (double)item.Width / (double)total_pre);
			}
		}

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewCell cell in this.dataGridView1.SelectedCells)
			{
				DataRow _DataRow = (cell.OwningRow.DataBoundItem as DataRowView).Row;
				AddrValueList _AddrValueList = this.DataArray.Of(_DataRow);
				string prop = cell.OwningColumn.DataPropertyName;
				AddrValue _AddrValue = _AddrValueList.FirstOrDefault(w=>w.Name== prop);
				if ( _AddrValue != null && !_AddrValue.ReadOnly) {
					if (this.serialPort1.IsOpen)
					{
						WritingValues = true;
						FormModify.Show(_AddrValue, this,   _AddrValueList,this.DataArray);
					}
					else
					{
						this.Message("请打开串口");
						this.timer1.Enabled = false;
					}
				}
			}
        }

		DataGridViewCellStyle dataGridViewCellStyleEditAble = null;
		DataGridViewCellStyle dataGridViewCellStyleReadOnly = null;
		private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
			if (e.RowIndex == -1||e.ColumnIndex==-1 || DataArray==null) return;

            try
            {
				DataGridViewCell cell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
				DataRow _DataRow = (cell.OwningRow.DataBoundItem as DataRowView).Row;
				AddrValueList _AddrValueList = this.DataArray.Of(_DataRow);
				string prop = cell.OwningColumn.DataPropertyName;
				AddrValue add = _AddrValueList.FirstOrDefault(w => w.Name == prop);
				if (add != null && !add.ReadOnly)
				{
					if (dataGridViewCellStyleEditAble == null)
					{
						dataGridViewCellStyleEditAble = (new DataGridViewCellStyle() { BackColor = Color.GreenYellow, ForeColor = Color.Black });
						dataGridViewCellStyleEditAble.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;//211, 223, 240
						dataGridViewCellStyleEditAble.BackColor = Color.GreenYellow;
						dataGridViewCellStyleEditAble.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
						dataGridViewCellStyleEditAble.ForeColor = Color.Black;
						dataGridViewCellStyleEditAble.SelectionBackColor = System.Drawing.SystemColors.Highlight;
						dataGridViewCellStyleEditAble.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
					}
					dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = dataGridViewCellStyleEditAble;
				}
				else
				{
					if (dataGridViewCellStyleReadOnly == null)
					{
						dataGridViewCellStyleReadOnly = (new DataGridViewCellStyle() { BackColor = Color.WhiteSmoke, ForeColor = Color.Black });
						dataGridViewCellStyleReadOnly.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;//211, 223, 240
						dataGridViewCellStyleReadOnly.BackColor = Color.WhiteSmoke;
						dataGridViewCellStyleReadOnly.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
						dataGridViewCellStyleReadOnly.ForeColor = Color.Black;
						dataGridViewCellStyleReadOnly.SelectionBackColor = System.Drawing.SystemColors.Highlight;
						dataGridViewCellStyleReadOnly.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
					}
					dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = dataGridViewCellStyleReadOnly;
				}
			}
            catch (Exception)
            {
				 
            }
			
		}

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
			foreach (DataGridViewCell cell in this.dataGridView1.SelectedCells)
			{
				DataRow _DataRow = (cell.OwningRow.DataBoundItem as DataRowView).Row;
				AddrValueList _AddrValueList = this.DataArray.Of(_DataRow);
				string prop = cell.OwningColumn.DataPropertyName;
				AddrValue add = _AddrValueList.FirstOrDefault(w => w.Name == prop);
				if (add != null && !add.ReadOnly)
				{
					dataGridView1_CellDoubleClick(null, null);
				}
			}
		}

    }

}
