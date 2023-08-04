# LlamaSharp WebAPI
An ASP.NET Core WebAPI (v7.0) using TensorFlow.Net and TensorFlow.Keras  Image Classification.

### Based on :
- https://github.com/SciSharp/TensorFlow.NET
- https://github.com/dotnet/samples/
- https://scisharp.github.io/tensorflow-net-docs/#/tutorials/ImageRecognition?id=_1-prepare-data

Look at the test project: TensorFlow.Net.ImageClassification.Tests


Temp:
Some sane configuration is missing from the Program.cs and Appsettings file, look at LLamaSharpApp.WebAPI for details
Fix by using this: https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-7.0


#### How to download the source and build:
Clone the repository and open the solution in Visual Studio 2022.

Download a model like: 
```
- llama-2-7b.ggmlv3.q8_0.bin
- wizardLM-7B.ggmlv3.q4_1.bin
- ggml-vic13b-uncensored-q4_1.bin
- ggml-vic13b-uncensored-q5_0.bin
- ggml-vicuna-13B-1.1-q4_0.bin
- ggml-vicuna-13B-1.1-q8_0.bin
- wizardlm-13b-v1.1-superhot-8k.ggmlv3.q4_1.bin (see references)

```


The web controllers are configured to use Authentication. 
Either fill out the appsettings.json section (Remember not to commit/publish the information).
Use either Environment variables, Azure Keyvault or UserSecret during development (or...).

Authentication can be disabled in the Program.cs file, remember to remove the [Authentication] attributes from the controllers.


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
