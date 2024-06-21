# Aspire Sample Application

Welcome to the Aspire Sample Application. This project is a comprehensive example of a chat application built with .NET Aspire, Semantic Kernel, and the `@microsoft/ai-chat-protocol` package. The frontend of the application is developed using React and Vite.

## Overview

The application consists of 2 main Projects:

- `ChatApp.WebApi`: This is a .NET Web API that handles chat interactions, powered by .NET Aspire and Semantic Kernel. It provides endpoints for the chat frontend to communicate with the chat backend. The `@microsoft/ai-chat-protocol` package is used to handle chat interactions, including streaming and non-streaming requests.

- `ChatApp.React`: This is a React app that provides the user interface for the chat application. It is built using Vite, a modern and efficient build tool. It uses the `@microsoft/ai-chat-protocol` package to handle chat interactions, allowing for flexible communication with the chat backend.

The app also includes a class library project, ChatApp.ServiceDefaults, that contains the service defaults used by the service projects.

## Pre-requisites

- .NET 8 SDK
- Optional Visual Studio 2022 17.10
- Node.js 20

## Running the app

If using Visual Studio, open the solution file ChatApp.sln and launch/debug the ChatApp.AppHost project.

If using the .NET CLI, run dotnet run from the ChatApp.AppHost directory.

For more information on local provisioning of Aspire applications, refer to the [Aspire Local Provisioning Guide](https://learn.microsoft.com/en-us/dotnet/aspire/deployment/azure/local-provisioning).


## Resources

- [Aspire Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/)
- [Semantic Kernel Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/semantic-kernel/)
- [Chat Protocol Documentation](https://learn.microsoft.com/en-us/dotnet/aspire/ai-chat-protocol/)

## License

This project is licensed under the terms of the MIT license. See the `LICENSE` file for the full license text.