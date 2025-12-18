using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MKSS.Util.Log;
using Quartz;
using Quartz.Spi;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MKSS.WindwsService.DeviceMonitor
{
    [LogTagClass(Title="任务调度")]
    public class Worker : BackgroundService
    {

        private readonly ILogger<Worker> _logger;
        public Worker(ILogger<Worker> logger)
        {
            ULogger.Info("实例入口...");
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ULogger.Info("执行...");
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
            ULogger.Info("执行结束...");
        }

    }

}
