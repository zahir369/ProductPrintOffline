using MKSS.APP.ZhuiSu;
using MKSS.APP.ZhuiSu.Util;
using MKSS.Model;
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
using MKSS.Service.ZhuiSu;

namespace MKSS.APP.ZhuiSu
{
    /// <summary>
    /// UIZhuiSuAdd.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuAddCondition : Window
    {
         
        public UIZhuiSuAddCondition()
        {
            InitializeComponent();
        }

        public long F_BatchId { get; set; }
        public long F_ConditionId { get; set; }
        public double F_StartTime { get; set; }
        public UIZhuiSuAddCondition(long F_BatchId,double F_StartTime)
        {
            InitializeComponent();
            this.F_BatchId = F_BatchId;
            this.F_StartTime = F_StartTime;
        }

        public UIZhuiSuAddCondition(long F_BatchId,long F_ConditionId, double F_StartTime)
        {
            InitializeComponent();
            this.F_BatchId = F_BatchId;
            this.F_ConditionId = F_ConditionId;
            this.F_StartTime = F_StartTime;
        }

        private void Txt_InputNumber(object sender, KeyEventArgs e)
        {
            InputNumber(e);
        }
         
        public static void InputNumber(KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back)
            {
                e.Handled = false;
            }
            else if ((e.Key >= Key.D0 && e.Key <= Key.D9 || e.Key == Key.Back))
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
                //System.Windows.MessageBox.Show("请输入数字");
                return;
            }
        }


        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BatchCondition _BatchCondition = new BatchCondition() { 
                     F_BatchId = this.F_BatchId, F_ConditionId = DateTime.Now.Ticks, F_StartTime = F_StartTime,
                       F_Humidity = double.Parse(this.F_Humidity.Text), F_Temperature = double.Parse(this.F_Temperature.Text),
                       F_Memo = F_Memo.Text
                };
                int res = UIZhuiSuData.Instance.BatchServices.BaseDal.Db.Insertable< BatchCondition >(_BatchCondition).ExecuteReturnIdentityAsync().Result;
                this.Cursor = Cursors.Arrow;
                this.DialogResult = res > 0;
                MessageBox.Show(string.Format("添加{0}", res > 0 ? "成功":"失败"));
                this.Close();
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show(ex.Message);
                this.DialogResult = false;
            }

        }


        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.F_ConditionId > 0) {
                    bool res = UIZhuiSuData.Instance.BatchServices.BaseDal.Db.Deleteable<BatchCondition>(F_ConditionId).ExecuteCommandHasChangeAsync().Result;
                    this.Cursor = Cursors.Arrow;
                    MessageBox.Show(string.Format("删除{0}", res ? "成功" : "失败"));
                    this.DialogResult = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                this.Cursor = Cursors.Arrow;
                MessageBox.Show(ex.Message);
                this.DialogResult = false;
            }

        }

    }

}