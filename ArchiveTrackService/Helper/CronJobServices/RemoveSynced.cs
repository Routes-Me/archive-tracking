using ArchiveTrackService.Abstraction;
using ArchiveTrackService.Helper.CronJobServices.CronJobExtensionMethods;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ArchiveTrackService.Helper.CronJobServices
{
    public class RemoveSynced : CronJobService, IDisposable
    {
        private readonly IServiceScope _scope;
        public RemoveSynced(IScheduleConfig<RemoveSynced> config, IServiceProvider scope) : base(config.CronExpression, config.TimeZoneInfo)
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
                _coordinateData.DeleteCoordinates(string.Empty);
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
