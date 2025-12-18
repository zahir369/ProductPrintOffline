using Quartz;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MKSS.Util.Log;
using System;

namespace MKSS.APP.LaoHuaElectroChemicalService.Job
{
    /// <summary>
    ///  创建的Job要实现IJob
    /// </summary>
    [DisallowConcurrentExecution]//禁止并发执行多个相同定义的
    [LogTagClass(Title = "定时读取")]
    public class LaoHuaTaskReadJob : IJob
    {

        private readonly ILogger<LaoHuaTaskReadJob> _logger;
        public LaoHuaTaskReadJob(ILogger<LaoHuaTaskReadJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            ULogger.Info(string.Format("刷新任务 定时读取 Start {0}...", DateTime.Now));
            //Program.LaohuaService.RefreshTask(); 
            ULogger.Info(string.Format("刷新任务 定时读取 End {0}...", DateTime.Now));
            return Task.CompletedTask;
        }

    }

}
