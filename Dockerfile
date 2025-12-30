# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ["CashLink.Api/CashLink.Api.csproj", "CashLink.Api/"]
RUN dotnet restore "CashLink.Api/CashLink.Api.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/CashLink.Api"
RUN dotnet build "CashLink.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "CashLink.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

# Copy database schema
COPY CashLink.Api/Database/ ./Database/

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "CashLink.Api.dll"]
