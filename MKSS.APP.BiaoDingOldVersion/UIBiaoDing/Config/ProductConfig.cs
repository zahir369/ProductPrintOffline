namespace DeviceDataMonitorWPF.UIBiaoDing.Config
{
    public class ProductConfig
    {
        public ProductConfig() {
            ValueBase = 1000; ValueAdd = 5; ValueMinus = 5;
            TimeTotal = 600;
            TimeInterval = 1000;
            Name = "自定义";
        }
        public long Code { get; set; }
        public string Name { get; set; }
        public int Zero { get; set; }
        public int Span { get; set; }
        public int ValueBase { get; set; }
        public int ValueAdd { get; set; }
        public int ValueMinus { get; set; }
        public int VoltageValueBase { get; set; }
        public int VoltageValueAdd { get; set; }
        public int VoltageValueMinus { get; set; }
        public int TimeTotal { get; set; }
        public int TimeInterval { get; set; }
        
    }

}
