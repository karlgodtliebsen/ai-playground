# LlmaSharp WebAPI
An ASP.NET Core WebAPI (v7.0) using LlmaSCharp.

This is early work.

#How to
Download the source and build


Download a model like: wizardLM-7B.ggmlv3.q4_1.bin
https://huggingface.co/TheBloke
https://huggingface.co/TheBloke/wizardLM-7B-GGML/resolve/main/wizardLM-7B.ggmlv3.q4_1.bin



Add the Model to the folder LlmaModels, and mark it for "Copy If Newer" or locate the model file elsewhere.

Modify the appsettings.json to point to the model.



On the first run, there is 'a load of model' penalty or maybe execution time? This can be solved later on.


# References
https://github.com/SciSharp/LLamaSharp
https://scisharp.github.io/SciSharp/

https://blog.maartenballiauw.be/post/2023/06/15/running-large-language-models-locally-your-own-chatgpt-like-ai-in-csharp.html
https://scisharp.github.io/LLamaSharp/0.4/ContributingGuide/#add-examples


### Authentication Identity
https://damienbod.com/

