# OpenCC#

[![Build Status](https://github.com/CXuesong/OpenCCSharp/actions/workflows/OpenCCSharp.yml/badge.svg?branch=main)](https://github.com/CXuesong/OpenCCSharp/actions/workflows/OpenCCSharp.yml) | [![Build Status](https://github.com/CXuesong/OpenCCSharp/actions/workflows/Benchmark.yml/badge.svg?branch=main)](https://github.com/CXuesong/OpenCCSharp/actions/workflows/Benchmark.yml) 

OpenCCSharp is a (WIP) .NET 8 library for Chinese text script variant conversion. This project is inspired by [BYVoid/OpenCC](https://github.com/BYVoid/OpenCC), and used similar approach in word segmentation. This project also used the same conversion dictionary in OpenCC project.

> While the v0.0.1-int.6 of the packages support both .NET 6.0 and .NET 8.0, further versions might drop support for .NET 6.0 as this .NET SDK reaching its EOL.

>  This library is in its very early INT phase and its API is still subject to drastic changes. There is also much optimizations yet to be employed.

To install the NuGet package with preset Chinese variant converters, use

```powershell
#  Package Management Console
Install-Package CXuesong.OpenCCSharp.Presets -Prerelease
#  .NET CLI
dotnet add package CXuesong.OpenCCSharp.Presets --prerelease
```

For now there is no detailed usage documented here, but you can getting started by playing with `OpenCCSharp.Presets.ChineseConversionPresets`.

This project also powers [OpenCC# WebApp](https://github.com/CXuesong/OpenCCSharp.WebApp), a serverless Chinese variant conversion web app, with the help of WASM.

## Packages

* [CXuesong.OpenCCSharp.Conversion](https://www.nuget.org/packages/CXuesong.OpenCCSharp.Conversion) | ![NuGet version (CXuesong.OpenCCSharp.Conversion)](https://img.shields.io/nuget/vpre/CXuesong.OpenCCSharp.Conversion.svg?style=flat-square) ![NuGet version (CXuesong.OpenCCSharp.Conversion)](https://img.shields.io/nuget/dt/CXuesong.OpenCCSharp.Conversion.svg?style=flat-square)
    * Provides object model and data structure for variant conversion.
* [CXuesong.OpenCCSharp.Presets](https://www.nuget.org/packages/CXuesong.OpenCCSharp.Presets) | ![NuGet version (CXuesong.OpenCCSharp.Presets)](https://img.shields.io/nuget/vpre/CXuesong.OpenCCSharp.Presets.svg?style=flat-square)![NuGet version (CXuesong.OpenCCSharp)](https://img.shields.io/nuget/dt/CXuesong.OpenCCSharp.Presets.svg?style=flat-square)
    * Provides pre-configured Chinese variant converters.

## Build

With Visual Studio 2022 or VSCode + .NET SDK 6.

## Benchmark

Currently, this library looks generally [3x faster](https://github.com/BYVoid/OpenCC#benchmark-%E5%9F%BA%E6%BA%96%E6%B8%AC%E8%A9%A6) than the OpenCC library implemented in native C++ on Ubuntu.

<details>
<summary>Open CC benchmark output</summary>

Compared benchmarks: BM_Convert/x v.s. DupSequence

https://github.com/BYVoid/OpenCC/actions/runs/10229279965/job/28302747342

```
1: Test command: /home/runner/work/OpenCC/OpenCC/build/perf/src/benchmark/performance
1: Working Directory: /home/runner/work/OpenCC/OpenCC/build/perf/src/benchmark
1: Test timeout computed to be: 1500
1: 2024-08-03T16:08:09+00:00
1: Running /home/runner/work/OpenCC/OpenCC/build/perf/src/benchmark/performance
1: Run on (4 X 3218.97 MHz CPU s)
1: CPU Caches:
1:   L1 Data 32 KiB (x2)
1:   L1 Instruction 32 KiB (x2)
1:   L2 Unified 512 KiB (x2)
1:   L3 Unified 32768 KiB (x1)
1: Load Average: 0.92, 0.41, 0.15
1: ------------------------------------------------------------------
1: Benchmark                        Time             CPU   Iterations
1: ------------------------------------------------------------------
1: BM_Initialization/hk2s        1.09 ms         1.09 ms          638
1: BM_Initialization/hk2t       0.152 ms        0.151 ms         4618
1: BM_Initialization/jp2t       0.235 ms        0.235 ms         2974
1: BM_Initialization/s2hk        30.2 ms         30.2 ms           23
1: BM_Initialization/s2t         29.6 ms         29.6 ms           24
1: BM_Initialization/s2tw        29.7 ms         29.7 ms           24
1: BM_Initialization/s2twp       30.0 ms         30.0 ms           23
1: BM_Initialization/t2hk       0.072 ms        0.072 ms         9975
1: BM_Initialization/t2jp       0.185 ms        0.184 ms         3795
1: BM_Initialization/t2s        0.959 ms        0.959 ms          731
1: BM_Initialization/tw2s        1.01 ms         1.01 ms          692
1: BM_Initialization/tw2sp       1.24 ms         1.24 ms          563
1: BM_Initialization/tw2t       0.097 ms        0.097 ms         7209
1: BM_Convert2M                   385 ms          385 ms            2
1: BM_Convert/100               0.757 ms        0.757 ms          929
1: BM_Convert/1000               7.74 ms         7.74 ms           90
1: BM_Convert/10000              79.3 ms         79.3 ms            9
1: BM_Convert/100000              837 ms          837 ms            1
1/1 Test #1: BenchmarkTest ....................   Passed   17.54 sec

100% tests passed, 0 tests failed out of 1
```
</details>

### Test cases

* OpenCCTest: Measuring the time and memory footprint when executing original OpenCC tests (`BM_Initialization/*`), excl. loading preset conversion rules. 
* BulkConversionTest: Measuring the time and memory footprint when executing original OpenCC tests (`BM_Convert*`), excl. loading preset conversion rules. 
* PresetLoadTest: Measuring the time and memory footprint when loading the preset conversion rules with `ChineseConversionPresets.GetConverterAsync`.

### Legends

```
  Mean        : Arithmetic mean of all measurements
  Error       : Half of 99.9% confidence interval
  StdDev      : Standard deviation of all measurements
  Gen 0       : GC Generation 0 collects per 1000 operations
  Gen 1       : GC Generation 1 collects per 1000 operations
  Gen 2       : GC Generation 2 collects per 1000 operations
  Allocated   : Allocated memory per single operation (managed only, inclusive, 1KB = 1024B)
  1 ms        : 1 Millisecond (0.001 sec)
```

### Windows

``` ini
BenchmarkDotNet v0.14.0, Windows 10 (10.0.20348.2582) (Hyper-V)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.303
  [Host]     : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
```
| Method     | arguments                    | Mean      | Error     | StdDev    | Gen0   | Allocated |
|----------- |----------------------------- |----------:|----------:|----------:|-------:|----------:|
| **OpenCCTest** | **HK -&gt; Hans [hk2s]**            |  **3.886 μs** | **0.0205 μs** | **0.0182 μs** | **0.0458** |     **832 B** |
| **OpenCCTest** | **HK -&gt; Hant [hk2t]**            |  **1.936 μs** | **0.0038 μs** | **0.0033 μs** | **0.0191** |     **344 B** |
| **OpenCCTest** | **Hans -&gt; HK [s2hk]**            |  **4.298 μs** | **0.0220 μs** | **0.0184 μs** | **0.0610** |    **1112 B** |
| **OpenCCTest** | **Hans -&gt; Hant [s2t]**           | **32.660 μs** | **0.0617 μs** | **0.0547 μs** | **0.1221** |    **2312 B** |
| **OpenCCTest** | **Hans -&gt; TW [s2twp]**           | **18.815 μs** | **0.0367 μs** | **0.0287 μs** | **0.0916** |    **1752 B** |
| **OpenCCTest** | **Hant -&gt; HK [t2hk]**            |  **1.228 μs** | **0.0029 μs** | **0.0026 μs** | **0.0267** |     **472 B** |
| **OpenCCTest** | **Hant -&gt; Hans [t2s]**           | **11.406 μs** | **0.0163 μs** | **0.0145 μs** | **0.0305** |     **512 B** |
| **OpenCCTest** | **Kyujitai -&gt; Shinjitai [t2jp]** |  **2.961 μs** | **0.0116 μs** | **0.0103 μs** | **0.0381** |     **672 B** |
| **OpenCCTest** | **Shinjitai -&gt; Kyujitai [jp2t]** |  **5.546 μs** | **0.0102 μs** | **0.0080 μs** | **0.0381** |     **680 B** |
| **OpenCCTest** | **TW -&gt; Hans [tw2sp]**           | **21.923 μs** | **0.0240 μs** | **0.0224 μs** | **0.0610** |    **1512 B** |
| **OpenCCTest** | **TW -&gt; Hant [tw2t]**            |  **8.032 μs** | **0.0214 μs** | **0.0189 μs** | **0.0305** |     **536 B** |

| Method             | arguments                                       | Mean         | Error       | StdDev    | Gen0    | Gen1    | Gen2    | Allocated  |
|------------------- |------------------------------------------------ |-------------:|------------:|----------:|--------:|--------:|--------:|-----------:|
| **BulkConversionTest** | **DupSequence 100 Hant -&gt; Hans (2700 chars)**       |     **223.3 μs** |     **0.19 μs** |   **0.16 μs** |  **0.2441** |       **-** |       **-** |    **5.41 KB** |
| **BulkConversionTest** | **DupSequence 1000 Hant -&gt; Hans (27000 chars)**     |   **2,226.0 μs** |     **3.84 μs** |   **3.40 μs** |       **-** |       **-** |       **-** |   **52.88 KB** |
| **BulkConversionTest** | **DupSequence 10000 Hant -&gt; Hans (270000 chars)**   |  **23,196.8 μs** |   **104.14 μs** |  **97.41 μs** | **93.7500** | **93.7500** | **93.7500** |  **528.29 KB** |
| **BulkConversionTest** | **DupSequence 100000 Hant -&gt; Hans (2700000 chars)** | **221,964.1 μs** | **1,001.78 μs** | **937.07 μs** |       **-** |       **-** |       **-** | **5273.91 KB** |
| **BulkConversionTest** | **Zuozhuan Hans -&gt; Hant (629833 chars)**            | **132,030.2 μs** |   **292.90 μs** | **273.98 μs** |       **-** |       **-** |       **-** | **1230.53 KB** |

| Method         | fromVariant | toVariant | Mean         | Error      | StdDev     | Gen0     | Gen1     | Gen2    | Allocated  |
|--------------- |------------ |---------- |-------------:|-----------:|-----------:|---------:|---------:|--------:|-----------:|
| **PresetLoadTest** | **Hans**        | **Hant**      | **23,272.27 μs** | **318.163 μs** | **297.610 μs** | **468.7500** | **281.2500** | **62.5000** | **8437.38 KB** |
| **PresetLoadTest** | **Hans**        | **HK**        | **22,383.29 μs** | **149.199 μs** | **132.261 μs** | **468.7500** | **218.7500** | **62.5000** | **8443.69 KB** |
| **PresetLoadTest** | **Hans**        | **TW**        | **25,596.93 μs** | **165.259 μs** | **154.583 μs** | **531.2500** | **437.5000** | **62.5000** | **8570.07 KB** |
| **PresetLoadTest** | **Hant**        | **Hans**      |    **953.79 μs** |   **4.662 μs** |   **3.893 μs** |  **19.5313** |   **7.8125** |       **-** |  **340.11 KB** |
| **PresetLoadTest** | **Hant**        | **HK**        |     **49.87 μs** |   **0.404 μs** |   **0.338 μs** |   **0.4883** |        **-** |       **-** |    **8.83 KB** |
| **PresetLoadTest** | **Hant**        | **TW**        |    **328.96 μs** |   **2.975 μs** |   **2.323 μs** |   **7.8125** |   **1.9531** |       **-** |  **134.37 KB** |
| **PresetLoadTest** | **Kyujitai**    | **Shinjitai** |    **110.13 μs** |   **1.419 μs** |   **1.258 μs** |   **1.4648** |        **-** |       **-** |   **29.24 KB** |
| **PresetLoadTest** | **Shinjitai**   | **Kyujitai**  |    **188.91 μs** |   **2.178 μs** |   **1.930 μs** |   **3.9063** |        **-** |       **-** |    **64.1 KB** |
| **PresetLoadTest** | **HK**          | **Hans**      |  **1,068.99 μs** |   **4.964 μs** |   **4.145 μs** |  **15.6250** |        **-** |       **-** |  **377.47 KB** |
| **PresetLoadTest** | **HK**          | **Hant**      |    **142.35 μs** |   **2.732 μs** |   **3.455 μs** |   **2.4414** |        **-** |       **-** |   **39.96 KB** |
| **PresetLoadTest** | **TW**          | **Hans**      |  **1,315.98 μs** |  **17.470 μs** |  **14.588 μs** |  **23.4375** |   **7.8125** |       **-** |  **491.61 KB** |
| **PresetLoadTest** | **TW**          | **Hant**      |    **370.78 μs** |   **7.387 μs** |   **6.909 μs** |   **7.8125** |   **1.9531** |       **-** |  **154.05 KB** |

### Linux (Ubuntu)

``` ini
BenchmarkDotNet v0.14.0, Ubuntu 22.04.4 LTS (Jammy Jellyfish)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.303
  [Host]     : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.7 (8.0.724.31311), X64 RyuJIT AVX2
```
| Method     | arguments                    | Mean      | Error     | StdDev    | Gen0   | Allocated |
|----------- |----------------------------- |----------:|----------:|----------:|-------:|----------:|
| **OpenCCTest** | **HK -&gt; Hans [hk2s]**            |  **4.059 μs** | **0.0228 μs** | **0.0191 μs** | **0.0076** |     **832 B** |
| **OpenCCTest** | **HK -&gt; Hant [hk2t]**            |  **1.917 μs** | **0.0113 μs** | **0.0094 μs** | **0.0038** |     **344 B** |
| **OpenCCTest** | **Hans -&gt; HK [s2hk]**            |  **4.252 μs** | **0.0074 μs** | **0.0062 μs** | **0.0076** |    **1112 B** |
| **OpenCCTest** | **Hans -&gt; Hant [s2t]**           | **31.786 μs** | **0.2386 μs** | **0.2232 μs** |      **-** |    **2312 B** |
| **OpenCCTest** | **Hans -&gt; TW [s2twp]**           | **17.514 μs** | **0.0852 μs** | **0.0755 μs** |      **-** |    **1752 B** |
| **OpenCCTest** | **Hant -&gt; HK [t2hk]**            |  **1.188 μs** | **0.0060 μs** | **0.0056 μs** | **0.0038** |     **472 B** |
| **OpenCCTest** | **Hant -&gt; Hans [t2s]**           | **10.442 μs** | **0.0553 μs** | **0.0518 μs** |      **-** |     **512 B** |
| **OpenCCTest** | **Kyujitai -&gt; Shinjitai [t2jp]** |  **3.051 μs** | **0.0106 μs** | **0.0099 μs** | **0.0076** |     **672 B** |
| **OpenCCTest** | **Shinjitai -&gt; Kyujitai [jp2t]** |  **5.683 μs** | **0.0255 μs** | **0.0239 μs** | **0.0076** |     **680 B** |
| **OpenCCTest** | **TW -&gt; Hans [tw2sp]**           | **20.195 μs** | **0.0904 μs** | **0.0801 μs** |      **-** |    **1512 B** |
| **OpenCCTest** | **TW -&gt; Hant [tw2t]**            |  **7.689 μs** | **0.0734 μs** | **0.0687 μs** |      **-** |     **536 B** |

| Method             | arguments                                       | Mean         | Error       | StdDev      | Gen0    | Gen1    | Gen2    | Allocated  |
|------------------- |------------------------------------------------ |-------------:|------------:|------------:|--------:|--------:|--------:|-----------:|
| **BulkConversionTest** | **DupSequence 100 Hant -&gt; Hans (2700 chars)**       |     **219.0 μs** |     **0.99 μs** |     **0.88 μs** |       **-** |       **-** |       **-** |    **5.41 KB** |
| **BulkConversionTest** | **DupSequence 1000 Hant -&gt; Hans (27000 chars)**     |   **2,208.0 μs** |     **9.52 μs** |     **8.44 μs** |       **-** |       **-** |       **-** |   **52.88 KB** |
| **BulkConversionTest** | **DupSequence 10000 Hant -&gt; Hans (270000 chars)**   |  **22,112.3 μs** |    **74.00 μs** |    **65.60 μs** | **93.7500** | **93.7500** | **93.7500** |  **527.85 KB** |
| **BulkConversionTest** | **DupSequence 100000 Hant -&gt; Hans (2700000 chars)** | **222,195.1 μs** | **2,005.28 μs** | **1,875.74 μs** |       **-** |       **-** |       **-** | **5273.91 KB** |
| **BulkConversionTest** | **Zuozhuan Hans -&gt; Hant (629833 chars)**            | **127,994.6 μs** |   **430.43 μs** |   **402.63 μs** |       **-** |       **-** |       **-** | **1230.53 KB** |

| Method         | fromVariant | toVariant | Mean         | Error      | StdDev     | Median       | Gen0     | Gen1    | Gen2    | Allocated  |
|--------------- |------------ |---------- |-------------:|-----------:|-----------:|-------------:|---------:|--------:|--------:|-----------:|
| **PresetLoadTest** | **Hans**        | **Hant**      | **22,507.68 μs** | **223.761 μs** | **209.306 μs** | **22,432.45 μs** |  **93.7500** | **93.7500** | **62.5000** | **8424.13 KB** |
| **PresetLoadTest** | **Hans**        | **HK**        | **22,539.12 μs** |  **91.660 μs** |  **81.254 μs** | **22,516.46 μs** |  **93.7500** | **93.7500** | **62.5000** | **8430.44 KB** |
| **PresetLoadTest** | **Hans**        | **TW**        | **24,090.32 μs** |  **68.093 μs** |  **56.860 μs** | **24,080.34 μs** | **156.2500** | **62.5000** | **62.5000** | **8556.82 KB** |
| **PresetLoadTest** | **Hant**        | **Hans**      |  **1,028.78 μs** |  **18.979 μs** |  **17.753 μs** |  **1,027.27 μs** |   **3.9063** |       **-** |       **-** |  **339.55 KB** |
| **PresetLoadTest** | **Hant**        | **HK**        |     **34.86 μs** |   **1.558 μs** |   **4.595 μs** |     **32.40 μs** |        **-** |       **-** |       **-** |     **8.8 KB** |
| **PresetLoadTest** | **Hant**        | **TW**        |    **352.72 μs** |   **6.252 μs** |  **13.853 μs** |    **349.84 μs** |        **-** |       **-** |       **-** |  **134.09 KB** |
| **PresetLoadTest** | **Kyujitai**    | **Shinjitai** |    **126.33 μs** |   **2.880 μs** |   **8.493 μs** |    **127.69 μs** |        **-** |       **-** |       **-** |    **29.2 KB** |
| **PresetLoadTest** | **Shinjitai**   | **Kyujitai**  |    **218.44 μs** |   **4.341 μs** |  **10.649 μs** |    **219.17 μs** |        **-** |       **-** |       **-** |   **63.96 KB** |
| **PresetLoadTest** | **HK**          | **Hans**      |  **1,149.91 μs** |  **18.179 μs** |  **20.935 μs** |  **1,144.17 μs** |   **3.9063** |       **-** |       **-** |  **376.92 KB** |
| **PresetLoadTest** | **HK**          | **Hant**      |    **139.83 μs** |   **2.965 μs** |   **8.695 μs** |    **138.59 μs** |        **-** |       **-** |       **-** |   **39.89 KB** |
| **PresetLoadTest** | **TW**          | **Hans**      |  **1,373.88 μs** |  **21.879 μs** |  **24.318 μs** |  **1,368.58 μs** |   **5.8594** |  **1.9531** |       **-** |  **490.77 KB** |
| **PresetLoadTest** | **TW**          | **Hant**      |    **373.56 μs** |   **7.362 μs** |  **14.531 μs** |    **367.88 μs** |        **-** |       **-** |       **-** |  **153.77 KB** |
