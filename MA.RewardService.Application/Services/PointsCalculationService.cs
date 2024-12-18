using MA.RewardService.Domain.Abstractions;

namespace MA.RewardService.Application.Services;

public class PointsCalculator: IPointsCalculator
{
    public int Calculate(int[] spinResult)
    {
        if (spinResult.Length != 3)
            throw new InvalidOperationException("Expected spin result must contain 3 digits");

        return spinResult.All(x => x == spinResult[0])
            ? spinResult[0] * 3
            : 0;
    }
}