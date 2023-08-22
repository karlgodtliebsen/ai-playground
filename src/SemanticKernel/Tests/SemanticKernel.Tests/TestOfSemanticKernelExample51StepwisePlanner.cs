using System.Diagnostics;

using AI.Test.Support.Fixtures;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Reliability;
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
public class TestOfSemanticKernelExample51StepwisePlanner
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;
    private readonly HostApplicationFactory hostApplicationFactory;
    private readonly IServiceProvider services;
    private readonly OpenAIOptions openAIOptions;
    const string Model = "gpt-3.5-turbo";
    private readonly BingOptions bingOptions;
    private readonly string skillsPath;

    public TestOfSemanticKernelExample51StepwisePlanner(SemanticKernelTestFixture fixture, ITestOutputHelper output)
    {
        this.hostApplicationFactory = fixture.WithOutputLogSupport(output).WithDockerSupport().Build();
        this.services = hostApplicationFactory.Services;
        this.logger = services.GetRequiredService<ILogger>();
        this.msLogger = services.GetRequiredService<ILogger<TestOfSemanticKernel>>();
        this.openAIOptions = services.GetRequiredService<IOptions<OpenAIOptions>>().Value;
        this.bingOptions = services.GetRequiredService<IOptions<BingOptions>>().Value;
        this.skillsPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Skills"));
    }


    [Fact]
    public async Task UseStepwisePlanner_Example51()
    {
        logger.Information("======== StepwisePlanner ========");
        string[] questions = new string[]
        {
            "Who is the current president of the United States? What is his current age divided by 2",
            // "Who is Leo DiCaprio's girlfriend? What is her current age raised to the (his current age)/100 power?",
            // "What is the capital of France? Who is that cities current mayor? What percentage of their life has been in the 21st century as of today?",
            // "What is the current day of the calendar year? Using that as an angle in degrees, what is the area of a unit circle with that angle?"
        };

        var kernel = GetKernel();
        foreach (var question in questions)
        {
            logger.Information("RunChatCompletion");
            await RunWithQuestion(kernel, question);
        }
    }

    private async Task RunWithQuestion(IKernel kernel, string question)
    {
        var bingConnector = new BingConnector(bingOptions.ApiKey);
        var webSearchEngineSkill = new WebSearchEngineSkill(bingConnector);

        kernel.ImportSkill(webSearchEngineSkill, "WebSearch");
        kernel.ImportSkill(new LanguageCalculatorSkill(kernel), "advancedCalculator");
        // kernel.ImportSkill(new SimpleCalculatorSkill(kernel), "basicCalculator");
        kernel.ImportSkill(new TimeSkill(), "time");

        logger.Information("*****************************************************");
        var sw = new Stopwatch();
        logger.Information("Question: " + question);

        var plannerConfig = new Microsoft.SemanticKernel.Planning.Stepwise.StepwisePlannerConfig();
        plannerConfig.ExcludedFunctions.Add("TranslateMathProblem");
        plannerConfig.MinIterationTimeMs = 1500;
        plannerConfig.MaxTokens = 2000;

        var planner = new StepwisePlanner(kernel, plannerConfig);
        sw.Start();
        var plan = planner.CreatePlan(question);

        var result = await plan.InvokeAsync(kernel.CreateNewContext());
        logger.Information("Result: " + result);
        if (result.Variables.TryGetValue("stepCount", out string? stepCount))
        {
            logger.Information("Steps Taken: " + stepCount);
        }

        if (result.Variables.TryGetValue("skillCount", out string? skillCount))
        {
            logger.Information("Skills Used: " + skillCount);
        }

        logger.Information("Time Taken: " + sw.Elapsed);
        logger.Information("*****************************************************");
    }

    private IKernel GetKernel()
    {
        var builder = new KernelBuilder();
        builder
            .WithOpenAIChatCompletionService(Model, openAIOptions.ApiKey);

        var kernel = builder
            .WithLogger(msLogger)
            .Configure(c => c.SetDefaultHttpRetryConfig(new HttpRetryConfig
            {
                MaxRetryCount = 3,
                UseExponentialBackoff = true,
                MinRetryDelay = TimeSpan.FromSeconds(3),
            }))
            .Build();

        return kernel;
    }


}
