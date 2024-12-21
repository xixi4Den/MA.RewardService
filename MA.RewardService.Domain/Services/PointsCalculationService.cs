using MA.RewardService.Domain.Abstractions;

namespace MA.RewardService.Domain.Services;

public class PointsCalculator: IPointsCalculator
{
    public int Calculate(int[] spinResult)
    {
        if (spinResult.Length != 3)
            throw new ArgumentException("Expected spin result must contain 3 digits");

        if (spinResult.Any(x => x > 9))
            throw new ArgumentException("Expected spin result must consist of digits between 0 and 9");
        
        return spinResult.All(x => x == spinResult[0])
            ? spinResult[0] * 3
            : 0;
    }
}