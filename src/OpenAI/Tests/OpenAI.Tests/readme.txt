
    //The following models are the temporary snapshots,
    //we will announce their deprecation dates once updated versions are available.
    //If you want to use the latest model version,
    //use the standard model names like gpt-4 or gpt-3.5-turbo.

    //string prompt = "Tell us something about .NET development.";
    //string prompt = "Please show a sample for a 'for loop' in C#";


        ENDPOINT	                MODEL NAME
        /v1/chat/completions	    gpt-4, gpt-4-0613, gpt-4-32k, gpt-4-32k-0613, gpt-3.5-turbo, gpt-3.5-turbo-0613, gpt-3.5-turbo-16k, gpt-3.5-turbo-16k-0613
        /v1/completions	            text-davinci-003, text-davinci-002, text-curie-001, text-babbage-001, text-ada-001
        /v1/edits	                text-davinci-edit-001, code-davinci-edit-001
        /v1/audio/transcriptions	whisper-1
        /v1/audio/translations	    whisper-1
        /v1/fine-tunes	            davinci, curie, babbage, ada
        /v1/embeddings	            text-embedding-ada-002, text-search-ada-doc-001
        /v1/moderations	            text-moderation-stable, text-moderation-latest




   Models
        whisper-1
        babbage
        davinci
        text-davinci-edit-001
        babbage-code-search-code
        text-similarity-babbage-001
        code-davinci-edit-001
        text-davinci-001
        ada
        babbage-code-search-text
        babbage-similarity
        code-search-babbage-text-001
        text-curie-001
        gpt-3.5-turbo-16k-0613
        code-search-babbage-code-001
        text-ada-001
        text-similarity-ada-001
        curie-instruct-beta
        gpt-3.5-turbo-0301
        ada-code-search-code
        ada-similarity
        code-search-ada-text-001
        text-search-ada-query-001
        davinci-search-document
        ada-code-search-text
        text-search-ada-doc-001
        davinci-instruct-beta
        text-similarity-curie-001
        code-search-ada-code-001
        ada-search-query
        text-search-davinci-query-001
        curie-search-query
        davinci-search-query
        babbage-search-document
        ada-search-document
        text-search-curie-query-001
        gpt-3.5-turbo
        text-search-babbage-doc-001
        gpt-3.5-turbo-0613
        curie-search-document
        text-search-curie-doc-001
        babbage-search-query
        text-babbage-001
        text-search-davinci-doc-001
        text-embedding-ada-002
        text-search-babbage-query-001
        curie-similarity
        curie
        text-similarity-davinci-001
        text-davinci-002
        text-davinci-003
        davinci-similarity
        gpt-3.5-turbo-16k



//https://github.com/OkGoDoIt/OpenAI-API-dotnet/blob/master/OpenAI_API/Model/Model.cs#L119

//Request model
//https://github.com/OkGoDoIt/OpenAI-API-dotnet/blob/master/OpenAI_API/Completions/CompletionRequest.cs

//var chatCompletion = await CreateChatCompletion(model: "gpt-3.5-turbo", messages: new[] { new ChatCompletionMessage
{ Name = "test", Role = "user", Content = "Hello!" } });


//var chatMessage = new ChatMessage { Name = "test", Role = ChatMessageRole.FromString("user"), Content = prompt };
// Messages = new List<ChatMessage>() { chatMessage },

//https://platform.openai.com/docs/api-reference/completions/create

curl https://api.openai.com/v1/completions \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer <API-Key>" \
  -d '{
    "model": "text-davinci-003",
    "prompt": "Say this is a test",
    "max_tokens": 7,
    "temperature": 0
  }'

curl https://api.openai.com/v1/completions   -H "Content-Type: application/json"  -H "Authorization: Bearer <ApiToken>"  
-d '{    "model": "text-davinci-003",    "prompt": "Say this is a test",    "max_tokens": 7,    "temperature": 0  }'

