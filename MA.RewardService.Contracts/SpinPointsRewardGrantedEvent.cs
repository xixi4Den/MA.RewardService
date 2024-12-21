namespace MA.RewardService.Contracts;

public class SpinPointsRewardGrantedEvent
{
    public int UserId { get; set; }
    public long Amount { get; init; }
}