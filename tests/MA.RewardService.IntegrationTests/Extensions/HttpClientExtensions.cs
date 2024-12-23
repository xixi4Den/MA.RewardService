using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MA.RewardService.Api.Contracts;

namespace MA.RewardService.IntegrationTests.Extensions;

public static class HttpClientExtensions
{
    public static async Task<MissionProgressResponse> GetProgress(this HttpClient httpClient, int userId)
    {
        var uri = new Uri("http://localhost/api/progress");
        var body = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = uri,
            Headers = {
                { "UserId", userId.ToString() }
            }
        };
        var response = await httpClient.SendAsync(body);
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<MissionProgressResponse>();
        result.Should().NotBeNull();
        
        return result!;
    }   
}