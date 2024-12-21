using MA.RewardService.Application;
using MA.RewardService.Application.Feature.HandleMissionProgress;
using MA.RewardService.Domain;
using MA.RewardService.Infrastructure.DataAccess;
using MA.RewardService.Infrastructure.Messaging;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDomainServices()
    .AddApplicationServices(builder.Configuration)
    .AddRedisDataAccessServices(builder.Configuration)
    .AddMessagingServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.Run();