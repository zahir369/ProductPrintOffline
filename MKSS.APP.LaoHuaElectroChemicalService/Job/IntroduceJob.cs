using Quartz;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MKSS.APP.LaoHuaElectroChemicalService.Job
{
    /// <summary>
    ///  创建的Job要实现IJob
    /// </summary>
    [DisallowConcurrentExecution]
    public class IntroduceJob : IJob
    {
        private readonly ILogger<IntroduceJob> _logger;
        public IntroduceJob(ILogger<IntroduceJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("My name is yang");
            return Task.CompletedTask;
        }
    }

}
