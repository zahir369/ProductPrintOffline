using Quartz;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MKSS.Util.Log;
using System;

namespace MKSS.WindwsService.DeviceMonitor.Job
{
    /// <summary>
    ///  创建的Job要实现IJob
    /// </summary>
    [DisallowConcurrentExecution]//禁止并发执行多个相同定义的
    [LogTagClass(Title = "定时读取")]
    public class ZhuiSuTaskReadJob : IJob
    {

        private readonly ILogger<ZhuiSuTaskReadJob> _logger;
        public ZhuiSuTaskReadJob(ILogger<ZhuiSuTaskReadJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            ULogger.Info(string.Format("刷新任务 Start {0}...",DateTime.Now));
            Program.ZhuiSuService.RefreshTask();
            ULogger.Info("定时读取 Start ...");
            Program.ZhuiSuService.ReadVoltageRecycle().Wait();
            ULogger.Info(string.Format("定时读取 End {0}...", DateTime.Now));
            return Task.CompletedTask;
        }

    }

}
