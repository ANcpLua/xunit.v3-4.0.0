using xunitv3.template.Examples.Output;

[assembly: CaptureConsole]

namespace xunitv3.template.Examples.Output;

/// <summary>With <see cref="CaptureConsoleAttribute"/>, <see cref="Console"/> writes are routed into the running test's output.</summary>
public sealed class OutputTests(ITestOutputHelper output)
{
    private const string Message = "captured from Console";

    [Fact]
    public void ConsoleOutputIsCapturedIntoTestOutput()
    {
        Console.WriteLine(Message);

        Assert.Contains(Message, output.Output);
    }

    [Fact]
    public void InjectedHelperIsTheContextHelper() => Assert.Same(output, TestContext.Current.TestOutputHelper);
}
