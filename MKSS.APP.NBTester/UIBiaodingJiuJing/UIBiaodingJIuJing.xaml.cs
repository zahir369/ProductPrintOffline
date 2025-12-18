using MKSS.APP.UIBiaodingJiuJing;
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

namespace MKSS.APP.NBTester.UIBiaodingJiuJing
{
     
    /// </summary>
    public partial class UIBiaodingJiuJing : UserControl
    {

        List<UIConnection> list = new List<UIConnection>();
        public UIBiaodingJiuJing()
        {
            InitializeComponent();
            for (int r = 1; r <= 4; r++)
            { 
                for (int c = 0; c < 4; c++)
                {
                    string n = string.Format("X{0}{1}", r, c);
                    UIConnection t1 = GetChildObject<UIConnection>(this.MainGrid, n);
                    if (t1 != null)
                    {
                        list.Add(t1);
                    }
                }
            }
        }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            foreach (var item in list)
            {
                item.Port = null;
            }

            int ii = 0;
            foreach (var item in SerialPort.GetPortNames())
            {
                //list[ii].Port = item;
                //list[ii].Start();
                ii++;
            }
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
          
    }
}