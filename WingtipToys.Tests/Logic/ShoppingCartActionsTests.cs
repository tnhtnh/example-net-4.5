using Xunit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using WingtipToys.Tests.Models;
using Bogus;

// Import the original models from the main project
using Product = WingtipToys.Models.Product;
using Category = WingtipToys.Models.Category;
using CartItem = WingtipToys.Models.CartItem;
using Order = WingtipToys.Models.Order;
using OrderDetail = WingtipToys.Models.OrderDetail;

namespace WingtipToys.Tests.Logic;

/// <summary>
/// Unit tests for shopping cart functionality
/// Tests the core business logic extracted from ShoppingCartActions
/// </summary>
public class ShoppingCartActionsTests : IDisposable
{
    private readonly TestProductContext _context;
    private readonly ShoppingCartService _shoppingCartService;
    private readonly Faker _faker;

    public ShoppingCartActionsTests()
    {
        var options = new DbContextOptionsBuilder<TestProductContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TestProductContext(options);
        _context.Database.EnsureCreated();
        
        _shoppingCartService = new ShoppingCartService(_context);
        _faker = new Faker();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task AddToCart_NewItem_ShouldCreateCartItem()
    {
        // Arrange
        var cartId = _faker.Random.Guid().ToString();
        var productId = 1; // Race Car from seed data

        // Act
        await _shoppingCartService.AddToCartAsync(cartId, productId);

        // Assert
        var cartItem = await _context.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        cartItem.Should().NotBeNull();
        cartItem!.Quantity.Should().Be(1);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task AddToCart_ExistingItem_ShouldIncrementQuantity()
    {
        // Arrange
        var cartId = _faker.Random.Guid().ToString();
        var productId = 1;

        // Add item first time
        await _shoppingCartService.AddToCartAsync(cartId, productId);

        // Act - Add same item again
        await _shoppingCartService.AddToCartAsync(cartId, productId);

        // Assert
        var cartItem = await _context.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        cartItem.Should().NotBeNull();
        cartItem!.Quantity.Should().Be(2);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task GetCartItems_ShouldReturnCorrectItems()
    {
        // Arrange
        var cartId = _faker.Random.Guid().ToString();
        await _shoppingCartService.AddToCartAsync(cartId, 1); // Race Car
        await _shoppingCartService.AddToCartAsync(cartId, 2); // Fighter Jet

        // Act
        var cartItems = await _shoppingCartService.GetCartItemsAsync(cartId);

        // Assert
        cartItems.Should().HaveCount(2);
        cartItems.Should().Contain(item => item.Product!.ProductName == "Race Car");
        cartItems.Should().Contain(item => item.Product!.ProductName == "Fighter Jet");
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task CalculateCartTotal_ShouldReturnCorrectSum()
    {
        // Arrange
        var cartId = _faker.Random.Guid().ToString();
        await _shoppingCartService.AddToCartAsync(cartId, 1); // Race Car - $15.99
        await _shoppingCartService.AddToCartAsync(cartId, 2); // Fighter Jet - $24.99

        // Act
        var total = await _shoppingCartService.CalculateCartTotalAsync(cartId);

        // Assert
        total.Should().Be(40.98m); // 15.99 + 24.99
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task RemoveFromCart_ShouldDecrementQuantity()
    {
        // Arrange
        var cartId = _faker.Random.Guid().ToString();
        var productId = 1;
        
        // Add item twice
        await _shoppingCartService.AddToCartAsync(cartId, productId);
        await _shoppingCartService.AddToCartAsync(cartId, productId);

        // Act
        await _shoppingCartService.RemoveFromCartAsync(cartId, productId);

        // Assert
        var cartItem = await _context.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        cartItem.Should().NotBeNull();
        cartItem!.Quantity.Should().Be(1);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task RemoveFromCart_LastItem_ShouldDeleteCartItem()
    {
        // Arrange
        var cartId = _faker.Random.Guid().ToString();
        var productId = 1;
        
        await _shoppingCartService.AddToCartAsync(cartId, productId);

        // Act
        await _shoppingCartService.RemoveFromCartAsync(cartId, productId);

        // Assert
        var cartItem = await _context.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        cartItem.Should().BeNull();
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task UpdateQuantity_ShouldSetCorrectQuantity()
    {
        // Arrange
        var cartId = _faker.Random.Guid().ToString();
        var productId = 1;
        var newQuantity = 5;
        
        await _shoppingCartService.AddToCartAsync(cartId, productId);

        // Act
        await _shoppingCartService.UpdateQuantityAsync(cartId, productId, newQuantity);

        // Assert
        var cartItem = await _context.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        cartItem.Should().NotBeNull();
        cartItem!.Quantity.Should().Be(newQuantity);
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task EmptyCart_ShouldRemoveAllItems()
    {
        // Arrange
        var cartId = _faker.Random.Guid().ToString();
        await _shoppingCartService.AddToCartAsync(cartId, 1);
        await _shoppingCartService.AddToCartAsync(cartId, 2);
        await _shoppingCartService.AddToCartAsync(cartId, 3);

        // Act
        await _shoppingCartService.EmptyCartAsync(cartId);

        // Assert
        var cartItems = await _shoppingCartService.GetCartItemsAsync(cartId);
        cartItems.Should().BeEmpty();
    }

    [Theory]
    [Trait("Category", "Unit")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task UpdateQuantity_InvalidQuantity_ShouldThrowException(int invalidQuantity)
    {
        // Arrange
        var cartId = _faker.Random.Guid().ToString();
        var productId = 1;
        
        await _shoppingCartService.AddToCartAsync(cartId, productId);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _shoppingCartService.UpdateQuantityAsync(cartId, productId, invalidQuantity));
    }

    [Fact]
    [Trait("Category", "Unit")]
    public async Task AddToCart_NonExistentProduct_ShouldThrowException()
    {
        // Arrange
        var cartId = _faker.Random.Guid().ToString();
        var nonExistentProductId = 9999;

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _shoppingCartService.AddToCartAsync(cartId, nonExistentProductId));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

/// <summary>
/// Simplified shopping cart service for testing
/// Extracts core business logic from the original ShoppingCartActions
/// </summary>
public class ShoppingCartService
{
    private readonly TestProductContext _context;

    public ShoppingCartService(TestProductContext context)
    {
        _context = context;
    }

    public async Task AddToCartAsync(string cartId, int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null)
            throw new InvalidOperationException($"Product with ID {productId} not found");

        var cartItem = await _context.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        if (cartItem == null)
        {
            cartItem = new CartItem
            {
                ItemId = Guid.NewGuid().ToString(),
                ProductId = productId,
                CartId = cartId,
                Product = product,
                Quantity = 1,
                UnitPrice = (decimal)(product.UnitPrice ?? 0),
                DateCreated = DateTime.UtcNow
            };
            _context.ShoppingCartItems.Add(cartItem);
        }
        else
        {
            cartItem.Quantity++;
        }

        await _context.SaveChangesAsync();
    }

    public async Task<List<CartItem>> GetCartItemsAsync(string cartId)
    {
        return await _context.ShoppingCartItems
            .Include(c => c.Product)
            .Where(c => c.CartId == cartId)
            .ToListAsync();
    }

    public async Task<decimal> CalculateCartTotalAsync(string cartId)
    {
        return await _context.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .SumAsync(c => c.Quantity * c.UnitPrice);
    }

    public async Task RemoveFromCartAsync(string cartId, int productId)
    {
        var cartItem = await _context.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        if (cartItem != null)
        {
            if (cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
            }
            else
            {
                _context.ShoppingCartItems.Remove(cartItem);
            }

            await _context.SaveChangesAsync();
        }
    }

    public async Task UpdateQuantityAsync(string cartId, int productId, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0", nameof(quantity));

        var cartItem = await _context.ShoppingCartItems
            .FirstOrDefaultAsync(c => c.CartId == cartId && c.ProductId == productId);

        if (cartItem != null)
        {
            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
        }
    }

    public async Task EmptyCartAsync(string cartId)
    {
        var cartItems = await _context.ShoppingCartItems
            .Where(c => c.CartId == cartId)
            .ToListAsync();

        _context.ShoppingCartItems.RemoveRange(cartItems);
        await _context.SaveChangesAsync();
    }
} 