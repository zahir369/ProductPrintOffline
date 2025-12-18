using System;
using System.Xml.Serialization;

namespace MKSS.APP.ConfigTool
{

	/// <summary>
	///  点表值
	/// </summary>
	public class AddrValue : AddrConfig {
		/// <summary>
		///  值
		/// </summary>
		public ushort Value { get; set; }
		/// <summary>
		///  值
		/// </summary>
		public DateTime RefreshTime { get; set; }
		/// <summary>
		///  值
		/// </summary>
		public string RefreshTimeStr { get { return RefreshTime.ToString("HH:mm:ss.fff"); } }
		/// <summary>
		///  值
		/// </summary>
		public string AddressStr { get { return "0X"+(Address.ToString("X")); } }
		/// <summary>
		///  值
		/// </summary>
		public string ReadOnlyStr { get { return ReadOnly ? "只读": "可写"; } }
		/// <summary>
		///  显示值
		/// </summary>
		public string ShowValue { 
			get {
				if (this.DropdownList != null && DropdownList.Length > 0) {
                    foreach (var item in DropdownList)
                    {
						if (item.Value == Value) return item.Name;
					}
					return "未知";
				}
				if (!string.IsNullOrEmpty(Fomula)) {
					FumulaExpression exp = new FumulaExpression(Fomula);
					return exp.Compute(Value).ToString();
				}
				return Value.ToString();
			} 
		}
		public override string ToString()
        {
            return string.Format("{0} {1} {2} {3} ",this.Name,this.Address,this.Value,this.RefreshTime);
        }
    }


}
