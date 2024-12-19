namespace MA.RewardService.Domain.Abstractions;

public interface IPointsCalculator
{
    int Calculate(byte[] spinResult);
}