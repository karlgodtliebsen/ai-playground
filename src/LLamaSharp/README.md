# LlmaSharp WebAPI

An ASP.NET Core WebAPI (v7.0) using LlamaSCharp and various models from Huggingface as well as llama version 1 and 2 models.

Some configuration is missing from the Program.cs and Appsettings file, look at LLamaSharpApp.WebAPI for details
Fix by using this: 
> https://learn.microsoft.com/en-us/aspnet/core/security/docker-https?view=aspnetcore-7.0



## Update October 2023
The next tasks are:
- Verify the new GGUF models
- Look into new Wizard, Orca and Dolphin models
- Use the integration og Semqntic Kernal and LLamaSharp


## How to download the source and build:

### Select a Llma.cpp/gguf model

Download a model from huggingface/TheBloke: 

> https://huggingface.co/TheBloke
### Changes August 2023
As of August 2023 this is the model recommendation from LlamaSharp

Recommentations from LlamaSharp:
- https://github.com/SciSharp/LLamaSharp
- v0.5.1	v0.5.1	Llama2 7b GGUF

#### Llama2 7B Guanaco QLoRA GGUF
- https://huggingface.co/TheBloke/llama-2-7B-Guanaco-QLoRA-GGUF
- llama-2-7b-guanaco-qlora.Q4_K_M.gguf	Q4_K_M	4	4.08 GB	6.58 GB		medium, balanced quality - recommended

### Needs testing
#### Guanaco-7B-Uncensored-GGUF
- https://huggingface.co/TheBloke/Guanaco-7B-Uncensored-GGUF
- guanaco-7b-uncensored.Q4_K_M.gguf	Q4_K_M	4	4.08 GB	6.58 GB	medium, balanced quality - recommended
	

#### Guanaco-13B-Uncensored-GGUF
- https://huggingface.co/TheBloke/Guanaco-13B-Uncensored-GGUF
- guanaco-13b-uncensored.Q4_K_M.gguf	Q4_K_M	4	7.87 GB	10.37 GB	medium, balanced quality - recommended


#### Llama-2-13B-Ensemble-v5-GGUF
- https://huggingface.co/TheBloke/Llama-2-13B-Ensemble-v5-GGUF
- llama-2-13b-ensemble-v5.Q4_K_M.gguf	Q4_K_M	4	7.87 GB	10.37 GB		medium, balanced quality - recommended


#### Llama-2-70B-Ensemble-v5-GGUF
- https://huggingface.co/TheBloke/Llama-2-70B-Ensemble-v5-GGUF
- llama-2-70b-ensemble-v5.Q4_K_M.gguf	Q4_K_M	4	41.42 GB	43.92 GB	medium, balanced quality - recommended


#### OpenChat v3.2 Super - GGUF
- https://huggingface.co/TheBloke/openchat_v3.2_super-GGUF
- https://huggingface.co/openchat/openchat_v3.2_super


## Model advice:
- https://replicate.com/blog/how-to-prompt-llama

- Llama 2 7B is really fast, but dumb. It’s good to use for simple things like summarizing or categorizing things.
- Llama 2 13B is a middle ground. It is much better at understanding nuance than 7B, and less afraid of being offensive (but still very afraid of being offensive). It does everything 7b does but better (and a bit slower). I think it works well for creative things like writing stories or poems.
- Llama 2 70B is the smartest Llama 2 variant. It’s also our most popular. We use it by default in our chat app. Use if for dialogue, logic, factual questions, coding, etc.


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
    "modelPath": "/somepath/location/llama-2-7B-Guanaco-QLoRA-GGUF",
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



### Llama 2 prompting:
- https://replicate.com/blog/how-to-prompt-llama



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

