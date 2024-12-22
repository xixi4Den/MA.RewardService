using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace MA.RewardService.Infrastructure.Configuration.FileSystem.HealthCheck;

public class MissionConfigFileHealthCheck(string filePath) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        return File.Exists(filePath)
            ? Task.FromResult(HealthCheckResult.Healthy())
            : Task.FromResult(HealthCheckResult.Unhealthy($"file not found: {filePath}"));
    }
}