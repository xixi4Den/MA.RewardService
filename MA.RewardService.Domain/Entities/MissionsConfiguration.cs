namespace MA.RewardService.Domain.Entities;

public class MissionsConfiguration
{
    public MissionsConfiguration(IList<Mission> missions, int repeatedIndex)
    {
        if (repeatedIndex <= 0 || repeatedIndex > missions.Count)
            throw new ArgumentException("Invalid repeated index", nameof(repeatedIndex));
        
        Missions = missions;
        RepeatedIndex = repeatedIndex;
    }
    
    public IList<Mission> Missions { get; }
    
    public int RepeatedIndex { get; }
}