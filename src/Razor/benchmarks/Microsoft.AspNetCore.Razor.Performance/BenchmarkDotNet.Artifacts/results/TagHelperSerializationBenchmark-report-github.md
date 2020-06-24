``` ini

BenchmarkDotNet=v0.10.13, OS=Windows 10.0.19041
Intel Core i7-8700 CPU 3.20GHz (Coffee Lake), 1 CPU, 12 logical cores and 6 physical cores
.NET Core SDK=5.0.100-preview.6.20310.4
  [Host] : .NET Core 5.0.0-preview.6.20305.6 (CoreCLR 5.0.20.30506, CoreFX 5.0.20.30506), 64bit RyuJIT

Toolchain=InProcessToolchain  RunStrategy=Throughput  

```
|                                    Method |      Mean |     Error |    StdDev |   Op/s |    Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|------------------------------------------ |----------:|----------:|----------:|-------:|---------:|--------:|--------:|----------:|
| &#39;Razor TagHelper Roundtrip Serialization&#39; | 13.885 ms | 0.1916 ms | 0.1792 ms |  72.02 | 156.2500 | 93.7500 | 31.2500 |  10.66 MB |
|           &#39;Razor TagHelper Serialization&#39; |  2.863 ms | 0.0567 ms | 0.1079 ms | 349.32 |  35.1563 | 35.1563 | 35.1563 |    1.2 MB |
|         &#39;Razor TagHelper Deserialization&#39; | 10.989 ms | 0.0455 ms | 0.0404 ms |  91.00 | 125.0000 | 31.2500 |       - |   9.45 MB |
