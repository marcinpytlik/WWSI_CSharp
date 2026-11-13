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
    public void ExhaustedRetryWithTimeoutIsTransient() =>
        Assert.Equal(FailureKind.Transient,
            FailureClassifier.Classify(
                new Microsoft.EntityFrameworkCore.Storage.RetryLimitExceededException(
                    "retry exhausted", new TimeoutException("timeout"))));

    [Fact]
    public void NonSqlInvalidOperationIsNotAnUnavailableDependency() =>
        Assert.False(FailureClassifier.IsTransientSqlDependency(
            new InvalidOperationException("programming error")));

    [Fact]
    public void WrappedTimeoutWithoutSqlIsNotAClassifiedSqlDependency() =>
        Assert.False(FailureClassifier.IsTransientSqlDependency(
            new InvalidOperationException("other failure", new TimeoutException())));

    [Theory]
    [InlineData(11001)]
    [InlineData(10061)]
    [InlineData(-2)]
    [InlineData(1205)]
    public void SqlConnectivityErrorsAreTransient(int number) =>
        Assert.True(FailureClassifier.IsTransientSqlErrorNumber(number));

    [Theory]
    [InlineData(2627)]
    [InlineData(2601)]
    [InlineData(207)]
    public void SqlConstraintAndSchemaErrorsAreNotTransient(int number) =>
        Assert.False(FailureClassifier.IsTransientSqlErrorNumber(number));

    [Fact]
    public void ValidationErrorIsPermanent() =>
        Assert.Equal(FailureKind.Permanent,
            FailureClassifier.Classify(new RequestValidationException("bad input")));
}
