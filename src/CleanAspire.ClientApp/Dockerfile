# Stage 1: Build the Blazor Client Application
FROM mcr.microsoft.com/dotnet/sdk:10.0-rc.1 AS build
WORKDIR /src

# Install Python for AOT compilation
RUN apt-get update && apt-get install -y python3 python3-pip && ln -s /usr/bin/python3 /usr/bin/python

# Copy the project files and restore dependencies
COPY ["src/CleanAspire.ClientApp/CleanAspire.ClientApp.csproj", "src/CleanAspire.ClientApp/"]
RUN dotnet restore "src/CleanAspire.ClientApp/CleanAspire.ClientApp.csproj"

# Install wasm-tools for AOT
RUN dotnet workload install wasm-tools --skip-manifest-update
RUN dotnet workload update

# Copy the entire source code and build the application in Release mode
COPY . .
RUN dotnet publish "src/CleanAspire.ClientApp/CleanAspire.ClientApp.csproj" -c Release -o /app/publish -p:DefineConstants=STANDALONE

# Stage 2: Serve the Blazor Client Application using Nginx
FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html

# Install OpenSSL to create a self-signed certificate
RUN apk add --no-cache openssl && \
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 -keyout /etc/ssl/private/nginx-selfsigned.key -out /etc/ssl/certs/nginx-selfsigned.crt -subj "/CN=localhost"

# Clean the default nginx content
RUN rm -rf ./*

# Copy the build output from the previous stage
COPY --from=build /app/publish/wwwroot .

# Copy the generated self-signed certificate and configure Nginx for HTTPS
COPY src/CleanAspire.ClientApp/nginx.conf /etc/nginx/nginx.conf

# Expose port 80 for HTTP traffic and 443 for HTTPS traffic
EXPOSE 80
EXPOSE 443

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]