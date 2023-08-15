## OpenAI.Client:

A dotnet Client 'OpenAI.Client' with a Test project and a WPF application that shows how to use it.

The official OpenAI documentation provides detailed information about the API reference and other relevant resources.

Read the documentation here: 
> https://platform.openai.com/docs/introduction
> https://platform.openai.com/docs/guides/gpt-best-practices
> https://platform.openai.com/docs/api-reference/


### TODO: Use Awesome ChatGPT Prompts
> https://github.com/f/awesome-chatgpt-prompts
> https://huggingface.co/datasets/Open-Orca/OpenOrca

The Art of ChatGPT Prompting: A Guide to Crafting Clear and Effective Prompts:
> https://fka.gumroad.com/l/art-of-chatgpt-prompting


### Configuration


#### OpenAI:
Add appsettings.json

```json
{
    "OpenAI": {
    "ApiKey": "<openai api key>",
    "OrganisationKey": "<organisation key>",
    "OpenAIUri": "https://api.openai.com/v1/"
  }
}
```

I recommend using user secrets to avoid having the API key in the Repository
However, you can also add *.development.json or *.IntegrationTests.json to the .gitignore if you want to use config files for secret storage.

