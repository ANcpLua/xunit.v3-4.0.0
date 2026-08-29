using Xunit.Sdk;

namespace xunitv3.template.Examples.Assertions;

/// <summary>
/// 4.0: Assert.OverrideMaxEnumerableLength / OverrideMaxStringLength change how much of a value the failure message
/// prints, per test. xunit.runner.json (printMaxEnumerableLength / printMaxStringLength) sets the assembly default.
/// </summary>
public sealed class PrintLengthTests
{
    private const int Limit = 2;
    private const string Ellipsis = "···";

    private static readonly int[] Expected = [1, 2, 3, 4, 5];
    private static readonly int[] Actual = [1, 2, 3, 4, 6];

    [Fact]
    public void EnumerableOutputIsTruncatedToTheOverride()
    {
        Assert.OverrideMaxEnumerableLength(Limit);

        var exception = Assert.Throws<EqualException>(() => Assert.Equal(Expected, Actual));

        Assert.Contains(Ellipsis, exception.Message);
    }

    [Fact]
    public void StringOutputIsTruncatedToTheOverride()
    {
        Assert.OverrideMaxStringLength(Limit);

        var exception = Assert.Throws<EqualException>(() => Assert.Equal(nameof(Expected), nameof(Actual)));

        Assert.Contains(Ellipsis, exception.Message);
    }
}
