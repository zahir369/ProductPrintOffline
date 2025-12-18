using System.Collections.Generic;

namespace MKSS.APP.ConfigTool
{
    /// <summary>
    ///  点表配置  
    /// </summary>
    public class AddrConfig { 
		/// <summary>
		///  名称（按钮名称，唯一）
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		///  排序号，不唯一
		/// </summary>
		public int Xh { get; set; }
		/// <summary>
		///  类别
		/// </summary>
		public string Type { get; set; }
		/// <summary>
		///  地址
		/// </summary>
		public ushort Address { get; set; }
		/// <summary>
		///  地址
		/// </summary>
		public bool ReadOnly { get; set; }
		/// <summary>
		///  地址
		/// </summary>
		public string ReadOnlyBtnText { get { return ReadOnly ? "只读" : "修改"; } }
		/// <summary>
		///  显示公式
		/// </summary>
		public string Fomula { get; set; }
		/// <summary>
		///  默认值
		/// </summary>
		public ushort DefaultValue { get; set; }
		/// <summary>
		///  显示单位
		/// </summary>
		public string DataUnit { get; set; }
		
		/// <summary>
		///  值域
		/// </summary>
		public string Range { get; set; }
		/// <summary>
		///  下拉框
		/// </summary>
		public AddrConfigItem[] DropdownList { get; set; }
		/// <summary>
		///  备注
		/// </summary>
		public string Memo { get; set; }
	}

}
