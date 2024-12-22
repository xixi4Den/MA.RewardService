namespace MA.RewardService.Contracts;

public class SpinPointsRewardGrantedEvent
{
    public Guid RewardId { get; set; }
    public int UserId { get; set; }
    public long Amount { get; init; }
}