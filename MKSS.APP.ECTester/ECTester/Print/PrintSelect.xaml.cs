using MKSS.APP.UserControls;
using MKSS.Model;
using MKSS.Service.ECTester;
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
using System.Windows.Shapes;

namespace MKSS.APP.ECTester.Print
{
    /// <summary>
    /// UISerialNo.xaml 的交互逻辑
    /// </summary>
    public partial class PrintSelect : Window
    {
        MKSS.APP.ECTester.UIZhuiSuC10Sensors Page;
        public PrintSelect()
        {
            InitializeComponent();
        }

        public static void ShowPrintSelect(MKSS.APP.ECTester.UIZhuiSuC10Sensors page)
        {

            PrintSelect ww = new PrintSelect();
            ww.Owner = MainWindow.Instance;
            ww.Page = page;
            ww.ShowInTaskbar = false;
            ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ww.TabItemSingle.IsSelected = true;
            ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ww.Show();

        }
            

         

        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            int xxf = 0;
            if (string.IsNullOrEmpty(PrintLabelCount.Text) || !int.TryParse(PrintLabelCount.Text, out xxf))
            {
                MessageBox.Show("请输入要打印的传感器个数");
                return;
            }
            ECTesterConfgig.Instance.PrintLabelCount = xxf;
            ECTesterConfgig.Save();

            PrintPreviewInner(XMode.None);
            this.Close();
        }


        private void PrintPreviewInner(XMode BarMode)
        {
 
            PrintDataList data = Page.PrintData(ECTesterConfgig.Instance.PrintLabelCount);
             

            PrintRendererInterface renderer = null;
            //根据模式切换布局
            if (ECTesterService.IndustryMode)
            {
                //工业传感器 8 * 8， 4 * *
                if (ECTesterConfgig.Instance.IndustryModeLabelSmall)
                {
                    renderer = new RendererIndustry(XMode.None);//小标签
                }
                else
                {
                    renderer = new RendererIndustryLarge(XMode.None);//大标签
                }
            }
            else
            {
                //水性传感器 16 * 4， 8 * 2
                renderer = new RendererAqueous(XMode.None);
            }

            if (ECTesterService.DebugPrint)
            {
                if (previewWndPub != null)
                {
                    previewWndPub.Close();
                    previewWndPub = null;
                }
                PrintPreviewWindow previewWnd = new PrintPreviewWindow(data, renderer);
                previewWnd.Owner = MainWindow.Instance;
                previewWnd.ShowInTaskbar = false;
                previewWndPub = previewWnd;
                previewWnd.ShowDialog();
            }
            else
            {
                PrintDialog printDlg = new PrintDialog();
                var doc = PrintPreviewWindow.LoadDocumentAndRender(data, renderer);
                printDlg.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Doc" + (Page.Batch == null ? "Default" : Page.Batch.F_BatchId));
            }
             

        }

        PrintPreviewWindow previewWndPub = null;


    }
}
