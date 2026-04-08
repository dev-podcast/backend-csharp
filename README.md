# DevPodcasts Backend

This repository contains the backend services for the DevPodcasts platform, including an API, a background worker for podcast updates, and a UI service.

## Project Structure

- **devpodcasts.server.api**: ASP.NET Core Web API for serving podcast data.
- **devpodcasts.worker.podcasts**: Background worker service that periodically fetches and updates podcast data.
- **devpodcasts.ui**: ASP.NET Core Web application hosting the frontend.
- **devpodcasts.domain**: Core domain entities and interfaces.
- **devpodcasts.common**: Shared utilities and helper classes.
- **devpodcasts.data.entityframework**: Data access layer using Entity Framework Core.
- **docker**: Contains Docker Compose configuration for local development and deployment.

## Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Docker](https://www.docker.com/products/docker-desktop)
- [Node.js](https://nodejs.org/) (required for UI build)

## Local Development

### Running with .NET CLI

You can run the individual services using the `dotnet run` command:

```bash
# Start the API
dotnet run --project devpodcasts.server.api

# Start the Worker
dotnet run --project devpodcasts.worker.podcasts

# Start the UI
dotnet run --project devpodcasts.ui
```

### Running with Docker Compose

To start all services including SQL Server:

```bash
cd docker
docker-compose up -d
```

## Deployment Pipeline

The project uses GitHub Actions for CI/CD.

### GitHub Actions Workflow

The `.github/workflows/docker-publish.yml` workflow is triggered on pushes and pull requests to the `main` or `master` branches. It performs the following steps:

1.  **Checkout**: Pulls the source code.
2.  **Login**: Authenticates with GitHub Container Registry (GHCR).
3.  **Build & Push**: Builds Docker images for the API, UI, and Worker services and pushes them to GHCR.

### Deployment via Docker

Once the images are pushed to GHCR, you can deploy them to any server using Docker Compose. Update the image names in `docker/docker-compose.yml` to point to your repository's images.

```bash
# Pull the latest images
docker-compose pull

# Start/Update the services
docker-compose up -d
```

## Configuration

Services can be configured via `appsettings.json` or Environment Variables:

- `ConnectionStrings__DefaultConnection`: Database connection string.
- `ASPNETCORE_ENVIRONMENT`: Set to `Production` for deployment.
- `ENABLE_SWAGGER`: Set to `true` to enable Swagger UI in production.
