# A Qdrant Vector database Wrapper 

This project contains a very good implementation of a Qdrant client.

Based on inspiration from:

https://github.com/openai/chatgpt-retrieval-plugin/blob/main/docs/providers/qdrant/setup.md
https://python.langchain.com/docs
https://github.com/SciSharp/BotSharp


Use Docker to run Qdrant:

```
    https://github.com/qdrant/qdrant/blob/master/QUICK_START.md:

    docker run -p 6333:6333 qdrant/qdrant

or

    docker run -p 6333:6333 -v c:\\temp\\qdrant_storage:/qdrant/storage qdrant/qdrant:latest
    
```


The test projects uses TestContainer to run Qdrant in a docker container.


## Missing features:

https://qdrant.tech/documentation/concepts/points/#update-vectors
https://qdrant.tech/documentation/concepts/payload/#delete-payload
https://qdrant.tech/documentation/concepts/payload/#payload-indexing



# References:
https://github.com/qdrant/qdrant
https://github.com/openai/chatgpt-retrieval-plugin/blob/main/docs/providers/qdrant/setup.md
https://colab.research.google.com/github/qdrant/examples/blob/master/qdrant_101_getting_started/getting_started.ipynb#scrollTo=rnpSspsAo8bR
https://python.langchain.com/docs
https://github.com/SciSharp/BotSharp

## Transformers: 
https://public.ukp.informatik.tu-darmstadt.de/reimers/sentence-transformers/v0.2/
