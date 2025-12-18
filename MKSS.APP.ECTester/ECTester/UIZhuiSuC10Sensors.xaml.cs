using MKSS.APP.ECTester.Print;
using MKSS.Model;
using MKSS.Service.ECTester;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace MKSS.APP.ECTester
{
    /// <summary>
    /// UIZhuiSuC10Sensors.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10Sensors : UserControl
    {
         
        SensorItemEnum mode = SensorItemEnum.SrcData;
        public SensorItemEnum Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
                CalcTitle();
                foreach (UIZhuiSuC10SensorItem item in ItemDic.Values)
                {
                    item.Mode = mode;
                }
            }
        }

        int DefNo = 0;
        public void CalcTitle()
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
             

            if (GroupData == null||Batch==null)
            {
                Refreshtime.Content = string.Format("刷新时间：____-__ __:__:__");
            }
            else {
                if (PreviousMode)
                {
                    
                }
                else
                {
                    Refreshtime.Content = string.Format("刷新时间：{0}", (Batch.F_AgingStartTime.AddSeconds(GroupData.F_AddTime)).ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }

            if (PreviousMode)
            {
                Refreshtime.Content = (DefNo >= 0) ? ("No." + DefNo.ToString("0000000")) : "";
                if (ECTesterConfgig.AutoPrintPPMSensibility)
                {
                    //PrintPreviewInner(BarCodeMode.None);
                }
                this.ShowName.Content = "灵敏度（nA/ppm）";
            }
            else {
                switch (mode)
                {
                    case SensorItemEnum.SrcData:
                        this.ShowName.Content = string.Format((SensorGroupData.ShowModeDescXX));
                        break;
                    case SensorItemEnum.ColorDiagram:
                        this.ShowName.Content = string.Format((SensorGroupData.ShowModeDescXX));
                        break;
                    default:
                        break;
                }
            }

        }

        public SensorItemEnum ColorDiagramFull
        {
            get
            {
                if (UIZhuiSuC10Model.Instance.SettingModel.ProductConfig == null) return SensorItemEnum.SrcData;
                return UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramFull;
            }
            set
            {
                UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.ColorDiagramFull = value;
                ECTesterConfgig.Save();
            }
        } 

        public Dictionary<string, UIZhuiSuC10SensorItem> ItemDic = new Dictionary<string, UIZhuiSuC10SensorItem>();
        public UIZhuiSuC10Sensors()
        {

            InitializeComponent();

            string[] region = ECTesterService.Regions;
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= ECTesterService.RegionSize; c++)
                {
                    string n = string.Format("Cell{0}{1}", boardNo, c);
                    UIZhuiSuC10SensorItem t1 = UIZhuiSuC10SensorsAdd.GetChildObject<UIZhuiSuC10SensorItem>(AllText, n);
                     
                    if (t1 != null)
                    {
                        ItemDic.Add(n,t1);
                    }
                }
            }

            try
            {
                PrintSetting.Visibility = ECTesterService.IndustryMode ? Visibility.Visible : Visibility.Collapsed;
                //根据模式切换布局
                if (ECTesterService.ECSensotMode == ECSensotMode.IndustryMode
                    ||
                    ECTesterService.ECSensotMode == ECSensotMode.AcidityMode)
                {
                    //工业传感器 8 * 8， 4 * *
                    foreach (var boardNo in ECTesterService.Regions)
                    {
                        for (int c = 1; c <= ECTesterService.RegionSize; c++)
                        {
                            string n = string.Format("Cell{0}{1}", boardNo, c);
                            if (ItemDic.ContainsKey(n))
                            {
                                UIZhuiSuC10SensorItem t1 = ItemDic[n];
                                Grid.SetColumn(t1, c % 4 == 0 ? 6 : (c % 4) * 2 - 2);
                                Grid.SetColumnSpan(t1, 2);
                                Grid.SetRow(t1, (c - 1) / 4 );
                                Grid.SetRowSpan(t1, 1);
                            }
                        }
                    }
                }

                if (ECTesterService.ECSensotMode == ECSensotMode.AqueousMode)
                {
                    //水性传感器 16 * 4， 8 * 2
                    foreach (var boardNo in ECTesterService.Regions)
                    {
                        for (int c = 1; c <= ECTesterService.RegionSize; c++)
                        {
                            string n = string.Format("Cell{0}{1}", boardNo, c);
                            if (ItemDic.ContainsKey(n))
                            {
                                UIZhuiSuC10SensorItem t1 = ItemDic[n];
                                Grid.SetColumn(t1, (c -1) % 8);
                                Grid.SetColumnSpan(t1, 1);
                                Grid.SetRow(t1, c <= 8 ? 0 : 2);
                                Grid.SetRowSpan(t1, 2);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回

        }

        public List<string> SelectSensorPositions {
            get {
                List<string> ret = new List<string>();
                string[] region = ECTesterService.Regions;
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= ECTesterService.RegionSize; c++)
                    {
                        string n = string.Format("Cell{0}{1}", boardNo, c);
                        if (!ItemDic.ContainsKey(n)) continue;
                        UIZhuiSuC10SensorItem t1 = ItemDic[n];
                        if (t1 != null && t1.IsChecked!=null && t1.IsChecked.Value)
                        {
                            ret.Add(string.Format("{0}{1}", boardNo, c));
                        }
                    }
                }
                return ret;
            }
        }

        public List<string> SelectSensorIds
        {
            get
            {
                List<string> ret = new List<string>();
                string[] region = ECTesterService.Regions;
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= ECTesterService.RegionSize; c++)
                    {
                        string n = string.Format("Cell{0}{1}", boardNo, c);
                        if (!ItemDic.ContainsKey(n)) continue;
                        UIZhuiSuC10SensorItem t1 = ItemDic[n];
                        if (t1 != null && t1.IsChecked != null && t1.IsChecked.Value && t1.Sensor != null) {
                            ret.Add(t1.Sensor.F_SensorId);
                        } 
                    }
                }
                return ret;
            }
        }

        public void SetFontColor()
        {
            string[] region = ECTesterService.Regions;
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= ECTesterService.RegionSize; c++)
                {
                    string n = string.Format("Cell{0}{1}", boardNo, c);
                    if (!ItemDic.ContainsKey(n)) continue;
                    UIZhuiSuC10SensorItem t1 = ItemDic[n];

                    if (t1 != null)
                    {
                        t1.SetFontColor();
                    }
                }
            } 
        }

        bool PreviousMode { get; set; }
        public Batch Batch { get; set; }
        public void IniTask(Model.Batch _Batch, List<Sensor> datas)
        {
            Batch = _Batch;
            GroupData = null;
            if (datas.Count > 0 && _Batch != null)
            {
                string[] region = ECTesterService.Regions;
                for (int r = 0; r < region.Length; r++)
                {
                    string boardNo = region[r];
                    for (int c = 1; c <= ECTesterService.RegionSize; c++)
                    {
                        string n = string.Format("Cell{0}{1}", boardNo, c);
                        if (!ItemDic.ContainsKey(n)) continue;
                        UIZhuiSuC10SensorItem t1 = ItemDic[n];
                        if (t1 != null)
                        {
                            t1.T1_Click(null, null);//刷新选中状态
                        }
                    }
                }
            }
            else
            {

            }
        }
        public SensorGroupData GroupData { get; set; }
        public void SetData(Model.Batch _Batch, SensorGroupData datas,int defNo ,bool _PreviousMode )
        {
            Batch = _Batch;
            GroupData = datas;
            PreviousMode = _PreviousMode;
            if (defNo>=0) DefNo = defNo;
            if (datas!=null && datas.Data.Count > 0 && _Batch != null)
            {

                string[] region = ECTesterService.Regions;
                foreach (var boardNo in region)
                {
                    for (int c = 1; c <= ECTesterService.RegionSize; c++)
                    {
                        string n = string.Format("Cell{0}{1}", boardNo, c);
                        if (!ItemDic.ContainsKey(n)) continue;
                        UIZhuiSuC10SensorItem t1 = ItemDic[n];
                        if (t1 != null) {
                            string F_SensorId = Sensor.CreateCensorId(Batch.F_BatchId, boardNo, c);
                            PosEnum posenum = Sensor.CreatePosEnum(Batch.F_BatchId, boardNo, c);
                            if (datas.Data.ContainsKey(posenum))
                            {
                                t1.SetSensorData(_Batch, datas, posenum);
                                if (t1.IsChecked!=null && t1.IsChecked.Value) {
                                    //t1.T1_Click(null, null);
                                }
                            }
                        } 
                    }
                }
            }
            else
            {

            }
            //Refreshtime.Visibility = mode == SensorItemEnum.ColorDiagram ? Visibility.Collapsed : Visibility.Visible;
            if (GroupData == null || Batch == null)
            {
                Refreshtime.Content = string.Format("刷新时间：____-__ __:__:__");
            }
            else
            {
                if (PreviousMode)
                {
                    
                }
                else { 
                    Refreshtime.Content = string.Format("刷新时间：{0}" , (Batch.F_AgingStartTime.AddSeconds(GroupData.F_AddTime)).ToString("yyyy-MM-dd HH:mm:ss"));
                }
            }
            if (PreviousMode)
            {
                Refreshtime.Content = (DefNo >= 0) ? ("No." + DefNo.ToString("0000000")) : "";
                if (ECTesterConfgig.AutoPrintPPMSensibility)
                {

                    UIZhuiSuC10 con10 = UIZhuiSuC10Model.Instance.View;
                    con10.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                        PrintPreviewInner(XMode.None);
                    });
                    
                }
            }

        }

        public void Refresh()
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            string[] region = ECTesterService.Regions;
            for (int r = 0; r < region.Length; r++)
            {
                string boardNo = region[r];
                for (int c = 1; c <= ECTesterService.RegionSize; c++)
                {
                    string n = string.Format("Cell{0}{1}", boardNo, c);
                    if (!ItemDic.ContainsKey(n)) continue;
                    UIZhuiSuC10SensorItem t1 = ItemDic[n];
                    if (t1 != null)
                    {
                        t1.Refresh();
                    }
                }
            }
            CalcTitle();
            AutoPrintPPMSensibilityLoading = true;
            AutoPrintPPMSensibility.IsChecked = ECTesterConfgig.AutoPrintPPMSensibility;
            DoPrint.IsEnabled = !ECTesterConfgig.AutoPrintPPMSensibility;
            AutoPrintPPMSensibilityLoading = false;
        }

        private void btnPrintPreview_Click(object sender, RoutedEventArgs e)
        {
            PrintPreviewInner(XMode.None);
        }

        public PrintDataList PrintData(int sCount = 64) {
            PrintDataList data = new PrintDataList()
            {
                List = new List<PrintDataItem>()
            };


            string[] region = ECTesterService.Regions;
            foreach (var boardNo in region)
            {
                for (int c = 1; c <= ECTesterService.RegionSize; c++)
                {
                    string n = string.Format("Cell{0}{1}", boardNo, c);
                    if (!ItemDic.ContainsKey(n)) continue;
                    if (data.List.Count >= sCount) continue;//只打印前 多少个
                    UIZhuiSuC10SensorItem t1 = ItemDic[n];
                    if (t1 != null)
                    {
                        if (GroupData != null && GroupData.Data.Count > 0 && Batch != null)
                        {
                            string F_SensorId = Sensor.CreateCensorId(Batch.F_BatchId, boardNo, c);
                            PosEnum posenum = Sensor.CreatePosEnum(Batch.F_BatchId, boardNo, c);
                            if (GroupData.Data.ContainsKey(posenum))
                            {
                                data.List.Add(new PrintDataItem()
                                {
                                    REGION = boardNo,
                                    REGION_INDEX = c,
                                    CONTENT = string.Format("{0}nA/ppm", t1.ColorPanelFull.Content)
                                });
                            }
                        }
                        else
                        {
                            data.List.Add(new PrintDataItem()
                            {
                                REGION = boardNo,
                                REGION_INDEX = c,
                                CONTENT = string.Format("-.--nA/ppm", t1.Content)
                            });
                        }

                    }
                }
            }
            return data;
        }
        private void PrintPreviewInner(XMode BarMode)
        {

            //TextBlock text = new TextBlock();
            //text.Text = "9.6nA/ppm";
            //text.LayoutTransform = new RotateTransform(90);
            //PrintDialog printDlg = new PrintDialog();
            //printDlg.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;
            //printDlg.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.Unknown, 4.00 / 25.4 * 96, 1.50 / 25.4 * 96);
            //printDlg.PrintVisual(text, "sum");

            //if (1 == 1) return;
            PrintDataList data = PrintData( );

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
                PrintPreviewWindow previewWnd = new PrintPreviewWindow( data, renderer);
                previewWnd.Owner = MainWindow.Instance;
                previewWnd.ShowInTaskbar = false;
                previewWndPub = previewWnd;
                previewWnd.ShowDialog();
            }
            else {
                PrintDialog printDlg = new PrintDialog();
                var doc = PrintPreviewWindow.LoadDocumentAndRender( data, renderer);
                printDlg.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "Doc" + (Batch == null ? "Default" : Batch.F_BatchId));
            }


            //// Create a PrintDialog 
            //PrintDialog printDlg = new PrintDialog();
            //printDlg.PrintTicket.PageOrientation = System.Printing.PageOrientation.Landscape;
            //printDlg.PrintTicket.PageMediaSize = new PageMediaSize(PageMediaSizeName.Unknown, 4.00 / 25.4 * 96, 1.50 / 25.4 * 96);
            //printDlg.PrintDocument(((IDocumentPaginatorSource)doc).DocumentPaginator, "sum");

            ////PrintDialog printDlg = new PrintDialog();
            ////LocalPrintServer ps = new LocalPrintServer();
            ////PrintQueue pq = ps.DefaultPrintQueue;

            ////PrintTicket pt = pq.UserPrintTicket;

            ////pt.PageOrientation = PageOrientation.Landscape;

            ////FlowDocument doc = CreateFlowDocumentSum();


            ////doc.PageHeight = 768;
            ////doc.PageWidth = 1104;

            ////PageMediaSize pageMediaSize = new PageMediaSize(doc.PageWidth, doc.PageHeight);
            ////pt.PageMediaSize = pageMediaSize;
            ////IDocumentPaginatorSource source = doc as IDocumentPaginatorSource;


            //printDlg.PrintDocument(source.DocumentPaginator, "sum");
            //previewWnd.Owner = MainWindow.Instance;
            //previewWnd.ShowInTaskbar = false;
            //previewWndPub = previewWnd;
            //previewWnd.ShowDialog();


            ////System.Drawing.Printing.PrintDocument pd = new System.Drawing.Printing.PrintDocument();
            ////pd.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(
            ////    (object sender, System.Drawing.Printing.PrintPageEventArgs ev)=>
            ////        {
            ////            ev.Graphics.DrawString("6.01nA/ppm", new System.Drawing.Font(new System.Drawing.FontFamily("等线"), 10.5), System.Drawing.Brushes.Black, 0, 0);
            ////        }

            ////    );
            ////pd.Print();

        }
        
        PrintPreviewWindow previewWndPub = null;

        bool AutoPrintPPMSensibilityLoading = false;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            AutoPrintPPMSensibilityLoading = true;
            AutoPrintPPMSensibility.IsChecked = ECTesterConfgig.AutoPrintPPMSensibility;
            DoPrint.IsEnabled = !ECTesterConfgig.AutoPrintPPMSensibility;
            AutoPrintPPMSensibilityLoading = false;
        }

        private void AutoPrintPPMSensibility_Checked(object sender, RoutedEventArgs e)
        {
            if (AutoPrintPPMSensibilityLoading) return;
            ECTesterConfgig.AutoPrintPPMSensibility = AutoPrintPPMSensibility.IsChecked != null && AutoPrintPPMSensibility.IsChecked.Value;
            ECTesterConfgig.Save();
            DoPrint.IsEnabled = !ECTesterConfgig.AutoPrintPPMSensibility;
        }
         

        /// <summary>
        ///  设置打印模板，打印模板只在工业传感器大标签打印模式生效
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PrintSetting_Click(object sender, RoutedEventArgs e)
        {
            if (ECTesterService.IndustryMode && !ECTesterConfgig.Instance.IndustryModeLabelSmall)
            {
                PrintTemplateDefine.ShowPrintTemplateDefine();
            }
        }

        private void DoPrint_Click(object sender, RoutedEventArgs e)
        {
            PrintSelect.ShowPrintSelect(this);
        }


    }
}
