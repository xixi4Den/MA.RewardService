using FluentAssertions;
using MA.RewardService.Api.Contracts;
using MA.RewardService.Contracts;
using MA.RewardService.Infrastructure.Messaging.Consumers;
using MA.RewardService.IntegrationTests.Extensions;
using MA.RewardService.IntegrationTests.Framework;
using MA.SlotService.Contracts;
using Moq;

namespace MA.RewardService.IntegrationTests.Scenarios;

[TestClass]
public class MissionsProgressTests
{
    private static IntegrationTestingWebAppFactory _webAppFactory;
    private HttpClient _httpClient;
    
    private const int UserId = 10;

    [ClassInitialize]
    public static void GlobalInit(TestContext context)
    {
        _webAppFactory = new IntegrationTestingWebAppFactory();
    }
    
    [TestInitialize]
    public async Task Init()
    {
        _httpClient = _webAppFactory.CreateDefaultClient();
    }
    
    [TestCleanup]
    public void Cleanup() => _httpClient.Dispose();

    [ClassCleanup(ClassCleanupBehavior.EndOfClass)]
    public static void GlobalCleanup() => _webAppFactory.Dispose();
    
    [TestMethod]
    public async Task UnknownPlayer_ShouldReturnEmptyProgress()
    {
        var result = await _httpClient.GetProgress(999);

        result.Should().BeEquivalentTo(new MissionProgressResponse
        {
            MissionIndex = 1,
            TotalPoints = 0,
            RemainingPoints = 0
        });
    }
    
    [TestMethod]
    public async Task ReceivesSpinEventWithNonWinningCombination_ShouldNotUpdateProgress()
    {
        var initialProgress = await _httpClient.GetProgress(UserId);

        var @event = new SpinProcessedEvent
        {
            SpinId = Guid.NewGuid(),
            Result = [1,2,3],
            UserId = UserId
        };
        await _webAppFactory.RunServiceBusConsumer<SpinProcessedEventConsumer, SpinProcessedEvent>(@event);
        
        var nextProgress = await _httpClient.GetProgress(UserId);
        nextProgress.Should().BeEquivalentTo(initialProgress);
    }
    
    [TestMethod]
    public async Task ReceivesSpinEventWithWinningCombination_ShouldUpdateProgress()
    {
        var initialProgress = await _httpClient.GetProgress(UserId);

        var @event = new SpinProcessedEvent
        {
            SpinId = Guid.NewGuid(),
            Result = [9,9,9],
            UserId = UserId
        };
        await _webAppFactory.RunServiceBusConsumer<SpinProcessedEventConsumer, SpinProcessedEvent>(@event);
        
        var nextProgress = await _httpClient.GetProgress(UserId);
        nextProgress.Should().NotBeEquivalentTo(initialProgress);
    }
    
    [TestMethod]
    public async Task ReceivesDuplicateSpinEvent_ShouldSkip()
    {
        var initialProgress = await _httpClient.GetProgress(UserId);

        var @event = new SpinProcessedEvent
        {
            SpinId = Guid.NewGuid(),
            Result = [9,9,9],
            UserId = UserId
        };
        await _webAppFactory.RunServiceBusConsumer<SpinProcessedEventConsumer, SpinProcessedEvent>(@event);
        
        var nextProgress = await _httpClient.GetProgress(UserId);
        nextProgress.Should().NotBeEquivalentTo(initialProgress);
        
        await _webAppFactory.RunServiceBusConsumer<SpinProcessedEventConsumer, SpinProcessedEvent>(@event);
        var finalProgress = await _httpClient.GetProgress(UserId);
        finalProgress.Should().BeEquivalentTo(nextProgress);
    }
    
    [TestMethod]
    public async Task MissionsAchievementFlow_ShouldProperlyGrantRewards()
    {
        var userId = 20;
        var initialProgress = await _httpClient.GetProgress(userId);

        // points scored, but the first mission not achieved yet
        var @event = new SpinProcessedEvent
        {
            SpinId = Guid.NewGuid(),
            Result = [3,3,3],
            UserId = userId
        };
        await _webAppFactory.RunServiceBusConsumer<SpinProcessedEventConsumer, SpinProcessedEvent>(@event);
        var nextProgress = await _httpClient.GetProgress(userId);
        nextProgress.Should().BeEquivalentTo(new MissionProgressResponse
        {
            TotalPoints = 9,
            RemainingPoints = 9,
            MissionIndex = 1,
        });
        ResetEventPublisherMock();
        _webAppFactory.EventDispatcherMock.Verify(x => x.PublishAsync(It.IsAny<It.IsAnyType>(), It.IsAny<CancellationToken>()), Times.Never);
        
        // the first mission achieved with 10 Spins as reward
        @event = new SpinProcessedEvent
        {
            SpinId = Guid.NewGuid(),
            Result = [1,1,1],
            UserId = userId
        };
        await _webAppFactory.RunServiceBusConsumer<SpinProcessedEventConsumer, SpinProcessedEvent>(@event);
        nextProgress = await _httpClient.GetProgress(userId);
        nextProgress.Should().BeEquivalentTo(new MissionProgressResponse
        {
            TotalPoints = 12,
            RemainingPoints = 2,
            MissionIndex = 2,
        });
        ValidateSpinsRewardEventPublished(10);
        ResetEventPublisherMock();

        // the second and the third missions achieved at once with 20 Spins and 10 Coins as reward
        @event = new SpinProcessedEvent
        {
            SpinId = Guid.NewGuid(),
            Result = [5,5,5],
            UserId = userId
        };
        await _webAppFactory.RunServiceBusConsumer<SpinProcessedEventConsumer, SpinProcessedEvent>(@event);
        nextProgress = await _httpClient.GetProgress(userId);
        nextProgress.Should().BeEquivalentTo(new MissionProgressResponse
        {
            TotalPoints = 27,
            RemainingPoints = 2,
            MissionIndex = 4,
        });
        ValidateSpinsRewardEventPublished(20);
        ValidateCoinsRewardEventPublished(10);
        ResetEventPublisherMock();
        
        // the last mission achieved with 100 Spins and 50 Coins as reward, next target mission rest to 1
        @event = new SpinProcessedEvent
        {
            SpinId = Guid.NewGuid(),
            Result = [7,7,7],
            UserId = userId
        };
        await _webAppFactory.RunServiceBusConsumer<SpinProcessedEventConsumer, SpinProcessedEvent>(@event);
        nextProgress = await _httpClient.GetProgress(userId);
        nextProgress.Should().BeEquivalentTo(new MissionProgressResponse
        {
            TotalPoints = 48,
            RemainingPoints = 3,
            MissionIndex = 1,
        });
        ValidateSpinsRewardEventPublished(100);
        ValidateCoinsRewardEventPublished(50);
        ResetEventPublisherMock();
    }

    private static void ResetEventPublisherMock()
    {
        _webAppFactory.EventDispatcherMock.Invocations.Clear();
    }
    
    private static void ValidateSpinsRewardEventPublished(int amount)
    {
        _webAppFactory.EventDispatcherMock.Verify(x =>
            x.PublishAsync(It.Is<SpinPointsRewardGrantedEvent>(e => e.Amount == amount), It.IsAny<CancellationToken>()), Times.Once);
    }
    
    private static void ValidateCoinsRewardEventPublished(int amount)
    {
        _webAppFactory.EventDispatcherMock.Verify(x =>
            x.PublishAsync(It.Is<CoinsRewardGrantedEvent>(e => e.Amount == amount), It.IsAny<CancellationToken>()), Times.Once);
    }
}