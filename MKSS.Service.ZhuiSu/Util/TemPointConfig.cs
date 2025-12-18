namespace MKSS.Service.ZhuiSu.Util
{
    public class TemPointConfig
	{
		public uint ExecuteIndex { get; set; }

		public double TargetTemperature { get; set; }

		public double Duration { get; set; }

		public uint TemperaturePointIndex { get; set; }

		public TemPointConfig(double targetTemperature, double duration, uint temperaturePointIndex)
		{
			this.TargetTemperature = targetTemperature;
			this.Duration = duration;
			this.TemperaturePointIndex = temperaturePointIndex;
		}
	}
}
