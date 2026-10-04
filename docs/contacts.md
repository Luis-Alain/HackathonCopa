# Contacts

Base route: `/api/contacts` · Status: ✅ implemented

| Method | Route | Description | Success | Errors |
|---|---|---|---|---|
| `GET` | `/api/contacts` | List all contacts, sorted by name | `200` | none |
| `GET` | `/api/contacts/{id}` | Get one contact | `200` | `404` |
| `POST` | `/api/contacts` | Create a contact | `201` | `400` |
| `PUT` | `/api/contacts/{id}` | Update a contact | `200` | `400`, `404` |
| `DELETE` | `/api/contacts/{id}` | Delete a contact | `204` | `404`, `409` |

## Request body

Used by `POST` and `PUT`:

```json
{
  "name": "Ana",
  "phone": "+50761234567"
}
```

| Field | Type | Rules |
|---|---|---|
| `name` | string | required, max 100 characters |
| `phone` | string | required, valid phone format, max 20 characters |

Both values are trimmed before saving.

## Response body

```json
{
  "id": 1,
  "name": "Ana",
  "phone": "+50761234567",
  "createdAt": "2026-10-04T06:18:54.614736Z"
}
```

`createdAt` is set by the server when the contact is created and never changes.

## Examples

### List contacts

```http
GET /api/contacts
```

`200 OK`

```json
[
  { "id": 1, "name": "Ana", "phone": "+50761234567", "createdAt": "2026-10-04T06:18:54.614736Z" }
]
```

### Get one contact

```http
GET /api/contacts/1
```

`200 OK` with the contact, or `404 Not Found` if the id doesn't exist.

### Create a contact

```http
POST /api/contacts
Content-Type: application/json

{ "name": "Ana", "phone": "+50761234567" }
```

`201 Created`, with the header `Location: http://localhost:5033/api/contacts/1` and the new contact in the body.

Invalid body, such as `{ "name": "", "phone": "abc" }`: `400 Bad Request` with the validation errors.

### Update a contact

```http
PUT /api/contacts/1
Content-Type: application/json

{ "name": "Ana María", "phone": "+50769876543" }
```

`200 OK` with the updated contact, `400` if the body is invalid, or `404` if the id doesn't exist.

### Delete a contact

```http
DELETE /api/contacts/1
```

`204 No Content`, or `404 Not Found` if the id doesn't exist (including a second delete of the same id).

`409 Conflict` if the contact has sent or received any message:

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Conflict",
  "status": 409,
  "detail": "Contact 4 has messages and can't be deleted."
}
```

Delete the contact's messages first.

## Source

- `WebAppApi/features/contacts/ContactEndpoints.cs`
- `WebAppApi/features/contacts/ContactService.cs`
- `WebAppApi/features/contacts/ContactModel.cs`
