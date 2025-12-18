
using MKSS.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MKSS.Services
{
    public class MenuService
    {

        public static List<MenuData> SysMenus = new List<MenuData>();

        public static event EventHandler MenuClickHandler;

        public static event EventHandler MenuChildClickHandler;

        static MenuService()
        {
            SysMenus = InitMenuData();
        }

        /// <summary>
        /// 初始化系统菜单数据
        /// </summary>
        /// <returns></returns>
        public static List<MenuData> InitMenuData()
        {
            const string UIBiaoDing = "DeviceDataMonitorWPF.UIBiaoDing.";
            const string UILaoHua = "DeviceDataMonitorWPF.UILaoHua.";
            const string UIZhanHui = "DeviceDataMonitorWPF.UIZhanHui.";

            List<MenuData> lstMenu = new List<MenuData>()
            {
                 new MenuData()
                {
                     MenuName="环境监测",
                     ImagePath="huanjing.png",
                     MenuDataChilds=new List<MenuData>()   {

                           new MenuData()
                         {
                            MenuName="国控数据",
                            BindControlName=UIZhanHui+"UCGuoKong"
                         }
                           ,
                           new MenuData()
                         {
                            MenuName="会议现场",
                            BindControlName=UIZhanHui+"UCHuiChang"
                         }

                     }
                },
                new MenuData()
                {
                     MenuName="检测标定",
                     ImagePath="biaoding.png",
                     MenuDataChilds=new List<MenuData>()   {

                           new MenuData()
                             {
                                MenuName="检测观察",
                                BindControlName=UILaoHua+"UILaoHuaGuanCha"
                             }
                               ,
                           new MenuData()
                         {
                            MenuName="设备标定",
                            BindControlName=UIBiaoDing+"UISheBeiBiaoDing"
                         }

                     }
                },
                new MenuData()
                {
                     MenuName="设备管理",
                     ImagePath="shebei.png",
                      MenuDataChilds=new List<MenuData>()
                     {
                         //new MenuData()
                         //{
                         //   MenuName="检测架管理",
                         //   BindControlName=UFPath+"frmTJHuanZheYongYao"
                         //},
                         new MenuData()
                         {
                            MenuName="通信链路管理",
                            BindControlName=UILaoHua+"frmSZJianChaXiang"
                         },
                           new MenuData()
                         {
                            MenuName="标定箱管理",
                            BindControlName=UILaoHua+"frmTJHuanZheYongYao"
                         },
                           new MenuData()
                         {
                            MenuName="托盘管理",
                            BindControlName=UILaoHua+"frmTJJianChaXiangZhanBi"
                         }
                           ,
                           new MenuData()
                         {
                            MenuName="传感器管理",
                            BindControlName=UILaoHua+"frmTJJianChaXiangZhanBi"
                         }

                     }
                },

                 new MenuData()
                {
                     MenuName="数据统计",
                     ImagePath="tongji.png",
                     MenuDataChilds=new List<MenuData>()
                     {
                         new MenuData()
                         {
                            MenuName="标定数量统计",
                            BindControlName=UILaoHua+"UCUserReg"
                         },
                          new MenuData()
                         {
                            MenuName="标定时长统计",
                            BindControlName=UILaoHua+"frmDJYongYaoGuanCha",
                            BindControlText="标定时长统计"
                         }
                     }
                },
                new MenuData()
                {
                     MenuName="系统设置",
                     ImagePath="shezhi.png",
                     MenuDataChilds=new List<MenuData>()
                     {
                         //new MenuData()
                         //{
                         //   MenuName="通信链路设置",
                         //   BindControlName=UFPath+"frmSZJianChaXiang"
                         //},
                          new MenuData()
                         {
                            MenuName="数据字典管理",
                            BindControlName=UILaoHua+"frmSZYongYaoMingCheng"
                         }
                     }
                }
            };

            return lstMenu;
        }

    }
}
