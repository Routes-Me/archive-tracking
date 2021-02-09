using ArchiveTrackService.Abstraction;
using ArchiveTrackService.Helper.CronJobServices.CronJobExtensionMethods;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ArchiveTrackService.Helper.CronJobServices
{
    public class SyncAnalytics : CronJobService, IDisposable
    {
        private readonly IServiceScope _scope;
        public SyncAnalytics(IScheduleConfig<SyncAnalytics> config, IServiceProvider scope) : base(config.CronExpression, config.TimeZoneInfo)
        {
            _scope = scope.CreateScope();
        }
        public override Task StartAsync(CancellationToken cancellationToken)
        {
            return base.StartAsync(cancellationToken);
        }
        public override Task DoWork(CancellationToken cancellationToken)
        {
            ICoordinateRepository _coordinateData = _scope.ServiceProvider.GetRequiredService<ICoordinateRepository>();
            try
            {
                _coordinateData.SyncOperationLogs();
            }
            catch (Exception) { }
            return Task.CompletedTask;
        }
        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
        public override void Dispose()
        {
            _scope?.Dispose();
        }
    }
}
