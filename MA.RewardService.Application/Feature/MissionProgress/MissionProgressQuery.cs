using MediatR;

namespace MA.RewardService.Application.Feature.MissionProgress;

public record MissionProgressQuery(int UserId): IRequest<Domain.Entities.MissionProgress>;