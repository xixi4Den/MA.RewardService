using MA.RewardService.Domain.Entities;

namespace MA.RewardService.Domain.Events;

public class MissionReachedEvent(IEnumerable<Reward> rewards)
{
    public IEnumerable<Reward> Rewards { get; } = rewards;
}