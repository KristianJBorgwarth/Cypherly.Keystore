FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# token-patched config
COPY nuget.config .
RUN --mount=type=secret,id=nuget_token \
    sed -i "s|%NUGET_TOKEN%|$(cat /run/secrets/nuget_token)|g" nuget.config

# project stubs for first restore
COPY ["src/Keystore.API/Keystore.API.csproj", "src/Keystore.API/"]
COPY ["src/Keystore.Application/Keystore.Application.csproj", "src/Keystore.Application/"]
COPY ["src/Keystore.Domain/Keystore.Domain.csproj", "src/Keystore.Domain/"]
COPY ["src/Keystore.Infrastructure/Keystore.Infrastructure.csproj", "src/Keystore.Infrastructure/"]

# single authenticated restore
RUN dotnet restore "src/Keystore.API/Keystore.API.csproj" --configfile nuget.config

# full source
COPY . .

WORKDIR /src/src/Keystore.API
RUN dotnet build Keystore.API.csproj --no-restore -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish Keystore.API.csproj --no-restore -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Keystore.API.dll"]
