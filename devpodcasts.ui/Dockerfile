FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
RUN apt-get update && apt-get install -y nodejs npm

# Copy project files and restore dependencies
COPY ["devpodcasts.ui/devpodcasts.ui.csproj", "devpodcasts.ui/"]
COPY ["devpodcasts.domain/devpodcasts.domain.csproj", "devpodcasts.domain/"]
COPY ["devpodcasts.common/devpodcasts.common.csproj", "devpodcasts.common/"]
COPY ["devpodcasts.data.entityframework/devpodcasts.data.entityframework.csproj", "devpodcasts.data.entityframework/"]

RUN dotnet restore "devpodcasts.ui/devpodcasts.ui.csproj"

# Copy the remaining files and build the application
COPY . .
WORKDIR "/src/devpodcasts.ui"
RUN dotnet publish "devpodcasts.ui.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production
EXPOSE 5000
ENTRYPOINT ["dotnet", "devpodcasts.ui.dll"]
