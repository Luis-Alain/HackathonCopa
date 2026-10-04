using Microsoft.EntityFrameworkCore;
using WebAppApi.data;
using WebAppApi.domain.entities;

namespace WebAppApi.features.Contacts;

public class ContactService
{
    private readonly AppDbContext _db;

    public ContactService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ContactResponse>> GetAllAsync()
    {
        return await _db.Contacts
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new ContactResponse(c.Id, c.Name, c.Phone, c.CreatedAt))
            .ToListAsync();
    }

    public async Task<ContactResponse?> GetByIdAsync(int id)
    {
        var contact = await _db.Contacts.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        return contact is null ? null : ContactResponse.FromEntity(contact);
    }

    public async Task<ContactResponse> CreateAsync(ContactRequest request)
    {
        var contact = new Contact
        {
            Name = request.Name.Trim(),
            Phone = request.Phone.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        _db.Contacts.Add(contact);
        await _db.SaveChangesAsync();

        return ContactResponse.FromEntity(contact);
    }

    // Returns null when the contact does not exist.
    public async Task<ContactResponse?> UpdateAsync(int id, ContactRequest request)
    {
        var contact = await _db.Contacts.FindAsync(id);
        if (contact is null)
        {
            return null;
        }

        contact.Name = request.Name.Trim();
        contact.Phone = request.Phone.Trim();
        await _db.SaveChangesAsync();

        return ContactResponse.FromEntity(contact);
    }

    public async Task<DeleteContactResult> DeleteAsync(int id)
    {
        // Messages restrict the delete, so check first instead of failing on the foreign key.
        var hasMessages = await _db.Messages.AnyAsync(m => m.SenderId == id || m.RecipientId == id);
        if (hasMessages)
        {
            return DeleteContactResult.HasMessages;
        }

        var deleted = await _db.Contacts.Where(c => c.Id == id).ExecuteDeleteAsync();
        return deleted > 0 ? DeleteContactResult.Deleted : DeleteContactResult.NotFound;
    }
}
