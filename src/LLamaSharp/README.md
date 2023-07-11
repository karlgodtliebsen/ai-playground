# LlmaSharp WebAPI
An ASP.NET Core WebAPI (v7.0) using LlmaSCharp.

This is early work.

## How to download the source and build:

Download a model like: wizardLM-7B.ggmlv3.q4_1.bin

https://huggingface.co/TheBloke

https://huggingface.co/TheBloke/wizardLM-7B-GGML/resolve/main/wizardLM-7B.ggmlv3.q4_1.bin

These models has been testet:
```
wizardLM-7B.ggmlv3.q4_1.bin
ggml-vic13b-uncensored-q4_1.bin
ggml-vic13b-uncensored-q5_0.bin
ggml-vicuna-13B-1.1-q4_0.bin
ggml-vicuna-13B-1.1-q8_0.bin
wizardlm-13b-v1.1-superhot-8k.ggmlv3.q4_1.bin
```


To build and use the projects, two projects with 'LlmaModels' folders need to be updated with the model of your choice:
```   
    LLamaSharpApp.WebAPI
    Embeddings.Qdrant.Tests
```
Add the Model to the 'LlmaModels' folders, and mark it for "Copy If Newer".

If you prefer to locate the model file(s) elsewhere, then modify the appsettings.json to point to the model.

The Asp.net Web APIs are configured to use Authentication:  
Either fill out the appsettings.json section (Remember not to commit/publish the information) or use Keyvault or UserSecret during development.

More information at: https://damienbod.com/

Authentication can be disabled in the Program.cs files, and remember to remove the [Authentication] attributes from the controllers, and Hardcode some User Identity. 




# References
https://github.com/SciSharp/LLamaSharp
https://scisharp.github.io/SciSharp/

https://blog.maartenballiauw.be/post/2023/06/15/running-large-language-models-locally-your-own-chatgpt-like-ai-in-csharp.html
https://scisharp.github.io/LLamaSharp/0.4/ContributingGuide/#add-examples


### Authentication Identity
https://damienbod.com/
https://damienbod.com/2020/05/29/login-and-use-asp-net-core-api-with-azure-ad-auth-and-user-access-tokens/

