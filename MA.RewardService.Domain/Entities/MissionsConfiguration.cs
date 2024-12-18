namespace MA.RewardService.Domain.Entities;

public class MissionsConfiguration
{
    public required IList<Mission> Missions { get; set; }
    
    public required int RepeatedIndex { get; set; }
}