
    //The following models are the temporary snapshots,
    //we will announce their deprecation dates once updated versions are available.
    //If you want to use the latest model version,
    //use the standard model names like gpt-4 or gpt-3.5-turbo.

    //string prompt = "Tell us something about .NET development.";
    //string prompt = "Please show a sample for a 'for loop' in C#";
    /*

        ENDPOINT	                MODEL NAME
        /v1/chat/completions	    gpt-4, gpt-4-0613, gpt-4-32k, gpt-4-32k-0613, gpt-3.5-turbo, gpt-3.5-turbo-0613, gpt-3.5-turbo-16k, gpt-3.5-turbo-16k-0613
        /v1/completions	            text-davinci-003, text-davinci-002, text-curie-001, text-babbage-001, text-ada-001
        /v1/edits	                text-davinci-edit-001, code-davinci-edit-001
        /v1/audio/transcriptions	whisper-1
        /v1/audio/translations	    whisper-1
        /v1/fine-tunes	            davinci, curie, babbage, ada
        /v1/embeddings	            text-embedding-ada-002, text-search-ada-doc-001
        /v1/moderations	            text-moderation-stable, text-moderation-latest
    */





    /*

ENDPOINT	                MODEL NAME
/v1/chat/completions	    gpt-4, gpt-4-0613, gpt-4-32k, gpt-4-32k-0613, gpt-3.5-turbo, gpt-3.5-turbo-0613, gpt-3.5-turbo-16k, gpt-3.5-turbo-16k-0613
/v1/completions	            text-davinci-003, text-davinci-002, text-curie-001, text-babbage-001, text-ada-001
/v1/edits	                text-davinci-edit-001, code-davinci-edit-001
/v1/audio/transcriptions	whisper-1
/v1/audio/translations	    whisper-1
/v1/fine-tunes	            davinci, curie, babbage, ada
/v1/embeddings	            text-embedding-ada-002, text-search-ada-doc-001
/v1/moderations	            text-moderation-stable, text-moderation-latest


var completion = await CreateCompletion(model: "text-davinci:001", prompt: "Say this is a test", maxTokens: 7, temperature: 0.0f, topP: 1.0f, n: 1, stream: false);
var chatCompletion = await CreateChatCompletion(model: "gpt-3.5-turbo", messages: new[] { new ChatCompletionMessage { Name = "test", Role = "user", Content = "Hello!" } });

var edit = await CreateEdit(model: "text-davinci-edit-001", instruction: "Fix the spelling mistakes", input: "What day of the wek is it?");
var image = await CreateImage(prompt: "A cute baby sea otter", n: 1, size: "1024x1024");
var embedding = await CreateEmbedding(model: "text-embedding-ada-002", input: new[]{"The food was delicious and the waiter..."}, user: "test");
var fineTuneFile = await CreateFile("training_data.jsonl", "fine-tune");
var fineTuneJob = await CreateFineTune(fineTuneFile.Id);
var retrievedFile = await RetrieveFile(fineTuneFile.Id);
var files = await ListFiles();
var fileContents = await DownloadFile(fineTuneFile.Id); //for free accounts it generates error "To help mitigate abuse, downloading of fine-tune training files is disabled for free accounts."
var fineTuneEvents = await ListFineTuneEvents(fineTuneJob.Id, false);
var moderation = await CreateModeration("I want to kill them.");

*/




//https://github.com/OkGoDoIt/OpenAI-API-dotnet/blob/master/OpenAI_API/Model/Model.cs#L119

//Request model
//https://github.com/OkGoDoIt/OpenAI-API-dotnet/blob/master/OpenAI_API/Completions/CompletionRequest.cs

//var chatCompletion = await CreateChatCompletion(model: "gpt-3.5-turbo", messages: new[] { new ChatCompletionMessage { Name = "test", Role = "user", Content = "Hello!" } });


//var chatMessage = new ChatMessage { Name = "test", Role = ChatMessageRole.FromString("user"), Content = prompt };
// Messages = new List<ChatMessage>() { chatMessage },

//https://platform.openai.com/docs/api-reference/completions/create

curl https://api.openai.com/v1/completions \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer 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" \
  -d '{
    "model": "text-davinci-003",
    "prompt": "Say this is a test",
    "max_tokens": 7,
    "temperature": 0
  }'


  curl https://api.openai.com/v1/completions   -H "Content-Type: application/json"  -H "Authorization: Bearer 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"  -d '{    "model": "text-davinci-003",    "prompt": "Say this is a test",    "max_tokens": 7,    "temperature": 0  }'

{"model": "text-davinci-003", "prompt": "Say this is a test", "max_tokens": 7,  "temperature": 0  }

"{\"model\":\"text-davinci-003\",\"prompt\":\"Say this is a test\",\"max_tokens\":7,\"temperature\":0,\"top_p\":1,\"n\":1,\"stream\":false,\"stop\":\"\\n\"}"



"{\n  \"id\": \"cmpl-7SVb3FGsdEWHmz19IfeBF0gQyjk7R\",\n  \"object\": \"text_completion\",\n  \"created\": 1687028997,\n  
\"model\": \"text-davinci-003\",\n  \"choices\": [\n    {\n      \"text\": \"\",\n      \"index\": 0,\n      \"logprobs\": null,\n      \"finish_reason\": \"stop\"\n    }\n  ],\n  \"usage\": {\n    \"prompt_tokens\": 5,\n    \"total_tokens\": 5\n  }\n}\n"

        //var completions = await GetCompletionsUsingManuelClientAsync(deploymentName, prompt, cancellationToken);


    private async Task<Completions?> GetCompletionsUsingManuelClientAsync(string deploymentName, string prompt, CancellationToken cancellationToken)
    {
        var requestUri = $"{options.OpenAIUri}{"completions"}";
        //https://api.openai.com/v1/completions
        //deploymentName text-davinci-003

        var payload = new CompletionRequest
        {
            Model = deploymentName,
            Prompt = prompt,
            MaxTokens = 7,
            Temperature = 0.0f,
            //TopP = 1.0f,
            //NumChoicesPerPrompt = 1,
            //Stream = false,
            //Logprobs = null,
            //Stop = "\n"
        };

        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.ApiKey);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(json));

        // Further authentication-header used for Azure openAI service
        httpClient.DefaultRequestHeaders.Add("api-key", options.ApiKey);
        httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
        httpClient.DefaultRequestHeaders.Add("OpenAI-Organization", options.OrganisationKey);

        var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        string jsonContent = JsonSerializer.Serialize(payload, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
        //string jsonContent = """{"model": "text-davinci-003","prompt": "Say this is a test","max_tokens": 7,"temperature": 0  }""";
        request.Content = new StringContent(jsonContent, UnicodeEncoding.UTF8, json);

        var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<Completions>(cancellationToken: cancellationToken);
            return result!;
        }
        return default;
    }


    
        var serializerOptions = new JsonSerializerOptions()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        string jsonContent = JsonSerializer.Serialize(payload, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });




 ChatCompletions
    {
    "model": "gpt-3.5-turbo",
    "messages": [{"role": "system", "content": "You are a helpful assistant."}, {"role": "user", "content": "Hello!"}]
  }'