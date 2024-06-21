// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Text;
using Azure.Identity;
using Microsoft.SemanticKernel;
using ChatApp.WebApi.Interfaces;
using ChatApp.WebApi.Model;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;

namespace ChatApp.WebApi.Services;

public class SemanticKernelConfig
{
    public string Model { get; set; }
    public string Key { get; set; }
    public bool UseAzureOpenAI { get; set; }
    public string Deployment { get; set; }
}

internal class SemanticKernelSession : ISemanticKernelSession
{
    private readonly Kernel _kernel;
    private readonly IStateStore<string> _stateStore;

    public Guid Id { get; private set; }

    internal SemanticKernelSession(Kernel kernel, IStateStore<string> stateStore, Guid sessionId)
    {
        _kernel = kernel;
        _stateStore = stateStore;
        Id = sessionId;
    }

    const string prompt = @"
        ChatBot can have a conversation with you about any topic.
        It can give explicit instructions or say 'I don't know' if it does not know the answer.

        {{$history}}
        User: {{$userInput}}
        ChatBot:";

    public async Task<AIChatCompletion> ProcessRequest(AIChatRequest message)
    {
        var chatFunction = _kernel.CreateFunctionFromPrompt(prompt);
        var userInput = message.Messages.Last();
        string history = await _stateStore.GetStateAsync(Id) ?? "";
        var arguments = new KernelArguments()
        {
            ["history"] = history,
            ["userInput"] = userInput.Content,
        };
        var botResponse = await chatFunction.InvokeAsync(_kernel, arguments);
        var updatedHistory = $"{history}\nUser: {userInput.Content}\nChatBot: {botResponse}";
        await _stateStore.SetStateAsync(Id, updatedHistory);
        return new AIChatCompletion(Message: new AIChatMessage
        {
            Role = AIChatRole.Assistant,
            Content = $"{botResponse}",
        })
        {
            SessionState = Id,
        };
    }

    public async IAsyncEnumerable<AIChatCompletionDelta> ProcessStreamingRequest(AIChatRequest message)
    {
        var chatFunction = _kernel.CreateFunctionFromPrompt(prompt);
        var userInput = message.Messages.Last();
        string history = await _stateStore.GetStateAsync(Id) ?? "";
        var arguments = new KernelArguments()
        {
            ["history"] = history,
            ["userInput"] = userInput.Content,
        };
        var streamedBotResponse = chatFunction.InvokeStreamingAsync(_kernel, arguments);
        StringBuilder response = new();
        await foreach (var botResponse in streamedBotResponse)
        {
            response.Append(botResponse);
            yield return new AIChatCompletionDelta(Delta: new AIChatMessageDelta
            {
                Role = AIChatRole.Assistant,
                Content = $"{botResponse}",
            })
            {
                SessionState = Id,
            };
        }
        var updatedHistory = $"{history}\nUser: {userInput.Content}\nChatBot: {response}";
        await _stateStore.SetStateAsync(Id, updatedHistory);
    }

}

public class SemanticKernelApp : ISemanticKernelApp
{
    private readonly SemanticKernelConfig _aiConfig;
    private readonly IStateStore<string> _stateStore;
    private readonly Lazy<Task<Kernel>> _kernel;
    private readonly OpenAIClient _openAIClient;

    private async Task<Kernel> InitKernel()
    {
        var builder = Kernel.CreateBuilder();
        if (_aiConfig.UseAzureOpenAI)
        {
            if (_aiConfig.Deployment is null || _openAIClient is null)
            {
                throw new InvalidOperationException("AzureOpenAI is enabled but AzureDeployment and AzureEndpoint are not set.");
            }
            builder.AddAzureOpenAIChatCompletion(_aiConfig.Deployment, _openAIClient);
        }
        else 
        {
            if (_aiConfig.Model is null || _aiConfig.Key is null)
            {
                throw new InvalidOperationException("AzureOpenAI is disabled but Model and APIKey are not set.");
            }
            builder.AddOpenAIChatCompletion(_aiConfig.Model, _aiConfig.Key);
        }
        return builder.Build();
    }


    public SemanticKernelApp(IOptions<SemanticKernelConfig> aiConfig, IStateStore<string> stateStore, OpenAIClient openAIClient)
    {
        _aiConfig = aiConfig.Value;
        _stateStore = stateStore;
        _openAIClient = openAIClient;
        _kernel = new(() => Task.Run(InitKernel));
    }


    public async Task<ISemanticKernelSession> CreateSession(Guid sessionId)
    {
        var kernel = await _kernel.Value;
        return new SemanticKernelSession(kernel, _stateStore, sessionId);
    }

    public async Task<ISemanticKernelSession> GetSession(Guid sessionId)
    {
        var kernel = await _kernel.Value;
        var state = await _stateStore.GetStateAsync(sessionId);
        if (state is null)
        {
            throw new KeyNotFoundException($"Session {sessionId} not found.");
        }
        return new SemanticKernelSession(kernel, _stateStore, sessionId);
    }
}