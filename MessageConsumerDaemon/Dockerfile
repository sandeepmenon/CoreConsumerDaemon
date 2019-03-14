FROM microsoft/dotnet:2.2-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY MessageConsumerDaemon/MessageConsumerDaemon.csproj MessageConsumerDaemon/
RUN dotnet restore MessageConsumerDaemon/MessageConsumerDaemon.csproj
COPY . .
WORKDIR /src/MessageConsumerDaemon
RUN dotnet build MessageConsumerDaemon.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish MessageConsumerDaemon.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "MessageConsumerDaemon.dll"]
