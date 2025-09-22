## CleanAspire - .NET 10 Minimal API + Blazor WebAssembly PWA Template with Aspire Support
[![.NET](https://github.com/neozhu/cleanaspire/actions/workflows/dotnet.yml/badge.svg)](https://github.com/neozhu/cleanaspire/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/neozhu/cleanaspire/actions/workflows/github-code-scanning/codeql/badge.svg)](https://github.com/neozhu/cleanaspire/actions/workflows/github-code-scanning/codeql)
[![Build and Push Docker Image](https://github.com/neozhu/cleanaspire/actions/workflows/docker.yml/badge.svg)](https://github.com/neozhu/cleanaspire/actions/workflows/docker.yml)

![blazorclient](./blazorclient.jpg)

### 🚀 Overview  

**CleanAspire** is a cutting-edge, open-source template built on **.NET 10** (and still compatible with **.NET 9** projects), designed to accelerate the development of **lightweight**, **fast**, and **simple** Blazor WebAssembly or Progressive Web Applications (PWA). It seamlessly integrates **Minimal APIs**, **Aspire**, and **Scalar** for modern API documentation.

With a focus on **Clean Architecture** and **extreme code simplicity**, CleanAspire provides developers with the tools to create responsive and maintainable web applications with minimal effort. The template also supports **Microsoft.Kiota** to simplify API client generation, ensuring consistency and productivity in every project.  

### 🎉 Auto-generate Code Using Chat GPTs

![chatgpt](./gpts.png)
 [Link to CleanAspire Code Generator](https://chatgpt.com/g/g-677e40252ff88191933bb84567b62e7b-cleanaspire-code-generator)
 

### 🌐 Offline Support  

CleanAspire fully supports **offline mode** through its integrated PWA capabilities, enabling your application to function seamlessly without an internet connection. By leveraging **Service Workers** and **browser caching**, the application can store essential resources and data locally, ensuring quick load times and uninterrupted access. Additionally, CleanAspire offers streamlined configuration options to help developers manage caching strategies and data synchronization effortlessly, guaranteeing that users receive the latest updates once the network is restored.

**Key Features of Offline Support:**

- **Service Workers Integration:** Efficiently handle caching and background synchronization to manage offline functionality.
- **Automatic Resource Caching:** Automatically caches essential assets and API responses, ensuring critical parts of the application are accessible offline.
- **Seamless Data Synchronization:** Maintains data consistency by synchronizing local changes with the server once the connection is reestablished.
- **User Experience Enhancements:** Provides fallback UI components and notifications to inform users about their offline status and any pending actions.

By incorporating robust offline capabilities, CleanAspire empowers developers to build resilient applications that deliver a consistent and reliable user experience, regardless of network conditions.


### 🔑 Key Features  

1. **Built-in Aspire Support**  
   - Fully integrated with **Aspire** for efficient application hosting and configuration.  
   - Simplifies the setup process while providing a robust foundation for lightweight applications.  

2. **Fast and Minimal .NET 10 Minimal APIs**
   - Uses the latest .NET 10 features to create high-performance and efficient APIs.
   - Includes **Scalar** for modern and concise OpenAPI documentation, replacing traditional Swagger tools.  

3. **Designed for Simplicity and Speed**  
   - Adopts extreme code simplicity for rapid development without sacrificing functionality.  
   - Ideal for developers looking to build quick, responsive Blazor WebAssembly applications or PWAs.  

4. **Blazor WebAssembly and PWA Integration**  
   - Combines the power of Blazor WebAssembly for interactive and lightweight client-side UIs.  
   - PWA capabilities ensure offline support and a seamless native-like experience, allowing users to access the application and data even when offline.  

5. **Streamlined API Client Integration**  
   - Utilizes **Microsoft.Kiota** to automatically generate strongly-typed API clients, reducing development overhead.  
   - Ensures consistent and error-free client-server communication.  

6. **Clean Architecture**  
   - Promotes modular, maintainable, and testable codebases through clear architectural layers.  

7. **Cloud-Ready with Docker**  
   - Preconfigured for Docker, enabling easy deployment to cloud platforms or local environments.  

8. **Real-Time Web Push Notifications**  
   - Integrated **Webpushr** to deliver instant browser notifications.  
   - Keeps users informed and engaged with real-time updates.  
   - Fully customizable notifications with targeted delivery and analytics support.  

9. **Integrated CI/CD Pipelines**  
   - Includes GitHub Actions workflows for automated building, testing, and deployment.  

10. **Offline Mode Support**  
    - **Offline mode enabled by default** to provide a seamless experience even without internet access.  
    - Uses **IndexedDB** to cache data locally, allowing the application to retrieve data and function offline.  
    - The system detects the online/offline status and fetches data from **IndexedDB** when offline, ensuring uninterrupted access to key features.  


### How to Create a New Object in a CRUD Application: A Step-by-Step Guide

https://github.com/neozhu/cleanaspire/issues/34

### 🌟 Why Choose CleanAspire?  

- **Lightweight and Fast:** Designed to create high-performance, minimal Blazor WebAssembly or PWA projects.  
- **Effortless Development:** Extreme simplicity in code makes it easy to start quickly and scale effectively.  
- **Advanced API Integration:** Automate client-side API generation with Microsoft.Kiota for faster results.  
- **Future-Ready Architecture:** Leverages the latest .NET 10 capabilities and Aspire hosting for modern web applications.
- 

### OpenAPI documentation
- https://apiservice.blazorserver.com/scalar/v1
- https://cleanapi.blazors.app:8443/scalar/v1

### Demo site
- https://cleanweb.blazors.app:8443/

### Blazor WebAssembly Standalone PWA
- https://standalone.blazorserver.com/
- https://standalone.blazors.app:8443/


### Here is an example of a docker-compose.yml file for a local Docker deployment:

```yml
version: '3.8'
services:
  cleanapi:
    image: blazordevlab/cleanaspire-api:0.0.73
    container_name: cleanapi
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - AllowedHosts=*
      - ASPNETCORE_URLS=http://+:80;https://+:443
      - ASPNETCORE_HTTP_PORTS=80
      - ASPNETCORE_HTTPS_PORTS=443
      - DatabaseSettings__DBProvider=sqlite
      - DatabaseSettings__ConnectionString=Data Source=CleanAspireDb.db
      - ClientBaseUrl=https://web.cleanaspire.blazors.app:8443
      - AllowedCorsOrigins=https://cleanweb.blazors.app:8443,https://standalone.blazors.app:8443
      - SendGrid__ApiKey=<your-api-key>
      - SendGrid__DefaultFromEmail=noreply@blazorserver.com
      - Authentication__Google__ClientId=<your-client-id>
      - Authentication__Google__ClientSecret=<your-secret-key>
      - Webpushr__Token=<your-webpushr-token>
      - Webpushr__ApiKey=<your-webpushr-api-key>
      - Webpushr__PublicKey=<your-webpushr-public-key>
      - Minio__Endpoint=<your-minio-server>
      - Minio__AccessKey=<your-minio-key>
      - Minio__SecretKey=<your-minio-secret>
      - Minio__BucketName=demo
    labels:
      - "traefik.enable=true"
      - "traefik.http.routers.cleanapi-secure.entrypoints=https"
      - "traefik.http.routers.cleanapi-secure.rule=Host(`cleanapi.blazors.app`)"
      - "traefik.http.routers.cleanapi-secure.tls=true"
      - "traefik.http.routers.cleanapi-secure.service=cleanapi"
      - "traefik.http.services.cleanapi.loadbalancer.server.port=443"
      - "traefik.http.services.cleanapi.loadbalancer.server.scheme=https"
      - "traefik.docker.network=proxy"
    networks:
      proxy:
    security_opt:
      - no-new-privileges:true
  cleanweb:
    image: blazordevlab/cleanaspire-webapp:0.0.73
    container_name: cleanweb
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - AllowedHosts=*
      - ASPNETCORE_URLS=http://+:80;https://+:443
      - ASPNETCORE_HTTP_PORTS=80
      - ASPNETCORE_HTTPS_PORTS=443
    labels:
      - "traefik.enable=true"
      - "traefik.http.routers.cleanweb-secure.entrypoints=https"
      - "traefik.http.routers.cleanweb-secure.rule=Host(`cleanweb.blazors.app`)"
      - "traefik.http.routers.cleanweb-secure.tls=true"
      - "traefik.http.routers.cleanweb-secure.service=cleanweb"
      - "traefik.http.services.cleanweb.loadbalancer.server.port=443"
      - "traefik.http.services.cleanweb.loadbalancer.server.scheme=https"
      - "traefik.docker.network=proxy"
    networks:
      proxy:
    security_opt:
      - no-new-privileges:true
  standalone:
    container_name: standalone
    image: blazordevlab/cleanaspire-standalone:0.0.73
    labels:
      - "traefik.enable=true"
      - "traefik.http.routers.standalone-secure.entrypoints=https"
      - "traefik.http.routers.standalone-secure.rule=Host(`standalone.blazors.app`)"
      - "traefik.http.routers.standalone-secure.tls=true"
      - "traefik.http.routers.standalone-secure.service=standalone"
      - "traefik.http.services.standalone.loadbalancer.server.port=443"
      - "traefik.http.services.standalone.loadbalancer.server.scheme=https"
      - "traefik.docker.network=proxy"
    networks:
      proxy:
    security_opt:
      - no-new-privileges:true
networks:
  proxy:
    external: true

```

### Quick Start

1. **Clone the Repo**:
   ```bash
   git clone https://github.com/neozhu/cleanaspire.git
   ```

2. **Navigate to the Project Folder**:
   ```bash
   cd CleanAspire
   ```

3. **Run the Application**:
   ```bash
   dotnet run
   ```

4. **Access the Application**:
   Open your browser and go to `https://localhost:5001` to see the Blazor WebAssembly PWA in action.

### How to Register and Configure Webpushr

1. **Register on Webpushr**  
   - Visit the [Webpushr website](https://www.webpushr.com/) and sign up for an account.  
   - Complete the registration process to access your dashboard.  

2. **Obtain Required Keys**  
   - Navigate to the API configuration section in your Webpushr dashboard.  
   - Copy the following keys:  
     - **Token**  
     - **API Key**  
     - **Public Key**  

3. **Add Configuration to `appsettings.json`**  
   Add the keys obtained from Webpushr into your application configuration file as follows:  
   ```json
   "Webpushr": {
       "Token": "your-webpushr-token",
       "APIKey": "your-webpushr-api-key",
       "PublicKey": "your-webpushr-public-key"
   }
   ```

4. **Integrate Webpushr in the Application**  
   - Use the `PublicKey` for initializing Webpushr on the client-side to enable browser notifications.  
   - Use the `Token` and `API Key` securely on the server-side for API communication with Webpushr.




### Steps to Add a New Entity Class

1. **Create a New Entity Class:**
   - Create a new class for the entity (e.g., `Product.cs`) in the `Entities` directory of the `CleanAspire.Domain` project.
   - This class should typically inherit from `BaseAuditableEntity` and implement the `IAuditTrial` interface, which allows you to track audit information such as created and modified timestamps.

   ```csharp
   public class Product : BaseAuditableEntity, IAuditTrial
   {
       public int Id { get; set; }
       public string Name { get; set; }
       public decimal Price { get; set; }
       // Other properties...
   }
   ```

2. **Add `DbSet<Product>` to `IApplicationDbContext`:**
   - In the `IApplicationDbContext.cs` file in the `CleanAspire.Application` project, add a `DbSet<Product>` property to represent the collection of `Product` entities in the database.

   ```csharp
   public interface IApplicationDbContext
   {
       DbSet<Product> Products { get; }
       // Other DbSets...
   }
   ```

3. **Implement `DbSet<Product>` in `ApplicationDbContext`:**
   - In the `ApplicationDbContext.cs` file in the `CleanAspire.Infrastructure` project, implement the `DbSet<Product>` property to interact with the database.

   ```csharp
   public class ApplicationDbContext : DbContext, IApplicationDbContext
   {
       public DbSet<Product> Products { get; set; }
       // Other DbSets...

       protected override void OnModelCreating(ModelBuilder modelBuilder)
       {
           base.OnModelCreating(modelBuilder);
           // Additional configurations
       }
   }
   ```

4. **Define Entity Configuration for `Product`:**
   - Define any necessary configurations for the `Product` entity, such as foreign keys, field lengths, etc.
   - Create a new configuration class (`ProductConfiguration.cs`) in the `Persistence\Configurations` directory under the `CleanAspire.Infrastructure` project. This is where you can specify constraints and configurations specific to the entity.

   ```csharp
   public class ProductConfiguration : IEntityTypeConfiguration<Product>
   {
       public void Configure(EntityTypeBuilder<Product> builder)
       {
           builder.Property(p => p.Name).HasMaxLength(100);
           // Add other property configurations here...
       }
   }
   ```

5. **Create Migration:**
   - Ensure that the correct startup project is selected. It is crucial to have either `CleanAspire.AppHost` or `CleanAspire.Api` set as the startup project.
   - Open the **Package Manager Console** in Visual Studio and set the default project to the one that matches your database configuration in `appsettings.json` (e.g., `Migrators.SQLite`, `Migrators.PostgreSQL`, or `Migrators.MSSQL`).

     - In **Package Manager Console**:
     ```powershell
     PM> Add-Migration Product
     PM> Update-Database
     ```

   - These commands will create the migration for the new `Product` entity and update the database accordingly.



### Architecture

CleanAspire is structured following the **Clean Architecture** approach, dividing the solution into distinct layers:

```
CleanAspire/
│
├── Solution Items/
│   ├── .editorconfig
│
├── src/
│   ├── CleanAspire.Api/                # API Layer - .NET Minimal API
│   │   └── CleanAspire.Api.csproj
│   ├── CleanAspire.AppHost/            # Hosting Layer - Application hosting and configuration
│   │   └── CleanAspire.AppHost.csproj
│   ├── CleanAspire.Application/        # Application Layer - Business Logic
│   │   └── CleanAspire.Application.csproj
│   ├── CleanAspire.ClientApp/          # Client App Layer - Blazor WebAssembly or other client logic
│   │   └── CleanAspire.ClientApp.csproj
│   ├── CleanAspire.Domain/             # Domain Layer - Core Entities, including EF entities
│   │   └── CleanAspire.Domain.csproj
│   ├── CleanAspire.Infrastructure/     # Infrastructure Layer - Data Access, External Services
│   │   └── CleanAspire.Infrastructure.csproj
│   ├── CleanAspire.ServiceDefaults/    # Service Defaults - Predefined configurations for services
│   │   └── CleanAspire.ServiceDefaults.csproj
│
├── src/Migrators/
│   ├── Migrators.MSSQL/                # SQL Server Migration Scripts
│   │   └── Migrators.MSSQL.csproj
│   ├── Migrators.PostgreSQL/           # PostgreSQL Migration Scripts
│   │   └── Migrators.PostgreSQL.csproj
│   ├── Migrators.SQLite/               # SQLite Migration Scripts
│   │   └── Migrators.SQLite.csproj
│
├── tests/
│   ├── CleanAspire.Tests/              # Unit and Integration Tests
│   │   └── CleanAspire.Tests.csproj
│
├── README.md                           # Project README
├── LICENSE                             # License Information
└── CleanAspire.sln                     # Solution File

```

### Contributions

Contributions are welcome! If you want to add features, report bugs, or just suggest improvements, please feel free to submit issues or pull requests.

### License

This project is licensed under the [MIT License](LICENSE).

### Get Involved

- **Star the Repository**: If you like CleanAspire, please give it a star! 🌟
- **Follow the Project**: Stay updated with the latest developments by watching the repository.
- **Join Discussions**: Share your thoughts and ideas to make CleanAspire even better.

### Contact

Feel free to reach out if you have questions or feedback. Let's build something amazing together with **CleanAspire**!

