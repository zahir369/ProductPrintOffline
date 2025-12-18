using MKSS.APP.SemiCatalysis;
using MKSS.Model;
using MKSS.Service.SemiCatalysis;
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

namespace MKSS.APP.SemiCatalysis
{
    /// <summary>
    /// UIZhuiSuCGK.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuTaskList : UserControl
    {
        public UIZhuiSuTaskList()
        {
            InitializeComponent();
            IniData();
        }

        public void IniData()
        {
            UIZhuiSuData.Instance.RefreshData();
            this.dataGrid.DataContext = UIZhuiSuData.Instance.DBListAll;
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
                    UIZhuiSuData.Instance.FinishBatch(_Batch,-1);
                    IniData();
                    MessageBox.Show("已停止检测任务");
                }
            }
        }

        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {

            Batch _Batch = this.dataGrid.SelectedItem as Batch;
            if (_Batch.EnumAgingStatus != EnumAgingStatus.Finished) {
                if (MessageBox.Show(
                    string.Format("正在检测，不需要重新开始。", _Batch.F_BatchName),
                    "信息", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                     
                }
                return;
            }
            
            if (_Batch != null)
            {

                //if (_Batch.F_AgingEndTime < DateTime.Now)
                {
                    TimeSpan ts = TimeSpan.FromSeconds( _Batch.F_AgingEndTime);
                    if (MessageBox.Show(
                        string.Format("确定要重新开始检测{0}吗？将继续检测：" + ts.TotalHours.ToString("f1") + "小时。", _Batch.F_BatchName),
                        "确定要结束", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        _Batch.F_AgingEndTime = _Batch.F_AgingEndTime;
                        _Batch.EnumAgingStatus = EnumAgingStatus.InAging; 
                        UIZhuiSuData.Instance.RestartBatch(_Batch);
                        IniData();
                        MessageBox.Show("已重新开始检测，检测时间：" + ts.TotalHours.ToString("f1") + "小时。");
                    }
                }
                //else
                {
                    //TimeSpan ts = _Batch.F_AgingEndTime - DateTime.Now;
                    //if (MessageBox.Show(
                    //    string.Format("确定要继续检测{0}吗？将继续检测：" + ts.TotalHours.ToString("f1") + "小时。", _Batch.F_BatchName),
                    //    "确定要结束", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    //{
                    //    UIZhuiSuData.Instance.RestartBatch(_Batch);
                    //    IniData();
                    //    MessageBox.Show("已继续检测任务，将继续检测：" + ts.TotalHours.ToString("f1") + "小时。");
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

                    UIZhuiSuData.Instance.DeleteBatch(_Batch);

                    try
                    {
                        this.Cursor = Cursors.AppStarting;
                        UIZhuiSuData.Instance.DeleteBatch(_Batch);
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
                try
                {
                    Cursor = Cursors.Wait;
                    MKSS.APP.UserControls.WaitWindow.ShowWindow("正在打开", "正在打开" + _Batch.F_BatchName + "......", this);
                    UIZhuiSuModel.Intance.PageContext.TabItemCurrent.IsSelected = true;
                    SemiCatalysisSaver.Batch = _Batch;
                    UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10._model.View.UIZhuiSuC10Sensors.SetFontColor();
                    UIZhuiSuC10Model.Instance.ECService.InitDataByDb(_Batch);
                    UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Chart.SetAxisLimits(0, _Batch.F_AgingEndTime * 1.1);
                    string sql_s = " F_BatchId='" + _Batch.F_BatchId + "' order by F_SensorId ";
                    List<Sensor> tabs = UIZhuiSuData.Instance.SensorServices.Query(sql_s).Result;
                    UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10Setting.IniData(_Batch, tabs);
                    UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10Setting.SettingModel.TxtTotalSpan = (int)_Batch.F_AgingEndTime;
                    UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10Setting.SettingModel.CurrentTimeSpan = TimeSpan.FromSeconds(_Batch.F_AgingEndTime);
                    

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally {
                    Cursor = Cursors.Arrow;
                    MKSS.APP.UserControls.WaitWindow.CloseWindow(this);
                }
                
            }
        }
         

    }
}
