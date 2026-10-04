# Messages

Status: ✅ implemented

Contacts send messages to each other. Every message has a **sender** and a **recipient**, both contacts.

## Endpoints

### `/api/messages`

| Method | Route | Description | Success | Errors |
|---|---|---|---|---|
| `GET` | `/api/messages` | List all messages, oldest first | `200` | none |
| `GET` | `/api/messages/{id}` | Get one message | `200` | `404` |
| `POST` | `/api/messages` | Send a message from one contact to another | `201` | `400`, `404` |
| `PUT` | `/api/messages/{id}` | Edit the text of a message | `200` | `400`, `404` |
| `PUT` | `/api/messages/{id}/read` | Mark a message as read | `200` | `404` |
| `DELETE` | `/api/messages/{id}` | Delete a message | `204` | `404` |

### `/api/contacts/{contactId}/...`

Messages seen from one contact's point of view:

| Method | Route | Description | Success | Errors |
|---|---|---|---|---|
| `GET` | `/api/contacts/{contactId}/conversations/{otherContactId}` | Messages between two contacts, **in both directions**, oldest first | `200` | `404` |
| `GET` | `/api/contacts/{contactId}/messages/inbox` | Messages the contact received, newest first | `200` | `404` |
| `GET` | `/api/contacts/{contactId}/messages/sent` | Messages the contact sent, newest first | `200` | `404` |

## Request bodies

### Send (`POST /api/messages`)

```json
{
  "senderId": 2,
  "recipientId": 4,
  "content": "Hola Luis"
}
```

| Field | Type | Rules |
|---|---|---|
| `senderId` | int | 1 or greater, the contact must exist |
| `recipientId` | int | 1 or greater, the contact must exist, different from `senderId` |
| `content` | string | required, max 256 characters, trimmed before saving |

### Edit (`PUT /api/messages/{id}`)

```json
{ "content": "Hola Luis (editado)" }
```

Only the text can change. Sender and recipient are fixed once the message is sent.

## Response body

```json
{
  "id": 1,
  "senderId": 2,
  "senderName": "Ana",
  "recipientId": 4,
  "recipientName": "Luis",
  "content": "Hola Luis",
  "createdAt": "2026-10-04T20:28:49.444892Z",
  "readAt": null
}
```

| Field | Set by |
|---|---|
| `senderName`, `recipientName` | server, read from the contacts |
| `createdAt` | server, when the message is sent |
| `readAt` | server; `null` until `PUT /api/messages/{id}/read` is called the first time. Later calls keep the original time |

## Errors when sending

| Case | Status | `detail` |
|---|---|---|
| `senderId` is `0`, or `content` is empty | `400` | validation errors (`errors` object) |
| `senderId == recipientId` | `400` | `A contact can't send a message to itself.` |
| Sender doesn't exist | `404` | `Sender contact {id} not found.` |
| Recipient doesn't exist | `404` | `Recipient contact {id} not found.` |

Example body:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Not Found",
  "status": 404,
  "detail": "Recipient contact 99 not found."
}
```

## Example: a conversation

1. Ana (id 2) writes to Luis (id 4):
   ```http
   POST /api/messages
   Content-Type: application/json

   { "senderId": 2, "recipientId": 4, "content": "Hola Luis" }
   ```
2. Luis answers:
   ```http
   POST /api/messages
   Content-Type: application/json

   { "senderId": 4, "recipientId": 2, "content": "Hola Ana!" }
   ```
3. Read the conversation. `/api/contacts/2/conversations/4` and `/api/contacts/4/conversations/2` return the same 2 messages, in order:
   ```http
   GET /api/contacts/2/conversations/4
   ```
4. Luis reads his inbox and marks the message as read:
   ```http
   GET /api/contacts/4/messages/inbox
   PUT /api/messages/1/read
   ```

If either contact in a conversation, inbox or sent request doesn't exist, the response is `404`.

## Deleting contacts

A contact who has sent or received messages **can't be deleted**: `DELETE /api/contacts/{id}` returns `409 Conflict`. Delete their messages first. See [contacts.md](contacts.md).

## Known limitations

- **No authentication:** the client chooses `senderId`, so anyone can send a message as any contact. In a real app the sender comes from the authenticated user, not from the body.
- **Anyone can edit or delete any message:** same reason; there's no user to check against.

## Source

- `WebAppApi/features/messages/MessageEndpoints.cs`
- `WebAppApi/features/messages/MessageService.cs`
- `WebAppApi/features/messages/MessageModel.cs`
