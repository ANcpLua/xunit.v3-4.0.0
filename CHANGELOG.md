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

### Output and context

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `CaptureConsoleAttribute` | `Xunit` | attribute | `Examples/Output/OutputTests.cs` | `OutputTests.ConsoleOutputIsCapturedIntoTestOutput` | ≤3.x | assembly-level |
| `ITestOutputHelper.Output` | `Xunit` | property | `Examples/Output/OutputTests.cs` | same | ≤3.x | |
| `TestContext.Current.TestOutputHelper` | `Xunit` | property | `Examples/Output/OutputTests.cs` | `OutputTests.InjectedHelperIsTheContextHelper` | ≤3.x | same instance as ctor-injected helper |
| `TestContext.Current.Test.TestDisplayName` / `.TestLabel` | `Xunit` | property | `Examples/Data/DataTests.cs`, `Examples/Fixtures/FixtureTests.cs` | `DataTests`, `FixtureTests`, `RetryTests` | 4.0.0 (`TestLabel`) | |
| `TestContext.Current.TestCase` | `Xunit` | property | `Examples/Attributes/AttributeTests.cs` | `CategoryTests` | ≤3.x | |

### Runner / CLI (Microsoft.Testing.Platform 2.3.3)

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `global.json` `test.runner = Microsoft.Testing.Platform` | — | config | `global.json` | `dotnet test` | SDK 10 | replaces `TestingPlatformDotnetTestSupport` |
| `--xunit-list tests\|full\|traits` | — | cli | — | display-name checks | 4.0.0 | |
| `--xunit-diagnostics on` | — | cli | — | retry diagnostics | 4.0.0 | |
| `--parallel none\|collections\|all` | — | cli | — | ordering stability | 4.0.0 | default `collections` |
| `--filter-trait Name=Value` | — | cli | — | `CategoryTests` | 4.0.0 | |

### Present in 4.0.0, no example yet

`ParallelizationAttribute` (assembly: `Mode`, `Algorithm`, `MaxThreads`) · `ITestClassOrderer` / `TestClassOrdererAttribute` · `ITestCollectionOrderer` / `TestCollectionOrdererAttribute` · `INotifyTestAssembly|Collection|Class|Method|CaseLifecycle[Async]` · `INotifyTestLifecycleAsync` · `FactAttribute.DisableParallelization` · `CollectionDefinitionAttribute.DisableParallelization` · `DataAttribute.Label` · `ITypeAwareDataAttribute` · `CaptureTraceAttribute` · `TestContext.Attachments/KeyValueStorage/Warnings` · `xunit.runner.json` `parallelMode`, `printMaxEnumerableLength`, `printMaxStringLength` · Native AOT (`xunit.v3.aot.mtp-v2`, `xunit.v3.extensibility.core.aot`, `xunit.v3.generatorutility`, `ITestMethodGenerator`, `DataAttributeGenerator`, `TraitGenerator`) — see `xunit/samples.xunit` `v3/Aot*`

### Divergences from release notes / docs (verified)

- `Assert.All` / `AllAsync`: the new parameter is `throwIfEmpty`, not `strict`.
- `[assembly: AssemblyFixture<T>]` is legal but xunit.analyzers 2.0.0 reports xUnit1041 on the consuming class; use `AssemblyFixture(typeof(T))`.
- `TheoryDiscoverer.CreateTestCasesForDataRow` without the `index` parameter is obsolete and will be removed in the next major.
