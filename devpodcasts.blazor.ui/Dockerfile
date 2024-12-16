FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["devpodcasts.blazor.ui/devpodcasts.blazor.ui.csproj", "devpodcasts.blazor.ui/"]
RUN dotnet restore "devpodcasts.blazor.ui/devpodcasts.blazor.ui.csproj"
COPY . .
WORKDIR "/src/devpodcasts.blazor.ui"
RUN dotnet build "devpodcasts.blazor.ui.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "devpodcasts.blazor.ui.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "devpodcasts.blazor.ui.dll"]
