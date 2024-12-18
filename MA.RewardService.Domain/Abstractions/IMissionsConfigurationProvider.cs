using MA.RewardService.Domain.Entities;

namespace MA.RewardService.Domain.Abstractions;

public interface IMissionsConfigurationProvider
{
    Task<MissionsConfiguration> GetAsync();
}