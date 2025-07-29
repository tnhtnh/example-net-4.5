# WingtipToys - .NET 4.5 E-commerce Application

[![Build Status](https://github.com/your-org/wingtip-toys/workflows/CI/badge.svg)](https://github.com/your-org/wingtip-toys/actions)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

This project is based on Microsoft's "Wingtip Toys Store" demo web portal - a 3-tier ASP.NET Web Forms application leveraging .NET Framework 4.5.2 with Entity Framework and SQL Server.

## 🏗️ **Architecture Overview**

- **Frontend**: ASP.NET Web Forms with Bootstrap 3.0
- **Backend**: .NET Framework 4.5.2, Entity Framework 6.1
- **Database**: SQL Server / Azure SQL Database
- **Authentication**: ASP.NET Identity with OWIN
- **Payment**: PayPal integration
- **Logging**: ELMAH error handling

## 💻 **Development on Mac**

### ⚠️ **Important Limitation**

This application uses **.NET Framework 4.5.2** which is **Windows-only**. The full application cannot run natively on Mac or in Linux containers. However, here are several ways to develop on Mac:

### **Option 1: Test Business Logic Locally (Recommended for Mac)**

```bash
# Run cross-platform tests on Mac
./scripts/run-tests-mac.sh

# Start supporting services (database, management tools)
docker-compose -f docker-compose.mac.yml up -d
```

### **Option 2: GitHub Codespaces (Full Windows Environment)**

1. **Open in GitHub Codespaces**:
   - Create a Windows-based Codespace for the full application
   - Use VS Code Remote Development with Windows VM

2. **Cloud Development**:
   - Full .NET Framework support in Windows environment
   - Access to Visual Studio and all Windows tools

### **Option 3: Local Business Logic Testing**

You can test the core business logic on Mac using our .NET Core test project:

```bash
# Navigate to test project
cd WingtipToys.Tests

# Run tests on Mac
dotnet test
```

### **Option 4: Parallels/VMware + Windows**

Run Windows in a VM to develop the full application locally.

## 🚀 **Quick Start**

### **Prerequisites**

- **Windows**: Visual Studio 2017+ with ASP.NET workload
- **Mac**: Docker Desktop or Visual Studio for Mac
- **Database**: SQL Server LocalDB, SQL Server, or Azure SQL Database

### **Windows Setup**

1. **Clone the repository**:
   ```bash
   git clone https://github.com/your-org/wingtip-toys.git
   cd wingtip-toys
   ```

2. **Open in Visual Studio**:
   ```bash
   start WingtipToys/WingtipToys.sln
   ```

3. **Update connection string** in `Web.config`:
   ```xml
   <connectionStrings>
     <add name="WingtipToys" 
          connectionString="Server=(localdb)\mssqllocaldb;Database=WingtipToysDB;Trusted_Connection=true" 
          providerName="System.Data.SqlClient" />
   </connectionStrings>
   ```

4. **Build and run**:
   - Press `F5` or `Ctrl+F5`
   - Application will initialize database automatically on first run

### **Mac Setup (Supporting Services Only)**

⚠️ **Important**: The main .NET Framework 4.5.2 application requires Windows containers and cannot run directly on Mac. However, you can run the supporting services (database, tools) and test the business logic:

1. **Install Docker Desktop**:
   ```bash
   # Download from: https://www.docker.com/products/docker-desktop
   ```

2. **Start supporting services**:
   ```bash
   # Clone repository
   git clone https://github.com/your-org/wingtip-toys.git
   cd wingtip-toys

   # Start database and management tools
   docker-compose -f docker-compose.mac.yml up -d
   ```

3. **Access services**:
   - **Database Management**: http://localhost:8081 (Adminer)
   - **Elasticsearch**: http://localhost:9200
   - **Kibana**: http://localhost:5601
   - **Redis**: localhost:6379

4. **Run business logic tests**:
   ```bash
   ./scripts/run-tests-mac.sh
   ```

## 🧪 **Testing**

### **Running Tests**

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test category
dotnet test --filter Category=Unit
```

### **Test Structure**

- **`WingtipToys.Tests/`**: .NET Core test project (Mac/Linux compatible)
  - `Unit/`: Unit tests for business logic
  - `Integration/`: Integration tests with in-memory database
  - `E2E/`: End-to-end browser tests

## 🔧 **Configuration**

### **Environment-Specific Settings**

The application supports multiple configuration sources:

1. **`appsettings.json`** (preferred for new deployments)
2. **Cloud Foundry User-Provided Services**
3. **`Web.config`** (fallback)

```json
// appsettings.json
{
  "ConnectionStrings": {
    "wingtiptoysdb": "Server=localhost;Database=WingtipToysDB;Trusted_Connection=true"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### **Database Setup**

1. **Automatic Initialization**: Database is created automatically on first run
2. **Manual Setup**: Use `ProductDatabaseInitializer` to seed data
3. **Azure SQL**: Update connection string for cloud deployment

## 🚢 **Deployment**

### **Azure App Service**

```bash
# Using Azure CLI
az webapp up --name wingtip-toys --resource-group myResourceGroup
```

### **Docker**

```bash
# Build for production
docker build -t wingtip-toys:latest .

# Run in production mode
docker run -p 80:80 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ConnectionStrings__WingtipToys="your-production-connection-string" \
  wingtip-toys:latest
```

### **Cloud Foundry**

```bash
# Push to Cloud Foundry
cf push wingtip-toys -f manifest.yml
```

## 🏢 **Architecture Decisions**

### **Why .NET Framework 4.5.2?**

- **Legacy Compatibility**: Maintains compatibility with existing Windows infrastructure
- **Web Forms**: Rapid development for enterprise applications
- **Entity Framework 6**: Mature ORM with rich features

### **Migration Path to .NET Core**

For cross-platform support, consider:

1. **API-First Approach**: Extract business logic to .NET Core Web API
2. **Blazor Migration**: Convert Web Forms to Blazor Server/WASM
3. **Containerization**: Use Docker for deployment consistency

## 🛠️ **Development Tools**

### **Recommended IDE Setup**

**Windows**:
- Visual Studio 2019/2022 Professional
- SQL Server Management Studio
- IIS Express (included)

**Mac**:
- Visual Studio for Mac
- Docker Desktop
- Azure Data Studio

### **Useful Extensions**

- Web Essentials
- Entity Framework Power Tools
- ELMAH Dashboard

## 📊 **Monitoring & Observability**

- **Error Logging**: ELMAH captures all unhandled exceptions
- **Performance**: Built-in ASP.NET tracing
- **Health Checks**: Custom health check endpoints

## 🔒 **Security Considerations**

- **Authentication**: ASP.NET Identity with OWIN middleware
- **Authorization**: Role-based access control
- **Data Protection**: Entity Framework SQL parameterization
- **HTTPS**: Enforced in production environments

## 🤝 **Contributing**

1. **Fork the repository**
2. **Create feature branch**: `git checkout -b feature/my-feature`
3. **Run tests**: `dotnet test`
4. **Commit changes**: `git commit -am 'Add my feature'`
5. **Push branch**: `git push origin feature/my-feature`
6. **Create Pull Request**

### **Development Workflow**

1. **Local Development**: Use Docker or Windows VM
2. **Testing**: GitHub Actions runs tests on every PR
3. **Code Review**: Required before merge to main
4. **Deployment**: Automatic deployment to staging/production

## 📞 **Support**

- **Issues**: Create GitHub issue for bugs/features
- **Discussions**: Use GitHub Discussions for questions
- **Documentation**: Check `/docs` folder for detailed guides

## 📄 **License**

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

**Note**: This application demonstrates classic ASP.NET Web Forms patterns. For new projects, consider modern alternatives like ASP.NET Core with Razor Pages or Blazor.
