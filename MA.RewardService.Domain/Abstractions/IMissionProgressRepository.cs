using MA.RewardService.Domain.Entities;

namespace MA.RewardService.Domain.Abstractions;

public interface IMissionProgressRepository
{
    Task<MissionProgress> GetAsync(int userId);

    Task UpdateAsync(int userId, MissionProgress currentProgress, MissionProgress newProgress);
}