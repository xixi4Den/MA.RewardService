using System.Text.Json;
using System.Text.Json.Serialization;
using MA.RewardService.Domain.Abstractions;
using MA.RewardService.Domain.Entities;
using MA.RewardService.Infrastructure.Configuration.FileSystem.Configuration;
using Microsoft.Extensions.Options;

namespace MA.RewardService.Infrastructure.Configuration.FileSystem;

public class MissionsConfigurationFileSystemProvider: IMissionsConfigurationProvider
{
    private MissionsConfig _missionsConfig;
    private JsonSerializerOptions _options;

    public MissionsConfigurationFileSystemProvider(IOptions<MissionsConfig> missionsConfigurationOptions)
    {
        _missionsConfig = missionsConfigurationOptions.Value;
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }
    
    public Task<MissionsConfiguration> GetAsync()
    {
        var jsonString = File.ReadAllText(_missionsConfig.FilePath);
        
        var rawResult = JsonSerializer.Deserialize<MissionsConfiguration>(jsonString, _options);
        if (rawResult is null)
            throw new InvalidOperationException("Missions configuration file is cannot be parsed");

        return Task.FromResult(new MissionsConfiguration(rawResult.Missions, rawResult.RepeatedIndex));
    }
}