using MA.RewardService.Application.Abstractions;
using MA.RewardService.Application.Feature.HandleMissionProgress;
using MA.RewardService.Domain.Abstractions;
using MA.RewardService.Domain.Entities;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace MA.RewardService.Application.Tests;

[TestClass]
public class HandleMissionProgressCommandHandlerTests
{
    private HandleMissionProgressCommandHandler _subject;
    private HandleMissionProgressCommand _command = new(666, [7,7,7]);
    
    private Mock<IPointsCalculator> _pointsCalculatorMock;
    private Mock<IMissionProgressProcessor> _missionProgressProcessorMock;
    private Mock<IMissionsConfigurationProvider> _missionConfigProviderMock;
    private Mock<IMissionProgressRepository> _missionProgressRepositoryMock;
    private Mock<IRewardsGrantor> _rewardsGrantorMock;

    [TestInitialize]
    public void Init()
    {
        _pointsCalculatorMock = new Mock<IPointsCalculator>();
        _missionProgressProcessorMock = new Mock<IMissionProgressProcessor>();
        _missionConfigProviderMock = new Mock<IMissionsConfigurationProvider>();
        _missionProgressRepositoryMock = new Mock<IMissionProgressRepository>();
        _rewardsGrantorMock = new Mock<IRewardsGrantor>();
        _subject = new HandleMissionProgressCommandHandler(_pointsCalculatorMock.Object,
            _missionProgressProcessorMock.Object,
            _missionConfigProviderMock.Object,
            _missionProgressRepositoryMock.Object,
            _rewardsGrantorMock.Object,
            new NullLogger<HandleMissionProgressCommandHandler>());

        SetupScoredPoints(10);
        SetupMissionProgressRead(MissionProgress.Empty());
    }

    [TestMethod]
    public async Task NoPointsScored_ShouldNotHandleMissionProgress()
    {
        SetupScoredPoints(0);

        await _subject.Handle(_command, CancellationToken.None);
        
        _missionProgressProcessorMock.Verify(x => x.Process(It.IsAny<MissionProgress>(), It.IsAny<int>(), It.IsAny<MissionsConfiguration>()), Times.Never);
    }
    
    [TestMethod]
    public async Task NoMissionsCompleted_ShouldNotGrantRewards()
    {
        SetupMissionProgressProcessing(new MissionProgressProcessingResult
        {
            NewProgress = MissionProgress.Create(1, 6, 6),
        });
        
        await _subject.Handle(_command, CancellationToken.None);
        
        _rewardsGrantorMock.Verify(x => x.GrantAsync(It.IsAny<int>(), It.IsAny<IEnumerable<Mission>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [TestMethod]
    public async Task SomeMissionsCompleted_ShouldGrantRewards()
    {
        SetupMissionProgressProcessing(new MissionProgressProcessingResult
        {
            NewProgress = MissionProgress.Create(1, 6, 6),
            AchievedMissions = [
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
            ]
        });
        
        await _subject.Handle(_command, CancellationToken.None);
        
        _rewardsGrantorMock.Verify(x => x.GrantAsync(It.IsAny<int>(), It.IsAny<IEnumerable<Mission>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private void SetupScoredPoints(int points)
    {
        _pointsCalculatorMock.Setup(x => x.Calculate(It.IsAny<int[]>()))
            .Returns(points);
    }
    
    private void SetupMissionProgressRead(MissionProgress result)
    {
        _missionProgressRepositoryMock.Setup(x => x.GetAsync(It.IsAny<int>()))
            .ReturnsAsync(result);
    }
    
    private void SetupMissionProgressProcessing(MissionProgressProcessingResult result)
    {
        _missionProgressProcessorMock.Setup(x =>
                x.Process(It.IsAny<MissionProgress>(), It.IsAny<int>(), It.IsAny<MissionsConfiguration>()))
            .Returns(result);
    }
}