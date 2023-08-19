using AI.Test.Support.Fixtures;

using FinancialAgents.Tests.Fixtures;

using Microsoft.Extensions.DependencyInjection;

using Xunit.Abstractions;


// https://github.com/mrspiggot/LucidateFinAgent/blob/master/FinAgentBasic.py
namespace FinancialAgents.Tests;

[Collection("FinancialAgents Collection")]
public class TestOfFinancialAgents
{
    private readonly ILogger logger;
    private readonly HostApplicationFactory hostApplicationFactory;

    public TestOfFinancialAgents(ITestOutputHelper output, FinancialAgentsTestFixture fixture)
    {
        this.hostApplicationFactory = fixture.WithLogging(output).Build();
        this.logger = hostApplicationFactory.Services.GetRequiredService<ILogger>();
    }

    [Fact]
    public async Task RunWebSearchSample()
    {
        //https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example11_WebSearchQueries.cs
        //IKernel kernel = Kernel.Builder
        //    .WithLogger(fixture.MsLogger)
        //    .Build();
        //// Load native skills
        //var skill = new SearchUrlSkill();
        //var bing = kernel.ImportSkill(skill, "search");

        //// Run
        //var ask = "What's the tallest building in London?";
        //var result = await kernel.RunAsync(
        //    ask,
        //    bing["BingSearchUrl"]
        //);

        //logger.Information(ask + "\n");
        //logger.Information(result.ToString());
    }

    [Fact]
    public async Task RunBingGoogleWebSearchSample()
    {
        //https://github.com/microsoft/semantic-kernel/blob/main/dotnet/samples/KernelSyntaxExamples/Example11_WebSearchQueries.cs

        //IKernel kernel = new KernelBuilder()
        //    .WithLogger(fixture.MsLogger)
        //    .WithOpenAITextCompletionService("text-davinci-003", openAIOptions.ApiKey)
        //    .Build();

        //// Load Bing skill
        //using var bingConnector = new BingConnector(fixture.BingOptions.ApiKey);
        //kernel.ImportSkill(new WebSearchEngineSkill(bingConnector), "bing");

        ////// Load Google skill
        //using var googleConnector = new GoogleConnector(fixture.GoogleOptions.ApiKey, fixture.GoogleOptions.SearchEngineId);
        //kernel.ImportSkill(new WebSearchEngineSkill(googleConnector), "google");

        //await Example1Async(kernel);
        //await Example2Async(kernel);
    }


    //    private async Task Example1Async(IKernel kernel)
    //    {
    //        logger.Information("======== Bing and Google Search Skill ========");

    //        // Run
    //        var question = "What's the largest building in the world?";
    //        var bingResult = await kernel.Func("bing", "search").InvokeAsync(question);
    //        //var googleResult = await kernel.Func("google", "search").InvokeAsync(question);

    //        logger.Information(question);
    //        logger.Information("----");
    //        logger.Information(bingResult.ToString());
    //        logger.Information("----");
    //        //logger.Information(googleResult.ToString());
    //    }

    //    private async Task Example2Async(IKernel kernel)
    //    {
    //        logger.Information("======== Use Search Skill to answer user questions ========");

    //        const string SemanticFunction = @"Answer questions only when you know the facts or the information is provided.
    //When you don't have sufficient information you reply with a list of commands to find the information needed.
    //When answering multiple questions, use a bullet point list.
    //Note: make sure single and double quotes are escaped using a backslash char.

    //[COMMANDS AVAILABLE]
    //- bing.search

    //[INFORMATION PROVIDED]
    //{{ $externalInformation }}

    //[EXAMPLE 1]
    //Question: what's the biggest lake in Italy?
    //Answer: Lake Garda, also known as Lago di Garda.

    //[EXAMPLE 2]
    //Question: what's the biggest lake in Italy? What's the smallest positive number?
    //Answer:
    //* Lake Garda, also known as Lago di Garda.
    //* The smallest positive number is 1.

    //[EXAMPLE 3]
    //Question: what's Ferrari stock price ? Who is the current number one female tennis player in the world?
    //Answer:
    //{{ '{{' }} bing.search ""what\\'s Ferrari stock price?"" {{ '}}' }}.
    //{{ '{{' }} bing.search ""Who is the current number one female tennis player in the world?"" {{ '}}' }}.

    //[END OF EXAMPLES]

    //[TASK]
    //Question: {{ $input }}.
    //Answer: ";

    //        var questions = "Who is the most followed person on TikTok right now? What's the exchange rate EUR:USD?";
    //        //logger.Information(questions);

    //        var oracle = kernel.CreateSemanticFunction(SemanticFunction, maxTokens: 200, temperature: 0, topP: 1);

    //        var context = kernel.CreateNewContext();
    //        context["externalInformation"] = "";
    //        var answer = await oracle.InvokeAsync(questions, context);

    //        // If the answer contains commands, execute them using the prompt renderer.
    //        if (answer.Result.Contains("bing.search", StringComparison.OrdinalIgnoreCase))
    //        {
    //            var promptRenderer = new PromptTemplateEngine();

    //            logger.Information("---- Fetching information from Google...");
    //            var information = await promptRenderer.RenderAsync(answer.Result, context);

    //            logger.Information("Information found:");
    //            // logger.Information(information);

    //            // The rendered prompt contains the information retrieved from search engines
    //            context["externalInformation"] = information;

    //            // Run the semantic function again, now including information from Bing
    //            answer = await oracle.InvokeAsync(questions, context);
    //        }
    //        else
    //        {
    //            logger.Information("AI had all the information, no need to query Google.");
    //        }

    //        logger.Information("---- ANSWER:");
    //        logger.Information(answer.ToString());
    //    }

    //    [Fact]
    //    public async Task RunAsync()
    //    {
    //        logger.Information("======== LLMPrompts ========");

    //        //Load Bing skill

    //        IKernel kernel = new KernelBuilder()
    //             .WithLogger(fixture.MsLogger)
    //             .WithOpenAITextCompletionService("text-davinci-002", openAIOptions.ApiKey, serviceId: "text-davinci-002")
    //             .WithOpenAITextCompletionService("text-davinci-003", openAIOptions.ApiKey)
    //             .Build();

    //        // Load native skill
    //        //using var bingConnector = new BingConnector(fixture.BingOptions.ApiKey);
    //        //var bing = new WebSearchEngineSkill(bingConnector);
    //        //var search = kernel.ImportSkill(bing, "bing");

    //        // Load semantic skill defined with prompt templates
    //        //string folder = RepoFiles.SampleSkillsPath();

    //        //var sumSkill = kernel.ImportSemanticSkillFromDirectory(
    //        //    folder,
    //        //    "SummarizeSkill");

    //        //// Run
    //        //var ask = "What's the tallest building in South America?";

    //        //var result1 = await kernel.RunAsync(
    //        //    ask,
    //        //    search["Search"]
    //        //);

    //        //var result2 = await kernel.RunAsync(
    //        //    ask,
    //        //    search["Search"],
    //        //    sumSkill["Summarize"]
    //        //);

    //        //var result3 = await kernel.RunAsync(
    //        //    ask,
    //        //    search["Search"],
    //        //    sumSkill["Notegen"]
    //        //);

    //        //logger.Information(ask + "\n");
    //        //logger.Information("Bing Answer: " + result1 + "\n");
    //        //logger.Information("Summary: " + result2 + "\n");
    //        //logger.Information("Notes: " + result3 + "\n");
    //    }

}
