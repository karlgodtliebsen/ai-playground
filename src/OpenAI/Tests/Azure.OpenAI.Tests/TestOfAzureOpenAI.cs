using Azure.AI.OpenAI;
using Azure.OpenAI.Tests.Utils;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using OpenAI.Client.Configuration;
using OpenAI.Client.OpenAI.Models.Chat;

using Xunit.Abstractions;

namespace Azure.OpenAI.Tests;


public class TestOfAzureOpenAI
{
    private readonly ITestOutputHelper output;
    private readonly ILogger logger;
    private readonly OpenAIClient azureOpenAIClient;
    private readonly HostApplicationFactory factory;
    private readonly OpenAIOptions options;


    public TestOfAzureOpenAI(ITestOutputHelper output)
    {
        this.output = output;
        this.factory = HostApplicationFactory.Build(
            environment: () => "IntegrationTests",
            serviceContext: (services, configuration) =>
            {
                services.AddAzureOpenAIConfiguration(configuration);
            },
            fixedDateTime: () => DateTimeOffset.UtcNow,
            output: () => output
        );
        logger = factory.Services.GetRequiredService<ILogger>();
        options = factory.Services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
        azureOpenAIClient = new OpenAIClient(options.ApiKey);
    }


    [Fact]
    public async Task VerifyInitialAccessToTextCompletionModel()
    {
        //https://platform.openai.com/docs/models/overview
        //text-davinci-003
        //Can do any language task with better quality, longer output, and consistent instruction-following
        //than the curie, babbage, or ada models. Also supports some additional features such as inserting text.
        string deploymentName = "text-davinci-003"; //text-davinci-003 text-davinci-002
        string prompt = "Tell us something about .NET development.";
        output.WriteLine($"Input: {prompt}");

        var completionsResponse = await azureOpenAIClient.GetCompletionsAsync(deploymentName, prompt, CancellationToken.None);
        string completion = completionsResponse.Value.Choices[0].Text;

        output.WriteLine(completion);
    }


    [Fact]
    public async Task VerifyInitialAccessToCodeCompletionModel()
    {
        //https://platform.openai.com/docs/models/overview
        //code-davinci-002
        //Optimized for code-completion tasks
        string deploymentName = "text-davinci-002"; //text-davinci-002 code-davinci-002

        string prompt = "\"Tell us something about .NET development.\";";
        output.WriteLine($"Input: {prompt}");

        var completionsResponse = await azureOpenAIClient.GetCompletionsAsync(deploymentName, prompt, CancellationToken.None);
        string completion = completionsResponse.Value.Choices[0].Text;

        output.WriteLine(completion);
    }

    [Fact]
    public async Task VerifyInitialAccessToChatGpt35CompletionAndChatModel()
    {
        //https://platform.openai.com/docs/models/overview
        //GPT-3.5 models can understand and generate natural language or code. Our most capable and cost effective model in
        //the GPT-3.5 family is gpt-3.5-turbo which has been optimized for chat but works well for traditional completions tasks as well.
        string deploymentName = "gpt-3.5-turbo";        //gpt-3.5-turbo-16k gpt-3.5-turbo   

        string prompt = "Please show a sample for a 'for loop' in C#";
        output.WriteLine($"Input: {prompt}");

        var options = new ChatCompletionsOptions()
        {
            Messages = {
                new ChatMessage(
                    new ChatRole(ChatMessageRole.User.ToString()),
                    prompt)
            }
        };

        var chatResponse = await azureOpenAIClient.GetChatCompletionsAsync(deploymentName, options, CancellationToken.None);
        string message = chatResponse.Value.Choices[0].Message.Content;
        output.WriteLine(message);
    }


    [Fact]
    public async Task VerifyInitialAccessToChatGpt4Model()
    {
        //https://platform.openai.com/docs/models/overview
        //On June 27th, 2023, gpt-4 will be updated to point from gpt-4-0314 to gpt-4-0613, the latest model iteration.
        string deploymentName = "gpt-4";   //gpt-4-0314 gpt-4-32k-0314  gpt-4
        string prompt = "Please show a sample for a 'for loop' in C#";
        output.WriteLine($"Input: {prompt}");

        var options = new ChatCompletionsOptions()
        {
            Messages = { new ChatMessage(new ChatRole(ChatMessageRole.User.ToString()), prompt) }
        };
        var chatResponse = await azureOpenAIClient.GetChatCompletionsAsync(deploymentName, options, CancellationToken.None);
        string message = chatResponse.Value.Choices[0].Message.Content;
        output.WriteLine(message);
    }


}
