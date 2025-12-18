using DeviceDataMonitorWPF;
using Microsoft.Win32;
using MKSS.Model;
using MKSS.Service.SemiTester;
using System;
using System.Collections.Generic;
using System.IO.Ports;
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

namespace MKSS.APP.SemiTester
{
    /// <summary>
    /// UIZhuiSuC10Legend.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10TittleSetting : UserControl
    {
        UIZhuiSuC10Model _model
        {
            get
            {
                return UIZhuiSuC10Model.Instance;
            }
        }
        public UIZhuiSuC10SettingModel SettingModel;
        public UIZhuiSuC10TittleSetting()
        {
            InitializeComponent();
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            MainWindow.Instance.Loaded += Instance_Loaded;
        }

        private void Instance_Loaded(object sender, RoutedEventArgs e)
        {

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回

            if (_model == null) return;
            SettingModel = _model.SettingModel;
            this.DataContext = SettingModel;

            this.SettingModel.TxtSerialPorts = SemiTesterConfgig.Instance.SerialPorts;
            this.SettingModel.TxtSensorGrougAddress = SemiTesterConfgig.Instance.SensorGrougAddress;
            this.SettingModel.TxtProductList = SemiTesterConfgig.Instance.ProductList;
            this.SettingModel.TxtModelList = SemiTesterConfgig.Instance.ModelList;

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
            foreach (ProductConfig item in SemiTesterConfgig.Instance.Product)
            {
                this.TxtProductList.Items.Add(item);
                if (item.Name == SettingModel.TxtProductList)
                {
                    this.TxtProductList.SelectedItem = item;
                    selP = item;
                }
            }
            if (selP == null && SemiTesterConfgig.Instance.Product.Count > 0)
            {
                this.TxtProductList.SelectedItem = SemiTesterConfgig.Instance.Product[0];
                selP = SemiTesterConfgig.Instance.Product[0];
            }
            if (selP != null)
            {
                this.SettingModel.TxtProductList = selP.Name;
                this.SettingModel.TxtTotalSpan = selP.TimeTotal;
                this.SettingModel.TxtGradingTimePoint = selP.GradingTimePoint;
                this.SettingModel.TxtTestTimePoints = selP.TestTimePoints;

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

            TxtProductList_SelectionChanged(null,null);
            TxtModelList_SelectionChanged(null, null);
            _model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.TxtProductList_SelectionChanged(null, null);
            _model.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.TxtModelList_SelectionChanged(null, null);

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
            ProductConfig c = this.TxtProductList.SelectedItem as ProductConfig;
            ProductModelConfig m = this.TxtModelList.SelectedItem as ProductModelConfig;
            SemiTesterConfgig.Instance.SerialPorts = this.SettingModel.TxtSerialPorts;
            SemiTesterConfgig.Instance.SensorGrougAddress = this.SettingModel.TxtSensorGrougAddress;
            if (this.SettingModel.TxtTotalSpan > 0) c.TimeTotal = this.SettingModel.TxtTotalSpan;
            if (this.SettingModel.TxtGradingTimePoint > 0) c.GradingTimePoint = this.SettingModel.TxtGradingTimePoint;
            if (!string.IsNullOrEmpty(TxtTestTimePoints.Text)) c.TestTimePoints = this.SettingModel.TxtTestTimePoints;
            SemiTesterConfgig.Instance.ProductList = c.Code;
            SemiTesterConfgig.Instance.ModelList = m.Name;
            SemiTesterConfgig.Save();
        }

        public void TxtProductList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回

            ProductConfig c = this.TxtProductList.SelectedItem as ProductConfig;
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


        private void TxtModelList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回

            ProductConfig c = this.TxtProductList.SelectedItem as ProductConfig;
            if (c == null || string.IsNullOrEmpty(TxtTotalSpan.Text))
            { 
                return;
            } 
            ProductModelConfig m = this.TxtModelList.SelectedItem as ProductModelConfig;
            if (m == null) return;
            SettingModel.ProductModelConfig = m;

            if (UIZhuiSuC10Model.Instance.View != null) UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.SetAxisLimits(0, c.TimeTotal * 1.2, 0, Service.SemiTester.SemiTesterService.VersionVoltage);
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
                if (int.Parse(TxtTotalSpan.Text) > 10) {
                    c.TimeTotal = int.Parse(TxtTotalSpan.Text);
                    c.ZeroPoint = int.Parse(TxtZeroPoint.Text);
                    c.SpanPoint = int.Parse(TxtSpanPoint.Text);
                    c.TestTimePoints = ProductConfig.AutoTestTimePoints(c.TimeTotal, c.ZeroPoint, c.SpanPoint);
                    this.TxtTestTimePoints.Text = c.TestTimePoints;
                    if (UIZhuiSuC10Model.Instance.View != null) UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.SetAxisLimits(0, c.TimeTotal * 1.2, 0, Service.SemiTester.SemiTesterService.VersionVoltage);
                }
               
            }
            catch (Exception)
            {

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ProductConfig c = UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtProductList.SelectedItem as ProductConfig;
            if (c == null)
            {
                MessageBox.Show("请选择产品型号。");
                return;
            }

            UIAddModel ad = new UIAddModel();
            ad.Owner = MainWindow.Instance;
            ad.Show();
        }

        void BtnExcelExport_Click(object sender, RoutedEventArgs e)
        {
            UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.BtnExcelExport_Click(null, null);
        }


    }
} 