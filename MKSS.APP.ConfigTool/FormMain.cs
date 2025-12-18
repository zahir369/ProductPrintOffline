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

namespace MKSS.APP.ConfigTool
{
    public partial class FormMain : Form
    {

		 
		SerialPort serialPort1 = new SerialPort();
		private ModbusFactory modbusFactory;
		public IModbusMaster _master;
		MainConfig config = null;
		List<AddrConfigExtend> queryExtend = new List<AddrConfigExtend>();
		public FormMain()
        {

            InitializeComponent();
            this.Text = "出错了！！！";
            CbxSerialPorts_MouseDoubleClick(null, null);
			config = MainConfig.Instance;

			this.Text = "美克盛世 "+config.SoftName;
			this.checkBox1.Text = config.AutoRefreshInterval + "ms自动刷新";
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
			if (!string.IsNullOrEmpty(defaultCOMM))
			{
				CbxSerialPorts.SelectedItem = defaultCOMM;
			}
			if (!string.IsNullOrEmpty(defaultAddr))
			{
				this.TxtAddr.Text = defaultAddr;
			}

			this.dataGridView1.DataSource = config.Addrs;
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
			this.TxtAddr.Enabled = !IsOpen;
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
						this.serialPort1.Parity = Parity.None;
						this.serialPort1.StopBits = StopBits.One;
						try
						{
							this.serialPort1.Open();
							this.Message("串口打开成功");
							this._master = modbusFactory.CreateRtuMaster(this.serialPort1);
							this._master.Transport.ReadTimeout = 1000;
							this._master.Transport.WriteTimeout = 1000;
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

		public bool WriteValue(AddrValue addr,ushort value) {
			try
			{
				 _master.WriteSingleRegister((byte)int.Parse(this.TxtAddr.Text), addr.Address, value);//测试读取数据
				addr.Value = value;
				return true;
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
                    try
                    {
                        foreach (AddrConfigExtend ext in this.queryExtend)
                        {
							ushort[] ret = null;
							try
							{
								ret = _master.ReadHoldingRegisters((byte)int.Parse(this.TxtAddr.Text), (ushort)ext.Min, (ushort)(ext.Max - ext.Min + 1));//测试读取数据
							}
							catch (Exception ex)
							{
								this.Message(string.Format("读取：{0} 出错", ext));
								this.Message(ex.Message);
								this.Message(ex.StackTrace);
								continue;
							}
							foreach (var item in ext.address_dic)
							{
								try
								{
									ushort val = ret[(item.Address - ext.Min)];
									item.Value = val;
									item.RefreshTime = DateTime.Now;
								}
								catch (Exception ex)
								{
									this.Message(string.Format("提取：{0} {1}出错", ext, item));
									this.Message(ex.Message);
									this.Message(ex.StackTrace);
								}
							}
							

					
						}
						this.BeginInvoke(new EventHandler(delegate {
							dataGridView1.Invalidate();
							this.dataGridView1.Update();
						}));
					}
                    catch (Exception ex)
                    {
						this.Message(ex.Message);
						this.Message(ex.StackTrace);
					}
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
				AddrValue _AddrValue = cell.OwningRow.DataBoundItem as AddrValue;
				if (_AddrValue != null && !_AddrValue.ReadOnly) {
					if (this.serialPort1.IsOpen)
					{
						FormModify _FormModify = new FormModify(_AddrValue, this);
						_FormModify.TopMost = true;
						_FormModify.Owner = this;
						_FormModify.StartPosition = FormStartPosition.CenterScreen;
						_FormModify.Show();
					}
					else
					{
						this.Message("请打开串口");
						this.timer1.Enabled = false;
					}
				}
			}
        }
		 

        private void dataGridView1_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
			if (e.RowIndex == -1||e.ColumnIndex==-1) return;
			if (e.ColumnIndex == dataGridView1.Columns["修改"].Index)
			{
				AddrValue add = dataGridView1.Rows[e.RowIndex].DataBoundItem as AddrValue;
				if (add.ReadOnly)
				{
					DataGridViewCellStyle dataGridViewCellStyle2 = (new DataGridViewCellStyle());
					dataGridViewCellStyle2.Padding = new Padding(0, 200, 0, 0);
					dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Style = dataGridViewCellStyle2;
				}
				else
				{

				}
			}
		}

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
			if (e.ColumnIndex == dataGridView1.Columns["修改"].Index) {
				AddrValue add = dataGridView1.Rows[e.RowIndex].DataBoundItem as AddrValue;
				if (!add.ReadOnly) {
					dataGridView1_CellDoubleClick(null,null);
				}
			}
		}
    }

}
