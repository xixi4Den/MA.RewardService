namespace MA.RewardService.Api.Contracts;

public class MissionProgressResponse
{
    public int MissionIndex { get; set; }
    public int TotalPoints { get; set; }
    public int RemainingPoints { get; set; }
}