using Quartz;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MKSS.Util.Log;
using MKSS.APP.LaoHuaService.MQTT;

namespace MKSS.APP.LaoHuaElectroChemicalService.Job
{

    /// <summary>
    ///  创建的Job要实现IJob 删除不用了
    /// </summary>
    [DisallowConcurrentExecution]//禁止并发执行多个相同定义的
    [LogTagClass(Title = "定时刷新")]
    public class LaoHuaTaskRefreshJob : IJob
    {

        private readonly ILogger<LaoHuaTaskRefreshJob> _logger;
        public LaoHuaTaskRefreshJob(ILogger<LaoHuaTaskRefreshJob> logger)
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
            Program.LaohuaService.RefreshTask();
            ULogger.Info("定时刷新 End...");
            return Task.CompletedTask;
        }



    }

}
