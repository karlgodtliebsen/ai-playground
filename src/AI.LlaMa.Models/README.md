## LlaMa Models
Download a model like: 

- wizardLM-7B.ggmlv3.q4_1.bin

Add the Models to the 'LlamaModels' folder in the 'AI.LlaMa.Models' project, and mark them as "Copy If Newer".

### Llama models from Huggingface (llma.cpp/ggml):
> https://huggingface.co/TheBloke

```
- wizardLM-7B.ggmlv3.q4_1.bin
- ggml-vic13b-uncensored-q4_1.bin
- ggml-vic13b-uncensored-q5_0.bin
- ggml-vicuna-13B-1.1-q4_0.bin
- ggml-vicuna-13B-1.1-q8_0.bin
- wizardlm-13b-v1.1-superhot-8k.ggmlv3.q4_1.bin
```

This model requires MetaAI registration/approval:
>https://huggingface.co/meta-llama
```
- llama-2-7b.ggmlv3.q8_0.bin
```
And finally:
- The lifetime of models are very short. So you might need to update the model names in the code.
