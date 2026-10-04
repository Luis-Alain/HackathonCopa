namespace WebAppApi.domain.entities;

public class Message
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public Contact Sender { get; set; } = null!;
    public int RecipientId { get; set; }
    public Contact Recipient { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ReadAt { get; set; }
}
