using MKSS.APP.ZhuiSu;
using MKSS.APP.ZhuiSu.Util;
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

namespace MKSS.APP.ZhuiSu
{

    /// <summary>
    ///  UIZhuiSuC64.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC64 : UserControl
    {

        UIZhuiSuC64Model _model = null;
        DispatcherTimer timer = new DispatcherTimer();
        public UIZhuiSuC64()
        {
            InitializeComponent();
            _model = base.DataContext as UIZhuiSuC64Model;
            _model.InitPage(this);

            timer.Interval = new TimeSpan(0, 0,1);//设置的间隔为5s
            timer.Tick += Timer_Tick;
            timer.IsEnabled = true;
            timer.Start();

        }
        public Batch SelectBatch { get; set; }
        public bool TimerRefreshIng { get { return _model.BoardCaseTable.status == sta_job_status.statistcing; } }
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
        public void SetData(List<PageBoardItem> data, Batch _batch)
        {

            Batch = _batch;
            int selectIndex = gridPager.SelectedIndex;
            if (selectIndex < 0) selectIndex = 0;
            int app = 1;
            gridPager.Items.Clear();
            List<UIZhuiSuPage> pagedatas = new List<UIZhuiSuPage>();
            for (int i = 0; i < data.Count; i = i + 2)
            {
                int from = i;
                int to = data.Count - 1 >= i + app ? i + app : data.Count - 1;
                UIZhuiSuPage d = new UIZhuiSuPage() { };
                d.From = data[from];
                d.To = data[to];
                d.Data = data.GetRange(from, to - from).ToList();
                pagedatas.Add(d);
                gridPager.Items.Add(d);
            }
            if (selectIndex >= gridPager.Items.Count) selectIndex = 0;
            if (gridPager.Items.Count > 0)
            {
                gridPager.SelectedIndex = selectIndex;
                //gridPager.SelectedItem = gridPager.Items[selectIndex];
            }

        }


        private void gridPager_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UIZhuiSuPage page = gridPager.SelectedItem as UIZhuiSuPage;
            if (page == null) return;
            UIZhuiSuC64Grid.SetData(page.From, page.To, Batch);
            this.UIZhuiSuC64Setting1.SetData(page.From, page.To, Batch);
        }

        
        
    }

}
