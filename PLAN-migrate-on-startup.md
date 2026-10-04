# Temporary plan · Apply migrations on startup (local + Azure)

> Temporary file. Delete it when you finish; don't commit it.

**Goal:** the app creates its own tables when it starts, so `no such table: Contacts` never happens again, locally or in Azure (`contacts-api`).

**Time:** ~1 h · **Related guide:** `guias/01-azure-app-service.md`, Ejercicio 2b

---

## Step 0 · Restore the deleted files (5 min)

Besides `app.db`, these tracked files are missing from your working copy (`git status` shows them as `D`):

- `.github/workflows/main_contacts-api.yml`: the GitHub Actions deploy to `contacts-api`
- `infra/main.tf`, `infra/variables.tf`, `infra/.terraform.lock.hcl`

1. If you didn't delete them on purpose, restore them:
   ```bash
   git restore .github infra
   ```
2. **`infra/terraform.tfstate` can't be restored from git**, because it was ignored. If it's gone, Terraform no longer knows about `myTFResourceGroup` in Azure. Before the next `terraform apply`, either delete that resource group in the portal, or import it (`terraform import`).

**Done when:** `git status` no longer shows any `D` lines.

## Step 1 · Fix your local database right now (2 min)

From `WebAppApi/`:

```bash
dotnet ef database update
sqlite3 app.db ".tables"
```

**Done when:** `.tables` lists `Contacts`, `Messages` and `__EFMigrationsHistory`, and `POST /api/contacts` returns 201.

This only fixes your machine. Steps 2 to 5 fix it for good.

## Step 2 · Migrate on startup in `Program.cs` (15 min)

1. Add the code **after** `var app = builder.Build();` and **before** the `app.Map...` calls.
2. `AppDbContext` is registered as *scoped*, and at startup there's no request scope. So you need to:
   - create a scope from `app.Services`
   - get the `AppDbContext` from that scope
   - call `Database.Migrate()` on it
3. Use `Migrate()`, **not** `EnsureCreated()`. `EnsureCreated` ignores migrations, and later `Migrate()` calls would fail on that database.

<details>
<summary>Hint: the shape of the code</summary>

```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    // ...
}
```

Search "EF Core apply migrations at runtime" if you get stuck.

</details>

**Done when:**

```bash
rm app.db
dotnet run --launch-profile http
```

- the console shows `Applying migration '..._Init'` and `'..._SenderRecipientMessages'`
- `sqlite3 app.db ".tables"` lists both tables
- `messages.http` (or the Postman collection) passes top to bottom

**Also check:** stop the app and run it again. This time no "Applying migration" lines appear, because the migrations are already applied.

## Step 3 · Where the database lives in Azure (10 min)

`Data Source=app.db` is relative to the app's folder, which in App Service is `/home/site/wwwroot`. **Every deployment replaces that folder**, so your data would disappear on each push.

1. In the portal, open `contacts-api` → **Settings → Environment variables → App settings**.
2. Add `ConnectionStrings__Default` = `Data Source=/home/app.db`. Or use the CLI:
   ```bash
   az webapp config appsettings set -n contacts-api -g <your-rg> \
     --settings "ConnectionStrings__Default=Data Source=/home/app.db"
   ```
3. Answer in your notes: why does `__` turn into `:` in .NET configuration? Which value wins: `appsettings.json` or the App Setting?

**Done when:** `az webapp config appsettings list -n contacts-api -g <your-rg> -o table` shows the setting.

## Step 4 · Deploy (10 min)

The workflow `.github/workflows/main_contacts-api.yml` deploys on every push to `main`. Restore it first (Step 0).

1. Commit the `Program.cs` change on a branch, open a PR, merge to `main`.
2. Watch the run in GitHub → **Actions**. Both jobs, `build` and `deploy`, must be green.
3. Open **Log stream** in the portal (or `az webapp log tail -n contacts-api -g <your-rg>`) and restart the app. Look for the `Applying migration` lines.

<details>
<summary>If the deploy fails or the app doesn't start</summary>

- **Build fails:** read the failing step in the Actions log. The workflow runs `dotnet build` at the repo root, which uses `WebAppApi.slnx`.
- **App returns 503 or "Application Error":** an exception during startup (for example in `Migrate()`) stops the app. Log stream shows the exception.
- **`unable to open database file`:** the folder in `Data Source` doesn't exist. SQLite creates the file, but not the folders. `/home` always exists.

</details>

## Step 5 · Verify in Azure (10 min)

```bash
curl -i https://<your-app-url>/healthz
npx newman run docs/postman/WebAppApi.postman_collection.json --env-var baseUrl=https://<your-app-url>
```

Use the URL shown in **Overview → Default domain**. It may have a random suffix.

1. **The collection passes** completely.
2. **Data survives a restart:** create a contact, restart the app, and check it's still there.
3. **Data survives a deployment:** push any small change, wait for the deploy, and check the contact is still there.
4. **In Kudu** (`https://<app>.scm.azurewebsites.net` → SSH or Bash), run `ls -la /home/app.db`.

**Done when:** all four checks pass.

## Step 6 · Close the loop (10 min)

- [ ] `docs/README.md` → "Running locally": `dotnet ef database update` is now optional, because the app migrates on startup
- [ ] `docs/README.md` → "Configuration": document `ConnectionStrings__Default` for Azure
- [ ] In `guias/01-azure-app-service.md`, Ejercicio 2b, mark what you learned in "Mis notas"
- [ ] Delete this file

## Know the limits (for the hackathon)

- **One instance only.** With several instances, all of them would migrate at once and share one SQLite file over network storage, which SQLite doesn't handle well. With several instances, use Azure SQL and run migrations from the pipeline (migration bundles).
- **A failing migration stops the app.** That's intended: it's better to fail at startup, with the error in Log stream, than to serve 500s.
- **Optional:** add `.AddDbContextCheck<AppDbContext>()` to the health checks (`docs/health-checks.md` → "Pending improvements"). Then `/healthz` returns 503 if the database is unreachable.
