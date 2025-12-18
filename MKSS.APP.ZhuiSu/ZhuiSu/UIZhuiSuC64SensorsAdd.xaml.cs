using MKSS.APP.ZhuiSu.Util;
using MKSS.Model;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace MKSS.APP.ZhuiSu
{
    /// <summary>
    /// UIZhuiSuC10SensorsAdd.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC64SensorsAdd : Window
    {

        TextBox FocusTextBox = null;
        List<TextBox> list = new List<TextBox>();
        public UIZhuiSuC64SensorsAdd()
        {
            InitializeComponent();
            for (int r = 1; r <= 8; r++)
            {
                for (int c = 1; c <= 8; c++)
                {
                    string n = string.Format("Cell{0}X{1}", r, c);
                    TextBox t1 = GetChildObject<TextBox>(AllText, n);
                    if (t1 != null) {
                        list.Add(t1);
                        if (r <= 4)
                        {
                            t1.ToolTip = string.Format("A{0}", ((r - 1) * 8 + c).ToString("00"));
                        }
                        else {
                            t1.ToolTip = string.Format("B{0}", ((r - 5) * 8 + c).ToString("00"));
                        }
                    }
                }
            }
            this.Cell1X1.Focus();
        }

        Batch Batch { get; set; }
        List<PageSensorModel> Sensors { get; set; }
        public void SetData(Batch b, List<PageSensorModel> list) {
            Batch = b; Sensors = list;
            try
            {

                if (this.Batch == null || this.Sensors == null || this.Sensors.Count == 0)
                {
                    throw new Exception("批次传感器为空。");
                }

                DataTable tab1 = UIZhuiSuData.Instance.SensorServices.BaseDal.QueryTable("SELECT F_SerialNO,F_SensorId FROM pd_sensor WHERE F_BatchId=" + Batch.F_BatchId + "").Result;
                Dictionary<string, string> dic1 = new Dictionary<string, string>();
                for (int i = 0; i < tab1.Rows.Count; i++)
                {
                    DataRow dr = tab1.Rows[i];
                    string F_SensorId = dr["F_SensorId"] + "";
                    dic1.Add(F_SensorId, dr["F_SerialNO"]==DBNull.Value?"": dr["F_SerialNO"].ToString());
                }

                for (int r = 1; r <= 8; r++)
                {
                    for (int c = 1; c <= 8; c++)
                    {
                        string n = string.Format("Cell{0}X{1}", r, c);
                        TextBox t1 = GetChildObject<TextBox>(AllText, n);
                        if (t1 != null)
                        {
                            int ndx = (r - 1) * 8 + c - 1;
                            if (list.Count > ndx) {
                                if(dic1.ContainsKey(list[ndx].SensorId)) t1.Text = dic1[list[ndx].SensorId];
                                t1.Tag = list[ndx];
                            }
                        }
                    }
                }

                foreach (PageSensorModel sen in this.Sensors)
                {
                    string id = sen.SensorId;
                }

            }
            catch (Exception ex)
            {

            }

        }

        private void Cell1PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) {
                int index_pre = -1;
                if(FocusTextBox!=null) index_pre = list.IndexOf(FocusTextBox);
                index_pre++;

                if (index_pre >= list.Count)
                {
                    this.BtnConfirm.Focus();
                }
                else {
                    list[index_pre].Focus();
                }
            }
        }
        private void Cell1X1_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox t = sender as TextBox;
            FocusTextBox = t;
        }

        /// <summary>
        /// 获取子控件
        /// </summary>
        /// <typeparam name="T">子控件类型</typeparam>
        /// <param name="obj">父控件</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            DependencyObject child = null;
            T grandChild = null;
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                child = VisualTreeHelper.GetChild(obj, i);


                if (child is T && (((T)child).Name == name && !string.IsNullOrEmpty(name)))
                {
                    return (T)child;
                }
                else
                {
                    grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }
            return null;
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in list)
            {
                item.Text = "";
            }
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (this.Batch == null || this.Sensors == null || this.Sensors.Count == 0)
                {
                    throw new Exception("批次传感器为空。");
                }

                StringBuilder sb = new StringBuilder();
                for (int r = 1; r <= 8; r++)
                {
                    for (int c = 1; c <= 8; c++)
                    {
                        string n = string.Format("Cell{0}X{1}", r, c);
                        TextBox t1 = GetChildObject<TextBox>(AllText, n);
                        if (t1 != null && t1.Tag!=null)
                        {
                            PageSensorModel sen = t1.Tag as PageSensorModel;
                            sb.Append(string.Format("update pd_sensor set F_SerialNO='{1}' where F_SensorId='{0}';", sen.SensorId, t1.Text));
                            UIZhuiSuC64Model.Instance.View.UIZhuiSuC64Grid.SetSerialNo(sen.SensorId, t1.Text);
                        }
                    }
                }
                int res = UIZhuiSuData.Instance.SensorServices.BaseDal.ExecuteCommand(sb.ToString()).Result;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

    }
}
