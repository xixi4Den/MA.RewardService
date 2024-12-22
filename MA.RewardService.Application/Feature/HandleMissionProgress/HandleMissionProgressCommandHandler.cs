using MA.RewardService.Application.Abstractions;
using MA.RewardService.Domain.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace MA.RewardService.Application.Feature.HandleMissionProgress;

public class HandleMissionProgressCommandHandler(
    IPointsCalculator pointsCalculator,
    IMissionProgressProcessor missionProgressProcessor,
    IMissionsConfigurationProvider missionsConfigurationProvider,
    IMissionProgressRepository missionProgressRepository,
    IRewardsGrantor rewardsGrantor,
    ISpinsLogRepository spinsLogRepository,
    ILogger<HandleMissionProgressCommandHandler> logger)
    : IRequestHandler<HandleMissionProgressCommand>
{
    public async Task Handle(HandleMissionProgressCommand request, CancellationToken ct)
    {
        if (await spinsLogRepository.ContainsAsync(request.SpindId))
        {
            logger.LogWarning("Spin {SpinId} already processed. Skip processing", request.SpindId);
            
            return;
        }
        
        var newPoints = pointsCalculator.Calculate(request.SpinResult);
        if (newPoints == 0)
        {
            logger.LogDebug("No points scored by user {UserId}. Skipping mission progress handling", request.UserId);
            
            return;
        }
        
        logger.LogDebug("{Score} points scored by {UserId}", newPoints, request.UserId);
        
        var currentProgress = await missionProgressRepository.GetAsync(request.UserId);
        var missionsConfig = await missionsConfigurationProvider.GetAsync();
        var processingResult = missionProgressProcessor.Process(request.UserId, currentProgress, newPoints, missionsConfig);
        
        await missionProgressRepository.UpdateAsync(request.UserId, currentProgress, processingResult.NewProgress);
        
        logger.LogDebug($"Mission progress updated successfully for user {request.UserId}");
        if (processingResult.AchievedMissions.Any())
            await rewardsGrantor.GrantAsync(request.UserId, processingResult.AchievedMissions, ct);

        await spinsLogRepository.AppendAsync(request.SpindId);
    }
}