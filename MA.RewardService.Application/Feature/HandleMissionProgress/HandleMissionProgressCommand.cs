using MediatR;

namespace MA.RewardService.Application.Feature.HandleMissionProgress;

public record HandleMissionProgressCommand(int UserId, Guid SpindId, int[] SpinResult): IRequest;