using System.Text.Json;
using QueueCacheDemo.Errors;
using Xunit;

public sealed class FailureClassifierTests
{
    [Fact]
    public void InvalidCommandIsPermanent() =>
        Assert.Equal(FailureKind.Permanent,
            FailureClassifier.Classify(new InvalidCommandException("bad schema")));

    [Fact]
    public void MalformedJsonIsPermanent() =>
        Assert.Equal(FailureKind.Permanent,
            FailureClassifier.Classify(new JsonException("invalid JSON")));

    [Fact]
    public void TimeoutIsTransient() =>
        Assert.Equal(FailureKind.Transient,
            FailureClassifier.Classify(new TimeoutException("timed out")));

    [Fact]
    public void UnavailableDependencyIsTransient() =>
        Assert.Equal(FailureKind.Transient,
            FailureClassifier.Classify(new DependencyUnavailableException("SQL Server")));

    [Fact]
    public void ValidationErrorIsPermanent() =>
        Assert.Equal(FailureKind.Permanent,
            FailureClassifier.Classify(new RequestValidationException("bad input")));
}
