using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MKSS.APP.WinNBOnlinePrinterTest
{
    public partial class SplashForm : Form
    {

        public static SplashForm Instance { get; set; }
        public SplashForm()
        {
            InitializeComponent();
            Instance = this;
        }
        public string Message { get { return labelMsg.Text; } set { labelMsg.Text = value; } }

    }
}
