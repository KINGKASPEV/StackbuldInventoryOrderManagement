# Base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Important: Railway uses a dynamic PORT env var.
ENV ASPNETCORE_URLS=http://*:${PORT}

EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["StackbuldInventoryOrderManagement.Api/StackbuldInventoryOrderManagement.Api.csproj", "StackbuldInventoryOrderManagement.Api/"]
COPY ["StackbuldInventoryOrderManagement.Common/StackbuldInventoryOrderManagement.Common.csproj", "StackbuldInventoryOrderManagement.Common/"]
COPY ["StackbuldInventoryOrderManagement.Domain/StackbuldInventoryOrderManagement.Domain.csproj", "StackbuldInventoryOrderManagement.Domain/"]
COPY ["StackbuldInventoryOrderManagement.Infrastructure/StackbuldInventoryOrderManagement.Persistence.csproj", "StackbuldInventoryOrderManagement.Infrastructure/"]
COPY ["StackbuldInventoryOrderManagement.Application/StackbuldInventoryOrderManagement.Application.csproj", "StackbuldInventoryOrderManagement.Application/"]

RUN dotnet restore "./StackbuldInventoryOrderManagement.Api/StackbuldInventoryOrderManagement.Api.csproj"
COPY . .
WORKDIR "/src/StackbuldInventoryOrderManagement.Api"
RUN dotnet build "./StackbuldInventoryOrderManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StackbuldInventoryOrderManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "StackbuldInventoryOrderManagement.Api.dll"]
