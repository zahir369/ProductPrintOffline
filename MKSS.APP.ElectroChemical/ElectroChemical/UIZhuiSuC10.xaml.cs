using MKSS.APP.ElectroChemical;
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

namespace MKSS.APP.ElectroChemical
{

    /// <summary>
    ///  UIZhuiSuC10.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10 : UserControl
    {

        public UIZhuiSuC10Model _model = null;
        DispatcherTimer timer = new DispatcherTimer();
        public UIZhuiSuC10()
        {
            InitializeComponent();
            _model = base.DataContext as UIZhuiSuC10Model;
            _model.InitPage(this);

            timer.Interval = new TimeSpan(0, 0,1);//设置的间隔为5s
            timer.Tick += Timer_Tick;
            timer.IsEnabled = true;
            timer.Start();

        }

        public Batch SelectBatch { get; set; }
        public bool TimerRefreshIng { get { return false; } }
        private void Timer_Tick(object sender, EventArgs e)
        {

            DateTime d = DateTime.Now;
            TimeNow.Content = d.ToString("yyyy-MM-dd HH:mm:ss")+(TimerRefreshIng?"...":"");

            if (d.Second % 5 == 0) {
                Batch batch = SelectBatch;
                if (batch != null && batch.EnumAgingStatus == EnumAgingStatus.InAging)
                {
                   
                }
            }

        }


        public Batch Batch { get; set; }
        public TimeSpan F_AddTime { get; set; }
        public SensorGroupData SensorDatas { get; set; }
        public void SetData(Model.Batch _batch, SensorGroupData data, TimeSpan f_AddTime)
        {
            Batch = _batch; 
            SensorDatas = data;
            F_AddTime = f_AddTime;
            UIZhuiSuC10Model.Instance.SettingModel.CurrentTimeSpan = F_AddTime;
            UIZhuiSuC10Model.Instance.SettingModel.CalcHeGe();
            UIZhuiSuC10Grid.SetData(_batch, data,   F_AddTime);
            UIZhuiSuC10Sensors.SetData(_batch, data);
            UIZhuiSuC10Chart.SetPageBoardItem(_batch, data);
            UIZhuiSuC10Setting.SetData(_batch, data, F_AddTime);
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.TabItemAll.IsSelected)
            {
                //刷新表格数据
                UIZhuiSuC10Model.Instance.View.UIZhuiSuC10Grid.SetCaption(_model.SettingModel.TxtTestTimePointsArr);
                UIZhuiSuC10Grid.SetData(Batch, SensorDatas, F_AddTime);
            }

        }
    }

}
