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
    [DynamicData(nameof(InvalidData))]
    public void InvalidSpinResult_ShouldThrowException(int[] spinResult)
    {
        var fn = () => _subject.Calculate(spinResult);

        fn.Should().Throw<ArgumentException>();
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
    
    private static IEnumerable<object[]> WinningData
    {
        get
        {
            for (var i = 1; i <= 9; i++)
            {
                yield return [new[] {i, i, i}, i*3];
            }
        }
    }
    
    private static IEnumerable<object[]> NonWinningData
    {
        get
        {
            yield return [new[] {0, 0, 1}];
            yield return [new[] {1, 2, 3}];
            yield return [new[] {4, 5, 4}];
            yield return [new[] {8, 9, 9}];
        }
    }
    
    private static IEnumerable<object[]> InvalidData
    {
        get
        {
            yield return [new[] {0, 0, 10}];
            yield return [new[] {1, 2, 3, 4}];
            yield return [new[] {4, 5}];
        }
    }
}