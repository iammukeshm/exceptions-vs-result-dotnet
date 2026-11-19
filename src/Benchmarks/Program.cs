using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

// Every benchmark runs on .NET 8 and .NET 10, so the runtime change is visible side by side.
var config = DefaultConfig.Instance
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core80).WithId(".NET 8"))
    .AddJob(Job.Default.WithRuntime(CoreRuntime.Core10_0).WithId(".NET 10"));

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
