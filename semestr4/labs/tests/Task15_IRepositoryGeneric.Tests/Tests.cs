using Xunit;

namespace Task15_IRepositoryGeneric.Tests;

public sealed class InMemoryRepositoryTests
{
    private sealed record P(int Id, string Name);

    [Fact]
    public void InMemoryRepository_Works()
    {
        var repo = new Task15_IRepositoryGeneric.InMemoryRepository<P, int>(p => p.Id);
        repo.Add(new P(1, "A"));
        repo.Add(new P(2, "B"));

        Assert.Equal("B", repo.GetById(2)?.Name);
        Assert.True(repo.Remove(1));
        Assert.Null(repo.GetById(1));
    }
}
