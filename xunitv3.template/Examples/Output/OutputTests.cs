using System.Diagnostics;
using xunitv3.template.Examples.Output;

[assembly: CaptureConsole]

namespace xunitv3.template.Examples.Output;

/// <summary>
/// With <see cref="CaptureConsoleAttribute"/> / <see cref="CaptureTraceAttribute"/> (assembly-level), <see cref="Console"/> and
/// <see cref="Trace"/> writes are routed into the running test's output.
/// </summary>
public sealed class OutputTests(ITestOutputHelper output)
{
    private const string Message = "captured from Console";
    private const string TraceMessage = "captured from Trace";
    private const string TextAttachment = "notes";
    private const string TextPayload = "plain text attachment";
    private const string BinaryAttachment = "blob";
    private const string MediaType = "application/octet-stream";
    private const string WarningText = "this test emits a warning";
    private const string StorageKey = "answer";
    private const int StorageValue = 42;

    private static readonly byte[] BinaryPayload = [0x78, 0x75, 0x6E, 0x69, 0x74];

    [Fact]
    public void TraceOutputIsCapturedIntoTestOutput()
    {
        Trace.WriteLine(TraceMessage);

        Assert.Contains(TraceMessage, output.Output);
    }

    // Attachments and warnings travel with the test result (TRX/CTRF reports); KeyValueStorage is per-test scratch space.
    [Fact]
    public void AttachmentsWarningsAndStorageLiveOnTheContext()
    {
        var context = TestContext.Current;

        context.AddAttachment(TextAttachment, TextPayload);
        context.AddAttachment(BinaryAttachment, BinaryPayload, MediaType);
        context.AddWarning(WarningText);
        context.KeyValueStorage[StorageKey] = StorageValue;

        Assert.Equal(TextPayload, context.Attachments![TextAttachment].AsString());
        Assert.Equal(MediaType, context.Attachments[BinaryAttachment].AsByteArray().MediaType);
        Assert.Contains(WarningText, context.Warnings!);
        Assert.Equal(StorageValue, context.KeyValueStorage[StorageKey]);
    }

    [Fact]
    public void ConsoleOutputIsCapturedIntoTestOutput()
    {
        Console.WriteLine(Message);

        Assert.Contains(Message, output.Output);
    }

    [Fact]
    public void InjectedHelperIsTheContextHelper() => Assert.Same(output, TestContext.Current.TestOutputHelper);
}
