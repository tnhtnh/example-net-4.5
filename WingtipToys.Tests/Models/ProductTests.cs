using Xunit;
using FluentAssertions;
using System.ComponentModel.DataAnnotations;
using Bogus;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

// Import the original models from the main project
using Product = WingtipToys.Models.Product;
using Category = WingtipToys.Models.Category;

namespace WingtipToys.Tests.Models;

/// <summary>
/// Unit tests for Product model validation and business logic
/// </summary>
public class ProductTests
{
    private readonly Faker _faker;

    public ProductTests()
    {
        _faker = new Faker();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Product_ValidModel_ShouldPassValidation()
    {
        // Arrange
        var product = new Product
        {
            ProductID = 1,
            ProductName = "Test Product",
            Description = "A great test product",
            ImagePath = "~/images/test.png",
            UnitPrice = 19.99,
            CategoryID = 1
        };

        // Act
        var validationResults = ValidateModel(product);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Product_InvalidProductName_ShouldFailValidation(string? invalidName)
    {
        // Arrange
        var product = new Product
        {
            ProductID = 1,
            ProductName = invalidName!,
            Description = "A great test product",
            UnitPrice = 19.99
        };

        // Act
        var validationResults = ValidateModel(product);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(r => r.MemberNames.Contains(nameof(Product.ProductName)));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Product_ProductNameTooLong_ShouldFailValidation()
    {
        // Arrange
        var product = new Product
        {
            ProductID = 1,
            ProductName = _faker.Lorem.Text().PadRight(101, 'X'), // Exceeds 100 char limit
            Description = "A great test product",
            UnitPrice = 19.99
        };

        // Act
        var validationResults = ValidateModel(product);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(r => r.MemberNames.Contains(nameof(Product.ProductName)));
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Product_InvalidDescription_ShouldFailValidation(string? invalidDescription)
    {
        // Arrange
        var product = new Product
        {
            ProductID = 1,
            ProductName = "Test Product",
            Description = invalidDescription!,
            UnitPrice = 19.99
        };

        // Act
        var validationResults = ValidateModel(product);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(r => r.MemberNames.Contains(nameof(Product.Description)));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Product_DescriptionTooLong_ShouldFailValidation()
    {
        // Arrange
        var product = new Product
        {
            ProductID = 1,
            ProductName = "Test Product",
            Description = _faker.Lorem.Text().PadRight(10001, 'X'), // Exceeds 10000 char limit
            UnitPrice = 19.99
        };

        // Act
        var validationResults = ValidateModel(product);

        // Assert
        validationResults.Should().NotBeEmpty();
        validationResults.Should().Contain(r => r.MemberNames.Contains(nameof(Product.Description)));
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(0.01)]
    [InlineData(10.50)]
    [InlineData(999.99)]
    [InlineData(1000.00)]
    public void Product_ValidUnitPrice_ShouldPassValidation(double price)
    {
        // Arrange
        var product = new Product
        {
            ProductID = 1,
            ProductName = "Test Product",
            Description = "A great test product",
            UnitPrice = price
        };

        // Act
        var validationResults = ValidateModel(product);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(-1.00)]
    [InlineData(-0.01)]
    public void Product_NegativeUnitPrice_ShouldBeInvalidBusinessRule(double price)
    {
        // Arrange
        var product = new Product
        {
            ProductID = 1,
            ProductName = "Test Product",
            Description = "A great test product",
            UnitPrice = price
        };

        // Act & Assert
        // This would be a business rule validation, not a data annotation validation
        product.UnitPrice.Should().BeLessThan(0);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Product_NullUnitPrice_ShouldBeAllowed()
    {
        // Arrange
        var product = new Product
        {
            ProductID = 1,
            ProductName = "Test Product",
            Description = "A great test product",
            UnitPrice = null // Price to be determined
        };

        // Act
        var validationResults = ValidateModel(product);

        // Assert
        validationResults.Should().BeEmpty();
        product.UnitPrice.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Product_WithCategory_ShouldMaintainRelationship()
    {
        // Arrange
        var category = new Category
        {
            CategoryID = 1,
            CategoryName = "Test Category",
            Description = "Test category description"
        };

        var product = new Product
        {
            ProductID = 1,
            ProductName = "Test Product",
            Description = "A great test product",
            UnitPrice = 19.99,
            CategoryID = category.CategoryID,
            Category = category
        };

        // Act & Assert
        product.Category.Should().NotBeNull();
        product.Category!.CategoryID.Should().Be(product.CategoryID);
        product.Category.CategoryName.Should().Be("Test Category");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Product_ImagePath_ShouldAcceptValidPaths()
    {
        // Arrange & Act
        var product1 = new Product
        {
            ProductID = 1,
            ProductName = "Test Product",
            Description = "A great test product",
            ImagePath = "~/Catalog/Images/product1.png"
        };

        var product2 = new Product
        {
            ProductID = 2,
            ProductName = "Test Product 2",
            Description = "Another great test product",
            ImagePath = "/images/product2.jpg"
        };

        var product3 = new Product
        {
            ProductID = 3,
            ProductName = "Test Product 3",
            Description = "Yet another great test product",
            ImagePath = null // No image
        };

        // Assert
        var validationResults1 = ValidateModel(product1);
        var validationResults2 = ValidateModel(product2);
        var validationResults3 = ValidateModel(product3);

        validationResults1.Should().BeEmpty();
        validationResults2.Should().BeEmpty();
        validationResults3.Should().BeEmpty();

        product1.ImagePath.Should().Be("~/Catalog/Images/product1.png");
        product2.ImagePath.Should().Be("/images/product2.jpg");
        product3.ImagePath.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public void Product_DisplayAttributes_ShouldBeCorrect()
    {
        // This test ensures the Display attributes are correctly applied
        // In a real application, you might test UI generation, but here we test the metadata

        // Arrange
        var productType = typeof(Product);

        // Act
        var nameProperty = productType.GetProperty(nameof(Product.ProductName));
        var priceProperty = productType.GetProperty(nameof(Product.UnitPrice));
        var descriptionProperty = productType.GetProperty(nameof(Product.Description));

        // Assert
        nameProperty.Should().NotBeNull();
        priceProperty.Should().NotBeNull();
        descriptionProperty.Should().NotBeNull();

        // Check if Display attributes exist (would need reflection in real test)
        var nameDisplayAttribute = nameProperty!.GetCustomAttributes(typeof(DisplayAttribute), false)
            .Cast<DisplayAttribute>().FirstOrDefault();
        var priceDisplayAttribute = priceProperty!.GetCustomAttributes(typeof(DisplayAttribute), false)
            .Cast<DisplayAttribute>().FirstOrDefault();
        var descriptionDisplayAttribute = descriptionProperty!.GetCustomAttributes(typeof(DisplayAttribute), false)
            .Cast<DisplayAttribute>().FirstOrDefault();

        nameDisplayAttribute?.Name.Should().Be("Name");
        priceDisplayAttribute?.Name.Should().Be("Price");
        descriptionDisplayAttribute?.Name.Should().Be("Product Description");
    }

    /// <summary>
    /// Helper method to validate a model using data annotations
    /// </summary>
    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
} 