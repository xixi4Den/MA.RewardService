using MA.RewardService.Domain.Abstractions;
using MA.RewardService.Domain.Entities;
using MA.RewardService.Domain.Events;

namespace MA.RewardService.Domain.Services;

public class MissionProgressProcessor : IMissionProgressProcessor
{
    public MissionProgressProcessingResult Process(
        MissionProgress currentProgress,
        int newPoints,
        MissionsConfiguration missionsConfig)
    {
        if (newPoints == 0)
            return new MissionProgressProcessingResult
            {
                NewProgress = currentProgress
            };
        
        var updatedTotalPoints = currentProgress.TotalPoints + newPoints;
        var updatedRemainingPoints = currentProgress.RemainingPoints + newPoints;

        var targetMission = missionsConfig.Missions[currentProgress.MissionIndex - 1];
        
        if (updatedRemainingPoints >= targetMission.PointsGoal)
        {
            var nextMissionIndex = GetNextMissionIndex(currentProgress, missionsConfig);

            return new MissionProgressProcessingResult
            {
                NewProgress = MissionProgress.Create(nextMissionIndex, updatedTotalPoints,
                    updatedRemainingPoints - targetMission.PointsGoal),
                Events = [new MissionReachedEvent(targetMission.Rewards)]
            };
        }

        return new MissionProgressProcessingResult
        {
            NewProgress = MissionProgress.Create(currentProgress.MissionIndex, updatedTotalPoints, updatedRemainingPoints)
        };
    }
    
    private static int GetNextMissionIndex(MissionProgress currentProgress, MissionsConfiguration missionsConfig)
    {
        return currentProgress.MissionIndex < missionsConfig.Missions.Count
            ? currentProgress.MissionIndex + 1
            : missionsConfig.RepeatedIndex;
    }
}