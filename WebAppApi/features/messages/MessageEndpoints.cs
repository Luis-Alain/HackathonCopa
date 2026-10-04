using Microsoft.AspNetCore.Http.HttpResults;

namespace WebAppApi.features.Messages;

public static class MessagesEndpoints
{
    public static void MapMessagesEndpoints(this IEndpointRouteBuilder app)
    {
        var messages = app.MapGroup("/api/messages").WithTags("Messages");

        messages.MapGet("/", GetMessages);
        messages.MapGet("/{id:int}", GetMessage).WithName(nameof(GetMessage));
        messages.MapPost("/", SendMessage);
        messages.MapPut("/{id:int}", UpdateMessage);
        messages.MapPut("/{id:int}/read", MarkMessageAsRead);
        messages.MapDelete("/{id:int}", DeleteMessage);

        // Messages seen from one contact's point of view.
        var contactMessages = app.MapGroup("/api/contacts/{contactId:int}").WithTags("Messages");

        contactMessages.MapGet("/conversations/{otherContactId:int}", GetConversation);
        contactMessages.MapGet("/messages/inbox", GetInbox);
        contactMessages.MapGet("/messages/sent", GetSent);
    }

    private static async Task<Ok<List<MessageResponse>>> GetMessages(MessageService service)
    {
        return TypedResults.Ok(await service.GetAllAsync());
    }

    private static async Task<Results<Ok<MessageResponse>, NotFound>> GetMessage(
        int id,
        MessageService service)
    {
        var message = await service.GetByIdAsync(id);
        return message is null ? TypedResults.NotFound() : TypedResults.Ok(message);
    }

    private static async Task<Results<CreatedAtRoute<MessageResponse>, ProblemHttpResult>> SendMessage(
        SendMessageRequest request,
        MessageService service)
    {
        var result = await service.SendAsync(request);

        return result.Error switch
        {
            SendMessageError.SameContact => TypedResults.Problem(
                detail: "A contact can't send a message to itself.",
                statusCode: StatusCodes.Status400BadRequest),
            SendMessageError.SenderNotFound => TypedResults.Problem(
                detail: $"Sender contact {request.SenderId} not found.",
                statusCode: StatusCodes.Status404NotFound),
            SendMessageError.RecipientNotFound => TypedResults.Problem(
                detail: $"Recipient contact {request.RecipientId} not found.",
                statusCode: StatusCodes.Status404NotFound),
            _ => TypedResults.CreatedAtRoute(result.Message!, nameof(GetMessage), new { id = result.Message!.Id })
        };
    }

    private static async Task<Results<Ok<MessageResponse>, NotFound>> UpdateMessage(
        int id,
        UpdateMessageRequest request,
        MessageService service)
    {
        var message = await service.UpdateAsync(id, request);
        return message is null ? TypedResults.NotFound() : TypedResults.Ok(message);
    }

    private static async Task<Results<Ok<MessageResponse>, NotFound>> MarkMessageAsRead(
        int id,
        MessageService service)
    {
        var message = await service.MarkAsReadAsync(id);
        return message is null ? TypedResults.NotFound() : TypedResults.Ok(message);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteMessage(
        int id,
        MessageService service)
    {
        return await service.DeleteAsync(id) ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<List<MessageResponse>>, ProblemHttpResult>> GetConversation(
        int contactId,
        int otherContactId,
        MessageService service)
    {
        var conversation = await service.GetConversationAsync(contactId, otherContactId);
        return conversation is null
            ? TypedResults.Problem(
                detail: $"Contact {contactId} or {otherContactId} not found.",
                statusCode: StatusCodes.Status404NotFound)
            : TypedResults.Ok(conversation);
    }

    private static async Task<Results<Ok<List<MessageResponse>>, NotFound>> GetInbox(
        int contactId,
        MessageService service)
    {
        var inbox = await service.GetInboxAsync(contactId);
        return inbox is null ? TypedResults.NotFound() : TypedResults.Ok(inbox);
    }

    private static async Task<Results<Ok<List<MessageResponse>>, NotFound>> GetSent(
        int contactId,
        MessageService service)
    {
        var sent = await service.GetSentAsync(contactId);
        return sent is null ? TypedResults.NotFound() : TypedResults.Ok(sent);
    }
}
