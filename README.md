# OpenCC#

[![Build Status](https://github.com/CXuesong/OpenCCSharp/actions/workflows/OpenCCSharp.yml/badge.svg?branch=main)](https://github.com/CXuesong/OpenCCSharp/actions/workflows/OpenCCSharp.yml) | [![Build Status](https://github.com/CXuesong/OpenCCSharp/actions/workflows/Benchmark.yml/badge.svg?branch=main)](https://github.com/CXuesong/OpenCCSharp/actions/workflows/Benchmark.yml) 

OpenCCSharp is a (WIP) .NET 6 library for Chinese handwritten script conversion. This project is inspired by [BYVoid/OpenCC](https://github.com/BYVoid/OpenCC), and used similar approach in word segmentation. This project also used the same conversion dictionary in OpenCC project.

>  This library is in its very early INT phase and its API is still subject to drastic changes. There is also much optimizations yet to be employed.

To install the NuGet package with preset Chinese variant converters, use

```powershell
#  Package Management Console
Install-Package CXuesong.OpenCCSharp.Presets -Prerelease
#  .NET CLI
dotnet add package CXuesong.OpenCCSharp.Presets --prerelease
```

For now there is no detailed usage documented here, but you can getting started by playing with `OpenCCSharp.Presets.ChineseConversionPresets`.

## Packages

* [CXuesong.OpenCCSharp.Conversion](https://www.nuget.org/packages/CXuesong.OpenCCSharp.Conversion) | ![NuGet version (CXuesong.OpenCCSharp.Conversion)](https://img.shields.io/nuget/vpre/CXuesong.OpenCCSharp.Conversion.svg?style=flat-square) ![NuGet version (CXuesong.OpenCCSharp.Conversion)](https://img.shields.io/nuget/dt/CXuesong.OpenCCSharp.Conversion.svg?style=flat-square)
    * Provides object model and data structure for variant conversion.
* [CXuesong.OpenCCSharp.Presets](https://www.nuget.org/packages/CXuesong.OpenCCSharp.Presets) | ![NuGet version (CXuesong.OpenCCSharp.Presets)](https://img.shields.io/nuget/vpre/CXuesong.OpenCCSharp.Presets.svg?style=flat-square)![NuGet version (CXuesong.OpenCCSharp)](https://img.shields.io/nuget/dt/CXuesong.OpenCCSharp.Presets.svg?style=flat-square)
    * Provides pre-configured Chinese variant converters.

## Build

With Visual Studio 2022 or VSCode + .NET SDK 6.

## Benchmark

This project is still in its very early phase and prefix matching is achieved by binary search in sorted array. Tries are yet to be adopted.

### Windows

``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.17763.2366 (1809/October2018Update/Redstone5)
Intel Xeon Platinum 8171M CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  Job-SNBGJP : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT

InvocationCount=1  UnrollFactor=1  

```
| Method     | arguments              |        Mean |     Error |    StdDev |      Median |
| ---------- | ---------------------- | ----------: | --------: | --------: | ----------: |
| OpenCCTest | HK -&gt; Hans [hk2s]   |   111.24 μs |  1.245 μs |  1.104 μs |   111.00 μs |
| OpenCCTest | Hani -&gt; Hant [jp2t] |   112.53 μs |  2.233 μs |  3.911 μs |   112.90 μs |
| OpenCCTest | Hans -&gt; HK [s2hk]   |   107.94 μs |  1.937 μs |  1.717 μs |   107.70 μs |
| OpenCCTest | Hans -&gt; Hant [s2t]  | 1,840.44 μs | 35.573 μs | 43.687 μs | 1,813.05 μs |
| OpenCCTest | Hans -&gt; TW [s2twp]  |   827.50 μs |  2.197 μs |  1.835 μs |   827.60 μs |
| OpenCCTest | Hant -&gt; Hani [t2jp] |    40.51 μs |  0.586 μs |  0.520 μs |    40.50 μs |
| OpenCCTest | Hant -&gt; Hans [t2s]  |   613.11 μs |  5.188 μs |  4.599 μs |   614.50 μs |
| OpenCCTest | TW -&gt; Hans [tw2sp]  |   877.91 μs | 17.471 μs | 27.200 μs |   862.25 μs |

| Method             | arguments                                          |         Mean |     Error |    StdDev |
| ------------------ | -------------------------------------------------- | -----------: | --------: | --------: |
| BulkConversionTest | DupSequence 100 Hant -&gt; Hans (2700 chars)       |     12.64 ms |  0.034 ms |  0.029 ms |
| BulkConversionTest | DupSequence 1000 Hant -&gt; Hans (27000 chars)     |    131.39 ms |  1.664 ms |  1.557 ms |
| BulkConversionTest | DupSequence 10000 Hant -&gt; Hans (270000 chars)   |  1,282.75 ms |  7.610 ms |  7.118 ms |
| BulkConversionTest | DupSequence 100000 Hant -&gt; Hans (2700000 chars) | 13,017.96 ms |  9.047 ms |  8.020 ms |
| BulkConversionTest | Zuozhuan Hans -&gt; Hant (629833 chars)            |  6,226.50 ms | 11.960 ms | 11.187 ms |

### Linux (Ubuntu)

``` ini

BenchmarkDotNet=v0.13.1, OS=ubuntu 20.04
Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.101
  [Host]     : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT
  Job-IGSTAA : .NET 6.0.1 (6.0.121.56705), X64 RyuJIT

InvocationCount=1  UnrollFactor=1  

```
| Method     | arguments              |        Mean |    Error |    StdDev |
| ---------- | ---------------------- | ----------: | -------: | --------: |
| OpenCCTest | HK -&gt; Hans [hk2s]   |    87.01 μs | 1.666 μs |  1.782 μs |
| OpenCCTest | Hani -&gt; Hant [jp2t] |    92.09 μs | 1.209 μs |  1.072 μs |
| OpenCCTest | Hans -&gt; HK [s2hk]   |   104.87 μs | 1.814 μs |  1.697 μs |
| OpenCCTest | Hans -&gt; Hant [s2t]  | 1,229.39 μs | 7.528 μs | 13.380 μs |
| OpenCCTest | Hans -&gt; TW [s2twp]  |   548.76 μs | 4.093 μs |  7.586 μs |
| OpenCCTest | Hant -&gt; Hani [t2jp] |    40.45 μs | 0.768 μs |  0.718 μs |
| OpenCCTest | Hant -&gt; Hans [t2s]  |   501.85 μs | 6.451 μs |  5.387 μs |
| OpenCCTest | TW -&gt; Hans [tw2sp]  |   591.57 μs | 8.091 μs | 15.393 μs |

| Method             | arguments                                          |         Mean |     Error |    StdDev |
| ------------------ | -------------------------------------------------- | -----------: | --------: | --------: |
| BulkConversionTest | DupSequence 100 Hant -&gt; Hans (2700 chars)       |     8.622 ms | 0.0709 ms | 0.0788 ms |
| BulkConversionTest | DupSequence 1000 Hant -&gt; Hans (27000 chars)     |    86.948 ms | 0.2389 ms | 0.2235 ms |
| BulkConversionTest | DupSequence 10000 Hant -&gt; Hans (270000 chars)   |   850.413 ms | 0.3474 ms | 0.2901 ms |
| BulkConversionTest | DupSequence 100000 Hant -&gt; Hans (2700000 chars) | 8,500.346 ms | 7.3097 ms | 6.8375 ms |
| BulkConversionTest | Zuozhuan Hans -&gt; Hant (629833 chars)            | 4,208.511 ms | 6.0026 ms | 5.6148 ms |
