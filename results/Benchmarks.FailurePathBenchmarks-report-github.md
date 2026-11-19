```

BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9974)
Intel Core Ultra 9 275HX 2.70GHz, 1 CPU, 24 logical and 24 physical cores
.NET SDK 10.0.404
  [Host]  : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v3
  .NET 10 : .NET 10.0.15 (10.0.15, 10.0.1526.53008), X64 RyuJIT x86-64-v3
  .NET 8  : .NET 8.0.34 (8.0.34, 8.0.3426.52217), X64 RyuJIT x86-64-v3


```
| Method            | Job     | Runtime   | Depth | Mean         | Error       | StdDev      | Ratio  | RatioSD | Gen0   | Allocated | Alloc Ratio |
|------------------ |-------- |---------- |------ |-------------:|------------:|------------:|-------:|--------:|-------:|----------:|------------:|
| **Result_Failure**    | **.NET 10** | **.NET 10.0** | **1**     |     **6.500 ns** |   **0.1467 ns** |   **0.1630 ns** |   **1.00** |    **0.03** |      **-** |         **-** |          **NA** |
| Exception_Failure | .NET 10 | .NET 10.0 | 1     | 1,459.417 ns |  29.1590 ns |  37.9150 ns | 224.66 |    7.86 | 0.0248 |     496 B |          NA |
| Result_Success    | .NET 10 | .NET 10.0 | 1     |     2.148 ns |   0.0587 ns |   0.0842 ns |   0.33 |    0.02 |      - |         - |          NA |
| Exception_Success | .NET 10 | .NET 10.0 | 1     |     1.486 ns |   0.0313 ns |   0.0261 ns |   0.23 |    0.01 |      - |         - |          NA |
|                   |         |           |       |              |             |             |        |         |        |           |             |
| Result_Failure    | .NET 8  | .NET 8.0  | 1     |     3.689 ns |   0.0850 ns |   0.0753 ns |   1.00 |    0.03 |      - |         - |          NA |
| Exception_Failure | .NET 8  | .NET 8.0  | 1     | 3,677.537 ns |  30.6856 ns |  25.6239 ns | 997.17 |   20.53 | 0.0305 |     568 B |          NA |
| Result_Success    | .NET 8  | .NET 8.0  | 1     |     1.894 ns |   0.0253 ns |   0.0211 ns |   0.51 |    0.01 |      - |         - |          NA |
| Exception_Success | .NET 8  | .NET 8.0  | 1     |     1.379 ns |   0.0123 ns |   0.0103 ns |   0.37 |    0.01 |      - |         - |          NA |
|                   |         |           |       |              |             |             |        |         |        |           |             |
| **Result_Failure**    | **.NET 10** | **.NET 10.0** | **10**    |    **24.680 ns** |   **0.3832 ns** |   **0.3585 ns** |   **1.00** |    **0.02** |      **-** |         **-** |          **NA** |
| Exception_Failure | .NET 10 | .NET 10.0 | 10    | 4,015.049 ns |  78.0139 ns |  72.9742 ns | 162.72 |    3.66 | 0.0687 |    1344 B |          NA |
| Result_Success    | .NET 10 | .NET 10.0 | 10    |    10.057 ns |   0.2198 ns |   0.6163 ns |   0.41 |    0.03 |      - |         - |          NA |
| Exception_Success | .NET 10 | .NET 10.0 | 10    |     7.855 ns |   0.1229 ns |   0.1262 ns |   0.32 |    0.01 |      - |         - |          NA |
|                   |         |           |       |              |             |             |        |         |        |           |             |
| Result_Failure    | .NET 8  | .NET 8.0  | 10    |    18.527 ns |   0.2429 ns |   0.2153 ns |   1.00 |    0.02 |      - |         - |          NA |
| Exception_Failure | .NET 8  | .NET 8.0  | 10    | 8,974.942 ns | 150.4363 ns | 173.2427 ns | 484.50 |   10.60 | 0.0916 |    1768 B |          NA |
| Result_Success    | .NET 8  | .NET 8.0  | 10    |     9.355 ns |   0.2027 ns |   0.1991 ns |   0.51 |    0.01 |      - |         - |          NA |
| Exception_Success | .NET 8  | .NET 8.0  | 10    |     7.807 ns |   0.0865 ns |   0.0722 ns |   0.42 |    0.01 |      - |         - |          NA |
