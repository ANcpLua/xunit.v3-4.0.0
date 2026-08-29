using System.Globalization;

namespace xunitv3.template.Examples.Attributes;

public sealed class UseCultureTests
{
    private const string Austrian = "de-AT";
    private const string American = "en-US";
    private const string British = "en-GB";
    private const decimal Value = 21.12m;
    private const string AustrianFormatted = "21,12";
    private const string AmericanFormatted = "21.12";

    [Fact, UseCulture(Austrian)]
    public void GermanCultureUsesCommaDecimalSeparator() => Assert.Equal(AustrianFormatted, Value.ToString(CultureInfo.CurrentCulture));

    [Fact, UseCulture(American, British)]
    public void CultureAndUiCultureCanDiffer()
    {
        Assert.Equal(AmericanFormatted, Value.ToString(CultureInfo.CurrentCulture));
        Assert.Equal(British, CultureInfo.CurrentUICulture.Name);
    }
}

[Category(CategoryTests.Integration)]
public sealed class CategoryTests
{
    public const string Integration = "Integration";
    private const string Fast = "Fast";

    [Fact, Category(Fast)]
    public void ClassAndMethodTraitsMerge()
    {
        var categories = TestContext.Current.TestCase!.Traits[CategoryAttribute.TraitName];

        Assert.Contains(Integration, categories);
        Assert.Contains(Fast, categories);
    }
}
