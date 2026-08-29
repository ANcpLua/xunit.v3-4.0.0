# Changelog — xUnit.net v3 API surface

Reverse API doc for this repository. One row per API member that has a **runnable, asserting example**
here; the file is the contract for what qyl-workspace may copy out and execute. Update it on every
version bump, in the same commit as the code.

Schema (fixed columns, one table per area, no merged cells):

`| API | Namespace | Kind | Example | Verified by | Since | Note |`

- `Kind` ∈ `attribute` · `interface` · `class` · `method` · `property` · `enum` · `config` · `cli`
- `Example` is the file that declares or applies the API; `Verified by` is the test that asserts the behaviour
- `Since` is the package version that introduced or last changed the member (`≤3.x` = unchanged from v3)
- All rows are verified against the restored package binaries, not the docs

## [4.0.0] — 2026-08-29

Packages: `xunit.v3.mtp-v2 4.0.0` · `xunit.analyzers 2.0.0` (transitive) · `Microsoft.Testing.Platform 2.3.3` (bundled) · `Microsoft.NET.Test.Sdk 18.9.0` · `Microsoft.Testing.Extensions.CodeCoverage 18.10.0` · SDK `10.0.400` · `net10.0`
Runner: `global.json` → `"test": { "runner": "Microsoft.Testing.Platform" }` (required on SDK 10 for `dotnet test`)

### Ordering

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `ITestMethodOrderer.OrderTestMethods<TTestMethod>` | `Xunit.v3` | interface | `Examples/Ordering/PriorityOrderer.cs` | `PriorityOrderingTests` | 4.0.0 | generic signature; `where TTestMethod : notnull, ITestMethod` |
| `TestMethodOrdererAttribute(Type)` | `Xunit` | attribute | `Examples/Ordering/OrderingTests.cs` | `PriorityOrderingTests` | 4.0.0 | valid on assembly, collection definition, class |
| `ITestCaseOrderer.OrderTestCases<TTestCase>` | `Xunit.v3` | interface | `Examples/Ordering/DescendingDisplayNameOrderer.cs` | `TestCaseOrderingTests` | 4.0.0 | generic signature replaces the 3.x one |
| `TestCaseOrdererAttribute(Type)` on a method | `Xunit` | attribute | `Examples/Ordering/OrderingTests.cs` | `TestCaseOrderingTests.RowsRunInDescendingOrder` | 4.0.0 | orders theory rows of one method |
| `TestClassAttribute.DisableParallelization` | `Xunit` | property | `Examples/Ordering/OrderingTests.cs` | `PriorityOrderingTests` (`--parallel all` ×3) | 4.0.0 | per-class opt-out under `parallelMode: all` |
| `IXunitTestMethod.Method` (`MethodInfo`) | `Xunit.v3` | property | `Examples/Ordering/PriorityOrderer.cs` | `PriorityOrderingTests` | ≤3.x | reflection access for attribute lookup |
| `ITestClassOrderer.OrderTestClasses<TTestClass>` | `Xunit.v3` | interface | `Examples/Ordering/AlphabeticalClassOrderer.cs` | `AlphaClassTests`, `BetaClassTests` | 4.0.0 | generic signature; `IReadOnlyCollection<TTestClass?>` |
| `TestClassOrdererAttribute(Type)` on a collection definition | `Xunit` | attribute | `Examples/Ordering/CollectionOrderingTests.cs` | same | 4.0.0 | assembly or collection definition |
| `ITestCollectionOrderer.OrderTestCollections<TTestCollection>` | `Xunit.v3` | interface | `Examples/Ordering/RecordingCollectionOrderer.cs` | `CollectionOrdererTests` | 4.0.0 | generic signature |
| `TestCollectionOrdererAttribute(Type)` | `Xunit` | attribute | `Examples/AssemblyPolicies.cs` | `CollectionOrdererTests` | ≤3.x | assembly-level |
| `ParallelizationAttribute { Mode, Algorithm, MaxThreads }` | `Xunit.v3` | attribute | `Examples/AssemblyPolicies.cs` | — (explicit default) | 4.0.0 | `Mode` left unset so `--parallel` still decides |
| `FactAttribute.DisableParallelization` | `Xunit` | property | `Examples/Ordering/DisableParallelizationTests.cs` | `SerialFactTests` (`--parallel all` ×4) | 4.0.0 | per-test opt-out |
| `CollectionDefinitionAttribute.DisableParallelization` | `Xunit` | property | `Examples/Ordering/DisableParallelizationTests.cs`, `CollectionOrderingTests.cs`, `Examples/Telemetry/TelemetryCollection.cs` | `SerialCollection*Tests`, ordered classes | ≤3.x | keeps a collection's classes serial under `--parallel all` |
| `TestContext.Current.TestCollection` | `Xunit` | property | `Examples/Ordering/CollectionOrderingTests.cs` | `CollectionOrdererTests` | ≤3.x | |

### Custom test cases and runners (retry)

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `XunitTestCaseDiscovererAttribute(Type)` | `Xunit.v3` | attribute | `Examples/Retry/RetryFactAttribute.cs` | `RetryTests` | ≤3.x | binds attribute → discoverer |
| `FactAttribute(sourceFilePath, sourceLineNumber)` ctor | `Xunit` | class | `Examples/Retry/RetryFactAttribute.cs` | `RetryTests.PassesOnSecondAttempt` | ≤3.x | `[CallerFilePath]`/`[CallerLineNumber]` |
| `TheoryAttribute(sourceFilePath, sourceLineNumber)` ctor | `Xunit` | class | `Examples/Retry/RetryTheoryAttribute.cs` | `RetryTests.PassesOnNthAttempt` | ≤3.x | |
| `IXunitTestCaseDiscoverer.Discover` | `Xunit.v3` | interface | `Examples/Retry/RetryFactDiscoverer.cs` | `RetryTests.PassesOnSecondAttempt` | ≤3.x | |
| `TheoryDiscoverer.CreateTestCasesForDataRow(…, string? index)` | `Xunit.v3` | method | `Examples/Retry/RetryTheoryDiscoverer.cs` | `RetryTests.PassesOnNthAttempt` | 4.0.0 | 5-arg overload is obsolete |
| `TheoryDiscoverer.CreateTestCasesForTheory` | `Xunit.v3` | method | `Examples/Retry/RetryTheoryDiscoverer.cs` | `RetryTests.PassesOnNthAttemptWhenEnumeratedAtRunTime` | ≤3.x | delay-enumerated path |
| `TestIntrospectionHelper.GetTestCaseDetails` | `Xunit.v3` | method | `Examples/Retry/RetryFactDiscoverer.cs` | `RetryTests` | ≤3.x | tuple: `TestCaseDisplayName, Explicit, SkipExceptions, SkipReason, SkipType, SkipUnless, SkipWhen, SourceFilePath, SourceLineNumber, Timeout, UniqueID, ResolvedTestMethod` |
| `TestIntrospectionHelper.GetTestCaseDetailsForTheoryDataRow` | `Xunit.v3` | method | `Examples/Retry/RetryTheoryDiscoverer.cs` | `RetryTests.PassesOnNthAttempt` | 4.0.0 | same tuple shape |
| `TestIntrospectionHelper.GetTraits(IXunitTestMethod, ITheoryDataRow)` | `Xunit.v3` | method | `Examples/Retry/RetryTheoryDiscoverer.cs` | `RetryTests.PassesOnNthAttempt` | 4.0.0 | merges method + row traits |
| `Xunit.Internal.CollectionExtensions.ToReadWrite` | `Xunit.Internal` | method | `Examples/Retry/RetryFactDiscoverer.cs` | `RetryTests` | ≤3.x | traits → `Dictionary<string, HashSet<string>>` |
| `XunitTestCase` 16-arg ctor (`testLabel`, `disableParallelization`) | `Xunit.v3` | class | `Examples/Retry/RetryTestCase.cs` | `RetryTests` | 4.0.0 | data-row overload |
| `XunitTestCase.Serialize/Deserialize(IXunitSerializationInfo)` | `Xunit.v3` | method | `Examples/Retry/RetryTestCase.cs` | `RetryTests` | ≤3.x | plus obsolete parameterless ctor |
| `XunitDelayEnumeratedTheoryTestCase` | `Xunit.v3` | class | `Examples/Retry/RetryDelayEnumeratedTestCase.cs` | `RetryTests.PassesOnNthAttemptWhenEnumeratedAtRunTime` | ≤3.x | |
| `ISelfExecutingXunitTestCase.Run(ExplicitOption, IMessageBus, object?[], ExceptionAggregator, CancellationTokenSource, ParallelMode, ExecutionScheduler, FixtureMappingManager)` | `Xunit.v3` | interface | `Examples/Retry/RetryTestCase.cs` | `RetryTests` | 4.0.0 | `ParallelMode` + `ExecutionScheduler` added |
| `XunitTestCaseRunnerBase<TContext, TTestCase, TTest>.RunTest` | `Xunit.v3` | class | `Examples/Retry/RetryTestCaseRunner.cs` | `RetryTests` (7 `retrying…` diagnostics) | 4.0.0 | |
| `XunitTestCaseRunnerBaseContext<TTestCase, TTest>` 12-arg ctor | `Xunit.v3` | class | `Examples/Retry/RetryTestCaseRunner.cs` | `RetryTests` | 4.0.0 | |
| `XunitTestRunner.Instance.Run(…, ParallelMode, ExecutionScheduler, beforeAfterAttributes, fixtureMappings)` | `Xunit.v3` | method | `Examples/Retry/RetryTestCaseRunner.cs` | `RetryTests` | 4.0.0 | |
| `XunitRunnerHelper.SkipTestCases` / `FailTestCases` | `Xunit.v3` | method | `Examples/Retry/RetryTestCaseRunner.cs` | `RetryTests` | ≤3.x | `sendTestCaseMessages: false` |
| `DynamicSkipToken.Value` | `Xunit.v3` | property | `Examples/Retry/RetryTestCaseRunner.cs` | `RetryTests` | ≤3.x | |
| `ExceptionAggregator.Clone/Clear/HasExceptions/ToException/RunAsync` | `Xunit.v3` | class | `Examples/Retry/RetryTestCaseRunner.cs` | `RetryTests` | ≤3.x | |
| `IMessageBus.QueueMessage(IMessageSinkMessage)` | `Xunit.v3` / `Xunit.Sdk` | interface | `Examples/Retry/DelayedMessageBus.cs` | `RetryTests` | ≤3.x | buffering bus |
| `TestContext.Current.SendDiagnosticMessage(format, args)` | `Xunit` | method | `Examples/Retry/RetryTestCaseRunner.cs` | `--xunit-diagnostics on` | ≤3.x | |
| `IClassFixture<T>` | `Xunit` | interface | `Examples/Retry/RetryTests.cs` | `RetryTests` | ≤3.x | survives per-attempt re-instantiation |

### Serialization

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `IXunitSerializable.Serialize/Deserialize(IXunitSerializationInfo)` | `Xunit.Sdk` | interface | `Examples/Serialization/Coordinate.cs` | `SerializationTests.CoordinateToStringDrivesDisplayName` | ≤3.x | needs public parameterless ctor; `ToString()` → display name |
| `IXunitSerializationInfo.AddValue/GetValue<T>` | `Xunit.Sdk` | interface | `Examples/Serialization/Coordinate.cs` | same | ≤3.x | |
| `IXunitSerializer.Serialize/Deserialize/IsSerializable(out failureReason)` | `Xunit.Sdk` | interface | `Examples/Serialization/MoneySerializer.cs` | `SerializationTests.MoneyRoundTripsThroughExternalSerializer`, `ExternalSerializerRejectsForeignTypes` | ≤3.x | |
| `RegisterXunitSerializerAttribute(Type serializer, params Type[] supported)` | `Xunit.Sdk` | attribute | `Examples/Serialization/MoneySerializer.cs` | same | ≤3.x | assembly-level |
| `TheoryDataRow<T1[,T2,T3]>` | `Xunit` | class | `Examples/Serialization/SerializationTests.cs`, `Examples/Data/DataTests.cs` | `SerializationTests`, `DataTests` | ≤3.x | up to 15 type args |

### Before/after attributes and traits

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `BeforeAfterTestAttribute.Before/After(MethodInfo, IXunitTest)` | `Xunit.v3` | class | `Examples/Attributes/UseCultureAttribute.cs` | `UseCultureTests` | ≤3.x | v3 signature carries `IXunitTest` |
| `ITraitAttribute.GetTraits()` | `Xunit.v3` | interface | `Examples/Attributes/CategoryAttribute.cs` | `CategoryTests.ClassAndMethodTraitsMerge`, `--filter-trait Category=Fast` | ≤3.x | multiple traits per attribute; assembly/class/method merge |
| `ITestCaseMetadata.Traits` (`TestContext.Current.TestCase.Traits`) | `Xunit.Sdk` | property | `Examples/Attributes/AttributeTests.cs` | `CategoryTests` | ≤3.x | `IReadOnlyDictionary<string, IReadOnlyCollection<string>>` |

### Fixtures and lifecycle

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `AssemblyFixtureAttribute(Type)` | `Xunit` | attribute | `Examples/Fixtures/TelemetryFixture.cs` | `FixtureTests.AssemblyFixtureIsASingleton` | ≤3.x | generic `AssemblyFixture<T>` compiles but trips analyzer xUnit1041 in 2.0.0 |
| `IAsyncLifetime.InitializeAsync/DisposeAsync` (`ValueTask`) | `Xunit` | interface | `Examples/Fixtures/TelemetryFixture.cs` | `FixtureTests.AssemblyFixtureIsInitializedBeforeTests` | ≤3.x | |
| `INotifyTestLifecycle.OnTestStarting/OnTestFinished(IXunitTest)` | `Xunit.v3` | interface | `Examples/Fixtures/TelemetryFixture.cs` | `FixtureTests.FixtureObservesTheRunningTest` | 4.0.0 | assembly fixture receives every test in the assembly |
| Primary-constructor fixture injection | `Xunit` | — | `Examples/Fixtures/FixtureTests.cs` | `FixtureTests` | ≤3.x | |
| `INotifyTestAssemblyLifecycle` | `Xunit.v3` | interface | `Examples/Fixtures/TelemetryFixture.cs` | `FixtureTests.AssemblyFixtureSawTheAssemblyStart` | 4.0.0 | on the assembly fixture |
| `INotifyTestCollectionLifecycle` | `Xunit.v3` | interface | `Examples/Fixtures/LifecycleLedger.cs` | `CollectionLifecycleTests` | 4.0.0 | on a collection fixture |
| `INotifyTestClassLifecycleAsync` / `INotifyTestMethodLifecycle` / `INotifyTestCaseLifecycleAsync` / `INotifyTestLifecycleAsync` | `Xunit.v3` | interface | `Examples/Fixtures/LifecycleLedger.cs` | `LifecycleTests` (order: class → method → case → test) | 4.0.0 | sync and async variants mixed on one class fixture |
| `ICollectionFixture<T>` + `CollectionDefinitionAttribute` | `Xunit` | interface, attribute | `Examples/Fixtures/LifecycleLedger.cs` | `CollectionLifecycleTests` | ≤3.x | |

### Data attributes and theory metadata

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `DataAttribute.GetData(MethodInfo, DisposalTracker)` | `Xunit.v3` | method | `Examples/Data/InlineCsvDataAttribute.cs` | `DataTests.CsvRowsAreTypedAndLabeled` | ≤3.x | returns `ValueTask<IReadOnlyCollection<ITheoryDataRow>>` |
| `DataAttribute.SupportsDiscoveryEnumeration()` | `Xunit.v3` | method | `Examples/Data/InlineCsvDataAttribute.cs` | `--xunit-list tests` shows 3 rows | ≤3.x | |
| `DataAttribute.ExplicitAsNullable` / `TimeoutAsNullable` | `Xunit.v3` | property | `Examples/Data/InlineCsvDataAttribute.cs` | `DataTests` | 4.0.0 | `Explicit`/`Timeout` getters are obsolete |
| `TheoryDataRow(params object?[])` + `Label/Skip/SkipType/SkipUnless/SkipWhen/TestDisplayName/Timeout/Explicit` | `Xunit` | class | `Examples/Data/InlineCsvDataAttribute.cs` | `DataTests.CsvRowsAreTypedAndLabeled` | 4.0.0 (`Label`) | |
| `TestPipelineException` | `Xunit.Sdk` | class | `Examples/Data/InlineCsvDataAttribute.cs` | — (arity guard) | ≤3.x | |
| `ITheoryDataRow.Label` → `ITestMetadata.TestLabel` | `Xunit` / `Xunit.Sdk` | property | `Examples/Data/DataTests.cs` | `DataTests.RowLabelBecomesTestLabel` | 4.0.0 | display name `Method [label]` |
| `ITheoryDataRow.DisableParallelization` | `Xunit` | property | `Examples/Data/DataTests.cs` | `DataTests.RowLabelBecomesTestLabel` | 4.0.0 | per-row opt-out |
| `TheoryAttribute.IncludeTestCaseIndex` | `Xunit` | property | `Examples/Data/DataTests.cs` | `DataTests.RowLabelBecomesTestLabel` (`Method_001 [label]`) | 4.0.0 | 1-based, zero-padded |
| `Assert.All(collection, action, throwIfEmpty)` | `Xunit` | method | `Examples/Data/DataTests.cs` | `DataTests.StrictAllRejectsEmptyCollections` | 4.0.0 | release notes call it "strict"; parameter is `throwIfEmpty` |
| `Xunit.Sdk.AllException` | `Xunit.Sdk` | class | `Examples/Data/DataTests.cs` | same | ≤3.x | |
| `DataAttribute.Label` | `Xunit.v3` | property | `Examples/Data/DataTests.cs` | `DataTests.AttributeLabelAndMemberTypeReachEveryRow` | 4.0.0 | one label for every row of the attribute |
| `ITypeAwareDataAttribute.MemberType` | `Xunit.v3` | property | `Examples/Data/InlineCsvDataAttribute.cs` | same | 4.0.0 | set by the framework before `GetData` |
| `TheoryDataRow.Traits` (`Dictionary<string, HashSet<string>>`) | `Xunit` | property | `Examples/Data/InlineCsvDataAttribute.cs` | same (`SourceType` trait) | ≤3.x | |

### Output and context

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `CaptureConsoleAttribute` | `Xunit` | attribute | `Examples/Output/OutputTests.cs` | `OutputTests.ConsoleOutputIsCapturedIntoTestOutput` | ≤3.x | assembly-level |
| `ITestOutputHelper.Output` | `Xunit` | property | `Examples/Output/OutputTests.cs` | same | ≤3.x | |
| `TestContext.Current.TestOutputHelper` | `Xunit` | property | `Examples/Output/OutputTests.cs` | `OutputTests.InjectedHelperIsTheContextHelper` | ≤3.x | same instance as ctor-injected helper |
| `TestContext.Current.Test.TestDisplayName` / `.TestLabel` | `Xunit` | property | `Examples/Data/DataTests.cs`, `Examples/Fixtures/FixtureTests.cs` | `DataTests`, `FixtureTests`, `RetryTests` | 4.0.0 (`TestLabel`) | |
| `TestContext.Current.TestCase` | `Xunit` | property | `Examples/Attributes/AttributeTests.cs` | `CategoryTests` | ≤3.x | |
| `CaptureTraceAttribute` | `Xunit` | attribute | `Examples/AssemblyPolicies.cs` | `OutputTests.TraceOutputIsCapturedIntoTestOutput` | ≤3.x | assembly-level |
| `TestContext.AddAttachment(name, string)` / `AddAttachment(name, byte[], mediaType)` + `Attachments` | `Xunit` | method, property | `Examples/Output/OutputTests.cs` | `OutputTests.AttachmentsWarningsAndStorageLiveOnTheContext`; TRX `<ResultFile>` `notes.txt` / `blob.bin` | ≤3.x | |
| `TestAttachment.AsString()` / `AsByteArray()` | `Xunit.Sdk` | method | `Examples/Output/OutputTests.cs` | same | ≤3.x | `AsByteArray()` returns `(ByteArray, MediaType)` |
| `TestContext.AddWarning` + `Warnings` | `Xunit` | method, property | `Examples/Output/OutputTests.cs` | same | ≤3.x | |
| `TestContext.KeyValueStorage` | `Xunit` | property | `Examples/Output/OutputTests.cs` | same | ≤3.x | per-test scratch dictionary |

### Runner / CLI (Microsoft.Testing.Platform 2.3.3)

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `global.json` `test.runner = Microsoft.Testing.Platform` | — | config | `global.json` | `dotnet test` | SDK 10 | replaces `TestingPlatformDotnetTestSupport` |
| `--xunit-list tests\|full\|traits` | — | cli | — | display-name checks | 4.0.0 | |
| `--xunit-diagnostics on` | — | cli | — | retry diagnostics | 4.0.0 | |
| `--parallel none\|collections\|all` | — | cli | — | ordering stability | 4.0.0 | default `collections` |
| `--filter-trait Name=Value` | — | cli | — | `CategoryTests` | 4.0.0 | |
| `--report-xunit-trx` / `--report-xunit-trx-filename` / `--results-directory` | — | cli | — | attachments appear as `<ResultFile>` | 4.0.0 | renamed from `-report-trx` |
| `--filter-class` / `--filter-method` (wildcards) | — | cli | — | targeted runs | 4.0.0 | |
| `xunit.runner.json` `printMaxEnumerableLength` / `printMaxStringLength` | — | config | `xunit.runner.json` | `PrintLengthTests` | 4.0.0 | assembly default for assertion output |

### Assertion output

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `Assert.OverrideMaxEnumerableLength(int?)` | `Xunit` | method | `Examples/Assertions/PrintLengthTests.cs` | `PrintLengthTests.EnumerableOutputIsTruncatedToTheOverride` (`···` in message) | 4.0.0 | per-test override |
| `Assert.OverrideMaxStringLength(int?)` | `Xunit` | method | `Examples/Assertions/PrintLengthTests.cs` | `PrintLengthTests.StringOutputIsTruncatedToTheOverride` | 4.0.0 | |
| `Xunit.Sdk.EqualException` | `Xunit.Sdk` | class | `Examples/Assertions/PrintLengthTests.cs` | same | ≤3.x | |

### Telemetry — traces, metrics, logs (`Examples/Telemetry`)

xUnit.net v3 has no ActivitySource, Meter, or logging bridge of its own; these rows are the patterns real xunit.v3 suites (Aspire, modelcontextprotocol/csharp-sdk) use to observe the three signals, adapted to 4.0.0. Packages: `OpenTelemetry 1.18.0`, `OpenTelemetry.Exporter.InMemory 1.18.0`, `Microsoft.Extensions.Diagnostics.Testing 10.9.0`, `Microsoft.Extensions.Logging 10.0.11`.

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| Per-test root span via `BeforeAfterTestAttribute` (`TracedTestAttribute`) | `Xunit.v3` / `System.Diagnostics` | attribute | `Examples/Telemetry/TracedTestAttribute.cs` | `TracesTests.TracedTestSpanIsTheParentOfSpansFromTheCodeUnderTest` | 4.0.0 | tags `test.case.name`, `xunit.test.id`; self-registers an `ActivityListener` |
| `ActivityListener` (`ShouldListenTo`, `Sample`, `ActivityStopped`) | `System.Diagnostics` | class | `Examples/Telemetry/TracesTests.cs` | `TracesTests.ActivityListenerObservesSpansWithoutTheOtelSdk` | — | no SDK needed |
| `Sdk.CreateTracerProviderBuilder().AddSource().AddProcessor().AddInMemoryExporter(List<Activity>)` + `ForceFlush` | `OpenTelemetry.Trace` | method | `Examples/Telemetry/TracesTests.cs` | `TracesTests.InMemoryExporterCapturesSpansTaggedWithTheTestId` | — | |
| `BaseProcessor<Activity>.OnStart` stamping `TestContext.Current.Test.UniqueID` (`XunitTestIdProcessor`) | `OpenTelemetry` | class | `Examples/Telemetry/TracedTestAttribute.cs` | same | 4.0.0 | xunit analogue of TUnit's correlation processor |
| `MetricCollector<T>(Meter, instrumentName)` / `GetMeasurementSnapshot()` / `LastMeasurement.Tags` | `Microsoft.Extensions.Diagnostics.Metrics.Testing` | class | `Examples/Telemetry/MetricsTests.cs` | `MetricsTests.MetricCollectorRecordsEveryMeasurementWithItsTags` | — | |
| `Sdk.CreateMeterProviderBuilder().AddMeter().AddInMemoryExporter(List<Metric>)`, `Metric.GetMetricPoints()`, `MetricPoint.GetSumLong()` | `OpenTelemetry.Metrics` | method | `Examples/Telemetry/MetricsTests.cs` | `MetricsTests.InMemoryMetricExporterAggregatesTheCounter` | — | |
| `FakeLogger<T>` / `Collector.GetSnapshot()` / `FakeLogRecord.StructuredState` | `Microsoft.Extensions.Logging.Testing` | class | `Examples/Telemetry/LogsTests.cs` | `LogsTests.FakeLoggerRecordsStructuredState` | — | assert on structured state |
| `ILoggerProvider` → `ITestOutputHelper` bridge (`TestOutputLoggerProvider`) | `Microsoft.Extensions.Logging` / `Xunit` | class | `Examples/Telemetry/TestOutputLoggerProvider.cs` | `LogsTests.LoggerOutputIsBridgedIntoTheTestOutput` | — | shape of modelcontextprotocol/csharp-sdk `tests/Common/Utils/XunitLoggerProvider.cs` |
| `[LoggerMessage]` source-generated logging in the SUT | `Microsoft.Extensions.Logging` | attribute | `Examples/Telemetry/OrderService.cs` | `LogsTests` | — | |
| `ActivitySource` / `Meter` / `Counter<long>` in the SUT | `System.Diagnostics(.Metrics)` | class | `Examples/Telemetry/OrderService.cs` | `TracesTests`, `MetricsTests` | — | process-wide instruments ⇒ tests run in a non-parallel collection |

### Native AOT (`xunitv3.template.Aot`, `xunitv3.template.Aot.Generator`)

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `xunit.v3.aot.mtp-v2` 4.0.0 + `<PublishAot>true</PublishAot>` | — | config | `xunitv3.template.Aot/xunitv3.template.Aot.csproj` | `dotnet publish -c Release -r osx-arm64` → 13.5 MB Mach-O arm64 binary, 3/3 pass | 4.0.0 | in-process runner reports `[native/osx-arm64]` |
| `xunit.v3.generatorutility` 4.0.0: `TraitGenerator(attributeTypeName).GetTraitValues(AttributeData)` + `[Generator(LanguageNames.CSharp)]` | `Xunit.Generators` | class | `xunitv3.template.Aot.Generator/Generators/CategoryAttributeGenerator.cs` | `AotTests.GeneratedTraitsMergeAcrossAssemblyClassAndMethod` | 4.0.0 | attribute matched by full name; plain `Attribute`, no `ITraitAttribute` |
| `<ProjectReference … OutputItemType="Analyzer" ReferenceOutputAssembly="false" />` | MSBuild | config | `xunitv3.template.Aot/xunitv3.template.Aot.csproj` | build | — | generator project is `netstandard2.0`, `IsRoslynComponent` |
| `[Fact]` / `[Theory]` + `[InlineData]` under codegen | `Xunit` | attribute | `xunitv3.template.Aot/AotTests.cs` | `AotTests` (3 tests) | 4.0.0 | |

### Present in 4.0.0, no example yet

`TestContext.CancelCurrentTest` · `TestContext.GetFixture<T>` · `ITestMethodGenerator` / `DataAttributeGenerator` / `CodeGenTestCaseBase` / `ISelfExecutingCodeGenTestCase` (AOT test-case and data-source generators — see `xunit/samples.xunit` `v3/AotRetryFact`, `v3/AotCsvDataSource`) · `--report-xunit-ctrf` / `-html` / `-junit` / `-nunit` · `xunit.runner.json` `parallelMode`

### Divergences from release notes / docs (verified)

- `Assert.All` / `AllAsync`: the new parameter is `throwIfEmpty`, not `strict`.
- `[assembly: AssemblyFixture<T>]` is legal but xunit.analyzers 2.0.0 reports xUnit1041 on the consuming class; use `AssemblyFixture(typeof(T))`.
- `TheoryDiscoverer.CreateTestCasesForDataRow` without the `index` parameter is obsolete and will be removed in the next major.
