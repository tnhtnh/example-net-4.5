#!/bin/bash

# run-tests-mac.sh
# Script to run WingtipToys tests on Mac/Linux using .NET Core test project

set -e  # Exit on any error

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Project paths
TEST_PROJECT="WingtipToys.Tests/WingtipToys.Tests.csproj"
COVERAGE_DIR="./coverage"

echo -e "${BLUE}🚀 WingtipToys Test Runner for Mac/Linux${NC}"
echo "=============================================="

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}❌ .NET SDK is not installed. Please install .NET 8.0 SDK:${NC}"
    echo "   https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

# Display .NET version
DOTNET_VERSION=$(dotnet --version)
echo -e "${GREEN}✅ .NET SDK Version: ${DOTNET_VERSION}${NC}"

# Check if test project exists
if [ ! -f "$TEST_PROJECT" ]; then
    echo -e "${RED}❌ Test project not found: $TEST_PROJECT${NC}"
    echo "   Please run this script from the project root directory."
    exit 1
fi

echo ""
echo -e "${YELLOW}📦 Restoring NuGet packages...${NC}"
dotnet restore "$TEST_PROJECT"

echo ""
echo -e "${YELLOW}🔨 Building test project...${NC}"
dotnet build "$TEST_PROJECT" --configuration Release --no-restore

echo ""
echo -e "${YELLOW}🧪 Running unit tests...${NC}"

# Create coverage directory
mkdir -p "$COVERAGE_DIR"

# Run tests with different options based on arguments
case "${1:-default}" in
    "coverage")
        echo -e "${BLUE}📊 Running tests with code coverage...${NC}"
        dotnet test "$TEST_PROJECT" \
            --configuration Release \
            --no-build \
            --verbosity normal \
            --collect:"XPlat Code Coverage" \
            --results-directory "$COVERAGE_DIR" \
            --settings coverlet.runsettings
        
        echo ""
        echo -e "${GREEN}📈 Coverage report generated in: $COVERAGE_DIR${NC}"
        ;;
    
    "watch")
        echo -e "${BLUE}👀 Running tests in watch mode...${NC}"
        echo -e "${YELLOW}Press Ctrl+C to exit watch mode${NC}"
        dotnet watch test "$TEST_PROJECT" \
            --configuration Release \
            --verbosity normal
        ;;
    
    "unit")
        echo -e "${BLUE}🎯 Running only unit tests...${NC}"
        dotnet test "$TEST_PROJECT" \
            --configuration Release \
            --no-build \
            --verbosity normal \
            --filter "Category=Unit"
        ;;
    
    "integration")
        echo -e "${BLUE}🔗 Running only integration tests...${NC}"
        dotnet test "$TEST_PROJECT" \
            --configuration Release \
            --no-build \
            --verbosity normal \
            --filter "Category=Integration"
        ;;
    
    "verbose")
        echo -e "${BLUE}📝 Running tests with detailed output...${NC}"
        dotnet test "$TEST_PROJECT" \
            --configuration Release \
            --no-build \
            --verbosity detailed \
            --logger "console;verbosity=detailed"
        ;;
    
    *)
        echo -e "${BLUE}🏃 Running all tests...${NC}"
        dotnet test "$TEST_PROJECT" \
            --configuration Release \
            --no-build \
            --verbosity normal
        ;;
esac

# Check test results
if [ $? -eq 0 ]; then
    echo ""
    echo -e "${GREEN}✅ All tests passed successfully! 🎉${NC}"
    echo ""
    echo -e "${BLUE}📋 Next steps:${NC}"
    echo "   • Review test results above"
    echo "   • Check code coverage: ./scripts/run-tests-mac.sh coverage"
    echo "   • Run tests in watch mode: ./scripts/run-tests-mac.sh watch"
    echo "   • Run only unit tests: ./scripts/run-tests-mac.sh unit"
    echo ""
    echo -e "${GREEN}🐳 To run supporting services with Docker:${NC}"
    echo "   docker-compose -f docker-compose.mac.yml up -d"
    echo "   open http://localhost:8081 (Database Management)"
else
    echo ""
    echo -e "${RED}❌ Some tests failed. Please review the output above.${NC}"
    exit 1
fi 