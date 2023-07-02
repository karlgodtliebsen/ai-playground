# LlmaSharp WebAPI
An ASP.NET Core WebAPI (v7.0) using LlmaSCharp.

This is early work.

#How to
Download the source and build


Download a model like: wizardLM-7B.ggmlv3.q4_1.bin
https://huggingface.co/TheBloke
https://huggingface.co/TheBloke/wizardLM-7B-GGML/resolve/main/wizardLM-7B.ggmlv3.q4_1.bin


Add the Model to the 'LlmaModels' folder, and mark it for "Copy If Newer".
If you prefer to locate the model file elsewhere, then modify the appsettings.json to point to the model.

The web controllers are configured to use Authentication.  Either fill out the appsettings.json section (Remember not to commit/publish the information).
Use either Keyvault or UserSecret during development.

You can disable this in the Program.cs file, and remove the [Authentication] attributes from the controllers.

```



# References
https://github.com/SciSharp/LLamaSharp
https://scisharp.github.io/SciSharp/

https://blog.maartenballiauw.be/post/2023/06/15/running-large-language-models-locally-your-own-chatgpt-like-ai-in-csharp.html
https://scisharp.github.io/LLamaSharp/0.4/ContributingGuide/#add-examples


### Authentication Identity
https://damienbod.com/
https://damienbod.com/2020/05/29/login-and-use-asp-net-core-api-with-azure-ad-auth-and-user-access-tokens/

