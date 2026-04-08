# Use the official .NET ASP.NET runtime as a parent image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
COPY ["devpodcasts.server.api/devpodcasts.server.api.csproj", "devpodcasts.server.api/"]
COPY ["devpodcasts.data.entityframework/devpodcasts.data.entityframework.csproj", "devpodcasts.data.entityframework/"]
COPY ["devpodcasts.domain/devpodcasts.domain.csproj", "devpodcasts.domain/"]
COPY ["devpodcasts.common/devpodcasts.common.csproj", "devpodcasts.common/"]

RUN dotnet restore "devpodcasts.server.api/devpodcasts.server.api.csproj"

# Copy the remaining source code and build the application
COPY . .
WORKDIR "/src/devpodcasts.server.api"
RUN dotnet build "devpodcasts.server.api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "devpodcasts.server.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image with the published application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ENABLE_SWAGGER=true
EXPOSE 8080
ENTRYPOINT ["dotnet", "devpodcasts.server.api.dll"]
