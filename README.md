# Exceptions vs Result in .NET 10: benchmarks

Companion code for the codewithmukesh article **"Are Exceptions Slow in .NET 10? I Benchmarked a Full Request"**.

The same code runs on .NET 8 and .NET 10 side by side with BenchmarkDotNet, so you can see what the runtime and ASP.NET Core changes did to the cost of a thrown exception.

## What is measured

| Class | What it does |
|-------|--------------|
| `FailurePathBenchmarks` | One failed validation reported to a caller 1 or 10 calls up the stack. `Result<T>` return vs throw + catch. Success paths included. `NoInlining` keeps the frames real. |
| `HttpBenchmarks` | A full request through the ASP.NET Core middleware pipeline (in-memory `TestServer`). A 400 from a `Result` + `Results.Problem` vs a 400 from a thrown `ValidationException` caught by `UseExceptionHandler` + an `IExceptionHandler` writing ProblemDetails. Logging providers cleared. |
| `HttpLoggingBenchmarks` | Same endpoints with a logger that formats every message and exception it receives, like a console or file logger would. Counts the log entries written. |

## Environment

```
BenchmarkDotNet v0.15.8, Windows 11 (10.0.26300.9974)
Intel Core Ultra 9 275HX 2.70GHz, 1 CPU, 24 logical and 24 physical cores
.NET SDK 10.0.404
  .NET 10 : .NET 10.0.15, X64 RyuJIT x86-64-v3
  .NET 8  : .NET 8.0.34, X64 RyuJIT x86-64-v3
```

## Results

Full BenchmarkDotNet reports are in [`results/`](results/).

### Failure path (micro)

| Method | Runtime | Depth | Mean | Allocated |
|--------|---------|-------|-----:|----------:|
| Result_Failure | .NET 10 | 1 | 6.5 ns | - |
| Exception_Failure | .NET 10 | 1 | 1,459 ns | 496 B |
| Result_Failure | .NET 8 | 1 | 3.7 ns | - |
| Exception_Failure | .NET 8 | 1 | 3,678 ns | 568 B |
| Result_Failure | .NET 10 | 10 | 24.7 ns | - |
| Exception_Failure | .NET 10 | 10 | 4,015 ns | 1,344 B |
| Result_Failure | .NET 8 | 10 | 18.5 ns | - |
| Exception_Failure | .NET 8 | 10 | 8,975 ns | 1,768 B |

### Full request

| Method | Runtime | Mean | Allocated |
|--------|---------|-----:|----------:|
| Success_Request (200) | .NET 10 | 6.16 μs | 8.62 KB |
| Result_400 | .NET 10 | 8.11 μs | 10.26 KB |
| Exception_400 | .NET 10 | 15.66 μs | 11.38 KB |
| Success_Request (200) | .NET 8 | 6.87 μs | 9.09 KB |
| Result_400 | .NET 8 | 9.31 μs | 9.64 KB |
| Exception_400 | .NET 8 | 26.22 μs | 11.88 KB |

### Full request with a logger attached

| Method | Runtime | Mean | Allocated | Log entries written |
|--------|---------|-----:|----------:|--------------------:|
| Result_400 | .NET 10 | 8.71 μs | 10.91 KB | 0 |
| Exception_400 | .NET 10 | 19.31 μs | 12.08 KB | 0 |
| Result_400 | .NET 8 | 10.09 μs | 10.82 KB | 0 |
| Exception_400 | .NET 8 | 34.44 μs | 19.77 KB | 237,672 |

On .NET 8 the exception handler middleware logs every exception at `Error` level, with the stack trace, even when your `IExceptionHandler` handles it. On .NET 10 it does not log when the handler returns `true` (breaking change in .NET 10, controlled by `ExceptionHandlerOptions.SuppressDiagnosticsCallback`).

Numbers from different benchmark classes come from separate runs. Compare rows inside one table.

## Run it

```bash
cd src/Benchmarks
dotnet run -c Release -f net10.0 -- --filter "*"
```

You need the .NET 10 SDK and the .NET 8 runtime installed. A full run takes about 25 minutes.

Run a single class:

```bash
dotnet run -c Release -f net10.0 -- --filter "*HttpBenchmarks*"
```
