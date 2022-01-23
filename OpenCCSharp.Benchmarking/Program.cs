using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using OpenCCSharp.UnitTest.Benchmarks;

var config = DefaultConfig.Instance
    .WithSummaryStyle(SummaryStyle.Default.WithMaxParameterColumnWidth(100))
    .AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig()));
BenchmarkSwitcher.FromAssembly(typeof(ConverterBenchmarks).Assembly).Run(args, config);
