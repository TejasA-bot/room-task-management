# Use official .NET 8 runtime as base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use .NET SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["RoomTaskManagement.API.csproj", "./"]
RUN dotnet restore "RoomTaskManagement.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src"
RUN dotnet build "RoomTaskManagement.API.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "RoomTaskManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variable
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "RoomTaskManagement.API.dll"]