using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Benchmarks;

// Same endpoints as HttpBenchmarks, but with a logger that formats every message it gets,
// the way a console or file logger would. This shows what the exception handler logs.
[MemoryDiagnoser]
public class HttpLoggingBenchmarks
{
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders();
        builder.Logging.AddProvider(new FormattingLoggerProvider());
        builder.Logging.SetMinimumLevel(LogLevel.Warning);
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();

        _app = builder.Build();
        _app.UseExceptionHandler();

        _app.MapGet("/orders/result", (int quantity) =>
        {
            Result<int> result = quantity <= 0
                ? new Error("order.quantity", "Quantity must be greater than zero.")
                : quantity;

            return result.IsSuccess
                ? Results.Ok(result.Value)
                : Results.Problem(
                    statusCode: StatusCodes.Status400BadRequest,
                    title: "Validation failed",
                    detail: result.Error!.Message);
        });

        _app.MapGet("/orders/exception", (int quantity) =>
        {
            if (quantity <= 0)
            {
                throw new ValidationException("order.quantity", "Quantity must be greater than zero.");
            }

            return Results.Ok(quantity);
        });

        await _app.StartAsync();
        _client = _app.GetTestClient();
    }

    [GlobalCleanup]
    public async Task Cleanup()
    {
        Console.WriteLine($"// Log entries written: {FormattingLoggerProvider.Count}");
        await _app.DisposeAsync();
    }

    [Benchmark(Baseline = true)]
    public Task<HttpResponseMessage> Result_400() => _client.GetAsync("/orders/result?quantity=0");

    [Benchmark]
    public Task<HttpResponseMessage> Exception_400() => _client.GetAsync("/orders/exception?quantity=0");
}

// Formats the message and the exception (stack trace included), then throws the text away.
public sealed class FormattingLoggerProvider : ILoggerProvider
{
    public static long Count;

    public ILogger CreateLogger(string categoryName) => new FormattingLogger();

    public void Dispose()
    {
    }

    private sealed class FormattingLogger : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            var text = formatter(state, exception) + Environment.NewLine + exception;
            if (text.Length > 0)
            {
                Interlocked.Increment(ref Count);
            }
        }
    }
}
