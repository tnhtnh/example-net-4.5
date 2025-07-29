# Multi-stage Dockerfile for WingtipToys .NET Framework 4.5.2 Application
# This Dockerfile builds the application and creates a production-ready image

# Build stage - Use Windows Server Core with .NET Framework SDK
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build

# Set working directory
WORKDIR /src

# Copy solution and project files
COPY WingtipToys/WingtipToys.sln ./
COPY WingtipToys/WingtipToys/packages.config ./WingtipToys/
COPY WingtipToys/WingtipToys/WingtipToys.csproj ./WingtipToys/

# Restore NuGet packages
RUN nuget restore WingtipToys.sln

# Copy source code
COPY WingtipToys/WingtipToys/ ./WingtipToys/

# Build the application
RUN msbuild WingtipToys/WingtipToys.csproj /p:Configuration=Release /p:Platform="Any CPU" /p:OutputPath=/app/bin/ /p:WebProjectOutputDir=/app

# Runtime stage - Use ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019 AS runtime

# Set working directory
WORKDIR /inetpub/wwwroot

# Copy built application from build stage
COPY --from=build /app .

# Create application directory structure
RUN mkdir App_Data

# Set up IIS configuration
RUN powershell -Command \
    Import-Module WebAdministration; \
    Set-ItemProperty -Path 'IIS:\Sites\Default Web Site' -Name physicalPath -Value 'C:\inetpub\wwwroot'; \
    Set-ItemProperty -Path 'IIS:\Sites\Default Web Site' -Name bindings -Value @{protocol='http';bindingInformation='*:80:'}

# Configure application pool for .NET Framework 4.5
RUN powershell -Command \
    Import-Module WebAdministration; \
    Set-ItemProperty -Path 'IIS:\AppPools\DefaultAppPool' -Name processModel.identityType -Value ApplicationPoolIdentity; \
    Set-ItemProperty -Path 'IIS:\AppPools\DefaultAppPool' -Name managedRuntimeVersion -Value 'v4.0'

# Set environment variables
ENV ASPNET_ENVIRONMENT=Production
ENV ConnectionStrings__WingtipToys="Server=tcp:host.docker.internal,1433;Database=WingtipToysDB;User Id=sa;Password=YourPassword123!;TrustServerCertificate=true;"

# Health check endpoint
HEALTHCHECK --interval=30s --timeout=3s --start-period=30s --retries=3 \
    CMD powershell -command "try { Invoke-WebRequest -Uri http://localhost -UseBasicParsing -TimeoutSec 3 | Out-Null; exit 0 } catch { exit 1 }"

# Expose port 80
EXPOSE 80

# Start IIS
CMD ["C:\\ServiceMonitor.exe", "w3svc"]

# Metadata
LABEL maintainer="WingtipToys Development Team"
LABEL version="1.0"
LABEL description="WingtipToys .NET Framework 4.5.2 E-commerce Application"
