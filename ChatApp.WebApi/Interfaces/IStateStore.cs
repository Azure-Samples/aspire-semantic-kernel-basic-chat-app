// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace ChatApp.WebApi.Interfaces;

public interface IStateStore<T>
{
    Task<T?> GetStateAsync(Guid sessionId);
    Task SetStateAsync(Guid sessionId, T state);
    Task RemoveStateAsync(Guid sessionId);
}
