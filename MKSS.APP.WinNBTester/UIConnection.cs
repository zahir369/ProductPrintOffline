using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MKSS.APP.WinNBTester
{

    //“√”和“×”
    public partial class UIConnection : UserControl
    {

        public NBTesterSaver Saver = null;
        public UIConnection()
        {
            InitializeComponent();
            if (DesignMode) return;

            Saver = new NBTesterSaver(this);
            Saver.IMEI = "--------------";
            Saver.IMSI = "--------------";
            Saver.ICCID = "--------------------";

            labelSAVECTWING.Visible = FormMain.VersionOnline;
            labelTestCtWing.Visible = FormMain.VersionOnline;
            this.labelIMEI.Text = Saver.IMEI;
            this.labelIMSI.Text = Saver.IMSI;
            this.labelICCID.Text = Saver.ICCID;
            string AssrtString = "";
            ICS = new Label[] {
                    IC1,IC2,IC3,IC4,IC5,IC6,IC7,labelTestCtWing,IC9,IC10,IC11,IC12,IC13,this.labelSAVEDB,labelSAVECTWING
            };

            foreach (var IC in ICS)
            {
                IC.Text = "-";
                IC.ForeColor = Color.Gray;
                ToolTip.SetToolTip(IC, string.Format("{0}", "未知"));
            }

        }

        ~UIConnection() {
            Saver.Dispose();
        }

        public void SaveToDB(bool sucess)
        { 
            Invoke((Action)delegate
            {
                var IC = labelSAVEDB;
                IC.Text = sucess ? "√" : "×";
                IC.ForeColor = sucess ? Color.Green : Color.Red;
                ToolTip.SetToolTip(IC, string.Format("{0}", "存储到产品数据库"));
                FormMain.Instance.RefreshData();
            });
        }
        public void SaveToCTWING(bool sucess)
        {
            Invoke((Action)delegate
            {
                var IC = labelSAVECTWING;
                IC.Text = sucess ? "√" : "×";
                IC.ForeColor = sucess ? Color.Green : Color.Red;
                ToolTip.SetToolTip(IC, string.Format("{0}", "入网登记，存储到电信平台"));
                FormMain.Instance.RefreshData();
            });
        }

        public void TestCTWING(bool sucess)
        {
            Invoke((Action)delegate
            {
                var IC = labelTestCtWing;
                IC.Text = sucess ? "√" : "×";
                IC.ForeColor = sucess ? Color.Green : Color.Red;
                ToolTip.SetToolTip(IC, string.Format("{0}", "入网登记测试，存储到电信平台"));
                FormMain.Instance.RefreshData();
            });
        }

        public void ResetIC()
        {

            Invoke((Action)delegate
            {
                Saver.IMEI = "--------------";
                Saver.IMSI = "--------------";
                Saver.ICCID = "--------------------";
                this.labelIMEI.Text = Saver.IMEI;
                this.labelIMSI.Text = Saver.IMSI;
                this.labelICCID.Text = Saver.ICCID;
                this.labelSignal.Text = string.Format("-%");
                foreach (var IC in ICS)
                {

                    IC.Text = "-";
                    IC.ForeColor = Color.Gray;
                    ToolTip.SetToolTip(IC, string.Format("{0}", "未知"));

                }

                var IC1 = labelSAVEDB;
                IC1.Text = "-";
                IC1.ForeColor =  Color.Gray;
                ToolTip.SetToolTip(IC1, string.Format("{0}", "存储到产品数据库"));
                Application.DoEvents();

            });

        }

        public System.Windows.Forms.Label LabelIMSI { get { return this.labelIMSI; } }
        public System.Windows.Forms.Label LabelIMEI { get { return this.labelIMEI; } }
        public System.Windows.Forms.Label LabelICCID { get { return this.labelICCID; } }
        public System.Windows.Forms.ToolTip ToolTip { get { return this.toolTip1; } }
        public Label[] ICS = null;
        public string Port { get; set; } = null;

        public void Start()
        {
            try
            {
                this.COM.Text = this.Port;
                Saver.BtnConnectCommand();
            }
            catch (Exception ex)
            {
                UpdateTextBox(string.Format("发送命令时出错 {0}", ex.Message));
                UpdateTextBox(ex.Message);
            }
            finally
            {

            }
        }

        public void UpdateTextBox(string msg)
        {
            this.Invoke( (Action)delegate 
            {
                this.TextLoggerResult.AppendText(System.Environment.NewLine);
                this.TextLoggerResult.AppendText(string.Format("[{0}]{1}", DateTime.Now.ToString("HH:mm:ss.fff"), msg));
            });
        }

        public string UpdateText
        {
            set
            {
                UpdateTextBox(value);
            }
        }


        public void UpdateTextBoxCommand(string msg)
        {
            this.Invoke((Action)delegate
            {
                this.TextLogger.AppendText(System.Environment.NewLine);
                this.TextLogger.AppendText(string.Format("[{0}]{1}", DateTime.Now.ToString("HH:mm:ss.fff"), msg));
                this.labelSignal.Text = string.Format("{0}%",Saver.Siganel==0?"-": Saver.Siganel.ToString());
            });
        }
         

    }
}