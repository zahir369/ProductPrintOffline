using System;
using System.ComponentModel;

namespace MKSS.Service.LaoHua.Util
{
    public class DataAgingSensorValue : INotifyPropertyChanged
    {
		public DateTime Time { get; set; }
		public int? Value { get; set; }
        public int? ValueDbl { get; set; }
        public ushort[] Data { get; set; }
        public ushort[] DataDbl { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public override string ToString()
        {
            return string.Format("{1}:{0}", Value, Time);
        }

    }

}
