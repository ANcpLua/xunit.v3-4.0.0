using System.Globalization;
using System.Reflection;
using Xunit.v3;

namespace xunitv3.template.Examples.Attributes;

/// <summary>
/// <see cref="BeforeAfterTestAttribute"/> (v3 signature: <c>Before/After(MethodInfo, IXunitTest)</c>) that swaps
/// <see cref="CultureInfo.CurrentCulture"/> and <see cref="CultureInfo.CurrentUICulture"/> for the duration of one test.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public sealed class UseCultureAttribute(string culture, string uiCulture) : BeforeAfterTestAttribute
{
    private CultureInfo? _originalCulture;
    private CultureInfo? _originalUiCulture;

    public UseCultureAttribute(string culture)
        : this(culture, culture)
    {
    }

    public CultureInfo Culture { get; } = CultureInfo.GetCultureInfo(culture);

    public CultureInfo UiCulture { get; } = CultureInfo.GetCultureInfo(uiCulture);

    public override void Before(MethodInfo methodUnderTest, IXunitTest test)
    {
        _originalCulture = CultureInfo.CurrentCulture;
        _originalUiCulture = CultureInfo.CurrentUICulture;

        CultureInfo.CurrentCulture = Culture;
        CultureInfo.CurrentUICulture = UiCulture;
    }

    public override void After(MethodInfo methodUnderTest, IXunitTest test)
    {
        CultureInfo.CurrentCulture = _originalCulture ?? CultureInfo.InvariantCulture;
        CultureInfo.CurrentUICulture = _originalUiCulture ?? CultureInfo.InvariantCulture;
    }
}
