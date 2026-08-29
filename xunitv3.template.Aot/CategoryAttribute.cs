namespace xunitv3.template.Aot;

/// <summary>Plain attribute: under Native AOT the trait values come from the source generator, not from ITraitAttribute.</summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public sealed class CategoryAttribute(string category) : Attribute
{
    public string Category { get; } = category;
}
