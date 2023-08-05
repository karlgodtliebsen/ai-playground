using LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;
using LLamaSharpApp.WebAPI.Domain.Models;
using LLamaSharpApp.WebAPI.Domain.Services;

using Riok.Mapperly.Abstractions;

namespace LLamaSharpApp.WebAPI.Controllers.Mappers;

/// <summary>
///  Riok.Mapperly Mapper for Request to Messages
/// </summary>
[Mapper]
public partial class RequestMessagesMapper
{
    private readonly OptionsMapper optionsMapper;
    private readonly IOptionsService configurationDomainService;

    /// <summary>
    /// Constructor for the RequestMessagesMapper
    /// </summary>
    /// <param name="optionsMapper"></param>
    /// <param name="configurationDomainService"></param>
    public RequestMessagesMapper(OptionsMapper optionsMapper, IOptionsService configurationDomainService)
    {
        this.optionsMapper = optionsMapper;
        this.configurationDomainService = configurationDomainService;
    }

    /// <summary>
    /// Maps the ChatMessageRequest to the ChatMessage
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public partial ChatMessage Map(ChatMessageRequest request);

    /// <summary>
    /// Maps the EmbeddingsRequest to the EmbeddingsMessage
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public partial EmbeddingsMessage Map(EmbeddingsRequest request);

    /// <summary>
    /// Maps the ExecutorInferRequest to the ExecutorInferMessage
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public partial ExecutorInferMessage Map(ExecutorInferRequest request);

    /// <summary>
    /// Maps the ExecutorTrainRequest to the ExecutorTrainMessage 
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public partial TokenizeMessage Map(TokenizeMessageRequest request);
    /// <summary>
    /// Maps the DeTokenizeMessageRequest to the DeTokenizeMessage
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public partial DeTokenizeMessage Map(DeTokenizeMessageRequest request);


    /// <summary>
    /// Maps the DeTokenizeMessageRequest to the DeTokenizeMessage
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public DeTokenizeMessage Map(DeTokenizeMessageRequest request, string userId)
    {
        var model = Map(request);
        model.UserId = userId;
        return model;
    }

    /// <summary>
    /// Maps the TokenizeMessageRequest to the TokenizeMessage
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public TokenizeMessage Map(TokenizeMessageRequest request, string userId)
    {
        var model = Map(request);
        model.UserId = userId;
        return model;
    }

    /// <summary>
    /// Maps the ExecutorInferRequest to the ExecutorInferMessage
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public ExecutorInferMessage Map(ExecutorInferRequest request, string userId)
    {
        var defaultOptions = configurationDomainService.GetDefaultLlamaModelOptions();
        var model = Map(request);
        model.UserId = userId;
        model.ModelOptions = optionsMapper.MapRestoreSensitiveData(request.ModelOptions!, defaultOptions);
        if (request.UsePersistedModelState.HasValue) model.UsePersistedModelState = request.UsePersistedModelState.Value;
        if (request.UsePersistedExecutorState.HasValue) model.UsePersistedExecutorState = request.UsePersistedExecutorState.Value;
        if (request.UseStatelessExecutor.HasValue) model.UseStatelessExecutor = request.UseStatelessExecutor.Value;
        return model;
    }


    /// <summary>
    /// Maps the ChatMessageRequest to the ChatMessage
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public ChatMessage Map(ChatMessageRequest request, string userId)
    {
        var defaultOptions = configurationDomainService.GetDefaultLlamaModelOptions();
        var model = Map(request);
        model.UserId = userId;
        model.ModelOptions = optionsMapper.MapRestoreSensitiveData(request.ModelOptions!, defaultOptions);
        return model;
    }

    /// <summary>
    /// Maps the EmbeddingsRequest to the EmbeddingsMessage
    /// </summary>
    /// <param name="request"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public EmbeddingsMessage Map(EmbeddingsRequest request, string userId)
    {
        var defaultOptions = configurationDomainService.GetDefaultLlamaModelOptions();
        var model = Map(request);
        model.UserId = userId;
        model.ModelOptions = optionsMapper.MapRestoreSensitiveData(request.ModelOptions!, defaultOptions);
        return model;
    }

}
