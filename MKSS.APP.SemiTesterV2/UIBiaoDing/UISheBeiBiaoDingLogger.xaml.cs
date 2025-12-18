using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MKSS.APP.UIBiaoDing
{
    /// <summary>
    /// UISheBeiBiaoDingLogger.xaml 的交互逻辑
    /// </summary>
    public partial class UISheBeiBiaoDingLogger : UserControl
    {
        public UISheBeiBiaoDingLogger()
        {
            InitializeComponent();
            if (UISheBeiBiaoDingViewModel.IsInDesignMode(this)) return;//设计模式直接返回
            //MKSS.Util.Log.ULogger.OnLog += ULogger_OnLog;
        }

        private void ULogger_OnLog(object messsage)
        {
            //直接刷新的数据，不需要长时间统计的直接显示
            this.Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    this.uLOgger.AppendText(messsage+"");
                    this.uLOgger.AppendText(System.Environment.NewLine);
                }
                catch (Exception)
                {

                }
            }));
        }
    }
}
