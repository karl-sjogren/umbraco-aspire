FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM node:22-alpine AS frontend-build
WORKDIR /src

COPY --link ./src/Umbraco.Aspire.Frontend/ .
RUN yarn
RUN yarn build --outDir /app/build

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY --link . .
RUN dotnet restore
RUN dotnet build ./src/Umbraco.Aspire.Umbraco/Umbraco.Aspire.Umbraco.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish ./src/Umbraco.Aspire.Umbraco/Umbraco.Aspire.Umbraco.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=frontend-build /app/build ./wwwroot/
# We need to make sure that the user running the app has write access to the umbraco folder, in order to write logs and other files.
# Since these are volumes they are created as root by the docker daemon.
USER root
RUN mkdir umbraco
RUN mkdir umbraco/Logs
RUN chown $APP_UID umbraco --recursive
USER $APP_UID
ENTRYPOINT ["dotnet", "Umbraco.Aspire.Umbraco.dll"]
