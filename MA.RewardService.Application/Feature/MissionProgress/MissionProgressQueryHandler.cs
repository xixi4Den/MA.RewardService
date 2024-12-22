using MA.RewardService.Domain.Abstractions;
using MediatR;

namespace MA.RewardService.Application.Feature.MissionProgress;

public class MissionProgressQueryHandler : IRequestHandler<MissionProgressQuery, Domain.Entities.MissionProgress>
{
    private readonly IMissionProgressRepository _repository;

    public MissionProgressQueryHandler(IMissionProgressRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<Domain.Entities.MissionProgress> Handle(MissionProgressQuery request, CancellationToken ct)
    {
        var result = await _repository.GetAsync(request.UserId);

        return result;
    }
}