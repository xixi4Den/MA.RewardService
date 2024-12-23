using HealthChecks.UI.Client;
using MA.RewardService.Api.Endpoints;
using MA.RewardService.Application;
using MA.RewardService.Domain;
using MA.RewardService.Host.Extensions;
using MA.RewardService.Infrastructure.Configuration.FileSystem;
using MA.RewardService.Infrastructure.Configuration.FileSystem.Extensions;
using MA.RewardService.Infrastructure.DataAccess;
using MA.RewardService.Infrastructure.DataAccess.Extensions;
using MA.RewardService.Infrastructure.Messaging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Resources;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDomainServices()
    .AddApplicationServices(builder.Configuration)
    .AddRedisDataAccessServices(builder.Configuration)
    .AddMessagingServices(builder.Configuration)
    .AddFileSystemConfiguration(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddRedisHealthCheck(builder.Configuration)
    .AddFileSystemConfigChecks(builder.Configuration);

var otlpEndpoint = builder.Configuration["EXPORTER_OTLP_ENDPOINT"];
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: "reward"))
    .AddTraces(builder.Configuration)
    .AddMetrics();;

var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapProgressEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPrometheusScrapingEndpoint();

// app.UseHttpsRedirection();

app.Run();

public partial class Program
{
}