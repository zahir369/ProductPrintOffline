using MKSS.Model;
using MKSS.Service.O2Tester;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;

namespace MKSS.APP.O2Tester
{
    /// <summary>
    /// UIZhuiSuCGK.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuTaskList : UserControl
    {
        public UIZhuiSuTaskList()
        {
            InitializeComponent();
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            IniData();
        }

        public void IniData()
        {
            UIZhuiSuData.Instance.RefreshData( new System.Diagnostics.Stopwatch());
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
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            Batch _Batch = this.dataGrid.SelectedItem as Batch;
            if (_Batch != null)
            {
                try
                {
                    Cursor = Cursors.Wait;
                    MKSS.APP.UserControls.WaitWindow.ShowWindow("正在打开", "正在打开" + _Batch.F_BatchName + "......", this);
                    UIZhuiSuModel.Intance.PageContext.TabItemCurrent.IsSelected = true;
                    O2TesterSaver.Batch = _Batch;

                    SensorGroupDataStander.OxygenStd = _Batch.F_SpanValue;
                    UIZhuiSuC10Setting.TrySpanTime(_Batch);

                    UIZhuiSuC10Model.Instance.SettingModel.TxtCurrentSpan = _Batch.F_SpanTime;
                    UIZhuiSuC10Model.Instance.SettingModel.TxtCurrentSpanValue = _Batch.F_SpanValue; 
                    string sql_s = " F_BatchId='" + _Batch.F_BatchId + "' order by F_SensorId ";
                    List<Sensor> tabs = UIZhuiSuData.Instance.SensorServices.Query(sql_s).Result;
                    UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10._model.View.UIZhuiSuC10SensorsPage.SetFontColor();
                    UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10Chart.SetBatch(_Batch, tabs.Select(w => w.PosEnum).ToList(), new Stopwatch());
                    SensorStaDataCache.Instance.CalculateStart();
                    UIZhuiSuC10Model.Instance.ECService.InitDataByDb(_Batch);
                    SensorStaDataCache.Instance.CalculateAuto(UIZhuiSuC10Model.Instance.ECService.BatchSensorDataDictionary.Values.ToList());
                    UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10Chart.UIZhuiSuC10SensorsSET.IniData(_Batch, tabs, new System.Diagnostics.Stopwatch());
                    UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10Chart.UIZhuiSuC10SensorsSET.SettingModel.TxtTotalSpan = (int)_Batch.F_AgingEndTime;
                    UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10Chart.UIZhuiSuC10SensorsSET.SettingModel.CurrentTimeSpan = TimeSpan.FromSeconds(_Batch.F_AgingEndTime);
                     
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
