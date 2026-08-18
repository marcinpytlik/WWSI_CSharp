using Xunit;
using Moq;

namespace Task22_MoqRepositoryTarget.Tests;

public sealed class MoqRepositoryTests
{
    [Fact]
    public void OrderService_Total_UsesRepository()
    {
        var repo = new Mock<Task22_MoqRepositoryTarget.IRepository<Task22_MoqRepositoryTarget.Product>>();
        repo.Setup(r => r.GetAll()).Returns(new List<Task22_MoqRepositoryTarget.Product>
        {
            new(1,"A",10m),
            new(2,"B",5m)
        });

        var svc = new Task22_MoqRepositoryTarget.OrderService(repo.Object);
        var total = svc.Total();

        Assert.Equal(15m, total);
        repo.Verify(r => r.GetAll(), Times.Once);
    }
}
