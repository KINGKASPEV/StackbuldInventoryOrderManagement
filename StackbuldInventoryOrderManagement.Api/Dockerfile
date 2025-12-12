# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["StackbuldInventoryOrderManagement.Api/StackbuldInventoryOrderManagement.Api.csproj", "StackbuldInventoryOrderManagement.Api/"]
COPY ["StackbuldInventoryOrderManagement.Common/StackbuldInventoryOrderManagement.Common.csproj", "StackbuldInventoryOrderManagement.Common/"]
COPY ["StackbuldInventoryOrderManagement.Domain/StackbuldInventoryOrderManagement.Domain.csproj", "StackbuldInventoryOrderManagement.Domain/"]
COPY ["StackbuldInventoryOrderManagement.Infrastructure/StackbuldInventoryOrderManagement.Persistence.csproj", "StackbuldInventoryOrderManagement.Infrastructure/"]
COPY ["StackbuldInventoryOrderManagement.Application/StackbuldInventoryOrderManagement.Application.csproj", "StackbuldInventoryOrderManagement.Application/"]

RUN dotnet restore "StackbuldInventoryOrderManagement.Api/StackbuldInventoryOrderManagement.Api.csproj"

COPY . .
WORKDIR "/src/StackbuldInventoryOrderManagement.Api"
RUN dotnet publish "StackbuldInventoryOrderManagement.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "StackbuldInventoryOrderManagement.Api.dll"]
