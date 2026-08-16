using DataAccessLayer.DataContexts;
using Microsoft.EntityFrameworkCore;

namespace Devjourney.BackgroundServices
{
    public class DataRetentionWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DataRetentionWorker> _logger;

        public DataRetentionWorker(IServiceProvider serviceProvider, ILogger<DataRetentionWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DataRetentionWorker is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PurgeOldSoftDeletedRecordsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing DataRetentionWorker.");
                }

                // Run once a day
                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private async Task PurgeOldSoftDeletedRecordsAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            var thresholdDate = DateTime.UtcNow.AddDays(-30);

            // Using ExecuteDeleteAsync for efficient bulk deletion on SQL Server
            var deletedPostsCount = await dbContext.Posts
                .IgnoreQueryFilters()
                .Where(p => p.DeletedAt != null && p.DeletedAt <= thresholdDate)
                .ExecuteDeleteAsync(stoppingToken);

            if (deletedPostsCount > 0)
            {
                _logger.LogInformation("Purged {Count} old soft-deleted posts.", deletedPostsCount);
            }

            var deletedUsersCount = await dbContext.Users
                .IgnoreQueryFilters()
                .Where(u => u.DeletedAt != null && u.DeletedAt <= thresholdDate)
                .ExecuteDeleteAsync(stoppingToken);

            if (deletedUsersCount > 0)
            {
                _logger.LogInformation("Purged {Count} old soft-deleted users.", deletedUsersCount);
            }
            
            // Add more entities as necessary
        }
    }
}
