using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MKSS.APP.UserControls
{
    /// <summary>
    /// WaitWindow.xaml 的交互逻辑
    /// </summary>
    public partial class WaitWindow : Window
    {
        static WaitWindow Instance = null;
        public WaitWindow()
        {
            InitializeComponent();
        } 
        public static void ShowWindow(string title, string desc, FrameworkElement target) {
            target.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                if (Instance == null)
                {
                    Instance = new WaitWindow();
                    Instance.Topmost = true;
                    Instance.InfoTitle.Text = title;
                    Instance.InfoDesc.Text = desc;
                    Instance.Show(); 
                    DispatcherHelper.DoEvents();
                }
                else
                {
                    Instance.InfoTitle.Text = title;
                    Instance.InfoDesc.Text = desc;
                    DispatcherHelper.DoEvents();
                }
            });
            
        }

        public static void CloseWindow(FrameworkElement target)
        {
            target.Dispatcher.Invoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                if (Instance != null)
                {
                    Instance.Close();
                    Instance = null;
                }
            });
            
        }

    }
    public static class DispatcherHelper
    {
        [SecurityPermissionAttribute(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public static void DoEvents()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(ExitFrames), frame);
            try { Dispatcher.PushFrame(frame); }
            catch (InvalidOperationException) { }
        }
        private static object ExitFrames(object frame)
        {
            ((DispatcherFrame)frame).Continue = false;
            return null;
        }
    }
}
