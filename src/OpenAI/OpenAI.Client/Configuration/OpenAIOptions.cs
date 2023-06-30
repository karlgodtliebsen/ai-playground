﻿namespace OpenAI.Client.Configuration;

public class OpenAIOptions
{
    /// <summary>
    /// Configuration ConfigSectionName
    /// </summary>
    public const string ConfigSectionName = "OpenAIOptions";

    public string OrganisationKey { get; set; } = null!;
    public string ApiKey { get; set; } = null!;
    public string OpenAIUri { get; set; } = "https://api.openai.com/v1/"!;


    public Uri GetBaseAddress()
    {
        return new Uri(OpenAIUri);
    }

}