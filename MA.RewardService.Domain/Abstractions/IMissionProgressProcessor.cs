using MA.RewardService.Domain.Entities;

namespace MA.RewardService.Domain.Abstractions;

public interface IMissionProgressProcessor
{
    MissionProgressProcessingResult Process(int userId, MissionProgress currentProgress,
        int newPoints,
        MissionsConfiguration missionsConfig);
}

public class MissionProgressProcessingResult
{
    public required MissionProgress NewProgress { get; init; }
    public IEnumerable<Mission> AchievedMissions { get; init; } = [];
}