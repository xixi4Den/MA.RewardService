namespace MA.RewardService.Infrastructure.Messaging.Configuration;

public class RabbitConfiguration
{
    public const string Key = "Rabbit";
    
    public string Host { get; set; }
    
    public string Username { get; set; }
    
    public string Password { get; set; }
}