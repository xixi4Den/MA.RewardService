using MA.RewardService.Application.Abstractions;
using MA.RewardService.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MA.RewardService.Application.Feature.HandleMissionProgress;

public class HandleMissionProgressCommandHandler: IRequestHandler<HandleMissionProgressCommand>
{
    private readonly IPointsCalculator _pointsCalculator;
    private readonly IMissionProgressProcessor _missionProgressProcessor;
    private readonly IMissionsConfigurationProvider _missionsConfigurationProvider;
    private readonly IMissionProgressRepository _missionProgressRepository;
    private readonly IRewardsGrantor _rewardsGrantor;
    private readonly ILogger<HandleMissionProgressCommandHandler> _logger;

    public HandleMissionProgressCommandHandler(IPointsCalculator pointsCalculator,
        IMissionProgressProcessor missionProgressProcessor,
        IMissionsConfigurationProvider missionsConfigurationProvider,
        IMissionProgressRepository missionProgressRepository,
        IRewardsGrantor rewardsGrantor,
        ILogger<HandleMissionProgressCommandHandler> logger)
    {
        _pointsCalculator = pointsCalculator;
        _missionProgressProcessor = missionProgressProcessor;
        _missionsConfigurationProvider = missionsConfigurationProvider;
        _missionProgressRepository = missionProgressRepository;
        _rewardsGrantor = rewardsGrantor;
        _logger = logger;
    }
    
    public async Task Handle(HandleMissionProgressCommand request, CancellationToken ct)
    {
        var newPoints = _pointsCalculator.Calculate(request.SpinResult);
        if (newPoints == 0)
        {
            _logger.LogDebug("No points scored by user {UserId}. Skipping mission progress handling", request.UserId);
            
            return;
        }
        
        _logger.LogDebug("{Score} points scored by {UserId}", newPoints, request.UserId);
        
        var currentProgress = await _missionProgressRepository.GetAsync(request.UserId);
        var missionsConfig = await _missionsConfigurationProvider.GetAsync();
        var processingResult = _missionProgressProcessor.Process(request.UserId, currentProgress, newPoints, missionsConfig);
        
        await _missionProgressRepository.UpdateAsync(request.UserId, currentProgress, processingResult.NewProgress);
        
        _logger.LogDebug($"Mission progress updated successfully for user {request.UserId}");
        if (processingResult.AchievedMissions.Any())
            await _rewardsGrantor.GrantAsync(request.UserId, processingResult.AchievedMissions, ct);
    }
}