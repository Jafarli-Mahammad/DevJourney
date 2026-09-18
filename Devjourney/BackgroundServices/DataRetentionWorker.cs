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
            var thresholdDate = DateTime.UtcNow.AddDays(-30);

            // Purge Posts
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                var totalDeletedPosts = await dbContext.Posts
                    .IgnoreQueryFilters()
                    .Where(p => p.DeletedAt != null && p.DeletedAt <= thresholdDate)
                    .ExecuteDeleteAsync(stoppingToken);

                if (totalDeletedPosts > 0)
                {
                    _logger.LogInformation("Purged {Count} old soft-deleted posts.", totalDeletedPosts);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while purging soft-deleted posts.");
            }

            // Purge Users
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                var totalDeletedUsers = await dbContext.Users
                    .IgnoreQueryFilters()
                    .Where(u => u.DeletedAt != null && u.DeletedAt <= thresholdDate)
                    .ExecuteDeleteAsync(stoppingToken);

                if (totalDeletedUsers > 0)
                {
                    _logger.LogInformation("Purged {Count} old soft-deleted users.", totalDeletedUsers);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while purging soft-deleted users.");
            }
            
            // Add more entities as necessary
        }
    }
}
