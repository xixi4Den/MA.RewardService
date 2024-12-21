using FluentAssertions;
using MA.RewardService.Domain.Entities;
using MA.RewardService.Domain.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace MA.RewardService.Domain.Tests;

[TestClass]
public class MissionProgressProcessorTests
{
    private MissionProgressProcessor _subject;
    private MissionsConfiguration _config;
    private Mission _mission1, _mission2, _mission3;

    private const int UserId = 111;

    [TestInitialize]
    public void Init()
    {
        _subject = new MissionProgressProcessor(NullLogger<MissionProgressProcessor>.Instance);

        _mission1 = new Mission
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
        };
        _mission2 = new Mission
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
        };
        _mission3 = new Mission
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
        };
        _config = new MissionsConfiguration([_mission1, _mission2, _mission3], 1);
    }

    [TestMethod]
    public void NoNewPoints_ShouldReturnCurrentProgress()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(UserId, currentProgress, 0, _config);

        result.NewProgress.Should().BeEquivalentTo(currentProgress);
    }
    
    [TestMethod]
    public void MissionNotReached_ShouldIncrementPoints()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(UserId, currentProgress, 15, _config);

        result.NewProgress.TotalPoints.Should().Be(65);
        result.NewProgress.RemainingPoints.Should().Be(35);
    }
    
    [TestMethod]
    public void MissionNotReached_ShouldNotChangeMissionIndex()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(UserId, currentProgress, 15, _config);

        result.NewProgress.MissionIndex.Should().Be(currentProgress.MissionIndex);
    }
    
    [TestMethod]
    public void MissionNotReached_ShouldReturnNoAchievedMissions()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(UserId, currentProgress, 15, _config);

        result.AchievedMissions.Should().BeEmpty();
    }
    
    [TestMethod]
    public void MissionReached_ShouldIncrementTotalPoints()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(UserId, currentProgress, 80, _config);

        result.NewProgress.TotalPoints.Should().Be(130);
    }
    
    [TestMethod]
    public void MissionReached_AllRemainingPointsUsed_ShouldSetRemainingPointsToZero()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(UserId, currentProgress, 80, _config);

        result.NewProgress.RemainingPoints.Should().Be(0);
    }
    
    [TestMethod]
    public void MissionReached_UnusedRemainingPointsLeft_ShouldProperlyUpdateRemainingPoints()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(UserId, currentProgress, 87, _config);

        result.NewProgress.RemainingPoints.Should().Be(7);
    }
    
    [TestMethod]
    public void MissionReached_MissionIsNotLast_ShouldIncrementMissionIndex()
    {
        var currentProgress = MissionProgress.Create(2, 19, 9);
        
        var result = _subject.Process(UserId, currentProgress, 16, _config);

        result.NewProgress.MissionIndex.Should().Be(3);
    }
    
    [DataTestMethod]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    public void MissionReached_MissionIsLast_ShouldResetToConfiguredIndex(int resetIndex)
    {
        _config = new MissionsConfiguration([_mission1, _mission2, _mission3], resetIndex);
        var currentProgress = MissionProgress.Create(3, 50, 20);
        
        var result = _subject.Process(UserId, currentProgress, 87, _config);

        result.NewProgress.MissionIndex.Should().Be(resetIndex);
    }

    [TestMethod]
    public void MissionReached_ShouldReturnAchievedMission()
    {
        var currentProgress = MissionProgress.Create(3, 50, 20);

        var result = _subject.Process(UserId, currentProgress, 87, _config);

        result.AchievedMissions.Should().BeEquivalentTo([_mission3]);
    }
    
    [TestMethod]
    public void MultipleMissionsReached_ShouldReturnAllAchievedMissions()
    {
        var currentProgress = MissionProgress.Create(1, 0, 0);

        var result = _subject.Process(UserId, currentProgress, 31, _config);

        result.AchievedMissions.Should().BeEquivalentTo([_mission1, _mission2]);
    }
    
    [TestMethod]
    public void MultipleMissionsReached_ShouldReturnProperProgress()
    {
        var currentProgress = MissionProgress.Create(1, 0, 0);

        var result = _subject.Process(UserId, currentProgress, 31, _config);

        result.NewProgress.Should().BeEquivalentTo(MissionProgress.Create(3, 31, 1));
    }
}