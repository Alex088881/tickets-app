# Build stage - используем .NET 9.0 SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["Tickets.Api/Tickets.Api.csproj", "Tickets.Api/"]
COPY ["Tickets.Application/Tickets.Application.csproj", "Tickets.Application/"]
COPY ["Tickets.Domain/Tickets.Domain.csproj", "Tickets.Domain/"]
COPY ["Tickets.Infrastructure/Tickets.Infrastructure.csproj", "Tickets.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "Tickets.Api/Tickets.Api.csproj"

# Copy everything else
COPY . .

# Build and publish
WORKDIR "/src/Tickets.Api"
RUN dotnet publish "Tickets.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage 
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create app directory and set permissions
RUN mkdir -p /app/data && chmod 755 /app/data

COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=30s --retries=3 \
    CMD curl -f http://localhost:8080/api/system/status || exit 1

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "Tickets.Api.dll"]