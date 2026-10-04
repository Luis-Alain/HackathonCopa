using System.ComponentModel.DataAnnotations;
using WebAppApi.domain.entities;

namespace WebAppApi.features.Contacts;

// What the client sends when creating or updating a contact.
public record ContactRequest(
    [Required, MaxLength(100)] string Name,
    [Required, Phone, MaxLength(20)] string Phone);

// What the API returns. Never expose the entity directly.
public record ContactResponse(int Id, string Name, string Phone, DateTime CreatedAt)
{
    public static ContactResponse FromEntity(Contact contact) =>
        new(contact.Id, contact.Name, contact.Phone, contact.CreatedAt);
}
