using MKSS.APP.LaoHua;
using MKSS.APP.LaoHua.UILaoHua;
using MKSS.APP.UIBiaoDing;
using MKSS.APP.UIBiaoDing.Config;
using MKSS.Core.Common.HttpRestSharp;
using MKSS.Model;
using MKSS.Service.UIBiaoDing;
using MKSS.Util;
using RestSharp; 
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
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

namespace MKSS.APP.LaoHua.UILaoHua
{

    /// <summary>
    /// UIScrwSelect.xaml 的交互逻辑
    /// </summary>
    public partial class UIScrwSelect : Window
    {
        public UIScrwSelect( )
        {
           
            InitializeComponent();
            for (DateTime i = DateTime.Now.AddYears(-1); i < DateTime.Now.AddMonths(1); i=i.AddMonths(1))
            {
                this.monthList.Items.Add(i.ToString("yyyyMM"));
            }
        }
         

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.monthList == null || this.isFinished == null || this.depName == null) return;
            this.dataGrid.DataContext = GetScrwList(this.monthList.SelectedValue + "", "", "", this.isFinished.IsChecked + "", SelectDepName + "", "");
        }

        public string SelectDepName {
            get {
                ListBoxItem item =(this.depName.SelectedValue as ListBoxItem);
                if (item == null) return "";
                return item.Content+"";
            }
        }
         


        /// <summary>
        /// 获取生产任务
        /// </summary> 
        /// <param name="key">名称</param>
        /// <param name="finished">是否完成</param>
        /// <param name="depName">部门名称</param>
        /// <param name="userName">销售人员名称</param>
        /// <param name="monthrange">订单开始结束月份</param>
        /// <param name="page">页数</param>
        /// <param name="pagesize">行数</param>
        /// <returns></returns>
        public List<SerialNoRuleScrwEntity> GetScrwList(string monthFrom , string monthTo, string key, string finished = "false", string depName = "电子",string userName="")
        {
             
            if (depName == "全部")
            {
                depName = "";
            }

            if (string.IsNullOrEmpty(key))
            {
                key = "";
            }
            string GraspAshx = ConfigurationManager.AppSettings["GraspAshx"];// Appsettings.App(new string[] { "GraspAshx" });// ConfigurationManager.AppSettings["GraspAshx"];// "http://117.160.239.252:30003/";
            string GraspAshxsub1 = GraspAshx + "AppWorkingOrder.ashx";
            string param = string.Format("finished={0}&depName={1}&userName={2}&monthFrom={3}&monthTo={4}&isscrw=true", finished, depName, userName, monthFrom, monthTo);
            //http://localhost:8899/AppWorkingOrder.ashx?finished=false&depName=&userName=&monthFrom=&monthTo
            var client = new RestSharpClient(GraspAshx);
            var request = client.Execute(new RestRequest($"{GraspAshxsub1}?{param}", Method.GET));
            if (request.StatusCode != HttpStatusCode.OK)
            {
                return new List<SerialNoRuleScrwEntity>();
            }

            try
            {
                MesResult temp = Newtonsoft.Json.JsonConvert.DeserializeObject<MesResult>(request.Content);
                List<SerialNoRuleScrwEntity> ret = new List<SerialNoRuleScrwEntity>();
                List<SerialNoRuleScrwEntity> retAll = new List<SerialNoRuleScrwEntity>();
                if (temp != null && temp.data != null)
                {
                    retAll.AddRange(temp.data);
                }

                if (!string.IsNullOrEmpty(key))
                {
                    retAll = retAll.Where(w => w.ProductFullName.IndexOf(key) >= 0 || w.OrderNumber.IndexOf(key) >= 0).ToList();
                }
                retAll = retAll.OrderByDescending(w => w.OrderNumber).ToList();
                return retAll;

            }
            catch (Exception ex)
            {
                return new List<SerialNoRuleScrwEntity>();
            }
            return new List<SerialNoRuleScrwEntity>();


        }

        private void OpenBatch_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SelectRwd())
            {
                this.DialogResult = true;
                this.Close();
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


        bool  SelectRwd() {

            SerialNoRuleEntity _ScrwEntity = this.dataGrid.SelectedItem as SerialNoRuleEntity;
            if (_ScrwEntity == null) {
                return false;
            }

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
            UISheBeiBiaoDingViewModel.Intance.PageContext.TxtProductList.ToolTip = title;
            UISheBeiBiaoDingViewModel.Intance.LabelMessage = title;
            UISheBeiBiaoDingViewModel.Intance.SelectRuleEntity = _ScrwEntity;

            return true;
        }

        private void monthList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.monthList == null || this.isFinished == null || this.depName == null) return;
            this.dataGrid.DataContext = GetScrwList(this.monthList.SelectedValue + "", "", "",this.isFinished.IsChecked+"",SelectDepName + "","");
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.monthList == null || this.isFinished == null || this.depName == null) return;
            this.dataGrid.DataContext = GetScrwList(this.monthList.SelectedValue + "", "", "", this.isFinished.IsChecked + "", SelectDepName + "", "");
        }

    }

}
