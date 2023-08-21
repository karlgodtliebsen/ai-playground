using AI.Test.Support.Fixtures;
using AI.VectorDatabase.Qdrant.VectorStorage.Models;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.SkillDefinition;
using Microsoft.SemanticKernel.Skills.Core;
using Microsoft.SemanticKernel.Skills.Web;
using Microsoft.SemanticKernel.Skills.Web.Bing;

using OpenAI.Client.Configuration;

using SemanticKernel.Tests.Configuration;
using SemanticKernel.Tests.Domain;
using SemanticKernel.Tests.Fixtures;
using SemanticKernel.Tests.Skills;

using Xunit.Abstractions;

using TimeSkill = SemanticKernel.Tests.Skills.TimeSkill;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel With Docker Collection")]
public class TestOfSemanticKernelExample31CustomPlanner
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly IServiceProvider services;
    private readonly OpenAIOptions openAIOptions;
    private readonly BingOptions bingOptions;
    private readonly string skillsPath;

    private const string CollectionName = "SemanticKernel-customplanner-test-collection";
    private const int VectorSize = 1536;

    const string Model = "gpt-3.5-turbo";
    const string CompletionModel = "text-davinci-003";
    const string EmbeddingModel = "text-embedding-ada-002";
    public TestOfSemanticKernelExample31CustomPlanner(SemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        this.hostApplicationFactory = fixture.WithOutputLogSupport(output).Build();
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.msLogger = services.GetRequiredService<ILogger<TestOfSemanticKernel>>();
        this.openAIOptions = services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
        this.bingOptions = services.GetRequiredService<IOptions<BingOptions>>().Value;
        this.skillsPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skills"));
    }


    [Fact]
    public async Task UseCustomPlanner_Example31()
    {
        bool recreateCollection = true;
        bool storeOnDisk = false;

        var factory = hostApplicationFactory.Services.GetRequiredService<IQdrantMemoryStoreFactory>();
        var memoryStorage = await factory.Create(CollectionName, VectorSize, Distance.COSINE, recreateCollection, storeOnDisk, CancellationToken.None);

        logger.Information("======== Custom Planner - Create and Execute Markup Plan ========");
        IKernel kernel = new KernelBuilder()
            .WithLogger(msLogger)
            .WithOpenAITextCompletionService(CompletionModel, openAIOptions.ApiKey)
            .WithOpenAITextEmbeddingGenerationService(EmbeddingModel, openAIOptions.ApiKey)
            .WithMemoryStorage(memoryStorage)
            .Build();


        // ContextQuery is part of the QASkill
        IDictionary<string, ISKFunction> skills = LoadQASkill(kernel);
        SKContext context = CreateContextQueryContext(kernel);

        // Setup defined memories for recall
        await RememberFactsAsync(kernel);

        // MarkupSkill named "markup"
        var markup = kernel.ImportSkill(new MarkupSkill(logger), "markup");

        // contextQuery "Who is my president? Who was president 10 years ago? What should I eat for dinner" | markup
        // Create a plan to execute the ContextQuery and then run the markup skill on the output
        var plan = new Plan("Execute ContextQuery and then RunMarkup");
        plan.AddSteps(skills["ContextQuery"], markup["RunMarkup"]);

        // Execute plan
        context.Variables.Update("Who is my president? Who was president 10 years ago? What should I eat for dinner");
        var result = await plan.InvokeAsync(context);

        logger.Information("Result:");
        logger.Information(result.Result);
        logger.Information("");
    }

    /* Example Output
    ======== Custom Planner - Create and Execute Markup Plan ========
    Markup:
    <response><lookup>Who is United States President</lookup><fact>Joe Biden was president 2 years ago</fact><opinion>For dinner, you might enjoy some sushi with your partner, since you both like it and you only ate it once this month</opinion></response>

    Original plan:
        Goal: Run a piece of xml markup

        Steps:
        Goal: response

        Steps:
        - bing.SearchAsync INPUT='Who is United States President' => markup.SearchAsync.result    - Microsoft.SemanticKernel.Planning.Plan. INPUT='Joe Biden was president 2 years ago' => markup.fact.result    - Microsoft.SemanticKernel.Planning.Plan. INPUT='For dinner, you might enjoy some sushi with your partner, since you both like it and you only ate it once this month' => markup.opinion.result

    Result:
    The president of the United States ( POTUS) [A] is the head of state and head of government of the United States of America. The president directs the executive branch of the federal government and is the commander-in-chief of the United States Armed Forces .
    Joe Biden was president 2 years ago
    For dinner, you might enjoy some sushi with your partner, since you both like it and you only ate it once this month
    */

    private SKContext CreateContextQueryContext(IKernel kernel)
    {
        var context = kernel.CreateNewContext();
        context.Variables.Set("firstname", "Jamal");
        context.Variables.Set("lastname", "Williams");
        context.Variables.Set("city", "Tacoma");
        context.Variables.Set("state", "WA");
        context.Variables.Set("country", "USA");
        context.Variables.Set("collection", CollectionName);//contextQueryMemories
        context.Variables.Set("limit", "5");
        context.Variables.Set("relevance", "0.3");
        return context;
    }

    private async Task RememberFactsAsync(IKernel kernel)
    {
        kernel.ImportSkill(new TextMemorySkill());

        List<string> memoriesToSave = new()
        {
            "I like pizza and chicken wings.",
            "I ate pizza 10 times this month.",
            "I ate chicken wings 3 time this month.",
            "I ate sushi 1 time this month.",
            "My partner likes sushi and chicken wings.",
            "I like to eat dinner with my partner.",
            "I am a software engineer.",
            "I live in Tacoma, WA.",
            "I have a dog named Tully.",
            "I have a cat named Butters.",
        };

        foreach (var memoryToSave in memoriesToSave)
        {
            //"contextQueryMemories"
            await kernel.Memory.SaveInformationAsync(CollectionName, memoryToSave, Guid.NewGuid().ToString());
        }
    }

    // ContextQuery is part of the QASkill
    // DependsOn: TimeSkill named "time"
    // DependsOn: BingSkill named "bing"
    private IDictionary<string, ISKFunction> LoadQASkill(IKernel kernel)
    {
        string folder = skillsPath;
        kernel.ImportSkill(new TimeSkill(), "time");
        var bing = new WebSearchEngineSkill(new BingConnector(bingOptions.ApiKey));
        var search = kernel.ImportSkill(bing, "bing");
        return kernel.ImportSemanticSkillFromDirectory(folder, "QASkill");
    }
}

