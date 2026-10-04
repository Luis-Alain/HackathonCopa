using Microsoft.AspNetCore.Http.HttpResults;

namespace WebAppApi.features.Contacts;

public static class ContactEndpoints
{
    public static void MapContactEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/contacts").WithTags("Contacts");

        group.MapGet("/", GetContacts);
        group.MapGet("/{id:int}", GetContact).WithName(nameof(GetContact));
        group.MapPost("/", CreateContact);
        group.MapPut("/{id:int}", UpdateContact);
        group.MapDelete("/{id:int}", DeleteContact);
    }

    private static async Task<Ok<List<ContactResponse>>> GetContacts(ContactService service)
    {
        return TypedResults.Ok(await service.GetAllAsync());
    }

    private static async Task<Results<Ok<ContactResponse>, NotFound>> GetContact(int id, ContactService service)
    {
        var contact = await service.GetByIdAsync(id);
        return contact is null ? TypedResults.NotFound() : TypedResults.Ok(contact);
    }

    private static async Task<CreatedAtRoute<ContactResponse>> CreateContact(ContactRequest request, ContactService service)
    {
        var contact = await service.CreateAsync(request);
        return TypedResults.CreatedAtRoute(contact, nameof(GetContact), new { id = contact.Id });
    }

    private static async Task<Results<Ok<ContactResponse>, NotFound>> UpdateContact(int id, ContactRequest request, ContactService service)
    {
        var contact = await service.UpdateAsync(id, request);
        return contact is null ? TypedResults.NotFound() : TypedResults.Ok(contact);
    }

    private static async Task<Results<NoContent, NotFound, ProblemHttpResult>> DeleteContact(int id, ContactService service)
    {
        return await service.DeleteAsync(id) switch
        {
            DeleteContactResult.Deleted => TypedResults.NoContent(),
            DeleteContactResult.HasMessages => TypedResults.Problem(
                detail: $"Contact {id} has messages and can't be deleted.",
                statusCode: StatusCodes.Status409Conflict),
            _ => TypedResults.NotFound()
        };
    }
}
