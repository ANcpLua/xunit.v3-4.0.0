using Xunit.v3;

namespace xunitv3.template.Examples.Attributes;

/// <summary>
/// Custom trait (<see cref="ITraitAttribute"/>). Traits from assembly, class, collection definition, and method merge,
/// and can be filtered with <c>--filter-trait Category=…</c> under Microsoft.Testing.Platform.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class CategoryAttribute(string category) : Attribute, ITraitAttribute
{
    public const string TraitName = "Category";

    public IReadOnlyCollection<KeyValuePair<string, string>> GetTraits() => [new(TraitName, category)];
}
