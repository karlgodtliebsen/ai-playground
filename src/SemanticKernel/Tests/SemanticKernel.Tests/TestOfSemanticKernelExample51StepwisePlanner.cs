using System.Diagnostics;

using AI.Test.Support.DockerSupport;
using AI.Test.Support.Fixtures;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Skills.Web;
using Microsoft.SemanticKernel.Skills.Web.Bing;

using OpenAI.Client.Configuration;

using SemanticKernel.Tests.Configuration;
using SemanticKernel.Tests.Fixtures;
using SemanticKernel.Tests.Skills;

using Xunit.Abstractions;

using TimeSkill = SemanticKernel.Tests.Skills.TimeSkill;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Collection")]
public class TestOfSemanticKernelExample51StepwisePlanner : IAsyncLifetime
{
    private readonly ILogger logger;
    private readonly ILoggerFactory loggerFactory;


    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly IServiceProvider services;
    private readonly OpenAIOptions openAIOptions;
    private readonly AzureOpenAIOptions azureOpenAIOptions;
    private readonly BingOptions bingOptions;
    const string Model = "gpt-3.5-turbo";

    private readonly string skillsPath;

    // Used to override the max allowed tokens when running the plan
    private int? ChatMaxTokens = null;
    private int? TextMaxTokens = null;

    // Used to quickly modify the chat model used by the planner
    private readonly string? chatModelOverride = null; //"gpt-35-turbo";
    private readonly string? textModelOverride = null; //"text-davinci-003";
    private readonly List<ExecutionResult> executionResults = new();
    private readonly SemanticKernelTestFixture fixture;

    private string? Suffix = null;

    public Task InitializeAsync()
    {
        return fixture.InitializeAsync();
    }

    public Task DisposeAsync()
    {
        return fixture.DisposeAsync();
    }

    public TestOfSemanticKernelExample51StepwisePlanner(SemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        this.fixture = fixture;
        this.hostApplicationFactory = fixture.WithOutputLogSupport<TestFixtureBaseWithDocker>(output).WithQdrantSupport().Build();
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.loggerFactory = services.GetRequiredService<ILoggerFactory>();
        this.openAIOptions = services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
        this.bingOptions = services.GetRequiredService<IOptions<BingOptions>>().Value;
        this.skillsPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skills"));
    }


    [Fact(Skip = "Need Azure Options clarified")]
    public async Task UseStepwisePlanner_Example51()
    {
        logger.Information("======== StepwisePlanner ========");
        //string[] questions = new string[]
        //{
        //    "Who is the current president of the United States? What is his current age divided by 2",
        //    // "Who is Leo DiCaprio's girlfriend? What is her current age raised to the (his current age)/100 power?",
        //    // "What is the capital of France? Who is that cities current mayor? What percentage of their life has been in the 21st century as of today?",
        //    // "What is the current day of the calendar year? Using that as an angle in degrees, what is the area of a unit circle with that angle?"
        //};


        string[] questions = new string[]
        {
            "What color is the sky?", "What is the weather in Seattle?", "What is the tallest mountain on Earth? How tall is it divided by 2?",
            "What is the capital of France? Who is that city's current mayor? What percentage of their life has been in the 21st century as of today?",
            "What is the current day of the calendar year? Using that as an angle in degrees, what is the area of a unit circle with that angle?",
            "If a spacecraft travels at 0.99 the speed of light and embarks on a journey to the nearest star system, Alpha Centauri, which is approximately 4.37 light-years away, how much time would pass on Earth during the spacecraft's voyage?"
        };

        foreach (var question in questions)
        {
            for (int i = 0; i < 1; i++)
            {
                await RunTextCompletion(question);
                await RunChatCompletion(question);
                //    logger.Information("RunChatCompletion");
                //    await RunWithQuestion(kernel, question);
            }
        }
    }

    private struct ExecutionResult
    {
        public string mode;
        public string? model;
        public string? question;
        public string? answer;
        public string? stepsTaken;
        public string? iterations;
        public string? timeTaken;
    }


    private async Task RunTextCompletion(string question)
    {
        logger.Information("RunTextCompletion");
        ExecutionResult currentExecutionResult = default;
        currentExecutionResult.mode = "RunTextCompletion";
        var kernel = GetKernel(ref currentExecutionResult);
        await RunWithQuestion(kernel, currentExecutionResult, question, TextMaxTokens);
    }

    private async Task RunChatCompletion(string question, string? model = null)
    {
        logger.Information("RunChatCompletion");
        ExecutionResult currentExecutionResult = default;
        currentExecutionResult.mode = "RunChatCompletion";
        var kernel = GetKernel(ref currentExecutionResult, true, model);
        await RunWithQuestion(kernel, currentExecutionResult, question, ChatMaxTokens);
    }

    private async Task RunWithQuestion(IKernel kernel, ExecutionResult currentExecutionResult, string question, int? MaxTokens = null)
    {
        currentExecutionResult.question = question;
        var bingConnector = new BingConnector(bingOptions.ApiKey);
        var webSearchEngineSkill = new WebSearchEngineSkill(bingConnector);

        kernel.ImportSkill(webSearchEngineSkill, "WebSearch");
        kernel.ImportSkill(new LanguageCalculatorSkill(kernel), "semanticCalculator");
        kernel.ImportSkill(new TimeSkill(), "time");

        // StepwisePlanner is instructed to depend on available functions.
        // We expose this function to increase the flexibility in it's ability to answer
        // given the relatively small number of functions we have in this example.
        // This seems to be particularly helpful in these examples with gpt-35-turbo -- even though it
        // does not *use* this function. It seems to help the planner find a better path to the answer.
        kernel.CreateSemanticFunction(
            "Generate an answer for the following question: {{$input}}",
            functionName: "GetAnswerForQuestion",
            skillName: "AnswerBot",
            description: "Given a question, get an answer and return it as the result of the function");

        logger.Information("*****************************************************");
        Stopwatch sw = new();
        logger.Information("Question: " + question);

        var plannerConfig = new Microsoft.SemanticKernel.Planning.Stepwise.StepwisePlannerConfig();
        plannerConfig.ExcludedFunctions.Add("TranslateMathProblem");
        plannerConfig.ExcludedFunctions.Add("DaysAgo");
        plannerConfig.ExcludedFunctions.Add("DateMatchingLastDayName");
        plannerConfig.MinIterationTimeMs = 1500;
        plannerConfig.MaxIterations = 25;

        if (!string.IsNullOrEmpty(Suffix))
        {
            plannerConfig.Suffix = $"{Suffix}\n{plannerConfig.Suffix}";
            currentExecutionResult.question = $"[Assisted] - {question}";
        }

        if (MaxTokens.HasValue)
        {
            plannerConfig.MaxTokens = MaxTokens.Value;
        }

        SKContext result;
        sw.Start();

        try
        {
            StepwisePlanner planner = new(kernel: kernel, config: plannerConfig);
            var plan = planner.CreatePlan(question);

            result = await plan.InvokeAsync(kernel.CreateNewContext());

            if (result.Result.Contains("Result not found, review _stepsTaken to see what", StringComparison.OrdinalIgnoreCase))
            {
                logger.Information("Could not answer question in " + plannerConfig.MaxIterations + " iterations");
                currentExecutionResult.answer = "Could not answer question in " + plannerConfig.MaxIterations + " iterations";
            }
            else
            {
                logger.Information("Result: " + result.Result);
                currentExecutionResult.answer = result.Result;
            }

            if (result.Variables.TryGetValue("stepCount", out string? stepCount))
            {
                logger.Information("Steps Taken: " + stepCount);
                currentExecutionResult.stepsTaken = stepCount;
            }

            if (result.Variables.TryGetValue("skillCount", out string? skillCount))
            {
                logger.Information("Skills Used: " + skillCount);
            }

            if (result.Variables.TryGetValue("iterations", out string? iterations))
            {
                logger.Information("Iterations: " + iterations);
                currentExecutionResult.iterations = iterations;
            }
        }
#pragma warning disable CA1031
        catch (Exception ex)
        {
            logger.Information("Exception: " + ex);
        }

        logger.Information("Time Taken: " + sw.Elapsed);
        currentExecutionResult.timeTaken = sw.Elapsed.ToString();
        executionResults.Add(currentExecutionResult);
        logger.Information("*****************************************************");
    }

    private IKernel GetKernel(ref ExecutionResult result, bool useChat = false, string? model = null)
    {
        var builder = new KernelBuilder();
        var maxTokens = 0;
        if (useChat)
        {
            builder.WithAzureChatCompletionService(
                model ?? chatModelOverride ?? azureOpenAIOptions.ChatDeploymentName,
            azureOpenAIOptions.Endpoint,
            azureOpenAIOptions.ApiKey,
                alsoAsTextCompletion: true,
                setAsDefault: true);

            maxTokens = ChatMaxTokens ?? (new Microsoft.SemanticKernel.Planning.Stepwise.StepwisePlannerConfig()).MaxTokens;
            result.model = model ?? chatModelOverride ?? azureOpenAIOptions.ChatDeploymentName;
        }
        else
        {
            builder.WithAzureTextCompletionService(
                model ?? textModelOverride ?? azureOpenAIOptions.DeploymentName,
                azureOpenAIOptions.Endpoint,
                azureOpenAIOptions.ApiKey);

            maxTokens = TextMaxTokens ?? (new Microsoft.SemanticKernel.Planning.Stepwise.StepwisePlannerConfig()).MaxTokens;
            result.model = model ?? textModelOverride ?? azureOpenAIOptions.DeploymentName;
        }

        logger.Information($"Model: {result.model} ({maxTokens})");

        var kernel = builder
            .WithLoggerFactory(loggerFactory)
            .WithRetryBasic(new()
            {
                MaxRetryCount = 3,
                UseExponentialBackoff = true,
                MinRetryDelay = TimeSpan.FromSeconds(3),
            })
            .Build();

        return kernel;
    }

    //private async Task RunWithQuestion(IKernel kernel, string question)
    //{
    //    var bingConnector = new BingConnector(bingOptions.ApiKey);
    //    var webSearchEngineSkill = new WebSearchEngineSkill(bingConnector);

    //    kernel.ImportSkill(webSearchEngineSkill, "WebSearch");
    //    kernel.ImportSkill(new LanguageCalculatorSkill(kernel), "advancedCalculator");
    //    // kernel.ImportSkill(new SimpleCalculatorSkill(kernel), "basicCalculator");
    //    kernel.ImportSkill(new TimeSkill(), "time");

    //    logger.Information("*****************************************************");
    //    var sw = new Stopwatch();
    //    logger.Information("Question: " + question);

    //    var plannerConfig = new Microsoft.SemanticKernel.Planning.Stepwise.StepwisePlannerConfig();
    //    plannerConfig.ExcludedFunctions.Add("TranslateMathProblem");
    //    plannerConfig.MinIterationTimeMs = 1500;
    //    plannerConfig.MaxTokens = 2000;

    //    var planner = new StepwisePlanner(kernel, plannerConfig);
    //    sw.Start();
    //    var plan = planner.CreatePlan(question);

    //    var result = await plan.InvokeAsync(kernel.CreateNewContext());
    //    logger.Information("Result: " + result);
    //    if (result.Variables.TryGetValue("stepCount", out string? stepCount))
    //    {
    //        logger.Information("Steps Taken: " + stepCount);
    //    }

    //    if (result.Variables.TryGetValue("skillCount", out string? skillCount))
    //    {
    //        logger.Information("Skills Used: " + skillCount);
    //    }

    //    logger.Information("Time Taken: " + sw.Elapsed);
    //    logger.Information("*****************************************************");
    //}

    //private IKernel GetKernel()
    //{
    //    var builder = new KernelBuilder();
    //    builder
    //        .WithOpenAIChatCompletionService(Model, openAIOptions.ApiKey);

    //    var kernel = builder
    //        .WithLoggerFactory(loggerFactory)
    //        .Configure(c => c.SetDefaultHttpRetryConfig(new HttpRetryConfig
    //        {
    //            MaxRetryCount = 3,
    //            UseExponentialBackoff = true,
    //            MinRetryDelay = TimeSpan.FromSeconds(3),
    //        }))
    //        .Build();

    //    return kernel;
    //}


}
