using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace ChatApp.WebApi
{
    // Adds OpenTelemetry support for the Semantic Kernel.
    // As the semantic conventions for gen_ai telemetry are still experimental,
    // it needs to be enabled with an App Context switch.
    // Sensitive mode will add the chat messages to the telemetry, which is useful
    // for debugging but should not be used in production.
    // For more details see https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/Demos/TelemetryWithAppInsights/README.md

    public static class SemanticKernelTelemetryExtensions
    {
        public static IHostApplicationBuilder AddSemanticKernelTelemetry(this IHostApplicationBuilder builder)
        {
            if (builder.Environment.IsDevelopment())
            {
                AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnosticsSensitive", true);
            }
            else
            {
                AppContext.SetSwitch("Microsoft.SemanticKernel.Experimental.GenAI.EnableOTelDiagnostics", true);
            }


            builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.AddMeter("Microsoft.SemanticKernel*")
                   .AddMeter("Azure.*");
            })
            .WithTracing(tracing =>
            {
                tracing.AddSource("Microsoft.SemanticKernel.*")
                .AddSource("Azure.*")
               .AddSource("Microsoft.ML.*");
            });

            return builder;
        }
    }
}
