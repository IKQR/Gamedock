FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["GameDock/Server/GameDock.Server.csproj", "GameDock/Server/"]
COPY ["GameDock/Client/GameDock.Client.csproj", "GameDock/Client/"]
COPY ["GameDock/Shared/GameDock.Shared.csproj", "GameDock/Shared/"]
COPY ["GameDock.Server.Domain/GameDock.Server.Domain.csproj", "GameDock.Server.Domain/"]
COPY ["GameDock.Server.Application/GameDock.Server.Application.csproj", "GameDock.Server.Application/"]
COPY ["GameDock.Server.Infrastructure/GameDock.Server.Infrastructure.csproj", "GameDock.Server.Infrastructure/"]
RUN dotnet restore "GameDock/Server/GameDock.Server.csproj"
COPY . .
WORKDIR "/src/GameDock/Server"
RUN dotnet build "GameDock.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "GameDock.Server.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GameDock.Server.dll"]