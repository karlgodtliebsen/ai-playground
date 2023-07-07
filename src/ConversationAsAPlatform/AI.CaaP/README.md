# Conversation as a Platform


This is early work.

## How to download the source and build:


```
Add the Model to the 'LlmaModels' folder, and mark it for "Copy If Newer".

If you prefer to locate the model file elsewhere, then modify the appsettings.json to point to the model.

The web controllers are configured to use Authentication.  Either fill out the appsettings.json section (Remember not to commit/publish the information).
Use either Keyvault or UserSecret during development.

It can be disabled in the Program.cs file, remember to remove the [Authentication] attributes from the controllers.

```



# References
https://github.com/SciSharp/BotSharp


