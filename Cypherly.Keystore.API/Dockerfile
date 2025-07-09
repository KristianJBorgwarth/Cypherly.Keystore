FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY nuget.config .
RUN --mount=type=secret,id=nuget_token \
    sed -i "s|%NUGET_TOKEN%|$(cat /run/secrets/nuget_token)|g" nuget.config

COPY . .

RUN dotnet restore Cypherly.Keystore.sln --configfile ./nuget.config

WORKDIR /src/Cypherly.Keystore.API
RUN dotnet build Cypherly.Keystore.API.csproj --no-restore -c $BUILD_CONFIGURATION -o /app/build
RUN dotnet publish Cypherly.Keystore.API.csproj --no-restore -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Cypherly.Keystore.API.dll"]
