using System;
using System.ComponentModel;

namespace MKSS.Service.ZhuiSu.Util
{
    public class DataAgingSensorValue : INotifyPropertyChanged
    {
		public DateTime Time { get; set; }
		public int? Value { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public override string ToString()
        {
            return string.Format("{1}:{0}", Value, Time);
        }

    }

}
