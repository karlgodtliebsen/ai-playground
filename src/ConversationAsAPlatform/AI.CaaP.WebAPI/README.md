# AI.CaaP.WebAPI
An ASP.NET Core WebAPI (v7.0) using AI.CaaP and OpenAI.


## How to download the source and build:


```
Add the Model to the 'LlmaModels' folder, and mark it for "Copy If Newer".

If you prefer to locate the model file elsewhere, then modify the appsettings.json to point to the model.

The web controllers are configured to use Authentication.  Either fill out the appsettings.json section (Remember not to commit/publish the information).
Use either Keyvault or UserSecret during development.

It can be disabled in the Program.cs file, remember to remove the [Authentication] attributes from the controllers.

```


# References
https://blog.maartenballiauw.be/post/2023/06/15/running-large-language-models-locally-your-own-chatgpt-like-ai-in-csharp.html


### Authentication Identity
https://damienbod.com/
https://damienbod.com/2020/05/29/login-and-use-asp-net-core-api-with-azure-ad-auth-and-user-access-tokens/
