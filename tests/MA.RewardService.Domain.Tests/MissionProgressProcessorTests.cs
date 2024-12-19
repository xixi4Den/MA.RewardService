using FluentAssertions;
using MA.RewardService.Domain.Entities;
using MA.RewardService.Domain.Events;
using MA.RewardService.Domain.Services;

namespace MA.RewardService.Domain.Tests;

[TestClass]
public class MissionProgressProcessorTests
{
    private MissionProgressProcessor _subject;
    private MissionsConfiguration _config;

    [TestInitialize]
    public void Init()
    {
        _subject = new MissionProgressProcessor();

        _config = new MissionsConfiguration
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
        };
    }

    [TestMethod]
    public void NoNewPoints_ShouldReturnCurrentProgress()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(currentProgress, 0, _config);

        result.NewProgress.Should().BeEquivalentTo(currentProgress);
    }
    
    [TestMethod]
    public void TargetMissionNotReached_ShouldIncrementPoints()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(currentProgress, 15, _config);

        result.NewProgress.TotalPoints.Should().Be(65);
        result.NewProgress.RemainingPoints.Should().Be(35);
    }
    
    [TestMethod]
    public void TargetMissionNotReached_ShouldNotChangeMissionIndex()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(currentProgress, 15, _config);

        result.NewProgress.MissionIndex.Should().Be(currentProgress.MissionIndex);
    }
    
    [TestMethod]
    public void TargetMissionNotReached_ShouldReturnNoEvents()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(currentProgress, 15, _config);

        result.Events.Should().BeEmpty();
    }
    
    [TestMethod]
    public void TargetMissionReached_ShouldIncrementTotalPoints()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(currentProgress, 80, _config);

        result.NewProgress.TotalPoints.Should().Be(130);
    }
    
    [TestMethod]
    public void TargetMissionReached_AllRemainingPointsUsed_ShouldSetRemainingPointsToZero()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(currentProgress, 80, _config);

        result.NewProgress.RemainingPoints.Should().Be(0);
    }
    
    [TestMethod]
    public void TargetMissionReached_UnusedRemainingPointsLeft_ShouldProperlyUpdateRemainingPoints()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(currentProgress, 87, _config);

        result.NewProgress.RemainingPoints.Should().Be(7);
    }
    
    [TestMethod]
    public void TargetMissionReached_TargetMissionIsNotLast_ShouldIncrementMissionIndex()
    {
        var currentProgress = MissionProgress.Create(2, 19, 9);
        
        var result = _subject.Process(currentProgress, 16, _config);

        result.NewProgress.MissionIndex.Should().Be(3);
    }
    
    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void TargetMissionReached_TargetMissionIsLast_ShouldResetToConfiguredIndex(int resetIndex)
    {
        _config.RepeatedIndex = resetIndex;
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(currentProgress, 87, _config);

        result.NewProgress.MissionIndex.Should().Be(resetIndex);
    }

    [TestMethod]
    public void TargetMissionReached_ShouldReturnMissionReachedEvent()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);

        var result = _subject.Process(currentProgress, 87, _config);

        result.Events.Should().BeEquivalentTo([
            new MissionReachedEvent([
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
            ])
        ]);
    }
}