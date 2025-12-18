namespace WpfChartTest
{
    public class SensorDataItem { 
        public double F_AddTime { get; set; }
        public double F_LoadDataValue { get; set; }
        public PosEnum PosEnum { get; set; }
        public bool F_DataValueEmp { get { return F_LoadDataValue == 0; } }
    }

    public class Batch { 
    
    }
}
