using MKSS.APP.O2Tester;
using MKSS.APP.UserControls;
using MKSS.Model;
using MKSS.Service.O2Tester;
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

namespace MKSS.APP.O2Tester
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
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.UIZhuiSuC10SensorsSET.TxtProductList_SelectionChanged(null, null);//刷新下拉框
            UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.UIZhuiSuC10SensorsSET.TxtModelList.SelectedItem = m;
            O2TesterConfgig.Save();
            this.Close();
        }
    }
}
