using Quartz;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MKSS.APP.LaoHuaElectroChemicalService.Job
{
    /// <summary>
    ///  创建的Job要实现IJob
    /// </summary>
    [DisallowConcurrentExecution]
    public class HelloWorldJob : IJob
    {
        private readonly ILogger<HelloWorldJob> _logger;
        public HelloWorldJob(ILogger<HelloWorldJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Hello world!");
            return Task.CompletedTask;
        }
    }

}
