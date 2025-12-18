using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MKSS.APP.Win4In1Tester
{

    //“√”和“×”
    public partial class UIConnection : UserControl
    {

        public Model4In1Tester Saver = null;
        public UIConnection()
        {
            InitializeComponent();
            if (DesignMode) return;

            Saver = new Model4In1Tester(this);
             
             
            string AssrtString = "";
            ICS = new WinNBTester.UILabelInfo[] {
                    IC1,IC2,IC3,IC4,IC5,IC6,IC7,IC8 
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
                
            });
        }

        public void ResetIC()
        {

            Invoke((Action)delegate
            {
                foreach (var IC in ICS)
                {
                    IC.SenV = null; 
                    ToolTip.SetToolTip(IC, string.Format("{0}", "未知"));
                }
                Application.DoEvents();

            });

        }
         
        public System.Windows.Forms.ToolTip ToolTip { get { return this.toolTip1; } }
        public WinNBTester.UILabelInfo[] ICS = null;
        public string Port { get; set; } = null;

        public void Start()
        {
            if (this.Port == null) return;
            try
            {
                this.COM.Text = this.Port;
                Saver.Connect();
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
                this.TextLogger.AppendText(System.Environment.NewLine);
                this.TextLogger.AppendText(string.Format("[{0}]{1}", DateTime.Now.ToString("HH:mm:ss.fff"), msg));
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
                this.labelV.Text = Saver.Protocal.ToString();
                this.TextLogger.AppendText(System.Environment.NewLine);
                this.TextLogger.AppendText(string.Format("[{0}]{1}", DateTime.Now.ToString("HH:mm:ss.fff"), msg)); 
            });
        }
         

    }
}