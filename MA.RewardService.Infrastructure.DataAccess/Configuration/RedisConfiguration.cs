namespace MA.RewardService.Infrastructure.DataAccess.Configuration;

public class RedisConfiguration
{
    public const string Key = "Redis";
    
    public string ConnectionString { get; set; }
}