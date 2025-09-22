# Stage 1: Build the API Application
FROM mcr.microsoft.com/dotnet/sdk:10.0-rc.1 AS build
WORKDIR /src

COPY ["src/CleanAspire.Api/CleanAspire.Api.csproj", "src/CleanAspire.Api/"]
COPY ["src/CleanAspire.Application/CleanAspire.Application.csproj", "src/CleanAspire.Application/"]
COPY ["src/CleanAspire.Domain/CleanAspire.Domain.csproj", "src/CleanAspire.Domain/"]
COPY ["src/CleanAspire.Infrastructure/CleanAspire.Infrastructure.csproj", "src/CleanAspire.Infrastructure/"]
COPY ["src/CleanAspire.ServiceDefaults/CleanAspire.ServiceDefaults.csproj", "src/CleanAspire.ServiceDefaults/"]
COPY ["src/Migrators/Migrators.MSSQL/Migrators.MSSQL.csproj", "src/Migrators/Migrators.MSSQL/"]
COPY ["src/Migrators/Migrators.PostgreSQL/Migrators.PostgreSQL.csproj", "src/Migrators/Migrators.PostgreSQL/"]
COPY ["src/Migrators/Migrators.SQLite/Migrators.SQLite.csproj", "src/Migrators/Migrators.SQLite/"]

RUN dotnet restore "src/CleanAspire.Api/CleanAspire.Api.csproj"

COPY . .
WORKDIR /src/src/CleanAspire.Api
RUN dotnet publish "./CleanAspire.Api.csproj" -c Release -o /app/publish

# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0-rc.1 AS runtime
WORKDIR /app

# Copy the build output from the previous stage
COPY --from=build /app/publish .

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
ENV ASPNETCORE_ENVIRONMENT=Development

# Start the application
ENTRYPOINT ["dotnet", "CleanAspire.Api.dll"]
