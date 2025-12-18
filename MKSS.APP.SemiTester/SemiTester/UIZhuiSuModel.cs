using Stylet;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Net.NetworkInformation;
using MKSS.Services;
using MKSS.Model;
using System.Windows.Controls;
using MKSS.Util.Log;
using MKSS.APP.SemiTester;

namespace MKSS.APP.SemiTester
{


    [LogTagClass(Title = "检测主界面")]
	public partial class UIZhuiSuModel : Screen {


		public static int MaxBoardCount = 99;
		public static int READ_SPEED = 200;//200毫秒钟读一次数据

		public static UIZhuiSuModel Intance { get; private set; }


		public UIZhuiSu PageContext { get;   set; } 


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

		public static bool IsInDesignMode(Control control)
		{
			return System.ComponentModel.DesignerProperties.GetIsInDesignMode(control);
		}



	}

}
