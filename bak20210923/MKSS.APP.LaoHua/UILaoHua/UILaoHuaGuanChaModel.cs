using Stylet;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;
using MKSS.APP.LaoHua.Config;
using MKSS.APP.LaoHua.Util;
using MKSS.Services;
using MKSS.Model;
using System.Windows.Controls;
using MKSS.Util.Log;
using MKSS.APP.LaoHua;
using DeviceDataMonitorWPF;

namespace MKSS.APP.LaoHua
{


	[LogTagClass(Title = "老化主界面")]
	public partial class UILaoHuaGuanChaModel : Screen {


		public static int MaxBoardCount = 99;
		public static int READ_SPEED = 200;//200毫秒钟读一次数据

		public static UILaoHuaGuanChaModel Intance { get; private set; }

		public MainWindow MainView { get; set; }

		public UILaoHuaGuanCha PageContext { get;   set; }
		/// <summary>
		///   默认批次，存放开发模式数据
		/// </summary>
		public PageBoardTable BoardCaseTable { get; set; } 


		public UILaoHuaGuanChaModel()
		{

			Intance = this; 

		}
		 



		public void InitPage(UILaoHuaGuanCha page)
		{
			PageContext = page;
			PageContext.Loaded += PageContext_Loaded;
		}

		public new UIElement View
		{
			get
			{
				return this.PageContext
	;
			}
		}

		private void PageContext_Loaded(object sender, RoutedEventArgs e)
        {
			if (this.PageContext == null) return;
			 
			
			 
		}
		 
		      

	}

}
