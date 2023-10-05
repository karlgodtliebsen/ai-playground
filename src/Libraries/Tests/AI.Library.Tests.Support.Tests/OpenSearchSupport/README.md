# OpenSearch Docker
https://hub.docker.com/r/opensearchproject/opensearch
https://github.com/opensearch-project/docker-images

default username:password
admin:admin

- docker run -p 9200:9200 -p 9600:9600 -e "discovery.type=single-node" --name opensearch-node -d opensearchproject/opensearch:latest


- curl -X GET "https://localhost:9200" -ku admin:admin
- curl -X GET "https://localhost:9200/_cat/nodes?v" -ku admin:admin
- curl -X GET "https://localhost:9200/_cat/plugins?v" -ku admin:admin


## Dashboard
https://hub.docker.com/r/opensearchproject/opensearch-dashboards


# Docker Compose
Sample found here:
https://opensearch.org/docs/latest/install-and-configure/install-opensearch/docker/#important-host-settings


# OpenSearch Dotnet Client
https://opensearch.org/docs/latest/clients/OSC-dot-net/


