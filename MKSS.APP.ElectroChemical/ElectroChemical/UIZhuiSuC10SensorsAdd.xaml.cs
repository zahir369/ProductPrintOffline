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

namespace MKSS.APP.ElectroChemical
{
    /// <summary>
    /// UIZhuiSuC10SensorsAdd.xaml 的交互逻辑
    /// </summary>
    public partial class UIZhuiSuC10SensorsAdd : Window
    {

        TextBox FocusTextBox = null;
        List<TextBox> list = new List<TextBox>();
        public UIZhuiSuC10SensorsAdd()
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
                    }
                }
            }
            this.Cell1X1.Focus();
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

        }
    }
}
