using MA.RewardService.Domain.Abstractions;
using MA.RewardService.Domain.Entities;
using MA.RewardService.Infrastructure.DataAccess.Exceptions;
using StackExchange.Redis;

namespace MA.RewardService.Infrastructure.DataAccess.Repositories;

public class MissionProgressRepository(IConnectionMultiplexer redis): IMissionProgressRepository
{
    private static class Fields
    {
        internal const string MissionIndex = "mission_index";
        internal const string TotalPoints = "total_points";
        internal const string RemainingPoints = "remaining_points";
    }

    public async Task<MissionProgress> GetAsync(int userId)
    {
        var key = GetKey(userId);
        var db = redis.GetDatabase();
        
        var fields = new RedisValue[] { Fields.MissionIndex, Fields.TotalPoints, Fields.RemainingPoints };
        
        var values = await db.HashGetAsync(key, fields);

        if (values.All(v => v.IsNull))
            return MissionProgress.Empty();

        return MissionProgress.Create((int) values[0], (int) values[1], (int) values[2]);
    }
    
    public async Task UpdateAsync(int userId, MissionProgress currentProgress, MissionProgress newProgress)
    {
        var key = GetKey(userId);
        var db = redis.GetDatabase();

        var transaction = db.CreateTransaction();
        transaction.AddCondition(Condition.HashEqual(key, Fields.TotalPoints, currentProgress.IsEmpty ? RedisValue.Null : currentProgress.TotalPoints));

        var updates = new[]
        {
            new HashEntry(Fields.MissionIndex, newProgress.MissionIndex),
            new HashEntry(Fields.TotalPoints, newProgress.TotalPoints),
            new HashEntry(Fields.RemainingPoints, newProgress.RemainingPoints)
        };

        _ = transaction.HashSetAsync(key, updates);

        var result = await transaction.ExecuteAsync();
        if (!result)
            throw new ConcurrentUpdateException();
    }
    
    private string GetKey(int userId) => $"user:{userId}:progress";
}