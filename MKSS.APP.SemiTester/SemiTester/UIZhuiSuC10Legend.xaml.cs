using DeviceDataMonitorWPF;
using Microsoft.Win32;
using MKSS.Model;
using MKSS.Service.SemiTester;
using MKSS.Util.Log;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
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

namespace MKSS.APP.SemiTester
{
    /// <summary>
    /// UIZhuiSuC10Legend.xaml 的交互逻辑
    /// </summary>
    [LogTagClass(Title = "任务管理")]
    public partial class UIZhuiSuC10Legend : UserControl
    {
        UIZhuiSuC10Model _model
        {
            get
            {
                return UIZhuiSuC10Model.Instance;
            }
        }
        public UIZhuiSuC10SettingModel SettingModel;
        public UIZhuiSuC10Legend()
        {
            InitializeComponent();
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (_model == null) return;
            SettingModel = this.DataContext as UIZhuiSuC10SettingModel;
            _model.SettingModel = SettingModel;
           

            IniData(null, null,new Stopwatch());
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            _model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtProductList.SelectionChanged += TxtProductList_SelectionChanged;
            _model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtModelList.SelectionChanged += TxtModelList_SelectionChanged;
            
        }

        public void RefreshButtons()
        {
            if (MainWindow.Instance.BtnFinish == null) return;
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.BtnPause.IsEnabled = this.SettingModel.CanBtnPause;
                this.BtnStart.IsEnabled = this.SettingModel.CanBtnStart;
                MainWindow.Instance.BtnFinish.IsEnabled = this.SettingModel.CanBtnFinish;
                if (this.SettingModel.BtnStartIng)
                {
                    this.BtnStart.Content = "正在开始(S)";
                }
                else {
                    this.BtnStart.Content = this.SettingModel.Started ? "重新开始(S)" : "开始(S)";
                }
                this.BtnStart.Background = this.SettingModel.Started
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#12559d"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3275fd"));

                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.BtnExcelExport.IsEnabled = this.SettingModel.CanBtnExcelExport;
                this.BtnPause.Content = UIZhuiSuC10Model.Instance.ECService.Pause ? "继续(P)" : "暂停(P)";
                this.BtnPause.Background = UIZhuiSuC10Model.Instance.ECService.Pause
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#12559d"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3275fd"));
            }));

        }
        public Batch BatchCurrent { get; set; }
        public List<Sensor> SensorsCurrent { get; set; }
        public void IniData(Batch batch, List<Sensor> sens, Stopwatch watcher)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回

            ULogger.Info("IniData RefreshButtons Start:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            RefreshButtons();

            if (sens == null) sens = new List<Sensor>();

            BatchCurrent = batch;
            SensorsCurrent = sens;
            if (batch != null)
            {
                this.BtnPause.IsEnabled = batch.EnumAgingStatus == EnumAgingStatus.InAging;
            }
            if (_model.View == null) return;
            _model.View.SelectBatch = batch;

            ULogger.Info("IniData DataTimerRefresh Start:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            _model.DataTimerRefresh(batch);
            ULogger.Info("IniData UIZhuiSuC10Grid IniTask Start:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            _model.View.UIZhuiSuC10Grid.IniTask(batch, sens);
            ULogger.Info("IniData UIZhuiSuC10SensorsPage IniTask Start:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            _model.View.UIZhuiSuC10SensorsPage.IniTask(batch, sens);
            ULogger.Info("IniData UIZhuiSuC10SensorsSET IniTask Start:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            _model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.IniTask(batch, sens);
            ULogger.Info("IniData UIZhuiSuC10SensorsSET UIZhuiSuC10Grid SetCaption:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Grid.SetCaption(this.SettingModel.TxtTestTimePointsArr);

            ULogger.Info("IniData SetBatch Start:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            _model.View.UIZhuiSuC10Chart.SetBatch(batch, sens.Select(w => w.PosEnum).ToList(), watcher);

        }
         

        Batch Batch { get; set; }
        public TimeSpan F_AddTime { get; set; }
        public void SetData(Model.Batch _Batch, SensorGroupData datas, TimeSpan _F_AddTime)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            Batch = _Batch;
            F_AddTime = _F_AddTime;
        }
        public void IniTask(Model.Batch _Batch, List<Sensor> datas)
        {
            Batch = _Batch;

        }

        void SaveConfig()
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (this.SettingModel == null) return;
            ProductConfig c = _model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtProductList.SelectedItem as ProductConfig;
            if (c == null) return;
            ProductModelConfig m = _model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtModelList.SelectedItem as ProductModelConfig;
            if (m == null) return;
            SemiTesterConfgig.Instance.SerialPorts = this.SettingModel.TxtSerialPorts;
            SemiTesterConfgig.Instance.SensorGrougAddress = this.SettingModel.TxtSensorGrougAddress;
            if (this.SettingModel.TxtTotalSpan > 0) c.TimeTotal = this.SettingModel.TxtTotalSpan;
            if (this.SettingModel.TxtGradingTimePoint > 0) c.GradingTimePoint = this.SettingModel.TxtGradingTimePoint;
            if (!string.IsNullOrEmpty(_model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtTestTimePoints.Text)) c.TestTimePoints = this.SettingModel.TxtTestTimePoints;
            SemiTesterConfgig.Instance.ProductList = c.Code;
            SemiTesterConfgig.Instance.ModelList = m.Name;
            SemiTesterConfgig.Save();
        }

        private static Object ExitFrame(Object state)
        {
            ((DispatcherFrame)state).Continue = false;
            return null;
        }
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrame), frame);
            Dispatcher.PushFrame(frame);
        }


        public async void BtnStart_Click(object sender, RoutedEventArgs e)
        {

            if (!BtnStart.IsEnabled) return;

            //强制设定焦点到按钮上，避免 空格等按键不生效
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.SliderProgress.Focus();

            try
            {
                this.SettingModel.BtnStartIng = true;

                Stopwatch watcher = new Stopwatch();
                watcher.Start();

                SaveConfig();
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.SetAxisLimits(0, (SettingModel.TxtTotalSpan) * 1.3,0, Service.SemiTester.SemiTesterService.VersionVoltage);

                ULogger.Info("Started SetAxisLimits 1:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                var testAddr = SettingModel.Address();
                ULogger.Info("Started SettingModel.Address:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                ProductConfig c = _model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtProductList.SelectedItem as ProductConfig;

                ULogger.Info("Started SettingModel.Started:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                //如果重新开始，那么先停止之前的
                if (this.SettingModel.Started)
                {
                    if (_model == null) return;
                    Batch _Batch = BatchCurrent;
                    if (_Batch != null)
                    {
                        this.BtnStart.Content = "正在停止(S)";
                        ULogger.Info("Started StopTask 1:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                        UIZhuiSuC10Model.Instance.ECService.StopTask();
                        ULogger.Info("Started StopTask 2:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                        UIZhuiSuData.Instance.FinishBatch(_Batch, F_AddTime.TotalSeconds);
                        ULogger.Info("Started FinishBatch:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                        UIZhuiSuC10Model.Instance.RefreshAddress(_Batch);
                        SettingModel.Started = false;  
                        ULogger.Info("Started RefreshAddress:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                        RefreshButtons(); 
                        ULogger.Info("Started RefreshButtons:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                    }
                    else
                    {
                        MessageBox.Show("上次项目 null。");
                        return;
                    }
                    ULogger.Info("Started false:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                }
                else
                {


                    //新开始 
                    if (_model == null) return;

                    if (!SemiTesterService.Debug && _model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtSerialPorts.SelectedItem == null)
                    {
                        MessageBox.Show("请选择通信串口。");
                        return;
                    }

                    if (testAddr == null || testAddr.Count == 0)
                    {
                        MessageBox.Show("输入有效的通道 " + SettingModel.TxtSensorGrougAddress + " 无效。");
                        return;
                    }

                    //测试通道是否正常
                    try
                    {
                        if (!SemiTesterService.Debug) {
                            //注释掉，避免启动耗时
                            //var testSucess = _model.ECService.TestConn(SettingModel.TxtSerialPorts, testAddr);
                            //if (!testSucess)
                            //{
                            //    MessageBox.Show("通道 " + SettingModel.TxtSensorGrougAddress + " 测试无效。");
                            //    return;
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("通道 " + SettingModel.TxtSensorGrougAddress + " 测试无效：" + ex.Message);
                        return;
                    }


                    if (c == null)
                    {
                        MessageBox.Show("请选择产品型号。");
                        return;
                    }


                    ProductModelConfig m = _model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtModelList.SelectedItem as ProductModelConfig;
                    if (m == null)
                    {
                        MessageBox.Show("请选择规格名称。");
                        return;
                    }


                    if (SettingModel.TxtTotalSpan <= 10)
                    {
                        MessageBox.Show("测试总时长最短10秒。");
                        return;
                    }

                    ULogger.Info("Started Prepare:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                    RefreshButtons();
                    ULogger.Info("Started RefreshButtons 1:" + watcher.Elapsed.TotalSeconds.ToString("f3")); 

                }

                ULogger.Info("RefreshTaskDoing 1:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                ////等待上次的任务完成
                while (_model.ECService.RefreshTaskDoing)
                {
                    Thread.Sleep(100);
                    DoEvents();
                }
                ULogger.Info("RefreshButtons 2:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                RefreshButtons();

                ULogger.Info("RefreshTaskDoing 2:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                UIZhuiSuAddModel w = new UIZhuiSuAddModel()
                {
                    TxtProductCode = c.Code.ToString(),
                    TxtProductName = c.Name,
                    TxtTimeTotal = SettingModel.TxtTotalSpan
                };
                w.AddBatch(watcher);
                ULogger.Info("AddBatch 2:" + watcher.Elapsed.TotalSeconds.ToString("f3")); 
                //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.SetSpan(0, 0, SettingModel.TxtTotalSpan);
                List<Sensor> sens = w.SelectSensors;
                _model.ECService.StartTask(w.SelectBatch, w.SelectSensors, c, SettingModel.TxtSerialPorts, testAddr);
                BatchCurrent = w.SelectBatch;
                SettingModel.Started = true;
                ULogger.Info("AddBatch StartTask:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                this._model.View.UIZhuiSuC10SensorsPage.SetFontColor();
                ULogger.Info("AddBatch SetFontColor:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                UIZhuiSuData.Instance.RefreshData(  watcher);
                ULogger.Info("RefreshData:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                IniData(BatchCurrent, sens, watcher);
                ULogger.Info("IniData:" + watcher.Elapsed.TotalSeconds.ToString("f3")); 
                watcher.Stop();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message );
            }
            finally {
                this.SettingModel.BtnStartIng = false;
                RefreshButtons();
            }
           
        }


        public void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            if (!BtnPause.IsEnabled) return;
            //强制设定焦点到按钮上，避免 空格等按键不生效
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.SliderProgress.Focus();
            SaveConfig();
            if (UIZhuiSuC10Model.Instance.ECService.Pause)
            {
                UIZhuiSuC10Model.Instance.ECService.ResumeTask();
                this.BtnPause.Content = "暂停(P)";
            }
            else
            {
                UIZhuiSuC10Model.Instance.ECService.PauseTask();
                this.BtnPause.Content = "继续(P)";
            }
            this.BtnPause.Background = UIZhuiSuC10Model.Instance.ECService.Pause
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#12559d"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3275fd"));

        }

        public void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            if (!MainWindow.Instance.BtnFinish.IsEnabled) return;
            if (_model == null) return;
            //强制设定焦点到按钮上，避免 空格等按键不生效
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.SliderProgress.Focus();
            SaveConfig();
            Batch _Batch = BatchCurrent;
            if (_Batch != null)
            {

                //if (MessageBox.Show(
                //    string.Format("确定要结束{0}吗？", _Batch.F_BatchName),
                //    "确定要结束", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    UIZhuiSuC10Model.Instance.ECService.StopTask();
                    UIZhuiSuData.Instance.FinishBatch(_Batch, F_AddTime.TotalSeconds);
                    UIZhuiSuC10Model.Instance.RefreshAddress(_Batch);
                    SettingModel.Started = false;
                    IniData(BatchCurrent, SensorsCurrent,new Stopwatch());
                }

            }

        }
        public void BtnExcelExport_Click(object sender, RoutedEventArgs e)
        {
            SaveConfig();
            if (_model == null) return;
            Batch _Batch = BatchCurrent;
            if (_Batch != null)
            {
                string name = _Batch.F_BatchName.Replace("#", "-")+"_"+SensorGroupData.ResistanceString(SensorGroupData.Resistance);
                var dlg = new SaveFileDialog()
                {
                    Title = _Batch.F_BatchName + "-另存为",
                    DefaultExt = "txt",
                    Filter = "Text files (*.xlsx)|*.xlsx|All files|*.*",
                    FileName = name
                };
                if (dlg.ShowDialog() == true)
                {
                    try
                    {
                        Cursor = Cursors.Wait;
                        _model.View.ProgressNow.Visibility = System.Windows.Visibility.Visible;
                        _model.ExportExcel(dlg.FileName, _Batch);
                        Cursor = Cursors.Arrow;
                        _model.View.ProgressNow.Visibility = System.Windows.Visibility.Hidden;
                        MessageBox.Show("导出成功。");
                    }
                    catch (System.Exception ex)
                    {
                        _model.View.ProgressNow.Visibility = System.Windows.Visibility.Hidden;
                        MessageBox.Show(ex.Message, "导出出错");
                    }
                }
            }
        }

        private void ButtonColorSelect_Click(object sender, RoutedEventArgs e)
        {
            if (sender == null) return;
            Label _Button = sender as Label;
            if (_Button == null) return;
            ColorBoxTest.ColorSelect _ColorSelect = new ColorBoxTest.ColorSelect((_Button.Background));

            //修复之前的 Bug，之前的  bug 会造成颜色设置有问题 2021/11/29
            ProductGradeExtEdit tag = _Button.Tag as ProductGradeExtEdit;
            SensorItemEnum se = tag.ItemEnum;

            var bv = (_ColorSelect.ShowDialog());
            if (bv != null && bv.Value)
            {

                _Button.Background = _ColorSelect.SelectColor;
                ProductGrade _ProductGrade = (_Button.Tag as ProductGradeExtEdit).Current;
                Color c1 = (_ColorSelect.SelectColor as SolidColorBrush).Color;
                if (se == SensorItemEnum.Final) _ProductGrade.FinalColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(c1.A, c1.R, c1.G, c1.B));
                if (se == SensorItemEnum.Delta) _ProductGrade.DeltaColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(c1.A, c1.R, c1.G, c1.B));
                if (se == SensorItemEnum.Start) _ProductGrade.StartColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(c1.A, c1.R, c1.G, c1.B));
                this._model.View.UIZhuiSuC10SensorsPage.Refresh();

                ProductModelConfig c = _model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtModelList.SelectedItem as ProductModelConfig;
                if (c != null)
                {
                    foreach (var item in c.Grades)
                    {
                        if (item.Grade == _ProductGrade.Grade)
                        {
                            if (se == SensorItemEnum.Final) item.FinalColor = _ProductGrade.FinalColor;
                            if (se == SensorItemEnum.Delta) item.DeltaColor = _ProductGrade.DeltaColor;
                            if (se == SensorItemEnum.Start) item.StartColor = _ProductGrade.StartColor;
                        }
                    }
                }

            }
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM();
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
        }
         

        public void TxtProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ProductConfig c = _model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtProductList.SelectedItem as ProductConfig;
            if (c == null) return;
            SettingModel.ProductConfig = c;
           
            this.SettingModel.TxtTotalSpan = c.TimeTotal;
            this.SettingModel.TxtGradingTimePoint = c.GradingTimePoint;
            this.SettingModel.TxtTestTimePoints = c.TestTimePoints;

        }

        public void TxtModelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            ProductModelConfig m = _model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtModelList.SelectedItem as ProductModelConfig;
            if (m == null) return;
            SettingModel.ProductModelConfig = m;
            RefreshMM();

            //            public string FinalColor { get; set; }
            //public string DeltaColor { get; set; }
            //public string StartColor { get; set; }

            ProductConfig c = _model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtProductList.SelectedItem as ProductConfig;
            if (c == null) return;
            SettingModel.ProductConfig = c;

            if (UIZhuiSuC10Model.Instance.View != null) UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.SetAxisLimits(0, c.TimeTotal * 1.2,0, Service.SemiTester.SemiTesterService.VersionVoltage);
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM();
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
        }


        private void SliderProgress_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (this._model.SettingModel == null) return;
            if (this._model.View == null) return;
            double last = UIZhuiSuC10Model.Instance.SettingModel.TxtCurrentSpanLatest;
            if ( SliderProgress.Value != last)
            {
                //this._model.View.UIZhuiSuC10Chart.SetSpan((int)SliderProgress.Value, 0, (int)SliderProgress.Maximum);
                MKSS.Model.SensorGroupData new_data = this._model.ECService[TimeSpan.FromSeconds(SliderProgress.Value)];
                if (new_data != null)
                {
                    this._model.View.UIZhuiSuC10SensorsPage.SetData(BatchCurrent, new_data, TimeSpan.FromSeconds(SliderProgress.Value));
                }
            }
        }

        public void SetCurrentTime(double val) {

            SliderProgress.Value = val;
            MKSS.Model.SensorGroupData new_data = this._model.ECService[TimeSpan.FromSeconds(SliderProgress.Value)];
            if (new_data != null)
            {
                this._model.View.UIZhuiSuC10SensorsPage.SetData(BatchCurrent, new_data, TimeSpan.FromSeconds(SliderProgress.Value));
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            //if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            //TextBox txt  = sender as TextBox;
            //if (txt != null)
            //{
            //    Panel.SetZIndex(txt, 1);
            //    txt.Background = new SolidColorBrush(Colors.Gray);
            //    txt.Margin = new Thickness(-36, -46, -20, -10);
            //    txt.FontSize = 32;
            //    txt.Width = 108;
            //    txt.Height = 46;
            //}
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox _TextBox = sender as TextBox;
            ProductGradeExtEdit _edit = _TextBox.Tag as ProductGradeExtEdit;
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (_edit.GradeEnd == _TextBox.Text) return;
            _edit.GradeEnd = _TextBox.Text;
            if(_edit.Next!=null) _edit.Next.GradeStart = _TextBox.Text;

            if (this._model == null || this._model.View == null || this._model.View.UIZhuiSuC10SensorsPage == null) return;
            Stopwatch watcher = new Stopwatch();
            watcher.Start();

            RefreshMM();
            ULogger.Info("RefreshMM结束["+ _edit.Current.Grade + "]:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM(); 
            ULogger.Info("Page.RefreshMM结束[" + _edit.Current.Grade + "] :" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            this._model.View.UIZhuiSuC10SensorsPage.Refresh();
            ULogger.Info("Page.Refresh[" + _edit.Current.Grade + "] :" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            watcher.Stop();

        }

        public void RefreshMM()
        {
            List<ProductGradeExtEdit> gList = UIZhuiSuC10Model.Instance.SettingModel.GradeList;
            UIZhuiSuC10MM.ItemsSource = gList; 
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox txx = sender as TextBox;
            string str = txx.Text;
            str = str.Replace("。", ".");
            if (txx.Text + "" != str)
            {
                txx.Text = str;
            }
        }
    }

    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString().EndsWith(".") ? "." : value;
        }
    }
    public class NullableDecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        { 
            string strValue = value as string; 
            if (string.IsNullOrEmpty(strValue)) { return null; } 
            decimal result; 
            if (strValue.IndexOf('.') == strValue.Length - 1 || strValue.IndexOf('0') == strValue.Length - 1 
                || !decimal.TryParse(strValue, out result)) { return DependencyProperty.UnsetValue; } 
            return result; 
        }
    }
}