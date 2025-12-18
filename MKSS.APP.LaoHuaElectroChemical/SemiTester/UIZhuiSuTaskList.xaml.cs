using MKSS.Model;
using MKSS.Service;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Diagnostics;
using MKSS.Service.LaoHuaElectroChemical;
using MKSS.Util.Log;
using DeviceDataMonitorWPF;

namespace MKSS.APP.SemiTester
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
            MainWindow.Instance.TaskFirst = UIZhuiSuData.Instance.DBListAll.FirstOrDefault();
        }

         
        private void BtnFinish_Click(object sender, RoutedEventArgs e)
        {
            Batch _Batch = this.dataGrid.SelectedItem as Batch;
            if (_Batch != null && _Batch.EnumAgingStatus!= EnumAgingStatus.Finished)
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
                    if (MessageBox.Show(
                        string.Format("确定要重新开始检测{0}吗？将继续检测。", _Batch.F_BatchName),
                        "确定要结束", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        _Batch.EnumAgingStatus = EnumAgingStatus.InAging; 
                        UIZhuiSuData.Instance.RestartBatch(_Batch);
                        IniData();
                        MessageBox.Show("已重新开始检测，检测时间。");
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
            if (ElectroChemicalSaver.Batch!=null 
                && ElectroChemicalSaver.Batch.EnumAgingStatus == EnumAgingStatus.InAging
                ) {
                MessageBox.Show("请先停止原有项目。");
                return;
            }
            OpenBatch(  _Batch,  this);
        }

        public static void OpenBatch(Batch _Batch, FrameworkElement _this) {
            
            if (_Batch != null)
            {

                if (UIZhuiSuC10Model.Instance.BatchCurrent!=null && _Batch.F_BatchId == UIZhuiSuC10Model.Instance.BatchCurrent.F_BatchId) return;
                try
                {

                    if (_Batch.EnumAgingStatus == EnumAgingStatus.Finished) {
                        _this.Cursor = Cursors.Wait;
                        MKSS.APP.UserControls.WaitWindow.ShowWindow("正在打开", "正在打开任务" + _Batch.F_BatchName + "......", _this);
                        UIZhuiSuModel.Intance.PageContext.TabItemCurrent.IsSelected = true;
                        ElectroChemicalSaver.Batch = _Batch;
                        SensorGroupDataStander.OxygenStd = _Batch.F_SpanValue;

                        UIZhuiSuC10Model.Instance.IniPageData(_Batch);
                        UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.SettingModel.CurrentTimeSpan = _Batch.F_AgingLastUpdateTime - _Batch.F_AgingStartTime;
                    }


                    if (_Batch.EnumAgingStatus != EnumAgingStatus.Finished)
                    {
                        if (MessageBox.Show(
                            string.Format("任务未正常结束，要继续老化任务吗？选“是”将打开并继续老化任务，选“否”仅打开项目。", _Batch.F_BatchName),
                       "确定要结束", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                        {

                            _this.Cursor = Cursors.Wait;
                            MKSS.APP.UserControls.WaitWindow.ShowWindow("正在打开", "正在打开任务" + _Batch.F_BatchName + "......", _this);
                            UIZhuiSuModel.Intance.PageContext.TabItemCurrent.IsSelected = true;
                            ElectroChemicalSaver.Batch = _Batch;
                            SensorGroupDataStander.OxygenStd = _Batch.F_SpanValue;

                            UIZhuiSuC10Model.Instance.IniPageData(_Batch);
                            UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.SettingModel.CurrentTimeSpan = _Batch.F_AgingLastUpdateTime - _Batch.F_AgingStartTime;

                            Stopwatch watcher = new Stopwatch();
                            watcher.Start();
                            UIZhuiSuC10Model.Instance.DataProvider.Query(UIZhuiSuC10Model.Instance.BatchCurrent, MqttTaskCommand.StartTask);
                            ULogger.Info("AddBatch StartTask:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                            UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10SensorsPage.SetFontColor();
                            ULogger.Info("AddBatch SetFontColor:" + watcher.Elapsed.TotalSeconds.ToString("f3"));
                            UIZhuiSuData.Instance.RefreshData(watcher);
                            UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.RefreshButtons();
                            watcher.Stop();

                        }
                        else {

                            _this.Cursor = Cursors.Wait;
                            MKSS.APP.UserControls.WaitWindow.ShowWindow("正在打开", "正在打开任务" + _Batch.F_BatchName + "......", _this);
                            UIZhuiSuModel.Intance.PageContext.TabItemCurrent.IsSelected = true;
                            ElectroChemicalSaver.Batch = _Batch;
                            SensorGroupDataStander.OxygenStd = _Batch.F_SpanValue;

                            UIZhuiSuC10Model.Instance.IniPageData(_Batch);
                            UIZhuiSuModel.Intance.PageContext.UIZhuiSuC10.UIZhuiSuC10SensorsPage.UIZhuiSuC10SensorsSET.SettingModel.CurrentTimeSpan = _Batch.F_AgingLastUpdateTime - _Batch.F_AgingStartTime;

                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    _this.Cursor = Cursors.Arrow;
                    MKSS.APP.UserControls.WaitWindow.CloseWindow(_this);
                }

            }
        }


    }
}
