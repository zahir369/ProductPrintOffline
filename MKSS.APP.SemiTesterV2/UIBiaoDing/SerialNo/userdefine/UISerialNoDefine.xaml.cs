using MKSS.APP.UIBiaoDing.Config;
using MKSS.APP.UIBiaoDing.Util;
using MKSS.APP.UserControls;
using MKSS.Model;
using MKSS.Service.LaoHua;
using MKSS.Service.UIBiaoDing;
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

namespace MKSS.APP.UIBiaoDing
{
    /// <summary>
    /// UISerialNo.xaml 的交互逻辑
    /// </summary>
    public partial class UISerialNoDefine : Window
    {
        public UISerialNoDefine()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool selP = false;
            this.TxtProductList.Items.Clear();
            SerialNoRuleEntity entity = UISheBeiBiaoDingViewModel.Intance.SelectRuleEntity;

            foreach (ProductConfig item in BiaoDingConfgig.Instance.Product)
            {
                this.TxtProductList.Items.Add(item); 
            }

            foreach (ProductConfig item in BiaoDingConfgig.Instance.Product)
            {
                if (item.Name == UISheBeiBiaoDingViewModel.Intance.TxtProductList)
                {
                    this.TxtProductList.SelectedItem = item;
                    selP = true;
                }
                if (entity != null)
                {
                    if (item.Name == entity.ProductFullName)
                    {
                        this.TxtProductList.SelectedItem = item;
                        selP = true;
                    }
                }
            }


            if (!selP && BiaoDingConfgig.Instance.Product.Count > 0) this.TxtProductList.SelectedItem = BiaoDingConfgig.Instance.Product[0];
            
            if (entity != null) {
                this.OrderNumber.Text = entity.OrderNumber+"";
            }

        }
         
         
        private void BtnScrwd_Click(object sender, RoutedEventArgs e)
        {
            if (SelectRwd())
            {
                this.DialogResult = true;
                this.Close();
            }
        }


        bool SelectRwd()
        {

            long test = 0;
            if (!long.TryParse(this.OrderNumber.Text, out test))
            {
                MessageBox.Show("请输入有效的编号前缀，必须是七位数字！");
                return false;
            }
            else {
                if (test < 1000000 || test > 9999999) {
                    MessageBox.Show("请输入有效的编号前缀，必须是七位数字！");
                    return false;
                }
            }


            ProductConfig productSel = this.TxtProductList.SelectedItem as ProductConfig;

            SerialNoRuleEntity _ScrwEntity = new SerialNoRuleUserDefineEntity() {
              OrderNumber=this.OrderNumber.Text, ProductCode = productSel.Code+"", ProductFullName = productSel.Name
            };

            _ScrwEntity.OrderNumberPrefix = UISheBeiBiaoDingViewModel.Intance.SerialNoFactory.SerialNoParser(_ScrwEntity.OrderNumber).Prefix;
            //************************************************************************
            APP.UIBiaoDing.UISheBeiBiaoDingViewModel.Intance.SelectRuleEntity = _ScrwEntity;
            //************************************************************************
            //return true;

            var find = false;
            var xx = UISheBeiBiaoDingViewModel.Intance.PageContext.TxtProductList.Items;
            foreach (var item in xx)
            {
                ProductConfig product = item as ProductConfig;
                if (product == null) continue;
                if (product.Code + "" == _ScrwEntity.ProductCode)
                {
                    UISheBeiBiaoDingViewModel.Intance.PageContext.TxtProductList.SelectedItem = product;
                    find = true;
                }
            }
            if (!find)
            {
                ProductConfig _ProductConfig = new ProductConfig()
                {
                    Code = long.Parse(_ScrwEntity.ProductCode),
                    Name = _ScrwEntity.ProductFullName,
                    ValueBase = 0,
                    VoltageValueBase = 0
                };
                UISheBeiBiaoDingViewModel.Intance.PageContext.TxtProductList.Items.Add(_ProductConfig);
                UISheBeiBiaoDingViewModel.Intance.PageContext.TxtProductList.SelectedItem = _ProductConfig;
            }
            UISheBeiBiaoDingViewModel.Intance.PageContext.BtnScrwdTip.Badge = "";
            string title = string.Format(" 型号：{1}，单号：{0}，数量：{2}，日期：{3}", _ScrwEntity.OrderNumber, _ScrwEntity.ProductFullName, _ScrwEntity.Qty, _ScrwEntity.Date); ;
            UISheBeiBiaoDingViewModel.Intance.PageContext.BtnScrwd.ToolTip = title;
            UISheBeiBiaoDingViewModel.Intance.PageContext.TxtProductList.Text = productSel.Name; 
            UISheBeiBiaoDingViewModel.Intance.LabelMessage = title;
            UISheBeiBiaoDingViewModel.Intance.SelectRuleEntity = _ScrwEntity;

            return true;
        }
         
    }
}
