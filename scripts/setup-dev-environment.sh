#!/bin/bash

# setup-dev-environment.sh
# Development environment setup script for WingtipToys on Mac/Linux

set -e  # Exit on any error

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
PURPLE='\033[0;35m'
NC='\033[0m' # No Color

echo -e "${BLUE}🚀 WingtipToys Development Environment Setup${NC}"
echo "=================================================="
echo ""

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Function to print status
print_status() {
    if [ $1 -eq 0 ]; then
        echo -e "${GREEN}✅ $2${NC}"
    else
        echo -e "${RED}❌ $2${NC}"
        return 1
    fi
}

# Function to print warning
print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

# Function to print info
print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

echo -e "${PURPLE}📋 Checking prerequisites...${NC}"
echo ""

# Check OS
OS="$(uname -s)"
case "${OS}" in
    Linux*)     MACHINE=Linux;;
    Darwin*)    MACHINE=Mac;;
    *)          MACHINE="UNKNOWN:${OS}"
esac
print_info "Operating System: $MACHINE"

# Check .NET SDK
if command_exists dotnet; then
    DOTNET_VERSION=$(dotnet --version 2>/dev/null || echo "unknown")
    print_status 0 ".NET SDK installed (version: $DOTNET_VERSION)"
    
    # Check if it's .NET 8.0 or later
    if [[ "$DOTNET_VERSION" =~ ^[8-9]\. ]] || [[ "$DOTNET_VERSION" =~ ^[1-9][0-9]\. ]]; then
        print_status 0 ".NET version is compatible (8.0+)"
    else
        print_warning ".NET 8.0+ recommended. Current version: $DOTNET_VERSION"
        echo "          Download from: https://dotnet.microsoft.com/download/dotnet/8.0"
    fi
else
    print_status 1 ".NET SDK not found"
    echo -e "${YELLOW}          Please install .NET 8.0 SDK:${NC}"
    echo "          https://dotnet.microsoft.com/download/dotnet/8.0"
    echo ""
fi

# Check Docker
if command_exists docker; then
    if docker --version >/dev/null 2>&1; then
        DOCKER_VERSION=$(docker --version | cut -d' ' -f3 | cut -d',' -f1)
        print_status 0 "Docker installed (version: $DOCKER_VERSION)"
        
        # Check if Docker is running
        if docker info >/dev/null 2>&1; then
            print_status 0 "Docker daemon is running"
        else
            print_warning "Docker is installed but not running"
            echo "          Please start Docker Desktop or Docker daemon"
        fi
    else
        print_status 1 "Docker installed but not working properly"
    fi
else
    print_status 1 "Docker not found"
    echo -e "${YELLOW}          Please install Docker Desktop:${NC}"
    if [ "$MACHINE" = "Mac" ]; then
        echo "          https://docs.docker.com/desktop/install/mac-install/"
    else
        echo "          https://docs.docker.com/desktop/install/linux-install/"
    fi
    echo ""
fi

# Check Docker Compose
if command_exists docker-compose || docker compose version >/dev/null 2>&1; then
    if command_exists docker-compose; then
        COMPOSE_VERSION=$(docker-compose --version 2>/dev/null | cut -d' ' -f3 | cut -d',' -f1 || echo "unknown")
        print_status 0 "Docker Compose installed (version: $COMPOSE_VERSION)"
    else
        print_status 0 "Docker Compose (v2) available via docker compose"
    fi
else
    print_warning "Docker Compose not found (will use 'docker compose' if available)"
fi

# Check Git
if command_exists git; then
    GIT_VERSION=$(git --version | cut -d' ' -f3)
    print_status 0 "Git installed (version: $GIT_VERSION)"
else
    print_status 1 "Git not found"
    echo -e "${YELLOW}          Please install Git for your system${NC}"
    echo ""
fi

# Check curl
if command_exists curl; then
    print_status 0 "curl available"
else
    print_warning "curl not found (recommended for testing)"
fi

echo ""
echo -e "${PURPLE}🔧 Setting up project...${NC}"
echo ""

# Make scripts executable
if [ -f "scripts/run-tests-mac.sh" ]; then
    chmod +x scripts/run-tests-mac.sh
    print_status 0 "Made test script executable"
fi

if [ -f "scripts/setup-dev-environment.sh" ]; then
    chmod +x scripts/setup-dev-environment.sh
    print_status 0 "Made setup script executable"
fi

# Check if test project exists and restore packages
if [ -f "WingtipToys.Tests/WingtipToys.Tests.csproj" ]; then
    print_info "Restoring .NET test project packages..."
    if dotnet restore WingtipToys.Tests/WingtipToys.Tests.csproj >/dev/null 2>&1; then
        print_status 0 "Test project packages restored"
    else
        print_status 1 "Failed to restore test project packages"
    fi
else
    print_warning "Test project not found"
fi

echo ""
echo -e "${PURPLE}🧪 Running quick health check...${NC}"
echo ""

# Try to build test project
if [ -f "WingtipToys.Tests/WingtipToys.Tests.csproj" ] && command_exists dotnet; then
    print_info "Building test project..."
    if dotnet build WingtipToys.Tests/WingtipToys.Tests.csproj --configuration Release --no-restore >/dev/null 2>&1; then
        print_status 0 "Test project builds successfully"
        
        # Run a quick test
        print_info "Running quick test..."
        if dotnet test WingtipToys.Tests/WingtipToys.Tests.csproj --configuration Release --no-build --verbosity quiet >/dev/null 2>&1; then
            print_status 0 "Tests are passing"
        else
            print_warning "Some tests are failing - check with: ./scripts/run-tests-mac.sh"
        fi
    else
        print_status 1 "Test project failed to build"
    fi
fi

echo ""
echo -e "${GREEN}🎉 Setup Summary${NC}"
echo "================"
echo ""

if command_exists dotnet && command_exists docker && command_exists git; then
    echo -e "${GREEN}✅ All core prerequisites are installed!${NC}"
    echo ""
    echo -e "${BLUE}📚 Next Steps:${NC}"
    echo ""
    echo -e "${YELLOW}1. Run tests on Mac/Linux:${NC}"
    echo "   ./scripts/run-tests-mac.sh"
    echo ""
    echo -e "${YELLOW}2. Start supporting services with Docker:${NC}"
    echo "   docker-compose -f docker-compose.mac.yml up -d"
    echo "   open http://localhost:8081 (Database Management)"
    echo ""
    echo -e "${YELLOW}3. View database in browser:${NC}"
    echo "   open http://localhost:8081 (Adminer)"
    echo ""
    echo -e "${YELLOW}4. Run tests with coverage:${NC}"
    echo "   ./scripts/run-tests-mac.sh coverage"
    echo ""
    echo -e "${YELLOW}5. Run tests in watch mode:${NC}"
    echo "   ./scripts/run-tests-mac.sh watch"
    echo ""
    echo -e "${BLUE}📖 Documentation:${NC}"
    echo "   • README.md - Complete setup guide"
    echo "   • WingtipToys.Tests/ - Cross-platform tests"
    echo "   • .github/workflows/ci.yml - CI/CD pipeline"
    echo ""
    echo -e "${GREEN}Happy coding! 🚀${NC}"
else
    echo -e "${YELLOW}⚠️  Some prerequisites are missing.${NC}"
    echo "   Please install the missing components and run this script again."
    echo ""
    echo -e "${BLUE}🔗 Quick Install Links:${NC}"
    if ! command_exists dotnet; then
        echo "   • .NET 8.0 SDK: https://dotnet.microsoft.com/download/dotnet/8.0"
    fi
    if ! command_exists docker; then
        echo "   • Docker Desktop: https://www.docker.com/products/docker-desktop"
    fi
    if ! command_exists git; then
        echo "   • Git: https://git-scm.com/downloads"
    fi
    echo ""
    echo "   Then run: ./scripts/setup-dev-environment.sh"
fi

echo ""
echo -e "${PURPLE}🛠️  Development Tools Available:${NC}"
echo "   • ./scripts/run-tests-mac.sh [coverage|watch|unit|integration|verbose]"
echo "   • docker-compose -f docker-compose.mac.yml up -d (start services)"
echo "   • docker-compose -f docker-compose.mac.yml down (stop services)"
echo "   • docker-compose -f docker-compose.mac.yml logs -f (view logs)"
echo "" 