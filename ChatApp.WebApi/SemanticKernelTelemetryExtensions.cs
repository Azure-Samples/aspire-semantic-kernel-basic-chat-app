using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace ChatApp.WebApi
{
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
