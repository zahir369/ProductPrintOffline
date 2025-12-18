using System;
using System.ComponentModel;

namespace MKSS.APP.LaoHua.Util
{
    public class PageSensorValue : INotifyPropertyChanged
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
