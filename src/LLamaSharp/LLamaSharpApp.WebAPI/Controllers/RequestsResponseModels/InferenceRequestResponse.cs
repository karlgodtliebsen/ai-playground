namespace LLamaSharpApp.WebAPI.Controllers.RequestsResponseModels;

/// <summary>
/// InferenceRequestResponse
/// </summary>
public sealed class InferenceRequestResponse
{

    /// <summary>number of tokens to keep from initial prompt</summary>
    public int? TokensKeep { get; set; } = default!;

    /// <summary>
    /// how many new tokens to predict (n_predict), set to -1 to inifinitely generate response
    /// until it complete.
    /// </summary>
    public int? MaxTokens { get; set; } = default!;

    /// <summary>logit bias for specific tokens</summary>
    public Dictionary<int, float>? LogitBias { get; set; } = default!;

    /// <summary>
    /// Sequences where the model will stop generating further tokens.
    /// </summary>
    public IEnumerable<string>? AntiPrompts { get; set; } = default!;

    /// <summary>path to file for saving/loading model eval state</summary>
    public string? PathSession { get; set; } = default!;

    /// <summary>string to suffix user inputs with</summary>
    public string? InputSuffix { get; set; } = default!;

    /// <summary>string to prefix user inputs with</summary>
    public string? InputPrefix { get; set; } = default!;

    /// <summary>0 or lower to use vocab size</summary>
    public int? TopK { get; set; } = default!;

    /// <summary>1.0 = disabled</summary>
    public float? TopP { get; set; } = default!;

    /// <summary>1.0 = disabled</summary>
    public float? TfsZ { get; set; } = default!;

    /// <summary>1.0 = disabled</summary>
    public float? TypicalP { get; set; } = default!;

    /// <summary>1.0 = disabled</summary>
    public float? Temperature { get; set; } = default!;

    /// <summary>1.0 = disabled</summary>
    public float? RepeatPenalty { get; set; } = default!;

    /// <summary>
    /// last n tokens to penalize (0 = disable penalty, -1 = context size) (repeat_last_n)
    /// </summary>
    public int? RepeatLastTokensCount { get; set; } = default!;

    /// <summary>
    /// frequency penalty coefficient
    /// 0.0 = disabled
    /// </summary>
    public float? FrequencyPenalty { get; set; } = default!;

    /// <summary>
    /// presence penalty coefficient
    /// 0.0 = disabled
    /// </summary>
    public float? PresencePenalty { get; set; } = default!;

    /// <summary>
    /// Mirostat uses tokens instead of words.
    /// algorithm described in the paper https://arxiv.org/abs/2007.14966.
    /// 0 = disabled, 1 = mirostat, 2 = mirostat 2.0
    /// </summary>
<<<<<<< HEAD
    public MirostatType? Mirostat { get; set; } = default!;
=======
    public MirotateType Mirostat { get; set; } = MiroStateType.Disable;
>>>>>>> main

    /// <summary>target entropy</summary>
    public float? MirostatTau { get; set; } = default!;

    /// <summary>learning rate</summary>
    public float? MirostatEta { get; set; } = default!;

    /// <summary>consider newlines as a repeatable token (penalize_nl)</summary>
    public bool? PenalizeNL { get; set; } = default!;
}
