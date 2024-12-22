using MA.RewardService.Domain.Entities;

namespace MA.RewardService.Domain.Abstractions;

public interface IGrantedRewardRepository
{
    Task AddAsync(int userId, RewardGranted reward);
}