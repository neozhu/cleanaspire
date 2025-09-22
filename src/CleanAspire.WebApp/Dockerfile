# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0-rc.1 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/CleanAspire.WebApp/CleanAspire.WebApp.csproj", "src/CleanAspire.WebApp/"]
COPY ["src/CleanAspire.ClientApp/CleanAspire.ClientApp.csproj", "src/CleanAspire.ClientApp/"]
COPY ["src/CleanAspire.ServiceDefaults/CleanAspire.ServiceDefaults.csproj", "src/CleanAspire.ServiceDefaults/"]
RUN dotnet restore "./src/CleanAspire.WebApp/CleanAspire.WebApp.csproj"
COPY . .
WORKDIR "/src/src/CleanAspire.WebApp"
RUN dotnet build "./CleanAspire.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CleanAspire.WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-rc.1 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Install OpenSSL
RUN apt-get update && apt-get install -y openssl

# Generate a self-signed certificate
RUN mkdir -p /app/https && \
    openssl req -x509 -newkey rsa:4096 -sha256 -days 3650 -nodes \
    -keyout /app/https/private.key -out /app/https/certificate.crt \
    -subj "/C=US/ST=State/L=City/O=Organization/CN=localhost" && \
    openssl pkcs12 -export -out /app/https/aspnetapp.pfx \
    -inkey /app/https/private.key -in /app/https/certificate.crt \
    -password pass:CREDENTIAL_PLACEHOLDER


# Setup environment variables for the application to find the certificate
ENV ASPNETCORE_URLS=http://+:80;https://+:443
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="CREDENTIAL_PLACEHOLDER"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/app/https/aspnetapp.pfx"


# Expose ports
EXPOSE 80 443

# Set the environment variable for ASP.NET Core to use Production settings
ENV ASPNETCORE_ENVIRONMENT=Production

# Enable Service Worker for PWA Installation
# COPY src/CleanAspire.ClientApp/wwwroot/service-worker.published.js wwwroot/service-worker.js
# COPY src/CleanAspire.ClientApp/wwwroot/manifest.json wwwroot/manifest.json

ENTRYPOINT ["dotnet", "CleanAspire.WebApp.dll"]