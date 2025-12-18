using MKSS.APP.ECTester;
using Microsoft.Win32;
using MKSS.Model;
using MKSS.Service.ECTester;
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

namespace MKSS.APP.ECTester
{
    /// <summary>
    /// UIZhuiSuC10Setting.xaml 的交互逻辑
    /// </summary>
    [LogTagClass(Title = "控制界面")]
    public partial class UIZhuiSuC10Setting : UserControl
    {
        UIZhuiSuC10Model _model
        {
            get
            {
                return UIZhuiSuC10Model.Instance;
            }
        }
        public UIZhuiSuC10SettingModel SettingModel;
        public UIZhuiSuC10Setting()
        {
            InitializeComponent();
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            SettingModel = this.DataContext as UIZhuiSuC10SettingModel;
            _model.SettingModel = SettingModel;
            IniData(null, null,new Stopwatch());
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            TxtProductList.SelectionChanged += TxtProductList_SelectionChanged;
            TxtModelList.SelectionChanged += TxtModelList_SelectionChanged;


            this.SettingModel.TxtSerialPorts = ECTesterConfgig.Instance.SerialPorts;
            this.SettingModel.TxtSensorGrougAddress = ECTesterConfgig.Instance.SensorGrougAddress;
            this.SettingModel.TxtProductList = ECTesterConfgig.Instance.ProductList;
            this.SettingModel.TxtModelList = ECTesterConfgig.Instance.ModelList;

            this.TxtSerialPorts.Items.Clear();
            foreach (var item in SerialPort.GetPortNames())
            {
                this.TxtSerialPorts.Items.Add(item);
                if (SettingModel != null && item == SettingModel.TxtSerialPorts)
                {
                    this.TxtSerialPorts.SelectedItem = item;
                }
            }
            if (this.TxtSerialPorts.SelectedItem == null && this.TxtSerialPorts.Items.Count > 0)
            {
                this.TxtSerialPorts.SelectedItem = this.TxtSerialPorts.Items[0];
            }

            ProductConfig selP = null;
            this.TxtProductList.Items.Clear();
            foreach (ProductConfig item in ECTesterConfgig.Instance.Product)
            {
                this.TxtProductList.Items.Add(item);
                if (item.Name == SettingModel.TxtProductList)
                {
                    this.TxtProductList.SelectedItem = item;
                    selP = item;
                }
            }
            if (selP == null && ECTesterConfgig.Instance.Product.Count > 0)
            {
                this.TxtProductList.SelectedItem = ECTesterConfgig.Instance.Product[0];
                selP = ECTesterConfgig.Instance.Product[0];
            }
            if (selP != null)
            {
                this.SettingModel.TxtProductList = selP.Name;
                this.SettingModel.TxtTotalSpan = selP.TimeTotal;
                this.SettingModel.TxtGradingTimePoint = selP.GradingTimePoint;
                this.SettingModel.TxtTestTimePoints = selP.TestTimePoints;
                this.SettingModel.TxtContainerPPM = selP.ContainerPPM;
                ECTesterConfgig.ContainerPPM = selP.ContainerPPM;
                ECTesterConfgig.AutoPrintPPMSensibility = selP.AutoPrintPPMSensibility;

                ProductModelConfig selM = null;
                this.TxtModelList.Items.Clear();
                foreach (ProductModelConfig item in selP.Models)
                {
                    this.TxtModelList.Items.Add(item);
                    if (item.Name == SettingModel.TxtModelList)
                    {
                        this.TxtModelList.SelectedItem = item;
                        selM = item;
                    }
                }
                if (selM == null && selP.Models.Count > 0)
                {
                    this.TxtModelList.SelectedItem = selP.Models[0];
                    this.SettingModel.TxtModelList = selP.Models[0].Name;
                }

            }

            TxtProductList_SelectionChanged(null, null);
            TxtModelList_SelectionChanged(null, null); 


        }


        private void TxtV1_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (string.IsNullOrEmpty(t.Text)) {
                return;
            }
            //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowStdLine(t.Name, double.Parse(t.Tag+""));
        }
        public void ClearTimeTexts()
        {
            if (UIZhuiSuC10Model.Instance.View == null) return;
            this.Dispatcher.Invoke(new Action(() =>
            {
                //var text = TxtV1;
                //text.Text = "";
                //text.Tag = "";
                //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowStdLine(text.Name, -1);
            }));
        }

        public void RefreshTimeTexts()
        {
            if (UIZhuiSuC10Model.Instance.View == null) return;
            this.Dispatcher.Invoke(new Action(() =>
            {
                //var text = TxtV1;
                //var x1 = SensorStaDataCache.Instance.Values.Where(w => w.VStart != null).ToList();
                //if (x1.Count > 0)
                //{
                //    text.Text = x1.Average(w => w.VStart.F_AddTime).ToString("f1");
                //    text.Tag = text.Text;
                //    UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowStdLine(text.Name, double.Parse(text.Tag + ""));
                //}

                //text = TxtV2;
                //x1 = SensorStaDataCache.Instance.Values.Where(w => w.VMaxStart != null).ToList();
                //if (x1.Count > 0)
                //{
                //    text.Text = x1.Average(w => w.VMaxStart.F_AddTime).ToString("f1");
                //    text.Tag = text.Text;
                //    UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowStdLine(text.Name, double.Parse(text.Tag + ""));
                //}


                //text = TxtV3;
                //x1 = SensorStaDataCache.Instance.Values.Where(w => w.VEnd != null).ToList();
                //if (x1.Count > 0)
                //{
                //    text.Text = x1.Average(w => w.VEnd.F_AddTime).ToString("f1");
                //    text.Tag = text.Text;
                //    UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowStdLine(text.Name, double.Parse(text.Tag + ""));
                //}


                //text = TxtT90;
                //x1 = SensorStaDataCache.Instance.Values.Where(w => w.V90 != null && w.VStart != null).ToList();
                //if (x1.Count > 0)
                //{
                //    text.Text = x1.Average(w => w.V90.F_AddTime - w.VStart.F_AddTime).ToString("f1");
                //    text.Tag = x1.Average(w => w.V90.F_AddTime).ToString("f1");
                //    UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.ShowStdLine(text.Name, double.Parse(text.Tag + ""));
                //}
                 
                 
            }));
        }

        public void RefreshButtons()
        {
            if (UIZhuiSuC10Model.Instance.View == null) return;
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.BtnPause.IsEnabled = this.SettingModel.CanBtnPause;
                this.BtnStart.IsEnabled = this.SettingModel.CanBtnStart;
                this.BtnFinish.IsEnabled = this.SettingModel.CanBtnFinish;
                
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

                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsSET.BtnExcelExport.IsEnabled = this.SettingModel.CanBtnExcelExport;
                this.BtnPause.Content = UIZhuiSuC10Model.Instance.ECService.Pause ? "继续(P)" : "暂停(P)";
                this.BtnPause.Background = UIZhuiSuC10Model.Instance.ECService.Pause
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#12559d"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3275fd"));

                this.BtnPauseColor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ECTesterConfgig.Instance.PauseColor));
                this.BtnPauseExceptColor.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(ECTesterConfgig.Instance.PauseExcepColor));
                this.BtnPauseFrom.Text = ECTesterConfgig.Instance.PauseValueExtend[0].ToString();
                this.BtnPauseTo.Text = ECTesterConfgig.Instance.PauseValueExtend[1].ToString();
                this.BtnPauseEnabled.IsChecked = ECTesterConfgig.Instance.PauseEnabled;

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
            _model.View.UIZhuiSuC10SensorsSET.IniTask(batch, sens);
            ULogger.Info("IniData UIZhuiSuC10SensorsSET UIZhuiSuC10Grid SetCaption:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Grid.SetCaption(this.SettingModel.TxtTestTimePointsArr);

            ULogger.Info("IniData SetBatch Start:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            _model.View.UIZhuiSuC10Chart.SetBatch(batch, sens.Select(w => w.PosEnum).ToList(), watcher);

            RefreshProgressText.Content = "00:00:00";
        }
         

        Batch Batch { get; set; }
        public TimeSpan F_AddTime { get; set; }
        public void SetData(Model.Batch _Batch, SensorGroupData datas, TimeSpan _F_AddTime)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            Batch = _Batch;
            F_AddTime = _F_AddTime;
            RefreshProgress.Value = _F_AddTime.TotalSeconds;
            RefreshProgressText.Content = string.Format("{0}:{1}:{2}", _F_AddTime.Hours.ToString("00"), _F_AddTime.Minutes.ToString("00"), _F_AddTime.Seconds.ToString("00"));
        }
        public void IniTask(Model.Batch _Batch, List<Sensor> datas)
        {
            Batch = _Batch;

        }


        private void TxtTotalSpan_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (this._model.SettingModel == null) return;
            ProductConfig c = this.TxtProductList.SelectedItem as ProductConfig;
            if (c == null || string.IsNullOrEmpty(TxtTotalSpan.Text))
            {
                return;
            }
            try
            {
                if (int.Parse(TxtTotalSpan.Text) > 10)
                {
                    c.TimeTotal = int.Parse(TxtTotalSpan.Text); 
                    c.TestTimePoints = ProductConfig.AutoTestTimePoints(c.TimeTotal, c.ZeroPoint, c.SpanPoint);
                    this.TxtTestTimePoints.Text = c.TestTimePoints;
                    if (UIZhuiSuC10Model.Instance.View != null) UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.SetAxisLimits(0, c.TimeTotal * 1.3);
                }

            }
            catch (Exception)
            {

            }
        }

        private void TxtSerialPorts_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            foreach (var item in SerialPort.GetPortNames())
            {
                if (!this.TxtSerialPorts.Items.Contains(item))
                {
                    this.TxtSerialPorts.Items.Add(item);
                }
            }
            foreach (var item in this.TxtSerialPorts.Items)
            {
                if (!SerialPort.GetPortNames().Contains(item))
                {
                    this.TxtSerialPorts.Items.Remove(item);
                    break;
                }
            }
        }

        void SaveConfig()
        {

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (this.SettingModel == null) return;
            ProductConfig c =  TxtProductList.SelectedItem as ProductConfig;
            if (c == null) return;
            ProductModelConfig m = TxtModelList.SelectedItem as ProductModelConfig;
            if (m == null) return;
            ECTesterConfgig.Instance.SerialPorts = this.SettingModel.TxtSerialPorts;
            ECTesterConfgig.Instance.SensorGrougAddress = this.SettingModel.TxtSensorGrougAddress;
            if (this.SettingModel.TxtTotalSpan > 0) c.TimeTotal = this.SettingModel.TxtTotalSpan;
            if (this.SettingModel.TxtGradingTimePoint > 0) c.GradingTimePoint = this.SettingModel.TxtGradingTimePoint;
            if (!string.IsNullOrEmpty(TxtTestTimePoints.Text)) c.TestTimePoints = this.SettingModel.TxtTestTimePoints;
            c.ContainerPPM = this.SettingModel.TxtContainerPPM;
            ECTesterConfgig.ContainerPPM = this.SettingModel.TxtContainerPPM;
            ECTesterConfgig.AutoPrintPPMSensibility = c.AutoPrintPPMSensibility;

            ECTesterConfgig.Save();
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
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsSET.RefreshProgress.Focus();

            try
            {
                this.SettingModel.BtnStartIng = true;

                Stopwatch watcher = new Stopwatch();
                watcher.Start();

                SaveConfig();
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.SetAxisLimits(0, (SettingModel.TxtTotalSpan) * 1.3 );

                ULogger.Info("Started SetAxisLimits 1:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                var testAddr = SettingModel.Address();
                if (testAddr == null || testAddr.Count != 4)
                {
                    MessageBox.Show("输入有效的通道 " + SettingModel.TxtSensorGrougAddress + " 无效。");
                    return;
                }

                ULogger.Info("Started SettingModel.Address:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                ProductConfig c = _model.View.UIZhuiSuC10SensorsSET.TxtProductList.SelectedItem as ProductConfig;

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

                    if (!ECTesterService.Debug && _model.View.UIZhuiSuC10SensorsSET.TxtSerialPorts.SelectedItem == null)
                    {
                        MessageBox.Show("请选择通信串口。");
                        return;
                    }


                    //测试通道是否正常
                    try
                    {
                        if (!ECTesterService.Debug) {
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


                    ProductModelConfig m = _model.View.UIZhuiSuC10SensorsSET.TxtModelList.SelectedItem as ProductModelConfig;
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
                SensorStaDataCache.Instance.CalculateStart();
                BatchCurrent = w.SelectBatch;
                SensorGroupDataStander.ClearSpan();
                ClearTimeTexts();
                RefreshProgress.Value = 0;
                RefreshProgress.Maximum = SettingModel.TxtTotalSpan;
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
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsSET.RefreshProgress.Focus();
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

            //刷新暂停色
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM();
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();

        }

        public void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            if (! BtnFinish.IsEnabled) return;
            if (_model == null) return;
            //强制设定焦点到按钮上，避免 空格等按键不生效
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsSET.RefreshProgress.Focus();
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
                string name = _Batch.F_BatchName.Replace("#", "-")+"_"+SensorGroupData.ShowModeString(SensorGroupData.ShowMode);
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
            
            ProductGradeExtEdit tag = _Button.Tag as ProductGradeExtEdit;
            SensorItemEnum se = tag.ItemEnum;

            var bv = (_ColorSelect.ShowDialog());
            if (bv != null && bv.Value)
            {

                _Button.Background = _ColorSelect.SelectColor;
                ProductGrade _ProductGrade = (_Button.Tag as ProductGradeExtEdit).Current;
                Color c1 = (_ColorSelect.SelectColor as SolidColorBrush).Color;
                if (se == SensorItemEnum.SrcData) _ProductGrade.ColorSet(System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(c1.A, c1.R, c1.G, c1.B)));
                this._model.View.UIZhuiSuC10SensorsPage.Refresh();

                ProductModelConfig c = _model.View.UIZhuiSuC10SensorsSET.TxtModelList.SelectedItem as ProductModelConfig;
                if (c != null)
                {
                    foreach (var item in c.Grades)
                    {
                        if (item.Grade == _ProductGrade.Grade)
                        {
                            if (se == SensorItemEnum.SrcData) item.ColorSet(_ProductGrade.Color());
                        }
                    }
                }

            }
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM();
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
        }
         

        public void TxtProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            ProductConfig c = _model.View.UIZhuiSuC10SensorsSET.TxtProductList.SelectedItem as ProductConfig;
            if (c == null) return;
            SettingModel.ProductConfig = c;

            this.SettingModel.TxtTotalSpan = c.TimeTotal;
            this.SettingModel.TxtGradingTimePoint = c.GradingTimePoint;
            this.SettingModel.TxtTestTimePoints = c.TestTimePoints;
             

            ProductModelConfig selM = null;
            this.TxtModelList.Items.Clear();
            foreach (ProductModelConfig item in c.Models)
            {
                this.TxtModelList.Items.Add(item);
                if (item.Name == SettingModel.TxtModelList)
                {
                    this.TxtModelList.SelectedItem = item;
                    selM = item;
                }
            }
            if (selM == null && c.Models.Count > 0)
            {
                this.TxtModelList.SelectedItem = c.Models[0];
            }

        }

        public void TxtModelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            ProductModelConfig m = TxtModelList.SelectedItem as ProductModelConfig;
            if (m == null) return;
            SettingModel.ProductModelConfig = m;
            RefreshMM(); 

            ProductConfig c = TxtProductList.SelectedItem as ProductConfig;
            if (c == null) return;
            SettingModel.ProductConfig = c;

            if (UIZhuiSuC10Model.Instance.View != null) UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.SetAxisLimits(0, c.TimeTotal * 1.3);
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM();
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();

        }

        private void TxtTestTimePoints_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (this._model.SettingModel == null) return;
            ProductConfig c = this.TxtProductList.SelectedItem as ProductConfig;
            if (c == null || string.IsNullOrEmpty(TxtTotalSpan.Text))
            {
                return;
            }
            if (c == null || string.IsNullOrEmpty(TxtTestTimePoints.Text)) return;
            c.TestTimePoints = TxtTestTimePoints.Text;
        }
          

        public void SetCurrentTime(double val) {

            RefreshProgress.Value = val;
            MKSS.Model.SensorGroupData new_data = this._model.ECService[TimeSpan.FromSeconds(RefreshProgress.Value)];
            if (new_data != null)
            {
                this._model.View.UIZhuiSuC10SensorsPage.SetData(BatchCurrent, new_data, TimeSpan.FromSeconds(RefreshProgress.Value));
            }
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
           
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
            SrcTitle.Text = SensorGroupData.ShowModeDescXX;
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProductConfig c = TxtProductList.SelectedItem as ProductConfig;
            if (c == null)
            {
                MessageBox.Show("请选择产品型号。");
                return;
            }

            UIAddModel ad = new UIAddModel();
            ad.Owner = MainWindow.Instance;
            ad.Show();
        }

        private void BtnSetSpan_Click(object sender, RoutedEventArgs e)
        {
            if (BatchCurrent == null) return;
            UIZhuiSuC10Model.Instance.SettingModel.TxtGradingTimePoint = (int)double.Parse(TxtGradingTimePoint.Text);
            BatchCurrent.F_SpanTime = UIZhuiSuC10Model.Instance.SettingModel.TxtGradingTimePoint;
            BatchCurrent.F_SpanValue = UIZhuiSuC10Model.Instance.SettingModel.TxtCurrentSpanValue;
            SensorGroupDataStander.YuLiuStd = BatchCurrent.F_SpanValue;
            bool b = UIZhuiSuData.Instance.BatchServices.Update(BatchCurrent).Result;
            TrySpanTime(BatchCurrent);
            this._model.View.UIZhuiSuC10SensorsPage.Refresh();
        }

        private void BtnShowHistory_Click(object sender, RoutedEventArgs e)
        { 

            var page = UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsHistory;
            page.Mode = SensorItemEnum.ColorDiagram;
            page.Visibility = page.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
            this.BtnShowHistoryTxt.Text = page.Visibility == Visibility.Visible ? "隐藏选择页面(B)" : "进入选择页面(B)";
            page.CalcTitle();

            if (page.Visibility == Visibility.Visible) {
                Batch batchPre = UIZhuiSuC10Model.Instance.PreBatch;
                if (batchPre != null)
                {
                    SensorGroupData data = UIZhuiSuC10Model.Instance.PreDataCache.FirstOrDefault(w => w.F_AddTime >= UIZhuiSuC10Model.Instance.SettingModel.TxtGradingTimePoint);
                    if (data != null)
                    {
                        page.SetData(batchPre, data, UIZhuiSuC10Model.Instance.PreFullQueryTimesText,true);
                        page.Refresh();
                    }
                }
                else {
                    page.SetData(batchPre, null, UIZhuiSuC10Model.Instance.PreFullQueryTimesText, true);
                    page.Refresh();
                }
            }
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsNongDu.Refresh();

        }

        static double TestSpanTimeLast = 0;
        /// <summary>
        /// 设置Span参考值
        /// </summary>
        /// <param name="BatchCurrent">批次</param>
        /// <param name="current_add_time">当前时间</param>
        public static void TrySpanTime(Batch BatchCurrent)
        {
            if (BatchCurrent == null) return;
            double span = BatchCurrent.F_SpanTime;//从数据库中读出Span点时间 
            if (span > 0 && span != TestSpanTimeLast)
            {
                List<SensorGroupData> gds = ECTesterService.QueryData(BatchCurrent.F_BatchId, span);
                SensorGroupDataStander.SetSpan(gds.FirstOrDefault(), span);
                TestSpanTimeLast = span;
            }
        }

        private void Txt_InputNumber(object sender, KeyEventArgs e)
        {
            InputNumber(e);
        }

        public static void InputNumber(KeyEventArgs e)
        {
            if (e.Key == Key.OemPeriod || e.Key == Key.OemComma || e.Key == Key.Decimal)
            {
                e.Handled = false;
                return;
            }

            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back)
            {
                e.Handled = false;
                return;
            }
            else if ((e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key == Key.Back))
            {
                e.Handled = false;
                return;
            }
            else
            {
                e.Handled = true;
                //System.Windows.MessageBox.Show("请输入数字");
                return;
            }
        }

        private void BtnPauseFrom_LostFocus(object sender, RoutedEventArgs e)
        {
            double d = 0;
            if (double.TryParse(BtnPauseFrom.Text, out d)) {
                ECTesterConfgig.Instance.PauseValueExtend[0] = d;
                ECTesterConfgig.Save();
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM();
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
            }
        }

        private void BtnPauseTo_LostFocus(object sender, RoutedEventArgs e)
        {
            double d = 0;
            if (double.TryParse(BtnPauseTo.Text, out d))
            {
                ECTesterConfgig.Instance.PauseValueExtend[1] = d;
                ECTesterConfgig.Save();
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM();
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
            }
        }

        private void BtnPauseColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == null) return;
            Label _Button = sender as Label;
            if (_Button == null) return;
            ColorBoxTest.ColorSelect _ColorSelect = new ColorBoxTest.ColorSelect((_Button.Background));

            var bv = (_ColorSelect.ShowDialog());
            if (bv != null && bv.Value)
            {

                _Button.Background = _ColorSelect.SelectColor;
                Color c1 = (_ColorSelect.SelectColor as SolidColorBrush).Color;
                ECTesterConfgig.Instance.PauseColor = (System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(c1.A, c1.R, c1.G, c1.B)));
                ECTesterConfgig.Save();
                this._model.View.UIZhuiSuC10SensorsPage.Refresh();
            }
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM();
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
        }

        private void BtnPauseExceptColor_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender == null) return;
            Label _Button = sender as Label;
            if (_Button == null) return;
            ColorBoxTest.ColorSelect _ColorSelect = new ColorBoxTest.ColorSelect((_Button.Background));

            var bv = (_ColorSelect.ShowDialog());
            if (bv != null && bv.Value)
            {

                _Button.Background = _ColorSelect.SelectColor;
                Color c1 = (_ColorSelect.SelectColor as SolidColorBrush).Color;
                ECTesterConfgig.Instance.PauseExcepColor = (System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(c1.A, c1.R, c1.G, c1.B)));
                ECTesterConfgig.Save();
                this._model.View.UIZhuiSuC10SensorsPage.Refresh();
            }
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM();
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
        }

        private void BtnPauseEnabled_Click(object sender, RoutedEventArgs e)
        {
            ECTesterConfgig.Instance.PauseEnabled = BtnPauseEnabled.IsChecked!=null && BtnPauseEnabled.IsChecked.Value;
            ECTesterConfgig.Save();
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM();
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
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