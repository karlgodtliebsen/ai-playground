# LlamaSharp WebAPI
An ASP.NET Core WebAPI (v7.0) using LlamaSCharp and LlaMa version 1 and 2 models.


###Info:
Some  configuration is missing from the Program.cs and Appsettings file, look at LLamaSharpApp.WebAPI for details
Fix by using this: 
> https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-7.0


#### How to download the source and build:
Clone the repository and open the solution in Visual Studio 2022.

Download a Llama model version 1 or version 2 like: 

```
- llama-2-7b.ggmlv3.q8_0.bin
- wizardLM-7B.ggmlv3.q4_1.bin
- ggml-vic13b-uncensored-q4_1.bin
- ggml-vic13b-uncensored-q5_0.bin
- ggml-vicuna-13B-1.1-q4_0.bin
- ggml-vicuna-13B-1.1-q8_0.bin
- wizardlm-13b-v1.1-superhot-8k.ggmlv3.q4_1.bin (see references)
```

Add the models to a folder and update the appsettings.json files with the path to the folder.
I use: '/projects/AI/LlamaModels' to store the models.


### Authentication
The Asp.net Web APIs are configured to use Authentication:  
Either fill out the appsettings.json section (Remember not to commit/publish the information) or use Keyvault or UserSecret during development.
Use either Environment variables, Azure Keyvault or UserSecret during development (or...).

More information at: 
> https://damienbod.com/
Authentication can be disabled in the Program.cs files, and remember to remove the [Authentication] attributes from the controllers, and Hardcode some User Identity. 

