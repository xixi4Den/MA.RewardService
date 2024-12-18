using FluentAssertions;
using MA.RewardService.Application.Services;

namespace MA.RewardService.Application.Tests;

[TestClass]
public class PointsCalculatorTests
{
    public PointsCalculator _subject;

    [TestInitialize]
    public void Init()
    {
        _subject = new PointsCalculator();
    }

    [DataTestMethod]
    [DynamicData(nameof(WinningData))]
    public void WinningCombination_ShouldReturnExpectedPoints(int[] spinResult, int expectedPoints)
    {
        var result = _subject.Calculate(spinResult);

        result.Should().Be(expectedPoints);
    }
    
    [DataTestMethod]
    [DynamicData(nameof(NonWinningData))]
    public void NonWinningCombination_ShouldReturnZero(int[] spinResult)
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
            for (int i = 1; i <= 9; i++)
            {
                yield return [new[] {i, i, i}, i*3];
            }
        }
    }
    
    public static IEnumerable<object[]> NonWinningData
    {
        get
        {
            yield return [new[] {0, 0, 1}];
            yield return [new[] {1, 2, 3}];
            yield return [new[] {4, 5, 4}];
            yield return [new[] {8, 9, 9}];
        }
    }
}