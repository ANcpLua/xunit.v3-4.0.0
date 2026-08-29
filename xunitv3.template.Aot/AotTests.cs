using Xunit;
using xunitv3.template.Aot;

[assembly: Category(AotTests.AssemblyLevel)]

namespace xunitv3.template.Aot;

[Category(ClassLevel)]
public sealed class AotTests
{
    public const string AssemblyLevel = "assembly";
    private const string ClassLevel = "class";
    private const string MethodLevel = "method";
    private const string TraitName = "Category";

    [Fact, Category(MethodLevel)]
    public void GeneratedTraitsMergeAcrossAssemblyClassAndMethod()
    {
        var categories = TestContext.Current.TestCase!.Traits[TraitName];

        Assert.Contains(AssemblyLevel, categories);
        Assert.Contains(ClassLevel, categories);
        Assert.Contains(MethodLevel, categories);
    }

    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(40, 2, 42)]
    public void TheoriesAreGeneratedToo(int left, int right, int sum) => Assert.Equal(sum, left + right);
}
