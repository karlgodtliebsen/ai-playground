namespace OpenAI.Client.Domain;

public interface IModelRequestFactory
{
    /// <summary>
    /// Creates a request for the specified model
    /// </summary>
    /// <typeparam name="T">The Request to Create an Instance of</typeparam>
    /// <param name="modelUri">the sub part of the uri</param>
    /// <param name="modelId">Id of Model, like one of gpt-4, gpt-4-0613, gpt-4-32k, gpt-4-32k-0613,</param>
    /// <returns></returns>
    /// <remarks>Details at https://platform.openai.com/docs/models/how-we-use-your-data</remarks>
    /// <exception cref="ArgumentException"></exception>
    T CreateRequest<T>(Func<T> create) where T : class, IModelRequest, new();
}