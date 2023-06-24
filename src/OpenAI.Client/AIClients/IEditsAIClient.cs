﻿using OpenAI.Client.Models.ChatCompletion;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IEditsAIClient
{
    Task<Response<Completions>?> GetEditsAsync(EditsRequest request, CancellationToken cancellationToken);

}