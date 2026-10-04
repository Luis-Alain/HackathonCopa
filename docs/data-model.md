# Data model

SQLite database managed by EF Core. The schema comes from the migrations in `WebAppApi/Migrations/` (`Init`, then `SenderRecipientMessages`).

```
Contact 1 ──── * Message   (as sender,    SenderId)
Contact 1 ──── * Message   (as recipient, RecipientId)
```

Each message has two foreign keys to `Contacts`: who sent it and who receives it. Both relationships are configured in `AppDbContext.OnModelCreating`, because EF Core can't infer which navigation goes with which key when there are two to the same table.

**Restrict, not cascade:** both foreign keys use `ON DELETE RESTRICT`. SQL Server / Azure SQL rejects two cascade paths to the same table, so cascade would break if the app moves off SQLite. As a result, a contact with messages can't be deleted (`409 Conflict`).

## Entities

### Contact (`domain/entities/Contact.cs`)

| Property | Type | Notes |
|---|---|---|
| `Id` | int | primary key, generated |
| `Name` | string | |
| `Phone` | string | |
| `CreatedAt` | DateTime | UTC, set on creation |

### Message (`domain/entities/Message.cs`)

| Property | Type | Notes |
|---|---|---|
| `Id` | int | primary key, generated |
| `SenderId` | int | foreign key to `Contact` |
| `Sender` | Contact | navigation property, never returned by the API |
| `RecipientId` | int | foreign key to `Contact` |
| `Recipient` | Contact | navigation property, never returned by the API |
| `Content` | string | |
| `CreatedAt` | DateTime | UTC, when the message was sent |
| `ReadAt` | DateTime? | UTC, `null` until the recipient reads it |

## DTOs

The API never returns entities. Each feature has `*Request` records (input) and a `*Response` record (output).

### Contacts (`features/contacts/ContactModel.cs`)

| Record | Fields |
|---|---|
| `ContactRequest` | `Name` (required, max 100), `Phone` (required, phone format, max 20) |
| `ContactResponse` | `Id`, `Name`, `Phone`, `CreatedAt` |

### Messages (`features/messages/MessageModel.cs`)

| Record | Fields |
|---|---|
| `SendMessageRequest` | `SenderId`, `RecipientId` (both range 1 to `int.MaxValue`), `Content` (required, max 256) |
| `UpdateMessageRequest` | `Content` (required, max 256) |
| `MessageResponse` | `Id`, `SenderId`, `SenderName`, `RecipientId`, `RecipientName`, `Content`, `CreatedAt`, `ReadAt` |

`MessageResponse.Projection` is an expression used inside EF queries, so `SenderName` and `RecipientName` come from a SQL join instead of loading the contacts.

## Validation notes

Validation runs through `builder.Services.AddValidation()` in `Program.cs`, using DataAnnotations attributes on the request records.

- `[Required]` doesn't work on non-nullable value types (`int`, `bool`, `DateTime`). A missing field silently becomes its default value (`0`, `false`, `0001-01-01`). That's why `SenderId` and `RecipientId` use `[Range]` instead of `[Required]`.
- Rules that need the database ("the contact must exist") or compare two fields ("sender and recipient must be different") live in the service, not in the DTO.

## Migrations

```bash
dotnet ef migrations add <Name>   # after changing an entity or AppDbContext
dotnet ef database update         # apply pending migrations to app.db
```
