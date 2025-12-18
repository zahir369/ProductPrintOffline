using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
 

namespace MKSS.Model.ViewModel

{
    /// <summary>
    /// 系统菜单数据
    /// </summary>
    public class MenuData
    {
        private string menuId = "";
        /// <summary>
        /// 菜单编号(唯一)
        /// </summary>
        public string MenuId
        {
            get
            {
                if (string.IsNullOrEmpty(menuId.Trim()))
                {
                    menuId = Guid.NewGuid().ToString();
                }
                return menuId;
            }
            set
            {
                menuId = value;
            }
        }
        
        /// <summary>
        /// 菜单父编号
        /// </summary>
        public string ParentId { get; set; }
        
        /// <summary>
        /// 菜单名称
        /// </summary>
        public string MenuName { get; set; } = "";

        /// <summary>
        /// 菜单描述
        /// </summary>
        public string MenuContent { get; set; } = "";      

        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImagePath { get; set; } = "";

        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; } = true;

        /// <summary>
        /// 是否接受管理员控制操作
        /// </summary>
        public bool IsAllowControl { get; set; } = false;

        ///// <summary>
        ///// 点击事件
        ///// </summary>
        //public EventHandler ClickHandler { get; set; }

        ///// <summary>
        ///// 显示指向的用户控件
        ///// </summary>
        //public UserControl BindUserControl { get; set; } = null;

        /// <summary>
        /// 控件的完全限定名称
        /// </summary>
        public string BindControlName { get; set; } = "";

        ///// <summary>
        ///// 控件显示名称
        ///// </summary>
        public string BindControlText { get; set; } = "";

        ///// <summary>
        ///// 用户控件的完全限定名称
        ///// </summary>
        //public string BindUserControlName { get; set; } = "";

        ///// <summary>
        ///// 用户控件显示名称
        ///// </summary>
        //public string BindUserControlText { get; set; } = "";

        ///// <summary>
        ///// 显示指向的窗体
        ///// </summary>
        //public Form BindForm { get; set; } = null;

        ///// <summary>
        ///// 窗体的完全限定名称
        ///// </summary>
        //public string BindFormName { get; set; } = "";

        /// <summary>
        /// 子菜单数据
        /// </summary>
        public List<MenuData> MenuDataChilds { get; set; } = null;

    }
}
