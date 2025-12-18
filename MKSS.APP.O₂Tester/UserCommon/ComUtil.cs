using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MKSS.APP.O2Tester.UserCommon
{
    public class ComUtil
    {
        /// <summary>
        /// 查找tabitem索引。找不到返回-1
        /// </summary>
        /// <param name="tb"></param>
        /// <param name="itemName"></param>
        /// <returns></returns>
        public static int FindItemIndex(TabControl tb, string itemName)
        {
            int result = -1;
            bool isHave = false;
            foreach (TabItem item in tb.Items)
            {
                result++;
                if (item.Header.ToString().Equals(itemName))
                {
                    isHave = true;
                    break;
                }
            }

            return isHave ? result : -1;
        }

        public static UserControl LoadUserControl(string controlUri)
        {
            try
            {
                //var uc = (UserControl)Application.LoadComponent(
                //    new Uri(
                //    $"MKSS.APP.O2Tester.UserControls.UCZhanHui;{controlUri}",
                //    System.UriKind.RelativeOrAbsolute)
                //         );
                //return uc;


                string ctrName = controlUri; //"WpfApplication1.UserControl1"; //WpfApplication1为要添加的Usercontrol的名称空间，UserControl1为要添加的Usercontrol的类名。

                Type t = Type.GetType(ctrName);

                var result = (UserControl)Activator.CreateInstance(t);

                return result;
            }
            catch
            {
                return null;
            }
        }

    }
}
