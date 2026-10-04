using Microsoft.EntityFrameworkCore;
using WebAppApi.data;
using WebAppApi.domain.entities;

namespace WebAppApi.features.Messages;

public class MessageService
{
    private readonly AppDbContext _db;

    public MessageService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<MessageResponse>> GetAllAsync()
    {
        return await _db.Messages
            .AsNoTracking()
            .OrderBy(m => m.CreatedAt)
            .Select(MessageResponse.Projection)
            .ToListAsync();
    }

    public async Task<MessageResponse?> GetByIdAsync(int id)
    {
        return await _db.Messages
            .AsNoTracking()
            .Where(m => m.Id == id)
            .Select(MessageResponse.Projection)
            .FirstOrDefaultAsync();
    }

    // Messages between two contacts, in both directions, oldest first.
    // Returns null when either contact does not exist.
    public async Task<List<MessageResponse>?> GetConversationAsync(int contactId, int otherContactId)
    {
        var existing = await _db.Contacts.CountAsync(c => c.Id == contactId || c.Id == otherContactId);
        var expected = contactId == otherContactId ? 1 : 2;
        if (existing != expected)
        {
            return null;
        }

        return await _db.Messages
            .AsNoTracking()
            .Where(m => (m.SenderId == contactId && m.RecipientId == otherContactId) ||
                        (m.SenderId == otherContactId && m.RecipientId == contactId))
            .OrderBy(m => m.CreatedAt)
            .Select(MessageResponse.Projection)
            .ToListAsync();
    }

    // Messages received by a contact, newest first. Returns null when the contact does not exist.
    public async Task<List<MessageResponse>?> GetInboxAsync(int contactId)
    {
        if (!await _db.Contacts.AnyAsync(c => c.Id == contactId))
        {
            return null;
        }

        return await _db.Messages
            .AsNoTracking()
            .Where(m => m.RecipientId == contactId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(MessageResponse.Projection)
            .ToListAsync();
    }

    // Messages sent by a contact, newest first. Returns null when the contact does not exist.
    public async Task<List<MessageResponse>?> GetSentAsync(int contactId)
    {
        if (!await _db.Contacts.AnyAsync(c => c.Id == contactId))
        {
            return null;
        }

        return await _db.Messages
            .AsNoTracking()
            .Where(m => m.SenderId == contactId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(MessageResponse.Projection)
            .ToListAsync();
    }

    public async Task<SendMessageResult> SendAsync(SendMessageRequest request)
    {
        if (request.SenderId == request.RecipientId)
        {
            return new SendMessageResult(null, SendMessageError.SameContact);
        }

        if (!await _db.Contacts.AnyAsync(c => c.Id == request.SenderId))
        {
            return new SendMessageResult(null, SendMessageError.SenderNotFound);
        }

        if (!await _db.Contacts.AnyAsync(c => c.Id == request.RecipientId))
        {
            return new SendMessageResult(null, SendMessageError.RecipientNotFound);
        }

        var message = new Message
        {
            SenderId = request.SenderId,
            RecipientId = request.RecipientId,
            Content = request.Content.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.Messages.Add(message);
        await _db.SaveChangesAsync();

        return new SendMessageResult(await GetByIdAsync(message.Id), SendMessageError.None);
    }

    // Returns null when the message does not exist.
    public async Task<MessageResponse?> UpdateAsync(int id, UpdateMessageRequest request)
    {
        var updated = await _db.Messages
            .Where(m => m.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(m => m.Content, request.Content.Trim()));

        return updated == 0 ? null : await GetByIdAsync(id);
    }

    // Sets ReadAt the first time; later calls keep the original time.
    // Returns null when the message does not exist.
    public async Task<MessageResponse?> MarkAsReadAsync(int id)
    {
        var message = await _db.Messages.FindAsync(id);
        if (message is null)
        {
            return null;
        }

        if (message.ReadAt is null)
        {
            message.ReadAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }

        return await GetByIdAsync(id);
    }

    // Returns false when the message does not exist.
    public async Task<bool> DeleteAsync(int id)
    {
        var deleted = await _db.Messages.Where(m => m.Id == id).ExecuteDeleteAsync();
        return deleted > 0;
    }
}
