using System.Globalization;
using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace xunitv3.template.Examples.Data;

/// <summary>
/// Custom <see cref="DataAttribute"/>: parses inline CSV, converts each field to the parameter type, and labels every
/// row with its source line so display names read <c>Method [1, 2, 3]</c> instead of the generic argument dump.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class InlineCsvDataAttribute(string csv) : DataAttribute
{
    private const char RowSeparator = '\n';
    private const char FieldSeparator = ',';
    private const string ArityMismatchFormat = "CSV row '{0}' has {1} field(s) but {2} expects {3} parameter(s)";

    public override ValueTask<IReadOnlyCollection<ITheoryDataRow>> GetData(MethodInfo testMethod, DisposalTracker disposalTracker)
    {
        var parameterTypes = Array.ConvertAll(testMethod.GetParameters(), static parameter => parameter.ParameterType);
        IReadOnlyCollection<ITheoryDataRow> rows =
        [
            .. csv
                .Split(RowSeparator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(line => ToRow(line, testMethod, parameterTypes)),
        ];

        return new(rows);
    }

    public override bool SupportsDiscoveryEnumeration() => true;

    private ITheoryDataRow ToRow(string line, MethodInfo testMethod, Type[] parameterTypes)
    {
        var fields = line.Split(FieldSeparator, StringSplitOptions.TrimEntries);
        if (fields.Length != parameterTypes.Length)
        {
            throw new TestPipelineException(string.Format(CultureInfo.InvariantCulture, ArityMismatchFormat, line, fields.Length, testMethod.Name, parameterTypes.Length));
        }

        var values = new object?[fields.Length];
        for (var index = 0; index < fields.Length; index++)
        {
            values[index] = Convert.ChangeType(fields[index], parameterTypes[index], CultureInfo.InvariantCulture);
        }

        return new TheoryDataRow(values)
        {
            Explicit = ExplicitAsNullable,
            Label = Label ?? line,
            Skip = Skip,
            SkipType = SkipType,
            SkipUnless = SkipUnless,
            SkipWhen = SkipWhen,
            TestDisplayName = TestDisplayName,
            Timeout = TimeoutAsNullable,
        };
    }
}
