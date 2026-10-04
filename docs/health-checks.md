# Health checks

Route: `GET /healthz` · Status: ✅ implemented

Used by monitoring tools (Azure App Service Health check, load balancers, Dynatrace) to know whether the app can serve requests.

| App state | Status code | Body (plain text) |
|---|---|---|
| All checks healthy | `200 OK` | `Healthy` |
| A check reports degraded | `200 OK` | `Degraded` |
| A check fails | `503 Service Unavailable` | `Unhealthy` |

The endpoint requires no authentication and answers on any port, so it works the same locally and in Azure.

## Registered checks

| Name | Class | What it checks |
|---|---|---|
| `Sample` | `SampleHealthCheck` (`WebAppApi/HealthCheck.cs`) | Placeholder; always healthy while `isHealthy = true` |

Configuration in `Program.cs`:

```csharp
builder.Services.AddHealthChecks()
    .AddCheck<SampleHealthCheck>("Sample");

app.MapHealthChecks("/healthz");
```

## Testing

```bash
curl -i http://localhost:5033/healthz
```

To see a failure, set `isHealthy = false` in `SampleHealthCheck`, restart the app, and expect `503 Unhealthy`.

## Pending improvements

- **Database check:** add the `Microsoft.Extensions.Diagnostics.HealthChecks.EntityFrameworkCore` package and chain `.AddDbContextCheck<AppDbContext>()`, so `/healthz` also confirms SQLite is reachable.
- **JSON output:** a custom `ResponseWriter` that returns the status of each check.

## Reference

[Health checks in ASP.NET Core](https://learn.microsoft.com/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-10.0)
