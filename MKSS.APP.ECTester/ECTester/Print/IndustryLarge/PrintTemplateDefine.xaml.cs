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
    public partial class PrintTemplateDefine : Window
    {
        public PrintTemplateDefine()
        {
            InitializeComponent();
        }

        public static void ShowPrintTemplateDefine( )
        {

            PrintTemplateDefine ww = new PrintTemplateDefine();
            ww.Owner = MainWindow.Instance;
            ww.ShowInTaskbar = false;
            ww.PrintTemplateCotent.Text = ECTesterConfgig.Instance.IndustryModeLabelLargeContent;
            ww.IndustryModeLabelXmm.Text = ECTesterConfgig.Instance.IndustryModeLabelXmm.ToString();
            ww.IndustryModeLabelYmm.Text = ECTesterConfgig.Instance.IndustryModeLabelYmm.ToString();
            ww.PrintTemplateCotent.Padding = new Thickness(ECTesterConfgig.Instance.IndustryModeLabelXmm * 10, ECTesterConfgig.Instance.IndustryModeLabelYmm * 10, 0, 0);
            ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ww.TabItemSingle.IsSelected = true;
            ww.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ww.IndustryModeLabelSmall.IsChecked = ECTesterConfgig.Instance.IndustryModeLabelSmall;
            ww.Show();

        }
           

        private void BtnTmpSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PrintTemplateCotent.Text))
            {
                MessageBox.Show("请输入打印内容");
                return;
            }
            ECTesterConfgig.Instance.IndustryModeLabelLargeContent = PrintTemplateCotent.Text;
            ECTesterConfgig.Instance.IndustryModeLabelSmall = IndustryModeLabelSmall.IsChecked != null && IndustryModeLabelSmall.IsChecked.Value;
            ECTesterConfgig.Instance.IndustryModeLabelYmm = double.Parse(IndustryModeLabelYmm.Text);
            ECTesterConfgig.Instance.IndustryModeLabelXmm = double.Parse(IndustryModeLabelXmm.Text);
            ECTesterConfgig.Save();
            this.Close();
        }


        private void Txt_InputNumber(object sender, KeyEventArgs e)
        {
            InputNumber(e);

        } 
        public void InputNumber(KeyEventArgs e)
        {
            if (e.Key == Key.Up)
            {
                IndustryModeLabelYmm.Text = (double.Parse(IndustryModeLabelYmm.Text) - 0.1).ToString("f1");
                PrintTemplateCotent.Padding = new Thickness(double.Parse(IndustryModeLabelXmm.Text) * 10, double.Parse(IndustryModeLabelYmm.Text) * 10, 0, 0);
                e.Handled = false; 
            }
            else if (e.Key == Key.Down)
            {
                IndustryModeLabelYmm.Text = (double.Parse(IndustryModeLabelYmm.Text) + 0.1).ToString("f1");
                PrintTemplateCotent.Padding = new Thickness(double.Parse(IndustryModeLabelXmm.Text) * 10, double.Parse(IndustryModeLabelYmm.Text) * 10, 0, 0);
                e.Handled = false; 
            }
            else if (e.Key == Key.Left)
            {
                IndustryModeLabelXmm.Text = (double.Parse(IndustryModeLabelXmm.Text) - 0.1).ToString("f1");
                PrintTemplateCotent.Padding = new Thickness(double.Parse(IndustryModeLabelXmm.Text) * 10, double.Parse(IndustryModeLabelYmm.Text) * 10, 0, 0);
                e.Handled = false; 
            }
            else if (e.Key == Key.Right)
            {
                IndustryModeLabelXmm.Text = (double.Parse(IndustryModeLabelXmm.Text) + 0.1).ToString("f1");
                PrintTemplateCotent.Padding = new Thickness(double.Parse(IndustryModeLabelXmm.Text) * 10, double.Parse(IndustryModeLabelYmm.Text) * 10, 0, 0);
                e.Handled = false; 
            }
            else
            {

                e.Handled = true;
                //System.Windows.MessageBox.Show("请输入数字");
                return;
            }
        }


        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            PrintPreviewInner(XMode.None);
        }

        private void PrintPreviewInner(XMode BarMode)
        {

            PrintDataList data = new PrintDataList()
            {
                List = new List<PrintDataItem>()
            };

            data.List.Add(new PrintDataItem()
            {
                REGION = "A",
                REGION_INDEX = 1,
                CONTENT = string.Format("{0}nA/ppm", 6.63)
            });

            RendererIndustryLarge renderer = new RendererIndustryLarge(XMode.None);//大标签
            renderer.IndustryModeLabelLargeContent = PrintTemplateCotent.Text;
            renderer.Margin = new Thickness(
                renderer.ToPixels(double.Parse(IndustryModeLabelXmm.Text)),
                renderer.ToPixels(double.Parse(IndustryModeLabelYmm.Text)), 0, 0);

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
                printDlg.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "测试打印标签");
            }

             

        }

        PrintPreviewWindow previewWndPub = null;
         

    }
}
