# Run Kafka docker to support streaming and docker qdrant to support vector database

- Kafka Confluent

- Kafka streaming:  https://lgouellec.github.io/kafka-streams-dotnet/

Kafka Support using docker:
```
docker compose  up -d
docker compose  -f docker-compose.yml up -d
```


Qdrant:

https://github.com/qdrant/qdrant/blob/master/QUICK_START.md
```
    docker run -p 6333:6333 qdrant/qdrant
or
    docker run -p 6333:6333 -v /temp/qdrant_storage:/qdrant/storage qdrant/qdrant:latest
```

To use TestContainer.Net, add this to appsettings a,d :

```
  "DockerLaunch": {
    "DockerSettings": [
      {
        "ImageName": "qdrant/qdrant:latest",
        "HostPort": 6343,
        "ContainerPort": 6333,
        "WaitForPort": 6333,
        "HostPath": "/temp/test_qdrant_storage",
        "ContainerPath": "/qdrant/storage"
      }
    ]
  },
  ```