using Xunit.Sdk;

namespace xunitv3.template.Examples.Data;

public sealed class DataTests
{
    private const string AdditionCsv = """
        1, 2, 3
        40, 2, 42
        -1, 1, 0
        """;

    private const string CsvLabel = "csv";
    private const string AsciiLabel = "ascii";
    private const string UnicodeLabel = "unicode";
    private const string AsciiText = "hello";
    private const string UnicodeText = "héllo";
    private const string LabelFormat = "[{0}]";
    private const string IndexedDisplayNameFormat = "{0}_{1:000} [{2}]";

    public static IEnumerable<TheoryDataRow<string, string, int>> LabeledRows =>
    [
        new(AsciiText, AsciiLabel, 1) { Label = AsciiLabel },
        new(UnicodeText, UnicodeLabel, 2) { Label = UnicodeLabel, DisableParallelization = true },
    ];

    [Theory, InlineCsvData(AdditionCsv)]
    public void CsvRowsAreTypedAndLabeled(int left, int right, int sum)
    {
        Assert.Equal(sum, left + right);
        Assert.Contains(string.Format(null, LabelFormat, TestContext.Current.Test!.TestLabel), TestContext.Current.Test.TestDisplayName);
    }

    // 4.0: DataAttribute.Label applies one label to every row of the attribute; ITypeAwareDataAttribute.MemberType
    // let the attribute stamp the declaring type as a trait.
    [Theory, InlineCsvData(AdditionCsv, Label = CsvLabel)]
    public void AttributeLabelAndMemberTypeReachEveryRow(int left, int right, int sum)
    {
        Assert.Equal(sum, left + right);
        Assert.Equal(CsvLabel, TestContext.Current.Test!.TestLabel);
        Assert.Contains(nameof(DataTests), TestContext.Current.TestCase!.Traits[InlineCsvDataAttribute.SourceTypeTrait]);
    }

    // 4.0: ITheoryDataRow.Label -> ITestMetadata.TestLabel, and IncludeTestCaseIndex appends a 1-based,
    // zero-padded row index to the method name: RowLabelBecomesTestLabel_001 [ascii]
    [Theory(IncludeTestCaseIndex = true), MemberData(nameof(LabeledRows))]
    public void RowLabelBecomesTestLabel(string text, string expectedLabel, int expectedIndex)
    {
        Assert.Equal(expectedLabel, TestContext.Current.Test!.TestLabel);
        Assert.EndsWith(
            string.Format(null, IndexedDisplayNameFormat, nameof(RowLabelBecomesTestLabel), expectedIndex, expectedLabel),
            TestContext.Current.Test.TestDisplayName);
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
