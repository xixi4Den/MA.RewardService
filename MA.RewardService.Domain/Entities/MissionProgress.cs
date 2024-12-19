namespace MA.RewardService.Domain.Entities;

public class MissionProgress
{
    private MissionProgress()
    {
    }

    public static MissionProgress Empty()
    {
        return new MissionProgress
        {
            IsEmpty = true,
            MissionIndex = 1
        };
    }
    
    public static MissionProgress Create(int missionIndex, int totalPoints, int remainingPoints)
    {
        return new MissionProgress
        {
            MissionIndex = missionIndex,
           TotalPoints = totalPoints,
           RemainingPoints = remainingPoints
        };
    }
    
    public bool IsEmpty { get; private init; }
    
    public int MissionIndex { get; private init; }
    public int TotalPoints { get; private init; }
    public int RemainingPoints { get; private init; }
}