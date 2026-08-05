# Récupérés avec `docker buildx imagetools inspect`.
FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine@sha256:d8ee39817ca03a3757288e83c37ed73cc969a286c603b827c7cbe33add1c2d1c AS build

WORKDIR /src

# Le manifeste est copié et restauré avant les sources pour préserver le cache NuGet.
COPY src/TpApi/TpApi.csproj src/TpApi/
COPY src/TpApi/packages.lock.json src/TpApi/
RUN dotnet restore src/TpApi/TpApi.csproj --locked-mode

COPY src/TpApi/ src/TpApi/
RUN dotnet publish src/TpApi/TpApi.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine@sha256:27b6b84beeede74fd16886177d360799c8e4299ceadfbd64eef57bafead7878a AS final

WORKDIR /app
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build --chown=$APP_UID:$APP_UID /app/publish/ ./

USER $APP_UID

HEALTHCHECK --interval=10s --timeout=3s --start-period=10s --retries=3 \
    CMD wget --no-verbose --tries=1 --spider http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "/app/TpApi.dll"]
