using MA.RewardService.Domain.Entities;
using MA.RewardService.Domain.Events;

namespace MA.RewardService.Domain.Abstractions;

public interface IMissionProgressProcessor
{
    MissionProgressProcessingResult Process(
        MissionProgress currentProgress,
        int newPoints,
        MissionsConfiguration missionsConfig);
}

public class MissionProgressProcessingResult
{
    public required MissionProgress NewProgress { get; init; }
    public IEnumerable<MissionReachedEvent> Events { get; init; } = [];
}