using MediatR;

namespace MA.RewardService.Application.Feature.HandleMissionProgress;

public record HandleMissionProgressCommand(int UserId, int[] SpinResult): IRequest;