using FluentAssertions;

using LlamaSharp.Tests.Fixtures;

using LLamaSharp.Domain.Domain.Models;
using LLamaSharp.Domain.Domain.Services;

using LLamaSharpApp.WebAPI.Controllers.Mappers;
using LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;

using Microsoft.Extensions.DependencyInjection;

namespace LlamaSharp.Tests;

public sealed class TestOfLlamaSharpCompositeClient : IClassFixture<IntegrationTestWebApplicationFactory>, IDisposable
{
    private readonly IntegrationTestWebApplicationFactory factory;

    public TestOfLlamaSharpCompositeClient(IntegrationTestWebApplicationFactory factory)
    {
        this.factory = factory;
    }

    public void Dispose()
    {
        //factory.Dispose();//Code smell: really annoying that this messes up the test runner
    }


    [Fact]
    public async Task VerifyThatLLamaCompositeClientCanCallInteractiveExecutorWithChat()
    {
        var optionsService = factory.Services.GetRequiredService<IOptionsService>();
        var optionsMapper = factory.Services.GetRequiredService<OptionsMapper>();
        var request = new ExecutorInferRequest
        {
            Prompt = "Below is an instruction that describes a task. Write a response that appropriately completes the request.",
            Text = "Tell me what day it is",
            ModelOptions = optionsMapper.Map(optionsService.GetDefaultLlamaModelOptions()),
            InferenceOptions = optionsMapper.Map(optionsService.GetDefaultInferenceOptions()),
            InferenceType = InferenceType.InteractiveExecutor,
        };
        request.InferenceOptions.Temperature = 0.6f;
        request.ModelOptions.ModelName = "llama-2-7b.ggmlv3.q8_0";
        request.ModelOptions.ContextSize = 1024;
        request.ModelOptions.Seed = 1337;
        //request.ModelOptions.GpuLayerCount = 5;

        var client = factory.Services.GetRequiredService<ILLamaCompositeOperationsClient>();
        var response = await client.InteractiveExecutorWithChat(request, CancellationToken.None);
        response.Should().NotBeNull();
    }

}
