using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using WebAppApi.domain.entities;

namespace WebAppApi.features.Messages;

// What the client sends to send a message from one contact to another.
public record SendMessageRequest(
    [Range(1, int.MaxValue)] int SenderId,
    [Range(1, int.MaxValue)] int RecipientId,
    [Required, MaxLength(256)] string Content);

// Sender and recipient can't change after sending; only the text can be edited.
public record UpdateMessageRequest(
    [Required, MaxLength(256)] string Content);

// What the API returns. Never expose the entity directly.
public record MessageResponse(
    int Id,
    int SenderId,
    string SenderName,
    int RecipientId,
    string RecipientName,
    string Content,
    DateTime CreatedAt,
    DateTime? ReadAt)
{
    // Used inside EF queries, so the names come from a SQL join instead of loading the contacts.
    public static readonly Expression<Func<Message, MessageResponse>> Projection = m =>
        new MessageResponse(
            m.Id,
            m.SenderId,
            m.Sender.Name,
            m.RecipientId,
            m.Recipient.Name,
            m.Content,
            m.CreatedAt,
            m.ReadAt);
}

public enum SendMessageError
{
    None,
    SameContact,
    SenderNotFound,
    RecipientNotFound
}

public record SendMessageResult(MessageResponse? Message, SendMessageError Error);
