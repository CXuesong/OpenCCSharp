using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using OpenCCSharp.UnitTest.Benchmarks;

var config = DefaultConfig.Instance.WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(100));
BenchmarkSwitcher.FromAssembly(typeof(ConverterBenchmarks).Assembly).Run(args, config);
