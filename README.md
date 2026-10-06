# WebAppApi · Copa Airlines Hackathon 2026 practice project

> **This project is for practicing for the Copa Airlines Hackathon 2026** (Oct 9, 2026, Santiago de Veraguas, Panama). It is a small, realistic app used to rehearse the challenge stack: .NET APIs, Azure App Service, Terraform, CI/CD and observability (Application Insights, KQL, Dynatrace). The study plan and the step-by-step guides are in [guias/README.md](guias/README.md).

REST API for managing contacts and the messages they send to each other. Built with .NET 10 minimal APIs, Entity Framework Core and SQLite.

| Document | Content |
|---|---|
| [docs/contacts.md](docs/contacts.md) | `/api/contacts` endpoints |
| [docs/messages.md](docs/messages.md) | `/api/messages` endpoints |
| [docs/data-model.md](docs/data-model.md) | Entities, DTOs and validation rules |
| [docs/health-checks.md](docs/health-checks.md) | `/healthz` endpoint |
| [docs/postman/WebAppApi.postman_collection.json](docs/postman/WebAppApi.postman_collection.json) | Postman collection with automated tests for every endpoint |
| [guias/README.md](guias/README.md) | Hackathon study plan, roles, runbook and cheat sheets |


## Running locally

From the `WebAppApi/` folder:

```bash
dotnet run --launch-profile http   # http://localhost:5033
```

The app creates `app.db` and applies any pending migrations when it starts, so there is no extra step. The `dotnet-ef` tool is only needed to create new migrations (`dotnet ef migrations add <Name>`).

| Profile | URL |
|---|---|
| `http` | `http://localhost:5033` |
| `https` | `https://localhost:7168` and `http://localhost:5033` |

### `.http` files

Run them from the editor (VS Code with the REST Client extension, or Visual Studio 2022 17.12+), top to bottom: later requests use ids created by earlier ones.

| File | Content |
|---|---|
| `WebAppApi/WebAppApi.http` | health check and OpenAPI |
| `WebAppApi/features/contacts/contacts.http` | contacts CRUD and its errors |
| `WebAppApi/features/messages/messages.http` | a full conversation between two contacts, its errors, and cleanup |

In Development, the OpenAPI document is available at `GET /openapi/v1.json`. Postman can import it with **Import → Link**.

## Testing with Postman

1. In Postman: **Import** → select `docs/postman/WebAppApi.postman_collection.json`.
2. Start the app (`dotnet run --launch-profile http`).
3. Right-click the collection → **Run collection** → **Run WebAppApi**.

The run creates its own contacts and messages, stores their ids in collection variables, and deletes everything at the end. Run the folders in order: later requests use ids saved by earlier ones.

| Variable | Default | Purpose |
|---|---|---|
| `baseUrl` | `http://localhost:5033` | change it to test another port or the Azure URL |
| `missingId` | `999999` | an id that must not exist, used for the `404` tests |

From the terminal, with [Newman](https://www.npmjs.com/package/newman):

```bash
npx newman run docs/postman/WebAppApi.postman_collection.json
npx newman run docs/postman/WebAppApi.postman_collection.json --env-var baseUrl=https://<your-app>.azurewebsites.net
```

## Configuration

`appsettings.json`:

```json
"ConnectionStrings": {
  "Default": "Data Source=app.db"
}
```

The SQLite file `app.db` is created in `WebAppApi/` and is ignored by git.

In Azure, Terraform sets these App Settings on the Web App. In an environment variable, `__` stands for `:` in the configuration key.

| App Setting | Value | Purpose |
|---|---|---|
| `ConnectionStrings__Default` | `Data Source=/home/app.db` | puts the SQLite file in `/home`, the only folder that persists between restarts and deployments in App Service for Linux |
| `ASPNETCORE_ENVIRONMENT` | `Production` | the OpenAPI endpoint is not mapped |
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | taken from the Application Insights resource | telemetry destination |

## Infrastructure (`infra/`)

Terraform with the `azurerm` provider `5.0.0`, region `westus`.

| File | Resources |
|---|---|
| `main.tf` | provider and resource group |
| `app.tf` | Linux App Service plan (B1), Linux Web App (.NET 10, HTTPS only, system-assigned identity, health check on `/healthz`, CORS for the frontend origin), Log Analytics workspace, diagnostic settings (HTTP, console and app logs plus metrics sent to Log Analytics), Application Insights, `api_url` output |

```bash
cd infra
terraform init
terraform plan -out tfplan
terraform apply tfplan
terraform destroy
```

- **The state is local** (`terraform.tfstate`, ignored by git). Only the machine that ran `apply` can change or destroy the resources.
- **Resource names get a random suffix** (`random_integer`). After a `destroy` followed by `apply` the Web App has a new name, so `AZURE_WEBAPP_NAME` in the workflow, the publish profile secret and any CORS or frontend setting that mentions it must be updated.

## CI/CD (GitHub Actions)

[`.github/workflows/deploy-api.yml`](.github/workflows/deploy-api.yml) runs on pushes to `main` that touch `WebAppApi/**`, `infra/**` or the workflow itself, and can also be started by hand (`workflow_dispatch`).

1. **build**: restore, build and publish `WebAppApi/WebAppApi.csproj` in Release, and upload the result as an artifact.
2. **deploy**: download the artifact and deploy it to the Web App named in `AZURE_WEBAPP_NAME`.
3. **smoke test**: call `/healthz` up to 10 times, 15 seconds apart, and fail if it never answers `200`.

It authenticates with a publish profile stored in the `AZURE_WEBAPP_PUBLISH_PROFILE` secret, so it needs no app registration in Entra ID. This only works while **SCM Basic Auth publishing credentials** are enabled on the Web App.

The hackathon uses Azure DevOps, so [guide 03](guias/03-azure-devops-pipelines.md) ports this flow to Azure Pipelines.

## Project structure

```
.
├── WebAppApi/
│   ├── Program.cs                  # service registration, startup migrations and endpoint mapping
│   ├── HealthCheck.cs              # SampleHealthCheck
│   ├── data/AppDbContext.cs        # EF Core DbContext (Contacts, Messages)
│   ├── domain/entities/            # Contact, Message
│   ├── features/
│   │   ├── contacts/               # ContactEndpoints, ContactService, ContactModel, contacts.http
│   │   └── messages/               # MessageEndpoints, MessageService, MessageModel, messages.http
│   └── Migrations/                 # EF Core migrations
├── docs/                           # API documentation and Postman collection
├── infra/                          # Terraform (main.tf, app.tf)
├── .github/workflows/              # deploy-api.yml
├── guias/                          # study plan and step-by-step guides
└── WebAppApi.slnx
```

Each feature follows the same three layers:

- **Model**: `record` DTOs. `*Request` is what the client sends, `*Response` is what the API returns. Entities are never exposed directly.
- **Service**: database logic. Returns `null` (or `false` for deletes) when something doesn't exist.
- **Endpoints**: HTTP routes. Turns the service results into status codes with `TypedResults`.

## Conventions

- **JSON** uses camelCase (`senderId`, `createdAt`).
- **Dates** are stored in UTC and use ISO 8601 (`2026-10-09T15:00:00Z`).
- **IDs** are integers generated by the database. Routes only accept numbers (`{id:int}`), so `/api/contacts/abc` returns `404`.
- **Errors with a body:** business errors (`400`, `404`, `409`) return a problem details body with a `detail` message, for example when sending a message to a contact that doesn't exist. A `404` for a missing id in the URL, and every `204 No Content`, have an empty body.

## Validation errors

Invalid request bodies are rejected with `400 Bad Request` before reaching the service:

```json
{
  "title": "One or more validation errors occurred.",
  "errors": {
    "Name": ["The Name field is required."],
    "Phone": ["The Phone field is not a valid phone number."]
  }
}
```

