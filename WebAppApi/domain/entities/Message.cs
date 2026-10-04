namespace WebAppApi.domain.entities;

public class Message
{
    public int Id { get; set; }
    public int ContactId { get; set; }
    public Contact Contact { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool Sent { get; set; }
}