# This stage is used to build the service project
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

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./StackbuldInventoryOrderManagement.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "StackbuldInventoryOrderManagement.Api.dll"]