using MKSS.APP.Win4In1Tester;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MKSS.APP.WinNBTester
{
    public partial class UILabelInfo : UserControl
    {
        public UILabelInfo()
        {
            InitializeComponent();
        }

        SenV _SenV = null;
        public SenV SenV {
            set {
                if (value == null)
                {
                    this.labelName.Text = "---";
                    this.labelUnit.Text = "---";
                    this.labelValue.Text = "---";
                    this.labelValue.ForeColor = Color.Black;
                }
                else {
                    _SenV = value;
                    if (_SenV.Overdued)
                    {
                        //2 秒后清空数据
                        this.labelName.Text = "---";
                        this.labelUnit.Text = "---";
                        this.labelValue.Text = "---";
                        this.toolTip1.SetToolTip(labelValue, string.Format(""));
                        this.labelValue.ForeColor = Color.Black;
                    }
                    else
                    {
                        this.labelName.Text = value.Name;
                        this.labelValue.Text = value.ValueActual.ToString();
                        this.toolTip1.SetToolTip(labelValue, string.Format("{0}", DateTime.Now.ToLongTimeString()));
                        this.labelUnit.Text = value.Unit;
                        this.labelValue.ForeColor = Color.Red;
                    }
                }
            }
        }

        public void RefreshInfo() {
            SenV = _SenV;
        }


    }
}
