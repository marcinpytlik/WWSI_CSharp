namespace Demo32;

public sealed class Report
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Status { get; set; } = "Queued";
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedUtc { get; set; }
}
