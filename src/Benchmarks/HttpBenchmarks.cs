using BenchmarkDotNet.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Benchmarks;

// A full ASP.NET Core request through the real middleware pipeline (in-memory TestServer).
[MemoryDiagnoser]
public class HttpBenchmarks
{
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    [GlobalSetup]
    public async Task Setup()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseTestServer();
        builder.Logging.ClearProviders();
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
    public async Task Cleanup() => await _app.DisposeAsync();

    [Benchmark(Baseline = true)]
    public Task<HttpResponseMessage> Success_Request() => _client.GetAsync("/orders/result?quantity=3");

    [Benchmark]
    public Task<HttpResponseMessage> Result_400() => _client.GetAsync("/orders/result?quantity=0");

    [Benchmark]
    public Task<HttpResponseMessage> Exception_400() => _client.GetAsync("/orders/exception?quantity=0");
}

public sealed class ValidationExceptionHandler(IProblemDetailsService problemDetails) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validation)
        {
            return false;
        }

        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = context,
            ProblemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation failed",
                Detail = validation.Message
            }
        });
    }
}
