var builder = DistributedApplication.CreateBuilder(args);

var useAzureOpenAI = bool.Parse(Environment.GetEnvironmentVariable("UseAzureOpenAI") ?? "true");
var azureDeployment = Environment.GetEnvironmentVariable("AzureDeployment") ?? "chat";
var azureEndpoint = Environment.GetEnvironmentVariable("AzureEndpoint");

var openAi = builder.AddAzureOpenAI("openAi")
    .AddDeployment(new AzureOpenAIDeployment(azureDeployment, "gpt-4o", "2024-05-13"));

var backend = builder.AddProject<Projects.ChatApp_WebApi>("backend")
    .WithReference(openAi)
    .WithEnvironment("AzureDeployment", azureDeployment)
    .WithEnvironment("AzureEndpoint", azureEndpoint);

var frontend = builder.AddNpmApp("frontend", "../ChatApp.React")
    .WithReference(backend)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
