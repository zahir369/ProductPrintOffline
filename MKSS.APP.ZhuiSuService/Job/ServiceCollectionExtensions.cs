using Microsoft.Extensions.DependencyInjection;

namespace MKSS.WindwsService.DeviceMonitor.Job
{
    /// <summary>
    ///  创建ServiceCollectionExtensions注入schedule
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static void AddJob(this IServiceCollection services)
        {

            services.AddSingleton(new JobSchedule(
               jobType: typeof(ZhuiSuTaskRefreshJob),
               cronExpression: "0/5 * * * * ?")); // run every 5 minutes

            //services.AddSingleton(new JobSchedule(
            //    jobType: typeof(ZhuiSuTaskReadJob), cronExpression: "*/10 * * * * ?"));// run every 1 minutes

            //services.AddSingleton(new JobSchedule(
            //    jobType: typeof(ZhuiSuTaskReadJob), cronExpression: "0/3 * * * * ?"));// run every 1 minutes

        }
    }

}
