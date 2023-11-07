# LlamaSharp WebAPI
An ASP.NET Core WebAPI (v7.0) using LlamaSCharp and LlaMa version 1 and 2 models.


### Info:
Some  configuration is missing from the Program.cs and Appsettings file, look at LLamaSharpApp.WebAPI for details
Fix by using this: 
- https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-7.0



### Changes August 2023
As of August 2023 this is the model recommendation from LlamaSharp

Recommentations from LlamaSharp:
- https://github.com/SciSharp/LLamaSharp
- v0.5.1	v0.5.1	Llama2 7b GGUF


Add the models to a folder and update the appsettings.json files with the path to the folder.
I use: '/projects/AI/LlamaModels' to store the models.

> Mote recommendation in main readme.md


### Llama 2 prompting:
- https://replicate.com/blog/how-to-prompt-llama

## Model advice:
- https://replicate.com/blog/how-to-prompt-llama

- Llama 2 7B is really fast, but dumb. It’s good to use for simple things like summarizing or categorizing things.
- Llama 2 13B is a middle ground. It is much better at understanding nuance than 7B, and less afraid of being offensive (but still very afraid of being offensive). It does everything 7b does but better (and a bit slower). I think it works well for creative things like writing stories or poems.
- Llama 2 70B is the smartest Llama 2 variant. It’s also our most popular. We use it by default in our chat app. Use if for dialogue, logic, factual questions, coding, etc.



### Authentication
The Asp.net Web APIs are configured to use Authentication:  
Either fill out the appsettings.json section (Remember not to commit/publish the information) or use Keyvault or UserSecret during development.
Use either Environment variables, Azure Keyvault or UserSecret during development (or...).

More information at: 
> https://damienbod.com/

Authentication can be disabled in the Program.cs files, and remember to remove the [Authentication] attributes from the controllers, and Hardcode some User Identity. 

