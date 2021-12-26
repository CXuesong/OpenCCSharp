using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using OpenCCSharp.UnitTest.Benchmarks;

var config = DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(100));
var results = BenchmarkRunner.Run(typeof(ConverterBenchmarks).Assembly, config);
if (results.Any(r => r.HasCriticalValidationErrors)) return -1;
return 0;
