# LlmaSharp WebAPI

An ASP.NET Core WebAPI (v7.0) using LlamaSCharp and various models from Huggingface as well as llama version 1 and 2 models.

Some configuration is missing from the Program.cs and Appsettings file, look at LLamaSharpApp.WebAPI for details
Fix by using this: 
> https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-7.0



## How to download the source and build:

### Select a Llma.cpp/ggml model

Download a model from huggingface/TheBloke: llama-2-7b.ggmlv3.q8_0.bin

> https://huggingface.co/TheBloke
> https://huggingface.co/TheBloke/wizardLM-7B-GGML/resolve/main/wizardLM-7B.ggmlv3.q4_1.bin
> https://huggingface.co/TheBloke/Llama-2-13B-chat-GGML
> https://huggingface.co/TheBloke/Llama-2-13B-chat-GGML/blob/main/README.md


### Llamasharp (Models version 1 and 2):
A dotnet Web API 'LLamaSharpApp.WebAPI' with a Test project that show how to use it. 


These models has been testet:
```
- wizardLM-7B.ggmlv3.q4_1.bin
- llama-2-7b.ggmlv3.q8_0.bin
- ggml-vic13b-uncensored-q4_1.bin
- ggml-vic13b-uncensored-q5_0.bin
- ggml-vicuna-13B-1.1-q4_0.bin
- ggml-vicuna-13B-1.1-q8_0.bin
- wizardlm-13b-v1.1-superhot-8k.ggmlv3.q4_1.bin
```


### Build
To build and use the projects, two projects with 'LlmaModels' folders need to be updated with the model of your choice:
```   
    LLamaSharpApp.WebAPI
    Embeddings.Qdrant.Tests
```

If you prefer to locate the model file(s) elsewhere, then modify the appsettings.json to point to the model.

Adjust this section in appsettings.json

```json
  "LlmaModel": {
    "modelPath": "/somepath/location/wizardLM-7B.ggmlv3.q4_1.bin",
  }

```

## TODO: 

### Examples to deep dive into:
> https://github.com/SciSharp/LLamaSharp/tree/master/LLama.Examples/NewVersion

### Models to test (if it makes sense):

 TheBloke/llama-2-7B-Guanaco-QLoRA-GGML:
> https://huggingface.co/TheBloke/llama-2-7B-Guanaco-QLoRA-GGML/blob/main/llama-2-7b-guanaco-qlora.ggmlv3.q5_0.bin

TheBloke/OpenOrca-Platypus2-13B-GGML:
> https://huggingface.co/TheBloke/OpenOrca-Platypus2-13B-GGML


TheBloke/WizardMath-13B-V1.0-GGML:
> https://huggingface.co/TheBloke/WizardMath-13B-V1.0-GGML

TheBloke/orca_mini_v3_13B-GGML:
> https://huggingface.co/TheBloke/orca_mini_v3_13B-GGML

TheBloke/h2ogpt-4096-llama2-13B-GGML:
> https://huggingface.co/TheBloke/h2ogpt-4096-llama2-13B-GGML

TheBloke/WizardLM-1.0-Uncensored-Llama2-13B-GGML:
> https://huggingface.co/TheBloke/WizardLM-1.0-Uncensored-Llama2-13B-GGML

### Look into:
WizardLM: Empoweroing Large Pre-Trained Language Models to Follow Complex Instructions
> https://huggingface.co/WizardLM/WizardLM-70B-V1.0#wizardlm-empowering-large-pre-trained-language-models-to-follow-complex-instructions
> https://huggingface.co/datasets/WizardLM/WizardLM_evol_instruct_70k
> https://huggingface.co/datasets/WizardLM/WizardLM_evol_instruct_V2_196k


### meta llama 2:
> https://ai.meta.com/research/publications/llama-2-open-foundation-and-fine-tuned-chat-models/


# References
> https://github.com/SciSharp/LLamaSharp
> https://scisharp.github.io/SciSharp/
> https://blog.maartenballiauw.be/post/2023/06/15/running-large-language-models-locally-your-own-chatgpt-like-ai-in-csharp.html
> https://scisharp.github.io/LLamaSharp/0.4/ContributingGuide/#add-examples
> https://huggingface.co/TheBloke
> https://huggingface.co/TheBloke/wizardLM-7B-GGML/resolve/main/wizardLM-7B.ggmlv3.q4_1.bin
> https://huggingface.co/meta-llama


### Authentication Identity
> https://damienbod.com/
> https://damienbod.com/2020/05/29/login-and-use-asp-net-core-api-with-azure-ad-auth-and-user-access-tokens/

