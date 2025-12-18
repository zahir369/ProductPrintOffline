using MKSS.APP.LaoHua.Util;
using MKSS.APP.LaoHua.UILaoHua;
using MKSS.Model;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MKSS.APP.UserControls;

namespace MKSS.APP.LaoHua
{
    /// <summary>
    /// UILaoHuaGuanChaCGK.xaml 的交互逻辑
    /// </summary>
    public partial class UILaoHuaGuanChaTaskList : UserControl
    {
        public UILaoHuaGuanChaTaskList()
        {
            InitializeComponent();

            if (UILaoHuaGuanChaModel.IsInDesignMode(this)) return;//设计模式直接返回
            IniData();
        }

        public void IniData()
        {
            WaitWindow.ShowWindow("正在刷新", "正在读取老化任务......", this);
            UILaoHuaGuanChaData.Instance.RefreshData();
            this.dataGrid.DataContext = UILaoHuaGuanChaData.Instance.DBListAll;
            WaitWindow.CloseWindow(this);
        }


        private void BtnAddBatch_Click(object sender, RoutedEventArgs e)
        {
            UILaoHuaGuanChaAdd w = new UILaoHuaGuanChaAdd();
            w.Topmost = true;
            bool? sucess = w.ShowDialog();
            if (sucess != null && sucess.Value)
            {

            }
            IniData();
        }
         
        private void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            Batch _Batch = this.dataGrid.SelectedItem as Batch;
            if (_Batch != null)
            {
                if (MessageBox.Show(
                    string.Format("确定要结束{0}吗？", _Batch.F_BatchName),
                    "确定要结束", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    UILaoHuaGuanChaData.Instance.FinishBatch(_Batch);
                    IniData();
                    MessageBox.Show("已停止老化任务");
                }
            }
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {

            Batch _Batch = this.dataGrid.SelectedItem as Batch;
            if (_Batch.EnumAgingStatus != EnumAgingStatus.Finished) {
                if (MessageBox.Show(
                    string.Format("正在老化，不需要重新开始。", _Batch.F_BatchName),
                    "信息", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                     
                }
                return;
            }
            
            if (_Batch != null)
            {

                //if (_Batch.F_AgingEndTime < DateTime.Now)
                {
                    TimeSpan ts = _Batch.F_AgingEndTime - _Batch.F_AgingStartTime;
                    if (MessageBox.Show(
                        string.Format("确定要重新开始老化{0}吗？将继续老化：" + ts.TotalHours.ToString("f1") + "小时。", _Batch.F_BatchName),
                        "确定要结束", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        _Batch.F_AgingEndTime = DateTime.Now + (ts);
                        _Batch.EnumAgingStatus = EnumAgingStatus.InAging;
                        UILaoHuaGuanChaData.Instance.BatchRelServices.Update(_Batch).Wait();
                        UILaoHuaGuanChaData.Instance.RestartBatch(_Batch);
                        IniData();
                        MessageBox.Show("已重新开始老化，老化时间：" + ts.TotalHours.ToString("f1") + "小时。");
                    }
                }
                //else
                {
                    //TimeSpan ts = _Batch.F_AgingEndTime - DateTime.Now;
                    //if (MessageBox.Show(
                    //    string.Format("确定要继续老化{0}吗？将继续老化：" + ts.TotalHours.ToString("f1") + "小时。", _Batch.F_BatchName),
                    //    "确定要结束", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    //{
                    //    UILaoHuaGuanChaData.Instance.RestartBatch(_Batch);
                    //    IniData();
                    //    MessageBox.Show("已继续老化任务，将继续老化：" + ts.TotalHours.ToString("f1") + "小时。");
                    //}
                }
                
            }

        }
        //第一个按钮点击事件
        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            Batch _Batch = this.dataGrid.SelectedItem as Batch;
            if (_Batch != null) {
                if (MessageBox.Show(
                    string.Format("确定要删除{0}吗？删除后数据不能恢复。", _Batch.F_BatchName),
                    "确认删除", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {

                    UILaoHuaGuanChaData.Instance.DeleteBatch(_Batch);

                    try
                    {
                        this.Cursor = Cursors.AppStarting;
                        UILaoHuaGuanChaData.Instance.DeleteBatch(_Batch);
                        this.Cursor = Cursors.Arrow;
                        IniData();
                        MessageBox.Show("删除成功");
                    }
                    catch (Exception ex)
                    {
                        IniData();
                        this.Cursor = Cursors.Arrow;
                        MessageBox.Show(ex.Message);
                    }


                }
            }
            //MessageBox.Show(users[datagrid.SelectedIndex].Name);
        }


        private void OpenBatch_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Batch _Batch = this.dataGrid.SelectedItem as Batch;
            if (_Batch != null)
            {
                WaitWindow.ShowWindow("正在打开", "正在打开批次" + _Batch.F_BatchName + "......", this); 
                UILaoHuaGuanChaModel.Intance.PageContext.TabItemCurrent.IsSelected = true;
                UILaoHuaGuanChaModel.Intance.PageContext.UILaoHuaGuanChaC10.IniData(_Batch);
                WaitWindow.CloseWindow(this);
            }
        }

    }
}
