namespace MA.RewardService.Domain.Abstractions;

public interface ISpinsLogRepository
{
    Task AppendAsync(Guid spinId);

    Task<bool> ContainsAsync(Guid spinId);
}