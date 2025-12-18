using DeviceDataMonitorWPF;
using MKSS.APP.UserControls;
using MKSS.Model;
using MKSS.Service.SemiTester;
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

namespace MKSS.APP.SemiTester
{
    /// <summary>
    /// UISerialNo.xaml 的交互逻辑
    /// </summary>
    public partial class UIAddModel : Window
    {
        public UIAddModel()
        {
            InitializeComponent(); 
        }

        private void BtnCance_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            ProductModelConfig m = ProductModelConfig.AddDefaultModel();
            m.Name = this.TxtNewValue.Text;
            UIZhuiSuC10Model.Instance.SettingModel.ProductConfig.Models.Add(m);
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtProductList_SelectionChanged(null, null);//刷新下拉框
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10SensorsPage.UIZhuiSuC10TittleSetting.TxtModelList.SelectedItem = m;
            SemiTesterConfgig.Save();
            this.Close();
        }
    }
}
