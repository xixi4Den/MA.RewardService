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
    private readonly ILogger<HandleMissionProgressCommandHandler> _logger;

    public HandleMissionProgressCommandHandler(IPointsCalculator pointsCalculator,
        IMissionProgressProcessor missionProgressProcessor,
        IMissionsConfigurationProvider missionsConfigurationProvider,
        IMissionProgressRepository missionProgressRepository,
        ILogger<HandleMissionProgressCommandHandler> logger)
    {
        _pointsCalculator = pointsCalculator;
        _missionProgressProcessor = missionProgressProcessor;
        _missionsConfigurationProvider = missionsConfigurationProvider;
        _missionProgressRepository = missionProgressRepository;
        _logger = logger;
    }
    
    public async Task Handle(HandleMissionProgressCommand request, CancellationToken cancellationToken)
    {
        var newPoints = _pointsCalculator.Calculate(request.SpinResult);
        if (newPoints == 0)
        {
            return;
        }
        
        var currentProgress = await _missionProgressRepository.GetAsync(request.UserId);
        var missionsConfig = await _missionsConfigurationProvider.GetAsync();
        var processingResult = _missionProgressProcessor.Process(currentProgress, newPoints, missionsConfig);
        
        var hasUpdatedSuccessfully = await _missionProgressRepository.UpdateAsync(request.UserId, currentProgress, processingResult.NewProgress);
        if (hasUpdatedSuccessfully)
        {
            _logger.LogDebug($"Mission progress updated successfully for user {request.UserId}");
        }
        else
        {
            _logger.LogError($"Concurrent modification detected while updating mission progress for user {request.UserId}");
            throw new Exception($"Concurrent modification detected while updating mission progress for user {request.UserId}");
        }
    }
}