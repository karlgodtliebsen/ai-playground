
# ai-playground
The purpose of this repository is to provide a playground for learning about using AI API, specifically OpenAI and other language models, using dotnet. It aims to demonstrate how to integrate and use AI products together with dotnet technologies.



## Update October 2023
The upcoming tasks are:
- Rebuild Semantic Kernel and Semantic Memory and KafkaNewsFeed+Semantic Memory: In progress
- Verify the new GGUF models.    Completed.
- Look into new Wizard, Orca and Dolphin models. Completed,
- OpenSource Model for OpenAI (Dolpin ?)
- Use the integration og Semantic Kernal and LLamaSharp. In progress.
- Better image classification



### OpenAI:
A dotnet Client 'OpenAI.Client' with a Test project and a WPF application that shows how to use it.

### Llamasharp (Model version 2 updated to gguf files):
A dotnet Web API 'LLamaSharpApp.WebAPI' with a Test project that show how to use it. 


### Changes August 2023
As of August 2023 this is the model recommendation from LlamaSharp

Recommentations from LlamaSharp:
- https://github.com/SciSharp/LLamaSharp
- v0.5.1	v0.5.1	Llama2 7b GGUF

#### Llama2 7B Guanaco QLoRA GGUF
- https://huggingface.co/TheBloke/llama-2-7B-Guanaco-QLoRA-GGUF
- llama-2-7b-guanaco-qlora.Q4_K_M.gguf	Q4_K_M	4	4.08 GB	6.58 GB		medium, balanced quality - recommended

### Needs testing
#### Guanaco-7B-Uncensored-GGUF
- https://huggingface.co/TheBloke/Guanaco-7B-Uncensored-GGUF
- guanaco-7b-uncensored.Q4_K_M.gguf	Q4_K_M	4	4.08 GB	6.58 GB	medium, balanced quality - recommended
	

#### Guanaco-13B-Uncensored-GGUF
- https://huggingface.co/TheBloke/Guanaco-13B-Uncensored-GGUF
- guanaco-13b-uncensored.Q4_K_M.gguf	Q4_K_M	4	7.87 GB	10.37 GB	medium, balanced quality - recommended


#### Llama-2-13B-Ensemble-v5-GGUF
- https://huggingface.co/TheBloke/Llama-2-13B-Ensemble-v5-GGUF
- llama-2-13b-ensemble-v5.Q4_K_M.gguf	Q4_K_M	4	7.87 GB	10.37 GB		medium, balanced quality - recommended


#### Llama-2-70B-Ensemble-v5-GGUF
- https://huggingface.co/TheBloke/Llama-2-70B-Ensemble-v5-GGUF
- llama-2-70b-ensemble-v5.Q4_K_M.gguf	Q4_K_M	4	41.42 GB	43.92 GB	medium, balanced quality - recommended

#### OpenChat v3.2 Super - GGUF
- https://huggingface.co/TheBloke/openchat_v3.2_super-GGUF
- https://huggingface.co/openchat/openchat_v3.2_super


## October 2023 Models

### Mistral Dolphin
- dolphin-2.1-mistral-7b.Q4_K_M.gguf
- 
### Mistral OpenOrca
- mistral-7b-openorca.Q4_K_M.gguf


## Projects:

### OpenAI:
 OpenAI Clients Library with Tests and a Console and Wpf application that shows how to use it.

### LLamaSharp:
A dotnet Web API 'LLamaSharpApp.WebAPI' with Domain Project and aa Test project that show how to use it.


Download a model like: 
- llama-2-7b.Q4_0.gguf
- llama-2-7b-guanaco-qlora.Q4_K_M.gguf (see references above)

Add the Models to a folder and update the appsettings.json files with the path to the folder.
I use: '/projects/AI/LlamaModels' to store the models.

More info can be found in the README.md file in the LlamaSharpApp.WebAPI project.

### Microsoft SemanticKernel and LlamaSharp:

A Domain and Test Project for using LlamaSharp in SemanticKernel.

### Microsoft.SemanticKernel:
A dotnet Test Project that uses the Microsoft Semantic Kernel Library (MSKLC) to play around with semantic kernel SDK

### Microsoft.SemanticMemory:
A dotnet Test Project that uses the early version of Microsoft Semantic Memory

### SemanticMemory + Kafka + Streaming of NewsFeed:
A dotnet Test Project that combines the early version of Microsoft Semantic Memory with Kafka streaming of NewsFeed


### Qdrant Vector database:
A dotnet Web API for Qdrant Vector database. 
A replacement implementation of the NuGet package 'Qdrant.Client' can be found in 'AI.VectorDatabase.Qdrant' together with a Test project that show how to use it.

### Conversation as a Platform:
A dotnet Web API 'AI.CaaP.WebAPI' + 'AI.CaaP.Repository' + 'AI.CaaP' for working on a Conversation as a Platform experience, with a Test project that show how to use it.

### Financial Agents:
The start of a dotnet Test project that uses inspiration from LucidateFinAgent to build specific knowledge for Planner and Execution Concepts, and Building Tools
Much more work is needed to make this a real project.
The implementation is based the Microsoft Semantic Kernel Library

### ImageClassification - TensorFlow.Net:
A set of Dotnet projects that uses Microsoft.ML for dotnet and TensorFlow.Net + TensorFlow.Keras to classify images. 
I use: '/projects/AI/image-dataSets' to store the images and the models.
More info in the README.md file in ML.Net.ImageClassification.Tests folder 


### Dependencies:
The projects uses other Open Source project: 

```
- Oneof
- Serilog
- SerilogTimings
- Destructurama
- Polly
- NetEscapades.AspNetCore.SecurityHeader
- Riok.Mapperly
- LLamaSharp
- LLamaSharp.SemanticKernel
- LLamaSharp.Backend.Cpu
- Llama models from Huggingface
- Qdrant Vector Db
- Microsoft Semantic Kerlenl Library (MSKLC)
- Microsoft Semantic Memory
- TiktokenSharp
- TensorFlow.Net
- TensorFlow.Keras
- Microsoft.ML
- Microsoft.ML.DataView
- Microsoft.ML.ImageAnalytics
- Microsoft.ML.Vision
- SciSharp.TensorFlow.Redist
- Microsoft.SemanticKernel
- Microsoft Semanticmenory
- TestContainer
- NSubstitue
- Kafka Confluent Client
- Kafka Streamwiz Client
```

The WEB API projects uses Azure AD for Authentication:

> https://damienbod.com/2020/05/29/login-and-use-asp-net-core-api-with-azure-ad-auth-and-user-access-tokens/



## Inspiration
Inspiration and resources for working with AI products is found from the following:

The official OpenAI documentation provides detailed information about the API reference and other relevant resources. You can find it at:

#### OpenAI API Reference:
> https://platform.openai.com/docs/api-reference/

#### OpenAI Documentation: 
> https://platform.openai.com/docs/

#### Azure OpenAI Services:
Microsoft Azure Cognitive Services also provide support for OpenAI. 
You can explore the overview and documentation of Azure OpenAI Services at:

> https://learn.microsoft.com/en-us/azure/cognitive-services/openai/overview
> https://devblogs.microsoft.com/dotnet/getting-started-azure-openai-dotnet/

#### Qdrant:
> https://qdrant.github.io/qdrant/redoc/index.html


## GitHub Repositories
Several GitHub repositories offer useful code examples, libraries, and resources related to OpenAI and dotnet integration. 

Please explore these resources to gain insights and discover more possibilities for using AI products together with dotnet.

Here are some repositories worth checking out:

#### ChatGPT by lencx:
> https://github.com/lencx/ChatGPT

#### Awesome ChatGPT Prompts:
> https://github.com/f/awesome-chatgpt-prompts

#### OpenAI-DotNet by RageAgainstThePixel:
> https://github.com/RageAgainstThePixel/OpenAI-DotNet

#### OpenAI with F# by yazeedobaid:
> https://github.com/yazeedobaid/openai-fsharp

#### Getting Started with Chat GPT Integration with C# Console Application by rmauro.dev:
> https://rmauro.dev/getting-started-with-chat-gpt-integration-with-csharp-console-application/

#### Integrate OpenAI with .NET Core and C# by dotnetoffice.com:
> https://www.dotnetoffice.com/2023/02/integrate-openai-with-net-core-and.html

#### OpenAiNet by Open Source Agenda:
> https://www.opensourceagenda.com/projects/openainet


#### Azure Authentication/Identity:
> https://damienbod.com/

> https://damienbod.com/2020/05/29/login-and-use-asp-net-core-api-with-azure-ad-auth-and-user-access-tokens/

### Llama models (llma.cpp/ggml) from Huggingface:
> https://huggingface.co/TheBloke:

```
- llama-2-7b.Q4_0.gguf
```
This model requires MetaAI registration/approval


### Work in progress:

- After August model updates more models must be tested
- Complete prompting setup for LlamaSharp, for instruction and chat completion modes.
- Upgrade to Dall-E 3
- Upgrade TensorFlow.Net


### Youtube:
```
- TheAIGRID
- Microsoft Developer
- Matthew Berman
- Adrian Twarog
- Dave Ebbelaar
- Matt Wolfe
- Obscurious
- ByteByteGo
```

## Configuration


#### OpenAI:
Add appsettings.json (I recommend using user secrets to avoid having the API key in the Repository)

```json
{
    "OpenAI": {
    "ApiKey": "<openai api key>",
    "OrganisationKey": "<organisation key>",
    "Endpoint": "https://api.openai.com/v1/"
  }
}
```

However, you can also add *.development.json or *.IntegrationTests.json to the .gitignore if you want to use config files for secret storage.


#### LlamaSharp:
Add this section into appsettings.json

```json
  "LlmaModel": {
     "modelPath": "/projects/AI/LlamaModels/llama-2-7b.Q4_0.gguf"
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


