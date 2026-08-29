using Xunit.Sdk;
using Xunit.v3;

namespace xunitv3.template.Examples.Ordering;

/// <summary>
/// <see cref="ITestClassOrderer"/>: orders the classes of a collection (or the assembly) by class name.
/// Apply with <c>[TestClassOrderer(typeof(AlphabeticalClassOrderer))]</c> on a collection definition or the assembly.
/// </summary>
public sealed class AlphabeticalClassOrderer : ITestClassOrderer
{
    public IReadOnlyCollection<TTestClass?> OrderTestClasses<TTestClass>(IReadOnlyCollection<TTestClass?> testClasses)
        where TTestClass : ITestClass =>
        [.. testClasses.OrderBy(static testClass => testClass?.TestClassName, StringComparer.Ordinal)];
}
