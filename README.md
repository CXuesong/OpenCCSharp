# OpenCC#

[![Build Status](https://github.com/CXuesong/OpenCCSharp/actions/workflows/OpenCCSharp.yml/badge.svg?branch=main)](https://github.com/CXuesong/OpenCCSharp/actions/workflows/OpenCCSharp.yml) | [![Build Status](https://github.com/CXuesong/OpenCCSharp/actions/workflows/Benchmark.yml/badge.svg?branch=main)](https://github.com/CXuesong/OpenCCSharp/actions/workflows/Benchmark.yml) 

OpenCCSharp is a (WIP) .NET 6 library for Chinese text script variant conversion. This project is inspired by [BYVoid/OpenCC](https://github.com/BYVoid/OpenCC), and used similar approach in word segmentation. This project also used the same conversion dictionary in OpenCC project.

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

Currently, this library looks generally [2x faster](https://github.com/BYVoid/OpenCC#benchmark-%E5%9F%BA%E6%BA%96%E6%B8%AC%E8%A9%A6) than the OpenCC library implemented in native C++.

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
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.17763.2452 (1809/October2018Update/Redstone5)
Intel Xeon Platinum 8171M CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  DefaultJob : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
```
| Method         | arguments                  |           Mean |         Error |        StdDev |      Gen 0 |   Allocated |
| -------------- | -------------------------- | -------------: | ------------: | ------------: | ---------: | ----------: |
| **OpenCCTest** | **HK -&gt; Hans [hk2s]**   |   **9.307 ??s** | **0.0993 ??s** | **0.0880 ??s** | **0.0305** |   **832 B** |
| **OpenCCTest** | **Hani -&gt; Hant [jp2t]** |  **14.629 ??s** | **0.1833 ??s** | **0.1714 ??s** | **0.0305** |   **680 B** |
| **OpenCCTest** | **Hans -&gt; HK [s2hk]**   |   **9.792 ??s** | **0.1168 ??s** | **0.0975 ??s** | **0.0458** | **1,112 B** |
| **OpenCCTest** | **Hans -&gt; Hant [s2t]**  | **101.216 ??s** | **0.9262 ??s** | **0.8663 ??s** |      **-** | **1,776 B** |
| **OpenCCTest** | **Hans -&gt; TW [s2twp]**  |  **62.318 ??s** | **1.2070 ??s** | **1.1290 ??s** |      **-** | **1,208 B** |
| **OpenCCTest** | **Hant -&gt; Hani [t2jp]** |   **7.321 ??s** | **0.0641 ??s** | **0.0599 ??s** | **0.0305** |   **672 B** |
| **OpenCCTest** | **Hant -&gt; Hans [t2s]**  |  **32.833 ??s** | **0.3501 ??s** | **0.3275 ??s** |      **-** |   **360 B** |
| **OpenCCTest** | **TW -&gt; Hans [tw2sp]**  |  **82.299 ??s** | **0.6781 ??s** | **0.6343 ??s** |      **-** | **1,512 B** |

| Method                 | arguments                                              |             Mean |           Error |          StdDev |    Allocated |
| ---------------------- | ------------------------------------------------------ | ---------------: | --------------: | --------------: | -----------: |
| **BulkConversionTest** | **DupSequence 100 Hant -&gt; Hans (2700 chars)**       |     **540.7 ??s** |     **6.25 ??s** |     **5.84 ??s** |     **5 KB** |
| **BulkConversionTest** | **DupSequence 1000 Hant -&gt; Hans (27000 chars)**     |   **5,442.9 ??s** |    **39.22 ??s** |    **34.77 ??s** |    **53 KB** |
| **BulkConversionTest** | **DupSequence 10000 Hant -&gt; Hans (270000 chars)**   |  **55,595.3 ??s** | **1,060.95 ??s** | **1,041.99 ??s** |   **528 KB** |
| **BulkConversionTest** | **DupSequence 100000 Hant -&gt; Hans (2700000 chars)** | **557,006.0 ??s** | **8,258.62 ??s** | **7,725.12 ??s** | **5,277 KB** |
| **BulkConversionTest** | **Zuozhuan Hans -&gt; Hant (629833 chars)**            | **295,094.7 ??s** | **5,475.38 ??s** | **5,121.68 ??s** | **1,231 KB** |

| Method             | fromVariant | toVariant |          Mean |         Error |        StdDev |        Gen 0 |        Gen 1 |        Gen 2 |     Allocated |
| ------------------ | ----------- | --------- | ------------: | ------------: | ------------: | -----------: | -----------: | -----------: | ------------: |
| **PresetLoadTest** | **Hans**    | **Hant**  | **69.522 ms** | **0.8456 ms** | **0.7061 ms** | **875.0000** | **500.0000** | **125.0000** | **14,699 KB** |
| **PresetLoadTest** | **Hans**    | **Hani**  | **69.549 ms** | **0.6370 ms** | **0.5958 ms** | **875.0000** | **500.0000** | **125.0000** | **14,755 KB** |
| **PresetLoadTest** | **Hans**    | **HK**    | **71.000 ms** | **1.4150 ms** | **1.4531 ms** | **875.0000** | **500.0000** | **125.0000** | **14,718 KB** |
| **PresetLoadTest** | **Hans**    | **TW**    | **72.064 ms** | **1.1314 ms** | **1.0583 ms** | **857.1429** | **428.5714** | **142.8571** | **14,953 KB** |
| **PresetLoadTest** | **Hant**    | **Hans**  |  **3.884 ms** | **0.0451 ms** | **0.0400 ms** |  **27.3438** |  **11.7188** |   **3.9063** |    **566 KB** |
| **PresetLoadTest** | **Hant**    | **Hani**  |  **1.576 ms** | **0.0235 ms** | **0.0196 ms** |   **3.9063** |   **1.9531** |        **-** |     **87 KB** |
| **PresetLoadTest** | **HK**      | **Hans**  |  **4.276 ms** | **0.0771 ms** | **0.0791 ms** |  **31.2500** |   **7.8125** |        **-** |    **652 KB** |
| **PresetLoadTest** | **TW**      | **Hans**  |  **5.231 ms** | **0.0498 ms** | **0.0465 ms** |  **46.8750** |  **15.6250** |   **7.8125** |    **891 KB** |

### Linux (Ubuntu)

``` ini
BenchmarkDotNet=v0.13.1, OS=ubuntu 20.04
Intel Xeon CPU E5-2673 v4 2.30GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  DefaultJob : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
```
| Method         | arguments               |          Mean |         Error |        StdDev |      Gen 0 |   Allocated |
| -------------- | ----------------------- | ------------: | ------------: | ------------: | ---------: | ----------: |
| **OpenCCTest** | **HK -> Hans [hk2s]**   |  **8.163 ??s** | **0.1603 ??s** | **0.2807 ??s** | **0.0305** |   **832 B** |
| **OpenCCTest** | **Hani -> Hant [jp2t]** | **12.021 ??s** | **0.2151 ??s** | **0.1907 ??s** | **0.0153** |   **680 B** |
| **OpenCCTest** | **Hans -> HK [s2hk]**   |  **7.959 ??s** | **0.1292 ??s** | **0.1145 ??s** | **0.0305** | **1,112 B** |
| **OpenCCTest** | **Hans -> Hant [s2t]**  | **78.375 ??s** | **1.5562 ??s** | **2.7661 ??s** |      **-** | **1,776 B** |
| **OpenCCTest** | **Hans -> TW [s2twp]**  | **49.265 ??s** | **0.9629 ??s** | **1.1826 ??s** |      **-** | **1,208 B** |
| **OpenCCTest** | **Hant -> Hani [t2jp]** |  **5.848 ??s** | **0.0869 ??s** | **0.0813 ??s** | **0.0229** |   **672 B** |
| **OpenCCTest** | **Hant -> Hans [t2s]**  | **25.582 ??s** | **0.4244 ??s** | **0.3970 ??s** |      **-** |   **360 B** |
| **OpenCCTest** | **TW -> Hans [tw2sp]**  | **65.888 ??s** | **1.2371 ??s** | **1.7342 ??s** |      **-** | **1,512 B** |

| Method                 | arguments                                           |             Mean |           Error |           StdDev |    Allocated |
| ---------------------- | --------------------------------------------------- | ---------------: | --------------: | ---------------: | -----------: |
| **BulkConversionTest** | **DupSequence 100 Hant -> Hans (2700 chars)**       |     **442.1 ??s** |     **8.19 ??s** |      **7.66 ??s** |     **5 KB** |
| **BulkConversionTest** | **DupSequence 1000 Hant -> Hans (27000 chars)**     |   **4,338.8 ??s** |    **84.25 ??s** |    **136.04 ??s** |    **53 KB** |
| **BulkConversionTest** | **DupSequence 10000 Hant -> Hans (270000 chars)**   |  **41,423.6 ??s** |   **555.38 ??s** |    **492.33 ??s** |   **528 KB** |
| **BulkConversionTest** | **DupSequence 100000 Hant -> Hans (2700000 chars)** | **439,468.6 ??s** | **8,666.34 ??s** | **10,316.66 ??s** | **5,275 KB** |
| **BulkConversionTest** | **Zuozhuan Hans -> Hant (629833 chars)**            | **241,657.4 ??s** | **4,802.48 ??s** |  **5,337.94 ??s** | **1,231 KB** |

| Method             | fromVariant | toVariant |          Mean |         Error |        StdDev |        Gen 0 |        Gen 1 |        Gen 2 |     Allocated |
| ------------------ | ----------- | --------- | ------------: | ------------: | ------------: | -----------: | -----------: | -----------: | ------------: |
| **PresetLoadTest** | **Hans**    | **Hant**  | **55.125 ms** | **1.0912 ms** | **1.9112 ms** | **666.6667** | **333.3333** | **111.1111** | **14,697 KB** |
| **PresetLoadTest** | **Hans**    | **Hani**  | **55.438 ms** | **0.9631 ms** | **0.8043 ms** | **625.0000** | **375.0000** | **125.0000** | **14,752 KB** |
| **PresetLoadTest** | **Hans**    | **HK**    | **55.132 ms** | **1.0585 ms** | **1.8538 ms** | **625.0000** | **375.0000** | **125.0000** | **14,717 KB** |
| **PresetLoadTest** | **Hans**    | **TW**    | **55.516 ms** | **1.0844 ms** | **1.6559 ms** | **666.6667** | **333.3333** | **111.1111** | **14,954 KB** |
| **PresetLoadTest** | **Hant**    | **Hans**  |  **3.531 ms** | **0.0695 ms** | **0.0800 ms** |  **19.5313** |   **7.8125** |        **-** |    **567 KB** |
| **PresetLoadTest** | **Hant**    | **Hani**  |  **1.543 ms** | **0.0282 ms** | **0.0264 ms** |   **1.9531** |        **-** |        **-** |     **88 KB** |
| **PresetLoadTest** | **HK**      | **Hans**  |  **3.871 ms** | **0.0593 ms** | **0.0526 ms** |  **23.4375** |   **7.8125** |        **-** |    **654 KB** |
| **PresetLoadTest** | **TW**      | **Hans**  |  **4.440 ms** | **0.0678 ms** | **0.0601 ms** |  **31.2500** |   **7.8125** |        **-** |    **895 KB** |
