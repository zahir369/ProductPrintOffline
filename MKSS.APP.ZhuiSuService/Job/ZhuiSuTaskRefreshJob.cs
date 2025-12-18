using Quartz;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MKSS.Util.Log;
using MKSS.APP.ZhuiSuService.MQTT;

namespace MKSS.WindwsService.DeviceMonitor.Job
{

    /// <summary>
    ///  创建的Job要实现IJob 删除不用了
    /// </summary>
    [DisallowConcurrentExecution]//禁止并发执行多个相同定义的
    [LogTagClass(Title = "定时刷新")]
    public class ZhuiSuTaskRefreshJob : IJob
    {

        private readonly ILogger<ZhuiSuTaskRefreshJob> _logger;
        public ZhuiSuTaskRefreshJob(ILogger<ZhuiSuTaskRefreshJob> logger)
        {
            _logger = logger;
            ULogger.OnLog += ULogger_OnLog;
        }

        private void ULogger_OnLog(object messsage)
        {
            _logger.LogInformation(messsage.ToString());
        }

        public Task Execute(IJobExecutionContext context)
        {
            ULogger.Info("定时刷新 Start...");
            Program.ZhuiSuService.RefreshTask();
            ULogger.Info("定时刷新 End...");
            return Task.CompletedTask;
        }



    }

}
