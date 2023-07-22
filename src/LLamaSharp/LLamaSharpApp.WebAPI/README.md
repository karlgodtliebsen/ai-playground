# LlamaSharp WebAPI
An ASP.NET Core WebAPI (v7.0) using LlamaSCharp.


#### How to download the source and build:

```
Clone the repository and open the solution in Visual Studio 2022.

Download a model like: 

- wizardLM-7B.ggmlv3.q4_1.bin (see references)

Add the Model to the 'LlamaModels' folder in the 'AI.LlaMa.Models' project, and mark it for "Copy If Newer".

If you prefer to locate the model file elsewhere, then modify the appsettings.json to point to the model and update .gitignore.

The web controllers are configured to use Authentication. 
Either fill out the appsettings.json section (Remember not to commit/publish the information).
Use either Environment variables, Azure Keyvault or UserSecret during development (or...).

Authentication can be disabled in the Program.cs file, remember to remove the [Authentication] attributes from the controllers.

```

### References
1. https://huggingface.co/TheBloke
2. https://huggingface.co/TheBloke/wizardLM-7B-GGML/resolve/main/wizardLM-7B.ggmlv3.q4_1.bin
3. https://github.com/SciSharp/LLamaSharp
4. https://scisharp.github.io/SciSharp/
5. https://blog.maartenballiauw.be/post/2023/06/15/running-large-language-models-locally-your-own-chatgpt-like-ai-in-csharp.html
6. https://scisharp.github.io/LLamaSharp/0.4/ContributingGuide/#add-examples
7. https://huggingface.co/meta-llama


### Authentication Identity
1. https://damienbod.com/
2. https://damienbod.com/2020/05/29/login-and-use-asp-net-core-api-with-azure-ad-auth-and-user-access-tokens/
