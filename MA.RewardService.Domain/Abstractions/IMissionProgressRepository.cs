using MA.RewardService.Domain.Entities;

namespace MA.RewardService.Domain.Abstractions;

public interface IMissionProgressRepository
{
    Task<MissionProgress> GetAsync(int userId);

    Task<bool> UpdateAsync(int userId, MissionProgress currentProgress, MissionProgress updatedProgress);
}