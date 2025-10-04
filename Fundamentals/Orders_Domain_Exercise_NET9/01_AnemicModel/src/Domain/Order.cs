
namespace Orders.Anemic.Domain;

public enum OrderStatus { New, Paid, Cancelled }

public class Order
{
    public Guid Id { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public List<OrderLine> Lines { get; set; } = new();
    public OrderStatus Status { get; set; } = OrderStatus.New;
}

public class OrderLine
{
    public string ProductId { get; set; } = string.Empty;
    public int Qty { get; set; }
    public decimal UnitPrice { get; set; }
}
