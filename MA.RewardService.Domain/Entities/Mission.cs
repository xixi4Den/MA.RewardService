namespace MA.RewardService.Domain.Entities;

public class Mission
{
    public int PointsGoal { get; set; }
    
    public required IEnumerable<Reward> Rewards { get; set; }
}