using MA.RewardService.Domain.Abstractions;
using StackExchange.Redis;

namespace MA.RewardService.Infrastructure.DataAccess.Repositories;

public class SpinsLogRepository(IConnectionMultiplexer redis): ISpinsLogRepository
{
    private const string ProcessedSpinsSetKey = "spins_log";
    private static readonly TimeSpan ProcessedSpinsTtl = TimeSpan.FromHours(24);

    public async Task AppendAsync(Guid spinId)
    {
        var db = redis.GetDatabase();
        
        var expirationTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + ProcessedSpinsTtl.TotalMilliseconds;
        await db.SortedSetAddAsync(ProcessedSpinsSetKey, spinId.ToString(), expirationTimestamp);
    }
    
    public async Task<bool> ContainsAsync(Guid spinId)
    {
        var db = redis.GetDatabase();
        
        var score = await db.SortedSetScoreAsync(ProcessedSpinsSetKey, spinId.ToString());
        await RemoveOutdatedSpins(db);

        return score.HasValue;
    }

    private static async Task RemoveOutdatedSpins(IDatabase db)
    {
        var currentTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await db.SortedSetRemoveRangeByScoreAsync(ProcessedSpinsSetKey, start: double.NegativeInfinity,
            stop: currentTimestamp);
    }
}