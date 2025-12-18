namespace DeviceDataMonitorWPF.UIBiaodingJiuJing.Config
{
    public class DataAgingProductConfig
    {
        public DataAgingProductConfig() { 
            TimeTotal = 3.0; 
            Name = "自定义";
        }
        public string Name { get; set; }
        public int VoltageValueBase { get; set; }
        public int VoltageValueAdd { get; set; }
        public int VoltageValueMinus { get; set; }
        public double TimeTotal { get; set; }
        
    }

}
