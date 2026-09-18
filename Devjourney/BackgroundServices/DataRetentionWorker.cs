using DataAccessLayer.DataContexts;
using Microsoft.EntityFrameworkCore;

namespace Devjourney.BackgroundServices
{
    public class DataRetentionWorker : BackgroundService
    {
        private const int BatchSize = 100;
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

            // 1. Purge Posts in bounded batches (clears dependent author references first)
            try
            {
                int totalDeletedPosts = 0;
                while (!stoppingToken.IsCancellationRequested)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                    var posts = await dbContext.Posts
                        .IgnoreQueryFilters()
                        .Where(p => p.DeletedAt != null && p.DeletedAt <= thresholdDate)
                        .Take(BatchSize)
                        .ToListAsync(stoppingToken);

                    if (posts.Count == 0)
                        break;

                    dbContext.Posts.RemoveRange(posts);
                    await dbContext.SaveChangesAsync(stoppingToken);
                    totalDeletedPosts += posts.Count;

                    if (posts.Count < BatchSize)
                        break;
                }

                if (totalDeletedPosts > 0)
                {
                    _logger.LogInformation("Purged {Count} old soft-deleted posts.", totalDeletedPosts);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while purging soft-deleted posts.");
            }

            // 2. Purge Users with bounded batches and per-user FK isolation
            try
            {
                var skippedUserIds = new HashSet<Guid>();
                int totalDeletedUsers = 0;

                while (!stoppingToken.IsCancellationRequested)
                {
                    using var scope = _serviceProvider.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                    var candidateUserIds = await dbContext.Users
                        .IgnoreQueryFilters()
                        .Where(u => u.DeletedAt != null && u.DeletedAt <= thresholdDate && !skippedUserIds.Contains(u.Id))
                        .Select(u => u.Id)
                        .Take(BatchSize)
                        .ToListAsync(stoppingToken);

                    if (candidateUserIds.Count == 0)
                        break;

                    try
                    {
                        var batchDeleted = await dbContext.Users
                            .IgnoreQueryFilters()
                            .Where(u => candidateUserIds.Contains(u.Id))
                            .ExecuteDeleteAsync(stoppingToken);

                        totalDeletedUsers += batchDeleted;
                    }
                    catch (DbUpdateException ex)
                    {
                        _logger.LogWarning(ex, "Batch delete failed due to constraint violation; falling back to per-user deletion.");

                        foreach (var userId in candidateUserIds)
                        {
                            if (stoppingToken.IsCancellationRequested)
                                break;

                            try
                            {
                                var userDeleted = await dbContext.Users
                                    .IgnoreQueryFilters()
                                    .Where(u => u.Id == userId)
                                    .ExecuteDeleteAsync(stoppingToken);

                                if (userDeleted > 0)
                                {
                                    totalDeletedUsers += userDeleted;
                                }
                                else
                                {
                                    skippedUserIds.Add(userId);
                                }
                            }
                            catch (DbUpdateException userEx)
                            {
                                _logger.LogWarning(userEx, "Failed to purge soft-deleted user {UserId} due to foreign key or constraint violation. Skipping.", userId);
                                skippedUserIds.Add(userId);
                            }
                        }
                    }

                    if (candidateUserIds.Count < BatchSize)
                        break;
                }

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
