﻿using OpenAI.Client.Models;
using OpenAI.Client.Models.Requests;
using OpenAI.Client.Models.Responses;

namespace OpenAI.Client.AIClients;

public interface IModerationAIClient
{
    Task<Response<ModerationResponse>?> GetModerationAsync(ModerationRequest request,
        CancellationToken cancellationToken);
}