#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Examples/ML.TensorFlowApp.WebAPI/ML.TensorFlowApp.WebAPI.csproj", "Examples/ML.TensorFlowApp.WebAPI/"]
COPY ["AI.Library/AI.Library.csproj", "AI.Library/"]
COPY ["ImageClassification/ImageClassification.Domain/ImageClassification.Domain.csproj", "ImageClassification/ImageClassification.Domain/"]
COPY ["ImageClassification/Ml.Net.ImageClassification.Domain/Ml.Net.ImageClassification.Domain.csproj", "ImageClassification/Ml.Net.ImageClassification.Domain/"]
RUN dotnet restore "Examples/ML.TensorFlowApp.WebAPI/ML.TensorFlowApp.WebAPI.csproj"
COPY . .
WORKDIR "/src/Examples/ML.TensorFlowApp.WebAPI"
RUN dotnet build "ML.TensorFlowApp.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ML.TensorFlowApp.WebAPI.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ML.TensorFlowApp.WebAPI.dll"]