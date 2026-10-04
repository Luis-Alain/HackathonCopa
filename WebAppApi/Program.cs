using Microsoft.EntityFrameworkCore;
using WebAppApi.data;
using WebAppApi.features.Contacts;
using WebAppApi.features.Messages;
using WebAppApi.HealthCheck;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddOpenApi();
builder.Services.AddValidation();

builder.Services.AddHealthChecks()
    .AddCheck<SampleHealthCheck>("Sample");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<MessageService>();

var app = builder.Build();

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // This will create the database if it doesn't exist and apply all pending migrations
    await dbContext.Database.MigrateAsync();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHealthChecks("/healthz");

app.UseHttpsRedirection();

app.MapContactEndpoints();

app.MapMessagesEndpoints();

app.Run();
