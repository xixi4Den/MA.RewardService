namespace MA.RewardService.Domain.Abstractions;

public interface IPointsCalculator
{
    int Calculate(int[] spinResult);
}