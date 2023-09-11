# LlamaSharp WebAPI
An ASP.NET Core WebAPI (v7.0) using LlamaSCharp and LlaMa version 1 and 2 models.


###Info:
Some  configuration is missing from the Program.cs and Appsettings file, look at LLamaSharpApp.WebAPI for details
Fix by using this: 
> https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-7.0



### Changes August 2023
As of August 2023 this is the model recommendation from LlamaSharp

Recommentations from LlamaSharp:
- https://github.com/SciSharp/LLamaSharp
- v0.5.1	v0.5.1	Llama2 7b GGUF


Add the models to a folder and update the appsettings.json files with the path to the folder.
I use: '/projects/AI/LlamaModels' to store the models.

> Mote recommendation in main readme.md


### Authentication
The Asp.net Web APIs are configured to use Authentication:  
Either fill out the appsettings.json section (Remember not to commit/publish the information) or use Keyvault or UserSecret during development.
Use either Environment variables, Azure Keyvault or UserSecret during development (or...).

More information at: 
> https://damienbod.com/

Authentication can be disabled in the Program.cs files, and remember to remove the [Authentication] attributes from the controllers, and Hardcode some User Identity. 

