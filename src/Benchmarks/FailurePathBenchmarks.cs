using System.Runtime.CompilerServices;
using BenchmarkDotNet.Attributes;

namespace Benchmarks;

// One failed validation, reported to a caller that is Depth calls up the stack.
[MemoryDiagnoser]
public class FailurePathBenchmarks
{
    private static readonly Error QuantityError = new("order.quantity", "Quantity must be greater than zero.");

    [Params(1, 10)]
    public int Depth;

    [Benchmark(Baseline = true)]
    public int Result_Failure()
    {
        var result = ValidateWithResult(Depth, quantity: 0);
        return result.IsSuccess ? result.Value : -1;
    }

    [Benchmark]
    public int Exception_Failure()
    {
        try
        {
            return ValidateWithException(Depth, quantity: 0);
        }
        catch (ValidationException)
        {
            return -1;
        }
    }

    [Benchmark]
    public int Result_Success() => ValidateWithResult(Depth, quantity: 3).Value;

    [Benchmark]
    public int Exception_Success() => ValidateWithException(Depth, quantity: 3);

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static Result<int> ValidateWithResult(int depth, int quantity)
    {
        if (depth == 0)
        {
            return quantity <= 0 ? QuantityError : quantity;
        }

        var inner = ValidateWithResult(depth - 1, quantity);
        return inner.IsSuccess ? inner.Value + 1 : inner;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static int ValidateWithException(int depth, int quantity)
    {
        if (depth == 0)
        {
            return quantity <= 0
                ? throw new ValidationException(QuantityError.Code, QuantityError.Message)
                : quantity;
        }

        return ValidateWithException(depth - 1, quantity) + 1;
    }
}
