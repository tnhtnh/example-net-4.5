using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Tests.Models;
using WingtipToys.Tests.Logic;

// Import the original models from the main project
using Product = WingtipToys.Models.Product;
using Category = WingtipToys.Models.Category;
using CartItem = WingtipToys.Models.CartItem;
using Order = WingtipToys.Models.Order;
using OrderDetail = WingtipToys.Models.OrderDetail;

namespace WingtipToys.Tests.Integration;

/// <summary>
/// Integration tests for complete shopping cart workflows
/// Tests the entire flow from product browsing to cart management
/// </summary>
[Collection("Database")]
public class ShoppingCartIntegrationTests : IAsyncLifetime
{
    private TestProductContext _context = null!;
    private ShoppingCartService _shoppingCartService = null!;

    public async Task InitializeAsync()
    {
        var options = new DbContextOptionsBuilder<TestProductContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestProductContext(options);
        await _context.Database.EnsureCreatedAsync();
        
        _shoppingCartService = new ShoppingCartService(_context);
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CompleteShoppingWorkflow_ShouldWorkEndToEnd()
    {
        // Arrange
        var cartId = Guid.NewGuid().ToString();

        // Act & Assert - Step 1: Browse products
        var products = await _context.Products.Include(p => p.Category).ToListAsync();
        products.Should().HaveCount(4); // From seed data
        products.Should().AllSatisfy(p => p.Category.Should().NotBeNull());

        // Act & Assert - Step 2: Add products to cart
        await _shoppingCartService.AddToCartAsync(cartId, 1); // Race Car - $15.99
        await _shoppingCartService.AddToCartAsync(cartId, 2); // Fighter Jet - $24.99
        await _shoppingCartService.AddToCartAsync(cartId, 1); // Add Race Car again

        var cartItems = await _shoppingCartService.GetCartItemsAsync(cartId);
        cartItems.Should().HaveCount(2);
        
        var raceCar = cartItems.First(c => c.ProductId == 1);
        raceCar.Quantity.Should().Be(2);
        raceCar.Product!.ProductName.Should().Be("Race Car");

        var fighterJet = cartItems.First(c => c.ProductId == 2);
        fighterJet.Quantity.Should().Be(1);
        fighterJet.Product!.ProductName.Should().Be("Fighter Jet");

        // Act & Assert - Step 3: Calculate total
        var total = await _shoppingCartService.CalculateCartTotalAsync(cartId);
        total.Should().Be(56.97m); // (15.99 * 2) + 24.99

        // Act & Assert - Step 4: Update quantities
        await _shoppingCartService.UpdateQuantityAsync(cartId, 1, 3); // Increase Race Car to 3
        
        cartItems = await _shoppingCartService.GetCartItemsAsync(cartId);
        raceCar = cartItems.First(c => c.ProductId == 1);
        raceCar.Quantity.Should().Be(3);

        total = await _shoppingCartService.CalculateCartTotalAsync(cartId);
        total.Should().Be(72.96m); // (15.99 * 3) + 24.99

        // Act & Assert - Step 5: Remove items
        await _shoppingCartService.RemoveFromCartAsync(cartId, 2); // Remove Fighter Jet
        
        cartItems = await _shoppingCartService.GetCartItemsAsync(cartId);
        cartItems.Should().HaveCount(1);
        cartItems.First().ProductId.Should().Be(1); // Only Race Car remains

        total = await _shoppingCartService.CalculateCartTotalAsync(cartId);
        total.Should().Be(47.97m); // 15.99 * 3
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task MultipleUsersShoppingConcurrently_ShouldIsolateCartData()
    {
        // Arrange
        var user1CartId = "user1-cart";
        var user2CartId = "user2-cart";
        var user3CartId = "user3-cart";

        // Act - Each user adds different products to their cart
        await _shoppingCartService.AddToCartAsync(user1CartId, 1); // User 1: Race Car
        await _shoppingCartService.AddToCartAsync(user1CartId, 2); // User 1: Fighter Jet

        await _shoppingCartService.AddToCartAsync(user2CartId, 3); // User 2: Fire Truck
        await _shoppingCartService.AddToCartAsync(user2CartId, 4); // User 2: Sailboat

        await _shoppingCartService.AddToCartAsync(user3CartId, 1); // User 3: Race Car
        await _shoppingCartService.AddToCartAsync(user3CartId, 1); // User 3: Race Car (quantity 2)

        // Assert - Each user's cart should be isolated
        var user1Items = await _shoppingCartService.GetCartItemsAsync(user1CartId);
        var user2Items = await _shoppingCartService.GetCartItemsAsync(user2CartId);
        var user3Items = await _shoppingCartService.GetCartItemsAsync(user3CartId);

        // User 1 assertions
        user1Items.Should().HaveCount(2);
        user1Items.Should().Contain(item => item.Product!.ProductName == "Race Car");
        user1Items.Should().Contain(item => item.Product!.ProductName == "Fighter Jet");
        var user1Total = await _shoppingCartService.CalculateCartTotalAsync(user1CartId);
        user1Total.Should().Be(40.98m); // 15.99 + 24.99

        // User 2 assertions
        user2Items.Should().HaveCount(2);
        user2Items.Should().Contain(item => item.Product!.ProductName == "Fire Truck");
        user2Items.Should().Contain(item => item.Product!.ProductName == "Sailboat");
        var user2Total = await _shoppingCartService.CalculateCartTotalAsync(user2CartId);
        user2Total.Should().Be(49.98m); // 29.99 + 19.99

        // User 3 assertions
        user3Items.Should().HaveCount(1);
        user3Items.First().Product!.ProductName.Should().Be("Race Car");
        user3Items.First().Quantity.Should().Be(2);
        var user3Total = await _shoppingCartService.CalculateCartTotalAsync(user3CartId);
        user3Total.Should().Be(31.98m); // 15.99 * 2

        // Verify totals are different
        user1Total.Should().NotBe(user2Total);
        user1Total.Should().NotBe(user3Total);
        user2Total.Should().NotBe(user3Total);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task CategoryBasedProductFiltering_ShouldWorkCorrectly()
    {
        // Act - Get products by category
        var carProducts = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Category!.CategoryName == "Cars")
            .ToListAsync();

        var planeProducts = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Category!.CategoryName == "Planes")
            .ToListAsync();

        var truckProducts = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Category!.CategoryName == "Trucks")
            .ToListAsync();

        var boatProducts = await _context.Products
            .Include(p => p.Category)
            .Where(p => p.Category!.CategoryName == "Boats")
            .ToListAsync();

        // Assert
        carProducts.Should().HaveCount(1);
        carProducts.First().ProductName.Should().Be("Race Car");

        planeProducts.Should().HaveCount(1);
        planeProducts.First().ProductName.Should().Be("Fighter Jet");

        truckProducts.Should().HaveCount(1);
        truckProducts.First().ProductName.Should().Be("Fire Truck");

        boatProducts.Should().HaveCount(1);
        boatProducts.First().ProductName.Should().Be("Sailboat");

        // Test adding products from each category to cart
        var cartId = Guid.NewGuid().ToString();
        
        await _shoppingCartService.AddToCartAsync(cartId, carProducts.First().ProductID);
        await _shoppingCartService.AddToCartAsync(cartId, planeProducts.First().ProductID);
        await _shoppingCartService.AddToCartAsync(cartId, truckProducts.First().ProductID);
        await _shoppingCartService.AddToCartAsync(cartId, boatProducts.First().ProductID);

        var cartItems = await _shoppingCartService.GetCartItemsAsync(cartId);
        cartItems.Should().HaveCount(4);

        // Verify each category is represented
        var categoryNames = cartItems.Select(item => item.Product!.Category!.CategoryName).ToList();
        categoryNames.Should().Contain("Cars");
        categoryNames.Should().Contain("Planes");
        categoryNames.Should().Contain("Trucks");
        categoryNames.Should().Contain("Boats");
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task LargeCartOperations_ShouldPerformEfficiently()
    {
        // Arrange
        var cartId = Guid.NewGuid().ToString();
        var operationCount = 100;

        // Act - Add many items to cart (testing performance)
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < operationCount; i++)
        {
            // Rotate through available products
            var productId = (i % 4) + 1;
            await _shoppingCartService.AddToCartAsync(cartId, productId);
        }

        stopwatch.Stop();

        // Assert - Operations should complete in reasonable time
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // 5 seconds max

        var cartItems = await _shoppingCartService.GetCartItemsAsync(cartId);
        cartItems.Should().HaveCount(4); // 4 unique products

        // Each product should have been added 25 times (100 / 4)
        cartItems.Should().AllSatisfy(item => item.Quantity.Should().Be(25));

        // Calculate total for large cart
        var total = await _shoppingCartService.CalculateCartTotalAsync(cartId);
        var expectedTotal = (15.99m + 24.99m + 29.99m + 19.99m) * 25; // Sum of all products * 25
        total.Should().Be(expectedTotal);
    }

    [Theory]
    [Trait("Category", "Integration")]
    [InlineData(1, "Race Car", 15.99)]
    [InlineData(2, "Fighter Jet", 24.99)]
    [InlineData(3, "Fire Truck", 29.99)]
    [InlineData(4, "Sailboat", 19.99)]
    public async Task AddSpecificProductToCart_ShouldCalculateCorrectPrice(int productId, string expectedName, decimal expectedPrice)
    {
        // Arrange
        var cartId = Guid.NewGuid().ToString();

        // Act
        await _shoppingCartService.AddToCartAsync(cartId, productId);

        // Assert
        var cartItems = await _shoppingCartService.GetCartItemsAsync(cartId);
        cartItems.Should().HaveCount(1);

        var cartItem = cartItems.First();
        cartItem.Product!.ProductName.Should().Be(expectedName);
        cartItem.UnitPrice.Should().Be(expectedPrice);
        cartItem.Quantity.Should().Be(1);

        var total = await _shoppingCartService.CalculateCartTotalAsync(cartId);
        total.Should().Be(expectedPrice);
    }
} 