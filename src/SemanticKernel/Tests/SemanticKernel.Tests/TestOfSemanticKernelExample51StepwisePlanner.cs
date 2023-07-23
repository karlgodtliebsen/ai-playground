

using System.Diagnostics;

using AI.Test.Support;

using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Planning;
using Microsoft.SemanticKernel.Reliability;
using Microsoft.SemanticKernel.Skills.Web;
using Microsoft.SemanticKernel.Skills.Web.Bing;

using SemanticKernel.Tests.Fixtures;
using SemanticKernel.Tests.Skills;

using Xunit.Abstractions;

using TimeSkill = SemanticKernel.Tests.Skills.TimeSkill;

namespace SemanticKernel.Tests;

[Collection("SemanticKernel Base Collection")]
public class TestOfSemanticKernelExample51StepwisePlanner
{
    private readonly ILogger logger;
    private readonly Microsoft.Extensions.Logging.ILogger msLogger;
    private readonly SemanticKernelTestFixtureBase fixture;
    private readonly HostApplicationFactory hostApplicationFactory;

    public TestOfSemanticKernelExample51StepwisePlanner(SemanticKernelTestFixtureBase fixture, ITestOutputHelper output)
    {
        fixture.Setup(output);
        this.logger = fixture.Logger;
        this.fixture = fixture;
        this.msLogger = fixture.MsLogger;
        this.hostApplicationFactory = fixture.Factory;
    }
    const string Model = "gpt-3.5-turbo";


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
        var bingConnector = new BingConnector(fixture.BingOptions.ApiKey);
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
            .WithOpenAIChatCompletionService(Model, fixture.OpenAIOptions.ApiKey);

        var kernel = builder
            .WithLogger(fixture.MsLogger)
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
