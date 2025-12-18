using Microsoft.Extensions.DependencyInjection;

namespace MKSS.APP.LaoHuaElectroChemicalService.Job
{
    /// <summary>
    ///  创建ServiceCollectionExtensions注入schedule
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static void AddJob(this IServiceCollection services)
        {
            //services.AddSingleton(new JobSchedule(
            //   jobType: typeof(LaoHuaTaskRefreshJob),
            //   cronExpression: "0 0/5 * * * ?")); // run every 5 minutes

            //services.AddSingleton(new JobSchedule(
            //    jobType: typeof(LaoHuaTaskReadJob), cronExpression: "*/10 * * * * ?"));// run every 1 minutes


            //*高频刷新低频存储 
            services.AddSingleton(new JobSchedule(
                jobType: typeof(LaoHuaTaskReadJob), cronExpression: "0/20 * * * * ?"));// run every 1 minutes

        }
    }

}
