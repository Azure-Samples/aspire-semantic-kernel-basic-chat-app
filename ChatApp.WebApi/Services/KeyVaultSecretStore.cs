// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Azure.Security.KeyVault.Secrets;

using ChatApp.WebApi.Interfaces;

namespace ChatApp.WebApi.Services;

public class KeyVaultSecretStore : ISecretStore
{
    private readonly SecretClient _secretClient;

    public KeyVaultSecretStore(SecretClient secretClient)
    {
        _secretClient = secretClient;
    }

    public async Task<string> GetSecretAsync(string secretName, CancellationToken cancellationToken)
    {
        KeyVaultSecret secret = await _secretClient.GetSecretAsync(secretName, cancellationToken: cancellationToken);
        return secret.Value;
    }
}
