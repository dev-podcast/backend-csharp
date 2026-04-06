# Use the official .NET SDK image for building the project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["devpodcasts.worker.podcasts/devpodcasts.worker.podcasts.csproj", "devpodcasts.worker.podcasts/"]
COPY ["devpodcasts.common/devpodcasts.common.csproj", "devpodcasts.common/"]
COPY ["devpodcasts.data.entityframework/devpodcasts.data.entityframework.csproj", "devpodcasts.data.entityframework/"]
COPY ["devpodcasts.domain/devpodcasts.domain.csproj", "devpodcasts.domain/"]

# Restore dependencies
RUN dotnet restore "devpodcasts.worker.podcasts/devpodcasts.worker.podcasts.csproj"

# Copy the remaining files
COPY . .

# Build and publish the worker project
WORKDIR "/src/devpodcasts.worker.podcasts"
RUN dotnet publish "devpodcasts.worker.podcasts.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the official .NET runtime image for running the project
FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set the entry point for the container
ENTRYPOINT ["dotnet", "devpodcasts.worker.podcasts.dll"]
