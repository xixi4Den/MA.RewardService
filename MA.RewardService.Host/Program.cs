using HealthChecks.UI.Client;
using MA.RewardService.Application;
using MA.RewardService.Application.Feature.HandleMissionProgress;
using MA.RewardService.Domain;
using MA.RewardService.Infrastructure.DataAccess;
using MA.RewardService.Infrastructure.DataAccess.Extensions;
using MA.RewardService.Infrastructure.Messaging;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDomainServices()
    .AddApplicationServices(builder.Configuration)
    .AddRedisDataAccessServices(builder.Configuration)
    .AddMessagingServices(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddRedisHealthCheck(builder.Configuration);

var app = builder.Build();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// TODO: remove
app.MapGet("/test", async (ISender sender) =>
{
    await sender.Send(new HandleMissionProgressCommand(22, [7, 7, 7]));
    // var tasks = Enumerable.Range(0, 10).Select(_ => sender.Send(new HandleMissionProgressCommand(2, [7, 7, 7])));
    // await Task.WhenAll(tasks);
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();