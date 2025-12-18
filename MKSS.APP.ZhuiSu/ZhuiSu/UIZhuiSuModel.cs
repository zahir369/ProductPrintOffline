using Stylet;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using System;
using MKSS.APP.ZhuiSu.Util;
using MKSS.Services;
using MKSS.Model;
using System.Windows.Controls;
using MKSS.Util.Log;
using MKSS.APP.ZhuiSu;

namespace MKSS.APP.ZhuiSu
{


	[LogTagClass(Title = "检测主界面")]
	public partial class UIZhuiSuModel : Screen {


		public static int MaxBoardCount = 10;
		public static int READ_SPEED = 200;//200毫秒钟读一次数据

		public static UIZhuiSuModel Intance { get; private set; }


		public UIZhuiSu PageContext { get;   set; }
		/// <summary>
		///   默认批次，存放开发模式数据
		/// </summary>
		public PageBoardTable BoardCaseTable { get; set; } 


		public UIZhuiSuModel()
		{
			Intance = this; 
		}

		public void InitPage(UIZhuiSu page)
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
