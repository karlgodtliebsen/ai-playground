using LLamaSharpApp.WebAPI.Models;

namespace LLamaSharpApp.WebAPI.Services;

public interface IChatService
{
    string Chat(ChatMessage input);
}
