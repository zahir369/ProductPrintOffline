using MKSS.APP.ECTester;
using Microsoft.Win32;
using MKSS.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
using System.Windows.Threading;

namespace MKSS.APP.ECTester
{

    /// <summary>
    ///  UIZhuiSuC10.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10 : UserControl
    {

        public UIZhuiSuC10Model _model = null;
        public UIZhuiSuC10()
        {
            InitializeComponent();

            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            _model = base.DataContext as UIZhuiSuC10Model;
            _model.InitPage(this);


        }

        public Batch SelectBatch { get; set; }
       


        public Batch Batch { get; set; }
        public TimeSpan F_AddTime { get; set; }
        public SensorGroupData SensorDatas { get; set; }
        public void SetData(Model.Batch _batch, SensorGroupData data, TimeSpan f_AddTime)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            Batch = _batch;
            SensorDatas = data;
            F_AddTime = f_AddTime;
            UIZhuiSuC10Model.Instance.SettingModel.CurrentTimeSpan = F_AddTime;
            //UIZhuiSuC10Model.Instance.SettingModel.CalcHeGe();
            UIZhuiSuC10Grid.SetData(_batch, data,   F_AddTime);
            UIZhuiSuC10SensorsPage.SetData(_batch, data,   f_AddTime);
            UIZhuiSuC10Chart.SetPageBoardItem(_batch, data);
            
        }

        public void SetBatchData(Model.Batch _batch, Dictionary<TimeSpan, SensorGroupData> datas)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)|| datas==null) return;//设计模式直接返回
            Batch = _batch;

            foreach (var f_AddTime in datas.Keys)
            {
                var data = datas[f_AddTime];
                SensorDatas = data;
                F_AddTime = f_AddTime;
                UIZhuiSuC10Model.Instance.SettingModel.CurrentTimeSpan = F_AddTime;
                //UIZhuiSuC10Model.Instance.SettingModel.CalcHeGe();
                UIZhuiSuC10Grid.SetData(_batch, data, F_AddTime);
            }
            UIZhuiSuC10SensorsPage.SetData(_batch, SensorDatas, F_AddTime); 

        }


        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UIZhuiSuModel.IsInDesignMode(this)) return;//设计模式直接返回
            if (this.TabItemAll.IsSelected)
            {
                //刷新表格数据
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Grid.SetCaption(_model.SettingModel.TxtTestTimePointsArr);
                UIZhuiSuC10Grid.SetData(Batch, SensorDatas, F_AddTime);
            }

        }
    }

}
