using MA.RewardService.Domain.Abstractions;
using MA.RewardService.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace MA.RewardService.Domain.Services;

public class MissionProgressProcessor(ILogger<MissionProgressProcessor> logger) : IMissionProgressProcessor
{
    public MissionProgressProcessingResult Process(int userId, MissionProgress currentProgress,
        int newPoints,
        MissionsConfiguration missionsConfig)
    {
        if (newPoints < 0)
            throw new ArgumentException("Points cannot be negative");
        
        if (newPoints == 0)
            return new MissionProgressProcessingResult
            {
                NewProgress = currentProgress
            };
        
        var updatedTotalPoints = currentProgress.TotalPoints + newPoints;
        var updatedRemainingPoints = currentProgress.RemainingPoints + newPoints;

        var targetMissionIndex = currentProgress.MissionIndex;
        var targetMission = missionsConfig.Missions[targetMissionIndex - 1];

        var achievedMissions = new List<Mission>();
        while (updatedRemainingPoints >= targetMission.PointsGoal)
        {
            logger.LogDebug("Mission {MissionIndex} completed by user {UserId}", targetMissionIndex, userId);
            achievedMissions.Add(targetMission);
            updatedRemainingPoints -= targetMission.PointsGoal;
            targetMissionIndex = GetNextMissionIndex(targetMissionIndex, missionsConfig);
            targetMission = missionsConfig.Missions[targetMissionIndex - 1];
        }

        return new MissionProgressProcessingResult
        {
            NewProgress = MissionProgress.Create(targetMissionIndex, updatedTotalPoints, updatedRemainingPoints),
            AchievedMissions = achievedMissions
        };
    }
    
    private static int GetNextMissionIndex(int currentMissionIndex, MissionsConfiguration missionsConfig)
    {
        return currentMissionIndex < missionsConfig.Missions.Count
            ? currentMissionIndex + 1
            : missionsConfig.RepeatedIndex;
    }
}