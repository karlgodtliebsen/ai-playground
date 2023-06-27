namespace OpenAI.Client.Domain;

public interface IModelRequest : IRequest
{
    string Model { get; set; }
}