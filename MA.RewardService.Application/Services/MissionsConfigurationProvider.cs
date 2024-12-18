using MA.RewardService.Domain.Abstractions;
using MA.RewardService.Domain.Entities;

namespace MA.RewardService.Application.Services;

public class TestMissionsConfigurationProvider : IMissionsConfigurationProvider
{
    public Task<MissionsConfiguration> GetAsync()
    {
        return Task.FromResult(new MissionsConfiguration
        {
            Missions =
            [
                new Mission
                {
                    PointsGoal = 10,
                    Rewards =
                    [
                        new Reward
                        {
                            Name = RewardName.Spins,
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
                            Name = RewardName.Coins,
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
                            Name = RewardName.Coins,
                            Value = 100,
                        },
                        new Reward
                        {
                            Name = RewardName.Spins,
                            Value = 100,
                        }
                    ]
                }
            ],
            RepeatedIndex = 1,
        });
    }
}