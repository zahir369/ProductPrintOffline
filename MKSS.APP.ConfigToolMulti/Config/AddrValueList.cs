using System.Collections.Generic;

namespace MKSS.APP.ConfigToolMulti
{
    public class AddrValueList : List<AddrValue> {
		public int DeviceAddr { get; set; }
		public AddrValueList(int addrs) {
			DeviceAddr = addrs;
		}
	}

}
