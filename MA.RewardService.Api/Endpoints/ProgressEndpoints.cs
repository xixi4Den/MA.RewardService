using MA.RewardService.Api.Contracts;
using MA.RewardService.Application.Feature.MissionProgress;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MA.RewardService.Api.Endpoints;

public static class ProgressEndpoints
{
    public static IEndpointRouteBuilder MapProgressEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/progress", async (
                [FromHeader(Name = "UserId")] int userId, IMediator mediator) =>
            {
                var query = new MissionProgressQuery(userId);
                var result = await mediator.Send(query);

                return Results.Ok(new MissionProgressResponse
                {
                    MissionIndex = result.MissionIndex,
                    TotalPoints = result.TotalPoints,
                    RemainingPoints = result.RemainingPoints
                });
            }).WithOpenApi()
            .WithTags("Progress")
            .WithSummary("Provides player's game progress")
            .Produces<MissionProgressResponse>();

        return endpoints;
    }
}