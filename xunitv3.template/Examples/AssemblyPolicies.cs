using Xunit.Sdk;
using Xunit.v3;
using xunitv3.template.Examples.Ordering;

// Assembly-wide knobs. Mode is deliberately left unset so `--parallel` on the command line still decides;
// the algorithm is pinned to the default explicitly to make the choice visible in source.
[assembly: Parallelization(Algorithm = ParallelAlgorithm.Conservative)]
[assembly: TestCollectionOrderer(typeof(RecordingCollectionOrderer))]
[assembly: CaptureTrace]
