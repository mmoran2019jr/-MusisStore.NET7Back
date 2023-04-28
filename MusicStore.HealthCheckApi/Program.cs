using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using MusicStore.DataAccess;
using MusicStore.HealthCheckApi.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<MusicStoreDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database"));
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), new[] { "api" })
    .AddDbContextCheck<MusicStoreDbContext>("Database", HealthStatus.Unhealthy, new[] { "database" })
    .AddDbContextCheck<MusicStoreDbContext>("Database2", HealthStatus.Degraded, new[] { "database" })
    .AddTypeActivatedCheck<PingHealthCheck>("Google", HealthStatus.Degraded, tags: new[] { "api" }, "google.com")
    .AddTypeActivatedCheck<PingHealthCheck>("Azure", HealthStatus.Degraded, tags: new[] { "api" }, "azure.com")
    .AddTypeActivatedCheck<PingHealthCheck>("Tienda", HealthStatus.Degraded, tags: new[] { "api" }, "mercadolibre.com")
    .AddTypeActivatedCheck<PingHealthCheck>("Servidor", HealthStatus.Degraded, tags: new[] { "api" }, "192.130.0.5");

builder.Services.AddHealthChecksUI().AddInMemoryStorage();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();

app.UseHealthChecksUI();
app.UseAuthorization();


app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    Predicate = x => x.Tags.Contains("api")
});

app.MapHealthChecks("/health/db", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse,
    Predicate = x => x.Tags.Contains("database")
});

app.Run();
