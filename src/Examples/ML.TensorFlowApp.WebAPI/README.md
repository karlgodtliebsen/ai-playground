# Microsoft ML.TensorFlow App WebAPI
An ASP.NET Core WebAPI (v7.0) using ML.TensorFlow.

Look at the test project: ML.Net.ImageClassification.Tests

Some sane configuration is missing from the Program.cs and Appsettings file, look at LLamaSharpApp.WebAPI for details



#### How to download the source and build:


The web controllers are configured to use Authentication. 
Either fill out the appsettings.json section (Remember not to commit/publish the information).
Use either Environment variables, Azure Keyvault or UserSecret during development (or...).

Authentication can be disabled in the Program.cs file, remember to remove the [Authentication] attributes from the controllers.


### Authentication Identity
1. https://damienbod.com/
2. https://damienbod.com/2020/05/29/login-and-use-asp-net-core-api-with-azure-ad-auth-and-user-access-tokens/
