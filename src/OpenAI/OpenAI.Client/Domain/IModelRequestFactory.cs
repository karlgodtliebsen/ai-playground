namespace OpenAI.Client.Domain;

public interface IModelRequestFactory
{
    /// <summary>
    /// Creates a request for the specified model
    /// </summary>
    /// <typeparam name="T">The Request to Create an Instance of</typeparam>
    /// <param name="create"></param>
    /// <returns></returns>
    /// <remarks>Details at https://platform.openai.com/docs/models/how-we-use-your-data</remarks>
    /// <exception cref="ArgumentException"></exception>
    T CreateRequest<T>(Func<T> create) where T : class, IModelRequest, new();


    IList<string> GetModels(string requestUri);
}
