using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Common.Background
{
    public readonly struct BackgroundJob
    {
        public string JobType { get; init; }
        public Guid EntityId { get; init; }
        public string? Payload { get; init; }
    }

    public interface IBackgroundTaskQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(BackgroundJob workItem);
        ValueTask<BackgroundJob> DequeueAsync(CancellationToken cancellationToken);
    }

    public class DefaultBackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<BackgroundJob> _queue;

        public DefaultBackgroundTaskQueue(int capacity = 100)
        {
            var options = new BoundedChannelOptions(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<BackgroundJob>(options);
        }

        public async ValueTask QueueBackgroundWorkItemAsync(BackgroundJob workItem)
        {
            await _queue.Writer.WriteAsync(workItem);
        }

        public async ValueTask<BackgroundJob> DequeueAsync(CancellationToken cancellationToken)
        {
            return await _queue.Reader.ReadAsync(cancellationToken);
        }
    }

    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger<QueuedHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, ILogger<QueuedHostedService> logger, IServiceProvider serviceProvider)
        {
            TaskQueue = taskQueue;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is running.");
            await BackgroundProcessing(stoppingToken);
        }

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    // Here you can use MediatR or a specific handler strategy
                    // var mediator = scope.ServiceProvider.GetRequiredService<MediatR.IMediator>();
                    
                    _logger.LogInformation("Processed background job: {JobType} for Entity: {EntityId}", workItem.JobType, workItem.EntityId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing a background work item.");
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");
            await base.StopAsync(stoppingToken);
        }
    }
}
