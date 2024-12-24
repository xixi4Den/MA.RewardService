using Testcontainers.Redis;

namespace MA.RewardService.IntegrationTests.Framework;

public static class TestContainerHelper
{
    private static readonly RedisContainer RedisContainer;

    public static string RedisConnectionString => RedisContainer.GetConnectionString();
    
    static TestContainerHelper()
    {
        RedisContainer = new RedisBuilder()
            .WithImage("redis:7.0")
            .WithAutoRemove(true)
            .Build();
    }

    public static async Task Start()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
        
        await RedisContainer.StartAsync(cts.Token);
    }

    public static async Task Stop()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));
        
        await RedisContainer.StopAsync(cts.Token);
    }
}