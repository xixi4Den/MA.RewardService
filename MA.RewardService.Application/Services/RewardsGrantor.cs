using MA.RewardService.Application.Abstractions;
using MA.RewardService.Contracts;
using MA.RewardService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MA.RewardService.Application.Services;

public class RewardsGrantor: IRewardsGrantor
{
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<RewardsGrantor> _logger;

    public RewardsGrantor(IEventPublisher eventPublisher,
        ILogger<RewardsGrantor> logger)
    {
        _eventPublisher = eventPublisher;
        _logger = logger;
    }
    
    public async Task GrantAsync(int userId, IEnumerable<Mission> missions, CancellationToken ct)
    {
        var totalRewardAmountByType = GetTotalRewardAmountByType(missions);

        foreach (var rewardType in totalRewardAmountByType.Keys)
        {
            var amount = totalRewardAmountByType[rewardType];
            await PublishRewardGrantedEvent(userId, rewardType, amount, ct);
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
    
    private async Task PublishRewardGrantedEvent(int userId, RewardType rewardType, int amount, CancellationToken ct)
    {
        switch (rewardType)
        {
            case RewardType.Spins:
                var spinsRewardedEvent = new SpinPointsRewardGrantedEvent{UserId = userId, Amount = amount};
                await _eventPublisher.PublishAsync(spinsRewardedEvent, ct);
                break;
            case RewardType.Coins:
                var coinsRewardedEvent = new CoinsRewardGrantedEvent{UserId = userId, Amount = amount};
                await _eventPublisher.PublishAsync(coinsRewardedEvent, ct);
                break;
            default:
                _logger.LogWarning("An event for reward type {Type} was not published because of missing mapping", rewardType);
                break;
        }
    }
}