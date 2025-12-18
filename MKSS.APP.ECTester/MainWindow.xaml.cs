
using MKSS.APP.ECTester.UIControls;
using MKSS.APP.ECTester.UserCommon;
using MKSS.APP.ECTester.UserControls;
using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using MKSS.IServices;
using MKSS.Model.ViewModel;
using MKSS.Services;
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
using MKSS.APP.ECTester;
using MKSS.Service.ECTester;
using MKSS.Util.Log;
using System.Windows.Threading;
using System.Printing;
using MKSS.APP.ECTester.Print;

namespace MKSS.APP.ECTester
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance;
        DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {

            MKSS.APP.UserControls.WaitWindow.ShowWindow("正在启动", "正在启动，请稍等......", this);
            Instance = this;
            InitializeComponent();

            switch (ECTesterService.ECSensotMode)
            {
                case ECSensotMode.IndustryMode:
                    this.Title = "智慧工厂生产数字化管理系统-电化学工业传感器测试系统";
                    this.MainTitle.Text = "智慧工厂生产数字化管理系统-电化学工业传感器测试系统";
                    break;
                case ECSensotMode.AqueousMode:
                    this.Title = "智慧工厂生产数字化管理系统-电化学水性传感器测试系统";
                    this.MainTitle.Text = "智慧工厂生产数字化管理系统-电化学水性传感器测试系统";
                    break;
                case ECSensotMode.AcidityMode:
                    this.Title = "智慧工厂生产数字化管理系统-电化学酸性两电极传感器测试系统";
                    this.MainTitle.Text = "智慧工厂生产数字化管理系统-电化学酸性两电极传感器测试系统";
                    break;
                default:
                    break;
            } 


            timer.Interval = new TimeSpan(0, 0, 1);//设置的间隔为5s
            timer.Tick += Timer_Tick;
            timer.IsEnabled = true;
            timer.Start();

        }
        public bool TimerRefreshIng { get { return false; } }
        private void Timer_Tick(object sender, EventArgs e)
        {

            DateTime d = DateTime.Now;
            TimeNow.Content = d.ToString("yyyy-MM-dd HH:mm:ss") + (TimerRefreshIng ? "..." : "");

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                UIZhuiSuC10Model.Instance.ECService.Cancel = true;
                ECTesterSaver.Dispose();
            }
            catch (Exception)
            {

            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                UIZhuiSuC10Model.Instance.ECService.Cancel = true;
                ECTesterSaver.Dispose();

            }
            catch (Exception)
            {

            }
        }

        void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10SensorsSET.BtnFinish_Click(null, null);
        }

        [LogTagClass(Title = "Window_KeyDown")]
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            ULogger.Log(e.Key.ToString()); 
            if (e.Key == Key.Enter || e.Key == Key.S)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsSET.BtnStart_Click(null, null);
            }
            if (e.Key == Key.Space || e.Key == Key.P)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsSET.BtnPause_Click(null, null);
            }


            if ( e.Key == Key.Q)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsSET.BtnFinish_Click(null, null);
            }
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsSET.BtnFinish_Click(null, null);
            }


            if ( e.Key == Key.E)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsSET.BtnExcelExport_Click(null, null);
            }
            if (e.Key == Key.U)
            {
                e.Handled = true;
                BtnExit_Click(null, null);
            }

            if ( e.Key == Key.Left)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowMode_Pre();
            }
            if ( e.Key == Key.Right)
            {
                e.Handled = true;
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowMode_Next();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            MKSS.APP.UserControls.WaitWindow.CloseWindow(this);


            /**
            PrintDataList data = new PrintDataList()
            {
                List = new List<PrintDataItem>()
            };

            data.List.Add(new PrintDataItem()
            {
                REGION = "A",
                REGION_INDEX = 1,
                CONTENT = string.Format("{0}nA/ppm", 6.63)
            }); ;
            data.List.Add(new PrintDataItem()
            {
                REGION = "A",
                REGION_INDEX = 2,
                CONTENT = string.Format("{0}nA/ppm", 6.64)
            }); ;
            data.List.Add(new PrintDataItem()
            {
                REGION = "A",
                REGION_INDEX = 3,
                CONTENT = string.Format("{0}nA/ppm", 6.65)
            }); 

            PrintRendererInterface renderer = new RendererIndustryLarge(XMode.None);//大标签

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
                printDlg.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Doc");
            }
            **/

            //PrintDialog printDlg = new System.Windows.Controls.PrintDialog();

            //PrintTicket pt = printDlg.PrintTicket;
            //Double printableWidth = pt.PageMediaSize.Width.Value;
            //Double printableHeight = pt.PageMediaSize.Height.Value;
            //PageMediaSize paper = new PageMediaSize(PageMediaSizeName.Unknown, 4.00 / 25.4 * 96, 1.50 / 25.4 * 96);
            //pt.PageMediaSize = paper;

            //Double xScale = (printableWidth - xMargin * 2) / printableWidth;
            //Double yScale = (printableHeight - yMargin * 2) / printableHeight;
            //this.Transform = new MatrixTransform(xScale, 0, 0, yScale, xMargin, yMargin);



            //System.Windows.Controls.Grid grid = new Grid()
            //{
            //    Width = paper.Width.Value,
            //    Height = paper.Height.Value
            //};
            //grid.RowDefinitions.Add(new RowDefinition());
            //for (int i = 0; i < 8; i++)
            //{
            //    grid.ColumnDefinitions.Add(new ColumnDefinition());
            //    TextBlock text = new TextBlock();
            //    text.Text = "1.6nA/ppm";
            //    text.FontSize = 8; 
            //    grid.Children.Add(text);
            //    Grid.SetColumn(text, i);
            //}

            //TextBlock text11 = new TextBlock();
            //text11.Text = "5.6nA/ppm";
            //text11.FontSize = 8; text11.Margin = new Thickness(0,0,0,0);
            //FlowDocument myFlowDocument = new FlowDocument();
            //myFlowDocument.FontSize = 8;
            //myFlowDocument.PageHeight = 4.00 / 25.4 * 96;
            //myFlowDocument.PageWidth = 1.50 / 25.4 * 96;
            //myFlowDocument.PagePadding = new Thickness(0, 0, 0, 0);
            //myFlowDocument.ColumnWidth = 1 / 25.4 * 96;
            //myFlowDocument.Blocks.Add(new Paragraph(new Run("5.6nA/ppm") { }) { Margin=new Thickness(3,3,3,3) });
            //printDlg.PrintQueue.DefaultPrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.Unknown, 4.00 / 25.4 * 96, 1.50 / 25.4 * 96);
            ////now print the visual to printer to fit on the one page.
            //printDlg.PrintDocument(((IDocumentPaginatorSource)myFlowDocument).DocumentPaginator, "Print Page");


            //PageMediaSize paper = new PageMediaSize(PageMediaSizeName.Unknown, 4.00 / 25.4 * 96, 1.50 / 25.4 * 96);
            //System.Windows.Controls.Grid grid = new Grid()
            //{
            //    Width = paper.Width.Value,
            //    Height = paper.Height.Value
            //};
            //grid.RowDefinitions.Add(new RowDefinition());
            //for (int i = 0; i < 8; i++)
            //{
            //    grid.ColumnDefinitions.Add(new ColumnDefinition());
            //    TextBlock text = new TextBlock();
            //    text.Text = "1.6nA/ppm";
            //    grid.Children.Add(text);
            //    Grid.SetColumn(text, i);
            //}

            //FlowDocument myFlowDocument = new FlowDocument();
            //myFlowDocument.Blocks.Add(grid); 

            ////text.LayoutTransform = new RotateTransform(90);
            //PrintDialog printDlg = new PrintDialog();
            //printDlg.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;
            //printDlg.PrintTicket.PageMediaSize = paper;
            //printDlg.PrintVisual(grid, "sum");


            /**
            DocumentMiniSNData data = new DocumentMiniSNData()
            {
                SN_LIST = new List<SN_ITEM>()
            };
            for (int r = 1; r <= 8; r++)
            {
                for (int c = 1; c <= 8; c++)
                {
                    data.SN_LIST.Add(new SN_ITEM() { ROW = r, COL = c, CONTENT = string.Format("{0}nA/ppm", (3 + 0.01 * ((r - 0) * 10 + c)).ToString("f2")) });
                }
            }

            if (previewWndPub != null)
            {
                previewWndPub.Close();
                previewWndPub = null;
            }
            PrintPreviewWindow previewWnd = new PrintPreviewWindow("ECTester\\Print\\DocumentMiniSN.xaml", data, new DocumentMiniSNRenderer(BarCodeMode.None));
            previewWnd.Owner = MainWindow.Instance;
            previewWnd.ShowInTaskbar = false;
            previewWndPub = previewWnd;
            previewWnd.ShowDialog();

            **/
        }
        PrintPreviewWindow previewWndPub = null;

        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            System.Environment.Exit(1);
        }

        private readonly MKSS.APP.ECTester.PaletteHelper _paletteHelper = new MKSS.APP.ECTester.PaletteHelper();
        private void ApplyTheme(bool isDark)
        {
            ITheme theme = _paletteHelper.GetTheme();
            IBaseTheme baseTheme = isDark ? new MaterialDesignDarkTheme() : (IBaseTheme)new MaterialDesignLightTheme();
            theme.SetBaseTheme(baseTheme);
            _paletteHelper.SetTheme(theme);
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var b = ToggleDarkStyle.IsChecked;
            ApplyTheme(b!=null && b.Value);
        }
    }
}
