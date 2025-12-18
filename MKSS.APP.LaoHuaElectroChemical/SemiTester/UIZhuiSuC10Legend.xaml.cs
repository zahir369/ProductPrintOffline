using DeviceDataMonitorWPF;
using Microsoft.Win32;
using MKSS.APP.LaoHuaService.MQTT;
using MKSS.Model;
using MKSS.Service;
using MKSS.Service.LaoHuaElectroChemical;
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
            TxtSerialPorts.Visibility =
                (LaoHuaDataProvider.UIMode == LaoHuaDataProviderUIMode.SerialPort) 
                ? Visibility.Visible : Visibility.Hidden;
            this.TxtSerialPorts.Items.Clear();
            foreach (var item in SerialPort.GetPortNames())
            {
                this.TxtSerialPorts.Items.Add(item);
                if (item == ElectroChemicalConfgig.Instance.SerialPorts)
                {
                    this.TxtSerialPorts.SelectedItem = item;
                }
            }
            if (this.TxtSerialPorts.SelectedItem == null && this.TxtSerialPorts.Items.Count > 0)
            {
                this.TxtSerialPorts.SelectedItem = this.TxtSerialPorts.Items[0];
            }

            IniData(null,  new Stopwatch());
        }


        private void TxtSerialPorts_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
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

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            SettingModel.ProductConfig = ElectroChemicalConfgig.Instance.Product[0];
            SettingModel.ProductModelConfig = ElectroChemicalConfgig.Instance.Product[0].Models[0];

            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM();
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
            RefreshMM();
            RadioButtonGroupChoiceChipAccent_SelectionChanged(null, null);
        }

        void SaveConfig()
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (this.SettingModel == null) return;
            ProductConfig c = SettingModel.ProductConfig;
            ProductModelConfig m = SettingModel.ProductModelConfig;
            ElectroChemicalConfgig.Instance.ProductList = c.Code;
            ElectroChemicalConfgig.Instance.ModelList = m.Name;
            ElectroChemicalConfgig.Instance.SerialPorts = TxtSerialPorts.SelectedItem + "";
            ElectroChemicalConfgig.Save();
        }

        public void RefreshButtons()
        {
            if ( BtnFinish == null) return;
            if(UIZhuiSuC10Model.Instance.View==null) return;
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.BtnPause.IsEnabled = this.SettingModel.CanBtnPause;
                this.BtnStart.IsEnabled = this.SettingModel.CanBtnStart;
                 BtnFinish.IsEnabled = this.SettingModel.CanBtnFinish;
                FullQueryTimesText.Content = LaoHuaDataProvider.Instance.Status.FullQueryTimes.ToString("000000");
                if (this.SettingModel.BtnStartIng)
                {
                    this.BtnStart.Content = "正在开始(S)";
                }
                else {
                    this.BtnStart.Content = this.SettingModel.Started!= LaHuaTaskStatus.Stoped ? "开始(S)" : "开始(S)";
                }
                this.BtnStart.IsEnabled = this.SettingModel.Started == LaHuaTaskStatus.Stoped;
                this.BtnStart.Background = this.SettingModel.Started != LaHuaTaskStatus.Stoped
                ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#12559d"))
                : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3275fd"));

                BtnExcelExport.IsEnabled = this.SettingModel.CanBtnExcelExport;
                this.BtnPause.Content = (UIZhuiSuC10Model.Instance.DataProvider.Status.TaskStatus == LaHuaTaskStatus.Paused) ? "继续(P)" : "暂停(P)";
                this.BtnPause.Background = (UIZhuiSuC10Model.Instance.DataProvider.Status.TaskStatus == LaHuaTaskStatus.Paused)
                    ? new SolidColorBrush((Color)ColorConverter.ConvertFromString("#12559d"))
                    : new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3275fd"));
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshCk(UIZhuiSuC10Model.Instance.DataProvider.Status.FloorsVisible);
            }));

        }

        public void RefreshErrorInfo(ErrorEntity status)
        { 
            if (UIZhuiSuC10Model.Instance.View == null) return;
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (status.Floor <= 0) {
                    this.ErrorInfo.Visibility = Visibility.Visible;
                    this.ErrorInfo.ToolTip = status.Message;
                }
                else {
                    var page = UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Of(status.Floor);
                    if (status.Floor > 0)
                    {
                        this.RefreshProgress.Value = status.Floor * 100 / 15;
                        this.RefreshProgressText.Content = status.Floor.ToString("00");
                    }
                    UIZhuiSuC10Model.Instance.SettingModel.CurrentTimeSpan = DateTime.Now - UIZhuiSuC10Model.Instance.BatchCurrent.F_AgingStartTime;
                    page.RefreshErrorInfo(status);
                }
            }));
        }

        public void ClearErrorInfo()
        {
            if (UIZhuiSuC10Model.Instance.View == null) return;
            this.Dispatcher.Invoke(new Action(() =>
            {
                this.ErrorInfo.Visibility = Visibility.Hidden;
            }));
        }

        public void ClearErrorInfo(MessageEntity status)
        {
            if (UIZhuiSuC10Model.Instance.View == null) return;
            this.Dispatcher.Invoke(new Action(() =>
            {
                if (status.Floor <= 0)
                {
                    this.ErrorInfo.Visibility = Visibility.Hidden;
                }
                else
                {
                    var page = UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Of(status.Floor);
                    page.ClearErrorInfo();
                }
            }));
        }

        public Batch BatchCurrent { get { return UIZhuiSuC10Model.Instance.BatchCurrent; }  } 
        public void IniData(Batch batch, Stopwatch watcher)
        {

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回 
            TrySpanTime(BatchCurrent);//设置Span参考值
            ULogger.Info("IniData RefreshButtons Start:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            RefreshButtons();

            if (batch != null)
            {
                this.BtnPause.IsEnabled = batch.EnumAgingStatus == EnumAgingStatus.InAging;
            }
            if (_model.View == null) return;
            _model.View.SelectBatch = batch;

            //ULogger.Info("IniData DataTimerRefresh Start:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            //_model.DataTimerRefresh(batch);
            //ULogger.Info("IniData UIZhuiSuC10Grid IniTask Start:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            //_model.View.UIZhuiSuC10Grid.IniTask(batch, sens);
            //ULogger.Info("IniData UIZhuiSuC10SensorsPage IniTask Start:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            //_model.View.UIZhuiSuC10SensorsPage.IniTask(batch, sens);
            //ULogger.Info("IniData UIZhuiSuC10SensorsSET IniTask Start:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            //_model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.IniTask(batch, sens);
            //ULogger.Info("IniData UIZhuiSuC10SensorsSET UIZhuiSuC10Grid SetCaption:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Grid.SetCaption(this.SettingModel.TxtTestTimePointsArr);

            //ULogger.Info("IniData SetBatch Start:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
            //_model.View.UIZhuiSuC10Chart.SetBatch(batch, sens.Select(w => w.PosEnum).ToList(), watcher);

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
                List<SensorGroupData> gds = ElectroChemicalService.QueryData(BatchCurrent.F_BatchId, span);
                SensorGroupDataStander.SetSpan(gds, span);
                TestSpanTimeLast = span;
            }
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


            try
            {
                this.SettingModel.BtnStartIng = true;

                Stopwatch watcher = new Stopwatch();
                watcher.Start();

                SaveConfig();
                 
                if (_model == null) return;

                if (LaoHuaDataProvider.UIMode == LaoHuaDataProviderUIMode.SerialPort)
                {
                    if (TxtSerialPorts.SelectedItem == null)
                    {
                        MessageBox.Show("请选择通信串口。");
                        return;
                    }
                    LaoHuaDataProviderSerialPort.Instance.Connection.COM = TxtSerialPorts.SelectedItem + "";
                }

                ULogger.Info("Started Prepare:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                RefreshButtons(); 

                ULogger.Info("RefreshTaskDoing 2:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                UIZhuiSuAddModel w = new UIZhuiSuAddModel()
                { 
                };
                w.AddBatch(watcher);
                ULogger.Info("AddBatch 2:" + watcher.Elapsed.TotalSeconds.ToString("f3")); 
                //UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.SetSpan(0, 0, SettingModel.TxtTotalSpan);

                UIZhuiSuC10Model.Instance.DataProvider.Query(UIZhuiSuC10Model.Instance.BatchCurrent, MqttTaskCommand.StartTask); 
                ULogger.Info("AddBatch StartTask:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                this._model.View.UIZhuiSuC10SensorsPage.SetFontColor();
                ULogger.Info("AddBatch SetFontColor:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                UIZhuiSuData.Instance.RefreshData(  watcher);
                ULogger.Info("RefreshData:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                IniData(BatchCurrent,   watcher);
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
            SaveConfig();
            if (this.BtnPause.Content.ToString().IndexOf("继续")>=0)
            { 
                UIZhuiSuC10Model.Instance.DataProvider.Query(BatchCurrent, MqttTaskCommand.ResumeTask);
                UIZhuiSuData.Instance.ResumeBatch(BatchCurrent, F_AddTime.TotalSeconds);
                this.BtnPause.Content = "暂停(P)";
                this.BtnPause.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3275fd"));
                return;
            }
            if (this.BtnPause.Content.ToString().IndexOf("暂停") >= 0)
            {
                UIZhuiSuC10Model.Instance.DataProvider.Query(BatchCurrent, MqttTaskCommand.PauseTask);
                UIZhuiSuData.Instance.PauseBatch(BatchCurrent, F_AddTime.TotalSeconds);
                this.BtnPause.Content = "继续(P)";
                this.BtnPause.Background =  new SolidColorBrush((Color)ColorConverter.ConvertFromString("#12559d"));
                return;
            }

        }

        public void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            if (!BtnFinish.IsEnabled) return;
            if (_model == null) return;
            SaveConfig();
            Batch _Batch = BatchCurrent;
            if (_Batch != null)
            {

                //if (MessageBox.Show(
                //    string.Format("确定要结束{0}吗？", _Batch.F_BatchName),
                //    "确定要结束", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    UIZhuiSuC10Model.Instance.DataProvider.Query(_Batch, MqttTaskCommand.StopTask);
                    UIZhuiSuData.Instance.FinishBatch(_Batch, F_AddTime.TotalSeconds);
                    UIZhuiSuC10Model.Instance.IniPageData(_Batch);
                    IniData(BatchCurrent, new Stopwatch());
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
                string name = _Batch.F_BatchName.Replace("#", "-");
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
            string name = _Button.Name;
            SensorItemValueEnum se = UIZhuiSuC10Model.Instance.SettingModel.ValueMode;

            var bv = (_ColorSelect.ShowDialog());
            if (bv != null && bv.Value)
            {

                _Button.Background = _ColorSelect.SelectColor;
                ProductGrade _ProductGrade = (_Button.Tag as ProductGradeExtEdit).Current;
                Color c1 = (_ColorSelect.SelectColor as SolidColorBrush).Color;
                if (se == SensorItemValueEnum.ValueND) _ProductGrade.NDColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(c1.A, c1.R, c1.G, c1.B));
                if (se == SensorItemValueEnum.ValueDY) _ProductGrade.ADColor = System.Drawing.ColorTranslator.ToHtml(System.Drawing.Color.FromArgb(c1.A, c1.R, c1.G, c1.B));
                //this._model.View.UIZhuiSuC10SensorsPage.Refresh();

                ProductModelConfig c =  SettingModel.ProductModelConfig;
                if (c != null)
                {
                    foreach (var item in c.Grades)
                    {
                        if (item.Grade == _ProductGrade.Grade)
                        {
                            if (se == SensorItemValueEnum.ValueND) item.NDColor = _ProductGrade.NDColor;
                            if (se == SensorItemValueEnum.ValueDY) item.ADColor = _ProductGrade.ADColor;
                        }
                    }
                }

            }
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.RefreshMM();
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.Refresh();
        }

        public void SetCurrentTime(double val) {

            //SliderProgress.Value = val;
            //MKSS.Model.SensorGroupData new_data = this._model.ECService[TimeSpan.FromSeconds(SliderProgress.Value)];
            //if (new_data != null)
            //{
            //    this._model.View.UIZhuiSuC10SensorsPage.SetData(BatchCurrent, new_data, TimeSpan.FromSeconds(SliderProgress.Value));
            //}
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

         
        public void BtnExit_Click(object sender, RoutedEventArgs e)
        { 
            System.Environment.Exit(1);
        }

        private void RadioButtonGroupChoiceChipAccent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_model == null|| this._model.SettingModel==null) return;
            this._model.SettingModel.ValueMode
                = (RadioButtonGroupChoiceChipAccent.SelectedValue + "").IndexOf("浓度")>=0 
                ? SensorItemValueEnum.ValueND : SensorItemValueEnum.ValueDY;
            SensorGroupData.ShowND = this._model.SettingModel.ValueMode == SensorItemValueEnum.ValueND;
            RefreshMM();
            this._model.View.UIZhuiSuC10SensorsPage.Refresh();
        }

        private void BtnSetSpan_Click(object sender, RoutedEventArgs e)
        {
            if (BatchCurrent == null) return;
            UIZhuiSuC10Model.Instance.SettingModel.TxtCurrentSpanValue = double.Parse(TxtCurrentSpanValue.Text);
            BatchCurrent.F_SpanTime = UIZhuiSuC10Model.Instance.SettingModel.TxtCurrentSpan;
            BatchCurrent.F_SpanValue = UIZhuiSuC10Model.Instance.SettingModel.TxtCurrentSpanValue;
            SensorGroupDataStander.OxygenStd = BatchCurrent.F_SpanValue;
            bool b = UIZhuiSuData.Instance.BatchServices.Update(BatchCurrent).Result;
            TrySpanTime(BatchCurrent);
            this._model.View.UIZhuiSuC10SensorsPage.Refresh();
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