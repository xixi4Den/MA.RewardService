using MA.RewardService.Application.Abstractions;
using MA.RewardService.Application.Services;
using MA.RewardService.Contracts;
using MA.RewardService.Domain.Abstractions;
using MA.RewardService.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MA.RewardService.Application.Tests;

[TestClass]
public class RewardsGrantorTests
{
    private RewardsGrantor _subject;
    private Mock<IEventPublisher> _eventPublisherMock;
    
    private const int UserId = 999;

    [TestInitialize]
    public void Init()
    {
        _eventPublisherMock = new Mock<IEventPublisher>();
        var rewardRepoMock = new Mock<IGrantedRewardRepository>();
        _subject = new RewardsGrantor(rewardRepoMock.Object, _eventPublisherMock.Object, NullLogger<RewardsGrantor>.Instance);
    }
    
    [TestMethod]
    public async Task SingleMissionWithSingleSpinsRewardCompleted_ShouldPublishSingleSpinsEvent()
    {
        Mission[] missions =
        [
            new()
            {
                Rewards =
                [
                    new Reward
                    {
                        Name = RewardType.Spins,
                        Value = 10
                    }
                ]
            }
        ];
        
        await _subject.GrantAsync(UserId, missions, CancellationToken.None);
        
        _eventPublisherMock.Verify(x => x.PublishAsync(It.Is<SpinPointsRewardGrantedEvent>(e => e.Amount == 10), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [TestMethod]
    public async Task SingleMissionWithSingleCoinsRewardCompleted_ShouldPublishSingleCoinsEvent()
    {
        Mission[] missions =
        [
            new()
            {
                Rewards =
                [
                    new Reward
                    {
                        Name = RewardType.Coins,
                        Value = 20
                    }
                ]
            }
        ];
        
        await _subject.GrantAsync(UserId, missions, CancellationToken.None);
        
        _eventPublisherMock.Verify(x => x.PublishAsync(It.Is<CoinsRewardGrantedEvent>(e => e.Amount == 20), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [TestMethod]
    public async Task SingleMissionWithMultipleDifferentRewardsCompleted_ShouldPublishEventPerRewardType()
    {
        Mission[] missions =
        [
            new()
            {
                Rewards =
                [
                    new Reward
                    {
                        Name = RewardType.Spins,
                        Value = 10
                    },
                    new Reward
                    {
                        Name = RewardType.Coins,
                        Value = 20
                    }
                ]
            }
        ];
        
        await _subject.GrantAsync(UserId, missions, CancellationToken.None);
        
        _eventPublisherMock.Verify(x => x.PublishAsync(It.Is<SpinPointsRewardGrantedEvent>(e => e.Amount == 10), It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisherMock.Verify(x => x.PublishAsync(It.Is<CoinsRewardGrantedEvent>(e => e.Amount == 20), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [TestMethod]
    public async Task MultipleMissionsWithDifferentRewardsCompleted_ShouldPublishEventPerRewardTypeWithSumAmount()
    {
        Mission[] missions =
        [
            new()
            {
                Rewards =
                [
                    new Reward
                    {
                        Name = RewardType.Spins,
                        Value = 10
                    },
                    new Reward
                    {
                        Name = RewardType.Coins,
                        Value = 20
                    }
                ]
            },
            new()
            {
                Rewards =
                [
                    new Reward
                    {
                        Name = RewardType.Spins,
                        Value = 100
                    }
                ]
            }
        ];
        
        await _subject.GrantAsync(UserId, missions, CancellationToken.None);
        
        _eventPublisherMock.Verify(x => x.PublishAsync(It.Is<SpinPointsRewardGrantedEvent>(e => e.Amount == 110), It.IsAny<CancellationToken>()), Times.Once);
        _eventPublisherMock.Verify(x => x.PublishAsync(It.Is<CoinsRewardGrantedEvent>(e => e.Amount == 20), It.IsAny<CancellationToken>()), Times.Once);
    }
}