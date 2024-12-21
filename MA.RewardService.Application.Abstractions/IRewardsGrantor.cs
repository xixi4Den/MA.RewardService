using MA.RewardService.Domain.Entities;

namespace MA.RewardService.Application.Abstractions;

public interface IRewardsGrantor
{
    Task GrantAsync(int userId, IEnumerable<Mission> missions, CancellationToken ct);
}