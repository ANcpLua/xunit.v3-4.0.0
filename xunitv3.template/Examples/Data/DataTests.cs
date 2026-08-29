using Xunit.Sdk;

namespace xunitv3.template.Examples.Data;

public sealed class DataTests
{
    private const string AdditionCsv = """
        1, 2, 3
        40, 2, 42
        -1, 1, 0
        """;

    private const string AsciiLabel = "ascii";
    private const string UnicodeLabel = "unicode";
    private const string AsciiText = "hello";
    private const string UnicodeText = "héllo";
    private const string LabelFormat = "[{0}]";

    public static IEnumerable<TheoryDataRow<string, string>> LabeledRows =>
    [
        new(AsciiText, AsciiLabel) { Label = AsciiLabel },
        new(UnicodeText, UnicodeLabel) { Label = UnicodeLabel, DisableParallelization = true },
    ];

    [Theory, InlineCsvData(AdditionCsv)]
    public void CsvRowsAreTypedAndLabeled(int left, int right, int sum)
    {
        Assert.Equal(sum, left + right);
        Assert.Contains(string.Format(null, LabelFormat, TestContext.Current.Test!.TestLabel), TestContext.Current.Test.TestDisplayName);
    }

    // 4.0: ITheoryDataRow.Label -> ITestMetadata.TestLabel, and IncludeTestCaseIndex prefixes each row with a zero-padded index.
    [Theory(IncludeTestCaseIndex = true), MemberData(nameof(LabeledRows))]
    public void RowLabelBecomesTestLabel(string text, string expectedLabel)
    {
        Assert.Equal(expectedLabel, TestContext.Current.Test!.TestLabel);
        Assert.Contains(string.Format(null, LabelFormat, expectedLabel), TestContext.Current.Test.TestDisplayName);
        Assert.Equal(AsciiText.Length, text.Length);
    }

    // 4.0: Assert.All / AllAsync gained `throwIfEmpty`; the default still passes on an empty collection.
    [Fact]
    public void StrictAllRejectsEmptyCollections()
    {
        Assert.All([], static (int _) => { });
        Assert.Throws<AllException>(() => Assert.All([], static (int _) => { }, throwIfEmpty: true));
    }
}
