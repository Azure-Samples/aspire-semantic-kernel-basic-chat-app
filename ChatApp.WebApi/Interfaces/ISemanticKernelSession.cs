// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using ChatApp.WebApi.Model;

namespace ChatApp.WebApi.Interfaces;
public interface ISemanticKernelSession
{
    Guid Id { get; }
    Task<AIChatCompletion> ProcessRequest(AIChatRequest request);
    IAsyncEnumerable<AIChatCompletionDelta> ProcessStreamingRequest(AIChatRequest request);
}
