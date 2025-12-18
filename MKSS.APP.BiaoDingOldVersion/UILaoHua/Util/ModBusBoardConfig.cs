using DeviceDataMonitorWPF.UIBiaoDing.Util;
using System.Collections.Generic;
using System;
using System.ComponentModel;

namespace DeviceDataMonitorWPF.UIBiaodingJiuJing
{
    public class ModBusBoardConfig : INotifyPropertyChanged
	{
        public DateTime Start { get; internal set; }
        public string IP { get; private set; }
		public int PORT { get; private set; }
		public ModBusBoard ModBusBoard { get; private set; }
		public ModBusBoardTable SerialModBusPool { get; private set; }

		public bool SerialComIsOpen { get { return ModBusBoard != null && ModBusBoard.IsOpen; } }
		public event PropertyChangedEventHandler PropertyChanged;

		private ModBusBoardConfig(string _IP, int _PORT, int case_no, int tunnel_no_addr, ModBusBoardTable _SerialModBusPool) {
			this.IP = _IP;
			this.PORT = _PORT;
			SerialModBusPool = _SerialModBusPool;
			this.ModBusBoard = SerialModBusPool[case_no, tunnel_no_addr];
		}
		static Dictionary<string,ModBusBoardConfig> Cache = new Dictionary<string, ModBusBoardConfig>();
		public static ModBusBoardConfig Instance(string _IP, int _PORT, int case_no, int tunnel_no_addr, ModBusBoardTable _SerialModBusPool) {
			string key = string.Format("{0}_{1}", case_no, tunnel_no_addr);
			if (Cache.ContainsKey(key)) return Cache[key];
			ModBusBoardConfig ret = new ModBusBoardConfig( _IP,  _PORT, case_no, tunnel_no_addr, _SerialModBusPool);
			Cache.Add(key,ret);
			return ret;
		}
		public static ModBusBoardConfig Of(int case_no, int tunnel_no_addr)
		{
			string key = string.Format("{0}_{1}", case_no, tunnel_no_addr);
			if (Cache.ContainsKey(key)) return Cache[key];
			return null;
		}

		public override string ToString()
		{
			return string.Format("{0}_{1}_{2}", this.IP, this.PORT, this.SerialComIsOpen);
		}

	}

}
