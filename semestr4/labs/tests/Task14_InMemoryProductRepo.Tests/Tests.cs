using Xunit;

namespace Task14_InMemoryProductRepo.Tests;

public sealed class InMemoryProductRepositoryTests
{
    [Fact]
    public void Repo_AddGetRemove_Works()
    {
        var repo = new Task14_InMemoryProductRepo.InMemoryProductRepository();
        repo.Add(new Task14_InMemoryProductRepo.Product(1, "A", 10m));
        repo.Add(new Task14_InMemoryProductRepo.Product(2, "B", 20m));

        Assert.Equal("A", repo.GetById(1)?.Name);
        Assert.Equal(2, repo.GetAll().Count);
        Assert.True(repo.Remove(1));
        Assert.Null(repo.GetById(1));
    }
}
