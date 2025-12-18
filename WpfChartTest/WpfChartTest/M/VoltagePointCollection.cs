namespace WpfChartTest
{
    public class VoltagePointCollection : RingArray<SensorDataItem>
    {
        private const int TOTAL_POINTS = 120 * 60 * 5;
        public VoltagePointCollection()
            : base(TOTAL_POINTS) // here i set how much values to show 
        {
        }
    }

}
