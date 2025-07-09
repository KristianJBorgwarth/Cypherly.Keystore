FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /src

COPY nuget.config ./nuget.config
RUN --mount=type=secret,id=nuget_token \
    sed -i "s|%NUGET_TOKEN%|$(cat /run/secrets/nuget_token)|g" nuget.config \


COPY ["Cypherly.Keystore.API/Cypherly.Keystore.API.csproj", "Cypherly.Keystore.API/"]
COPY ["Cypherly.Keystore.Application/Cypherly.Keystore.Application.csproj", "Cypherly.Keystore.Application/"]
COPY ["Cypherly.Keystore.Domain/Cypherly.Keystore.Domain.csproj", "Cypherly.Keystore.Domain/"]
COPY ["Cypherly.Keystore.Infrastructure/Cypherly.Keystore.Infrastructure.csproj", "Cypherly.Keystore.Infrastructure/"]
RUN dotnet restore "Cypherly.Keystore.API/Cypherly.Keystore.API.csproj" --configfile ./nuget.config

COPY . .

WORKDIR "/src/Cypherly.Keystore.API"
RUN dotnet build "./Cypherly.Keystore.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Cypherly.Keystore.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cypherly.Keystore.API.dll"]
