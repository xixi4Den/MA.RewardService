using HealthChecks.UI.Client;
using MA.RewardService.Api.Endpoints;
using MA.RewardService.Application;
using MA.RewardService.Domain;
using MA.RewardService.Infrastructure.Configuration.FileSystem;
using MA.RewardService.Infrastructure.Configuration.FileSystem.Extensions;
using MA.RewardService.Infrastructure.DataAccess;
using MA.RewardService.Infrastructure.DataAccess.Extensions;
using MA.RewardService.Infrastructure.Messaging;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

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

app.UseHttpsRedirection();

app.Run();