using MKSS.APP.ECTester.UserCommon;
using MKSS.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MKSS.APP.ECTester.UserControls
{
    /// <summary>
    /// UserButton.xaml 的交互逻辑
    /// </summary>
    public partial class UserButton : UserControl//, INotifyPropertyChanged
    {
        // public event PropertyChangedEventHandler PropertyChanged;
        //  private string buttongImagePath = "/Resources/Images/tongji.png";
        //public string ButtonImagePath
        //{
        //    get { return buttongImagePath; }//获取值时将私有字段传出；
        //    set
        //    {
        //        buttongImagePath = value;//赋值时将值传给私有字段
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("ButtonImagePath"));//一旦执行了赋值操作说明其值被修改了，则立马通过INotifyPropertyChanged接口告诉UI值被修改了
        //    }
        //}

        #region 按钮图片 ButtonImagePath

        /// <summary>
        /// 按钮图片
        /// </summary>
        public string ButtonImagePath
        {
            get { return (string)GetValue(ButtonImagePathProperty); }
            set { SetValue(ButtonImagePathProperty, value); }
        }
        public static readonly DependencyProperty ButtonImagePathProperty = DependencyProperty.Register("ButtonImagePath",
          typeof(string), typeof(UserButton), new PropertyMetadata("", new PropertyChangedCallback(OnButtonImagePathPropertyChange)));
        static void OnButtonImagePathPropertyChange(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            UserButton selectorBtn = (UserButton)sender;
        }
        #endregion

        #region 按钮名称 ButtonText 

        /// <summary>
        /// 按钮名称
        /// </summary>
        public string ButtonText
        {
            get { return (string)GetValue(ButtonTextProperty); }
            set { SetValue(ButtonTextProperty, value); }
        }

        public static readonly DependencyProperty ButtonTextProperty = DependencyProperty.Register
            (
            "ButtonText",
            typeof(string),
            typeof(UserButton),
            new PropertyMetadata("", new PropertyChangedCallback(OnButtonTextPropertyChange))
            );

        static void OnButtonTextPropertyChange(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            UserButton selectorBtn = (UserButton)sender;
            selectorBtn.txtButtonName.Text = args.NewValue.ToString();
        }

        #endregion

        #region 菜单按钮绑定的数据 MenuData   

        public static readonly DependencyProperty MenuDataProperty = DependencyProperty.Register
         (
         "MenuData",
         typeof(MenuData),
         typeof(UserButton)
         );
        public MenuData MenuData
        {
            get { return (MenuData)GetValue(MenuDataProperty); }
            set { SetValue(MenuDataProperty, value); }
        }
        #endregion

        #region 打开菜单命令 OpenMenuCommand

        public static readonly DependencyProperty OpenMenuCommandProperty = DependencyProperty.Register
        (
        "OpenMenuCommand",
        typeof(ICommand),
        typeof(UserButton)
        );

        public ICommand OpenMenuCommand
        {
            get { return (ICommand)GetValue(OpenMenuCommandProperty); }
            set { SetValue(OpenMenuCommandProperty, value); }
        }
        #endregion

        #region 命令输入参数 CommandParameter

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register
          (
          "CommandParameter",
          typeof(object),
          typeof(UserButton)
          );

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        #endregion


        public UserButton()
        {
            this.DataContext = this;

            InitializeComponent();
        }
    }
}
