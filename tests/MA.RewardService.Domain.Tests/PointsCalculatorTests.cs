using FluentAssertions;
using MA.RewardService.Domain.Services;

namespace MA.RewardService.Domain.Tests;

[TestClass]
public class PointsCalculatorTests
{
    private PointsCalculator _subject;

    [TestInitialize]
    public void Init()
    {
        _subject = new PointsCalculator();
    }

    [DataTestMethod]
    [DynamicData(nameof(WinningData))]
    public void WinningCombination_ShouldReturnExpectedPoints(byte[] spinResult, int expectedPoints)
    {
        var result = _subject.Calculate(spinResult);

        result.Should().Be(expectedPoints);
    }
    
    [DataTestMethod]
    [DynamicData(nameof(NonWinningData))]
    public void NonWinningCombination_ShouldReturnZero(byte[] spinResult)
    {
        var result = _subject.Calculate(spinResult);

        result.Should().Be(0);
    }
    
    [TestMethod]
    public void ThreeZeros_ShouldReturnZeroPoints()
    {
        var result = _subject.Calculate([0, 0, 0]);

        result.Should().Be(0);
    }
    
    public static IEnumerable<object[]> WinningData
    {
        get
        {
            for (byte i = 1; i <= 9; i++)
            {
                yield return [new[] {i, i, i}, i*3];
            }
        }
    }
    
    public static IEnumerable<object[]> NonWinningData
    {
        get
        {
            yield return [new byte[] {0, 0, 1}];
            yield return [new byte[] {1, 2, 3}];
            yield return [new byte[] {4, 5, 4}];
            yield return [new byte[] {8, 9, 9}];
        }
    }
}