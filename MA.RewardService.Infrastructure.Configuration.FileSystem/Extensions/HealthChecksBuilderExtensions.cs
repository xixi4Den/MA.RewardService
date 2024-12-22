using MA.RewardService.Infrastructure.Configuration.FileSystem.Configuration;
using MA.RewardService.Infrastructure.Configuration.FileSystem.HealthCheck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MA.RewardService.Infrastructure.Configuration.FileSystem.Extensions;

public static class HealthChecksBuilderExtensions
{
    public static IHealthChecksBuilder AddFileSystemConfigChecks(this IHealthChecksBuilder builder, IConfiguration configuration)
    {
        var path = configuration.GetValue<string>($"{MissionsConfig.Key}:{nameof(MissionsConfig.FilePath)}");
        builder
            .AddTypeActivatedCheck<MissionConfigFileHealthCheck>(
                "MissionConfigFile",
                args: [path]);

        return builder;
    }
}