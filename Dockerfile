# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
# copy csproj and restore as distinct layers
COPY *.sln .
COPY *.csproj ./
RUN dotnet restore

# copy everything else and build app
COPY . ./
WORKDIR "/src/."
RUN dotnet build -c Release -o /app/build
FROM build AS publish
RUN dotnet publish -c release -o /app/publish 

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HypothyroBot.dll"]