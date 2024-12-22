using System.Text.Json;
using MA.RewardService.Domain.Abstractions;
using MA.RewardService.Domain.Entities;
using StackExchange.Redis;

namespace MA.RewardService.Infrastructure.DataAccess.Repositories;

public class GrantedRewardRepository(IConnectionMultiplexer redis): IGrantedRewardRepository
{
    public async Task AddAsync(int userId, RewardGranted reward)
    {
        var db = redis.GetDatabase();
        var key = GetKey(userId);
        
        await db.ListRightPushAsync(key, JsonSerializer.Serialize(reward));
    }

    private string GetKey(int userId) => $"granted_rewards:{userId}";
}