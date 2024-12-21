using MA.RewardService.Domain.Abstractions;
using MA.RewardService.Domain.Entities;

namespace MA.RewardService.Application.Services;

public class MissionsConfigurationHardcodedProvider : IMissionsConfigurationProvider
{
    public Task<MissionsConfiguration> GetAsync()
    {
        return Task.FromResult(new MissionsConfiguration([
            new Mission
            {
                PointsGoal = 10,
                Rewards =
                [
                    new Reward
                    {
                        Name = RewardType.Spins,
                        Value = 10,
                    }
                ]
            },
            new Mission
            {
                PointsGoal = 20,
                Rewards =
                [
                    new Reward
                    {
                        Name = RewardType.Coins,
                        Value = 10,
                    }
                ]
            },
            new Mission
            {
                PointsGoal = 100,
                Rewards =
                [
                    new Reward
                    {
                        Name = RewardType.Coins,
                        Value = 100,
                    },
                    new Reward
                    {
                        Name = RewardType.Spins,
                        Value = 100,
                    }
                ]
            }
        ], 1));
    }
}