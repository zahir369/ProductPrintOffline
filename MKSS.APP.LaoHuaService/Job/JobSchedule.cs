using System;

namespace MKSS.WindwsService.DeviceMonitor.Job
{
    /// <summary>
    ///  配置Job
    /// </summary>
    public class JobSchedule
    {
        public JobSchedule(Type jobType, string cronExpression)
        {
            JobType = jobType;
            CronExpression = cronExpression;
        }

        public Type JobType { get; }
        public string CronExpression { get; }
    }

}
