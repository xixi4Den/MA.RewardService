using MA.RewardService.Application.Abstractions;
using MA.RewardService.Contracts;
using MA.RewardService.Domain.Abstractions;
using MA.RewardService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MA.RewardService.Application.Services;

public class RewardsGrantor(
    IGrantedRewardRepository repository,
    IEventPublisher eventPublisher,
    ILogger<RewardsGrantor> logger)
    : IRewardsGrantor
{
    public async Task GrantAsync(int userId, IEnumerable<Mission> missions, CancellationToken ct)
    {
        var totalRewardAmountByType = GetTotalRewardAmountByType(missions);

        foreach (var rewardType in totalRewardAmountByType.Keys)
        {
            var amount = totalRewardAmountByType[rewardType];
            var reward = new RewardGranted
            {
                RewardId = Guid.NewGuid(),
                Name = rewardType,
                Value = amount,
            };
            await repository.AddAsync(userId, reward);
            await PublishRewardGrantedEvent(userId, reward, ct);
        }
    }

    private static Dictionary<RewardType, int> GetTotalRewardAmountByType(IEnumerable<Mission> missions)
    {
        var result = missions
            .SelectMany(x => x.Rewards)
            .ToLookup(x => x.Name)
            .ToDictionary(x => x.Key, x => x.Sum(r => r.Value));
        
        return result;
    }
    
    private async Task PublishRewardGrantedEvent(int userId, RewardGranted reward, CancellationToken ct)
    {
        switch (reward.Name)
        {
            case RewardType.Spins:
                var spinsRewardedEvent = new SpinPointsRewardGrantedEvent{RewardId = reward.RewardId, UserId = userId, Amount = reward.Value};
                await eventPublisher.PublishAsync(spinsRewardedEvent, ct);
                break;
            case RewardType.Coins:
                var coinsRewardedEvent = new CoinsRewardGrantedEvent{RewardId = reward.RewardId, UserId = userId, Amount = reward.Value};
                await eventPublisher.PublishAsync(coinsRewardedEvent, ct);
                break;
            default:
                logger.LogWarning("An event for reward type {Type} was not published because of missing mapping", reward.Name);
                break;
        }
    }
}