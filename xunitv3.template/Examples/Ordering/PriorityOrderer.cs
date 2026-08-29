using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace xunitv3.template.Examples.Ordering;

/// <summary>
/// xUnit.net v3 4.0 <see cref="ITestMethodOrderer"/>: orders the methods of a class by
/// <see cref="TestPriorityAttribute"/> (ascending), then by ordinal method name.
/// Apply with <c>[TestMethodOrderer(typeof(PriorityOrderer))]</c> on a class, a collection definition, or the assembly.
/// </summary>
public sealed class PriorityOrderer : ITestMethodOrderer
{
    public const int DefaultPriority = 0;

    public IReadOnlyCollection<TTestMethod?> OrderTestMethods<TTestMethod>(IReadOnlyCollection<TTestMethod?> testMethods)
        where TTestMethod : notnull, ITestMethod =>
        [.. testMethods
            .OrderBy(static method => Priority(method))
            .ThenBy(static method => method?.MethodName, StringComparer.Ordinal)];

    private static int Priority(ITestMethod? method) =>
        (method as IXunitTestMethod)?.Method.GetCustomAttribute<TestPriorityAttribute>()?.Priority ?? DefaultPriority;
}
