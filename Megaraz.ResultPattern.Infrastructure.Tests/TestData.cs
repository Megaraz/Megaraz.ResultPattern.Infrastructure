using Megaraz.ResultPattern;

namespace Megaraz.ResultPattern.Infrastructure.Tests;

internal static class TestData
{
    internal static ErrorContext Context { get; } = new(OperationType.Get, "Account");
}
