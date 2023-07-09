
# ai-playground
The purpose of this repository is to provide a playground for learning about using AI API, specifically OpenAI and other language models, using dotnet. It aims to demonstrate how to integrate and use AI products together with dotnet technologies.

### OpenAI:
A dotnet Client 'OpenAI.Client' with a Test project and a WPF application that shows how to use it.

### Llamasharp:
A dotnet Web API 'LLamaSharpApp.WebAPI' with a Test project that show how to use it. This project could be a candidate for a missing feature at Llamasharp.


### Qdrant Vector database:
A dotnet Web API for Qdrant Vector database 'AI.VectorDatabase.Qdrant' has been implemented and a Test project that show how to use it. A new implementation of the NuGet package 'Qdrant.Client' has been implemented

### Conversation as a Platform:
A dotnet Web API 'AI.CaaP.WebAPI' + 'AI.CaaP.Repository' + 'AI.CaaP' for working on a Conversation as a Platform experience, with a Test project that show how to use it.


### Dependencies:
The projects uses other Open Source project: 

Oneof, Serilog, SerilogTimings, Destructurama, Polly, NetEscapades.AspNetCore.SecurityHeader, Riok.Mapperly, LLamaSharp,  LLamaSharp.Backend.Cpu.

The WEB API projects uses Azure AD for Authorization:

https://damienbod.com/2020/05/29/login-and-use-asp-net-core-api-with-azure-ad-auth-and-user-access-tokens/


## Inspiration
Inspiration and resources for working with AI products is found from the following:

The official OpenAI documentation provides detailed information about the API reference and other relevant resources. You can find it at:

#### OpenAI API Reference:
https://platform.openai.com/docs/api-reference/

#### OpenAI Documentation: 
https://platform.openai.com/docs/

#### Azure OpenAI Services:
Microsoft Azure Cognitive Services also provide support for OpenAI. You can explore the overview and documentation of Azure OpenAI Services at:
https://learn.microsoft.com/en-us/azure/cognitive-services/openai/overview
https://devblogs.microsoft.com/dotnet/getting-started-azure-openai-dotnet/

#### Qdrant:
https://qdrant.github.io/qdrant/redoc/index.html


## GitHub Repositories
Several GitHub repositories offer useful code examples, libraries, and resources related to OpenAI and dotnet integration. Here are some repositories worth checking out:
Please explore these resources to gain insights and discover more possibilities for using AI products together with dotnet.

#### ChatGPT by lencx:
https://github.com/lencx/ChatGPT

#### Awesome ChatGPT Prompts:
https://github.com/f/awesome-chatgpt-prompts

#### OpenAI-DotNet by RageAgainstThePixel:
https://github.com/RageAgainstThePixel/OpenAI-DotNet

#### OpenAI with F# by yazeedobaid:
https://github.com/yazeedobaid/openai-fsharp

#### Getting Started with Chat GPT Integration with C# Console Application by rmauro.dev:
https://rmauro.dev/getting-started-with-chat-gpt-integration-with-csharp-console-application/

#### Integrate OpenAI with .NET Core and C# by dotnetoffice.com:
https://www.dotnetoffice.com/2023/02/integrate-openai-with-net-core-and.html

#### OpenAiNet by Open Source Agenda:
https://www.opensourceagenda.com/projects/openainet


#### Azure Authentication/Identity:
https://damienbod.com/
https://damienbod.com/2020/05/29/login-and-use-asp-net-core-api-with-azure-ad-auth-and-user-access-tokens/


### Youtube:
```
TheAIGRID
Microsoft Developer
Matthew Berman
Adrian Twarog
Dave Ebbelaar
Matt Wolfe
Obscurious
ByteByteGo
```

## Configuration

#### OpenAI:
Add appsettings.json (I recommend using user secrets to avoid having the API key in the Repository)

```json
{
    "OpenAI": {
    "ApiKey": "<openai api key>",
    "OrganisationKey": "<organisation key>",
    "OpenAIUri": "https://api.openai.com/v1/"
  }
}
```

#### LlamaSharp:
Add this section into appsettings.json


```json
  "LlmaModel": {
    "modelPath": "LlmaModels\\wizardLM-7B.ggmlv3.q4_1.bin",
  }

```


#### Qdrant:
Add this section into appsettings.json

```json
  "Qdrant": {
    "Url": "http://localhost:6333/"
  }

```

## Tasks
The following tasks can be accomplished using this repository:

Explore the provided code examples to understand how to integrate OpenAI and other language models with dotnet.

Experiment with different AI models and their capabilities in a dotnet environment.

Build your own AI-powered applications using the knowledge gained from this repository.

Contribute to the repository by adding additional code samples, enhancements, or bug fixes.

Feel free to modify the existing code or add new features according to your requirements and learning objectives.

## Contributing
If you would like to contribute to this repository, please follow these guidelines:

Fork the repository.
Create a new branch for your feature or bug fix.
Make your changes and test them thoroughly.
Commit your changes with clear and descriptive commit messages.
Push your branch to your forked repository.
Create a pull request, explaining the changes you have made.
Your contributions are highly appreciated, and together we can improve the learning experience for everyone using AI products with dotnet.

## License
This repository is licensed under the MIT License. You are free to use, modify, and distribute the code for both commercial and non-commercial purposes. However, please note that any AI models or services used in conjunction with this code may have their own licensing requirements, so make sure to comply with their respective terms and conditions.

## About the Author
This repository is maintained by Karl Godtliebsen. You can connect with me on [GitHub] (https://github.com/karlgodtliebsen)


