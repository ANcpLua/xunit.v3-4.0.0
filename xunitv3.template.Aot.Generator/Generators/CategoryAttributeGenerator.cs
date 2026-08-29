using Microsoft.CodeAnalysis;
using Xunit.Generators;

namespace xunitv3.template.Aot.Generator;

/// <summary>
/// Native AOT has no reflection-based trait discovery, so traits are generated at compile time: the generator
/// matches the attribute by full name and emits the (name, value) pairs into the generated test registration.
/// </summary>
[Generator(LanguageNames.CSharp)]
public sealed class CategoryAttributeGenerator() : TraitGenerator(AttributeTypeName)
{
    public const string AttributeTypeName = "xunitv3.template.Aot.CategoryAttribute";
    public const string TraitName = "Category";

    protected override IEnumerable<(string name, string value)> GetTraitValues(AttributeData attribute)
    {
        if (attribute.ConstructorArguments.Length < 1
            || attribute.ConstructorArguments[0].Kind != TypedConstantKind.Primitive
            || attribute.ConstructorArguments[0].Value is not string category)
        {
            yield break;
        }

        yield return (TraitName, category);
    }
}
