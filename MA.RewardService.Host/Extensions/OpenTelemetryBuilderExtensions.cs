using MassTransit.Logging;
using MassTransit.Monitoring;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace MA.RewardService.Host.Extensions;

public static class OpenTelemetryBuilderExtensions
{
    public static OpenTelemetryBuilder AddTraces(this OpenTelemetryBuilder builder, IConfiguration configuration)
    {
        var otlpEndpoint = configuration["EXPORTER_OTLP_ENDPOINT"];

        return builder.WithTracing(x =>
        {
            x.AddSource(DiagnosticHeaders.DefaultListenerName);
            x.AddAspNetCoreInstrumentation();
            if (otlpEndpoint != null)
            {
                x.AddZipkinExporter(options =>
                {
                    options.Endpoint = new Uri(otlpEndpoint);
                });
            }
            else
            {
                x.AddConsoleExporter();
            }
        });
    }

    public static OpenTelemetryBuilder AddMetrics(this OpenTelemetryBuilder builder)
    {
        return builder.WithMetrics(metrics => metrics
            .AddAspNetCoreInstrumentation()
            .AddMeter(InstrumentationOptions.MeterName)
            .AddMeter("Microsoft.AspNetCore.Hosting")
            .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
            .AddPrometheusExporter());
    }
}