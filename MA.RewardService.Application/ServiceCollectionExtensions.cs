using System.Reflection;
using MA.RewardService.Application.Services;
using MA.RewardService.Domain.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace MA.RewardService.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        
        services.AddSingleton<IMissionsConfigurationProvider, TestMissionsConfigurationProvider>();
        
        return services;
    }
}