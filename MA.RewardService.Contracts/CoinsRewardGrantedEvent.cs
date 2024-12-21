namespace MA.RewardService.Contracts;

public class CoinsRewardGrantedEvent
{
    public int UserId { get; set; }
    public long Amount { get; init; }
}