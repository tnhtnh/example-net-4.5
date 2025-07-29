using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

// Import the original models from the main project
using Product = WingtipToys.Models.Product;
using Category = WingtipToys.Models.Category;
using CartItem = WingtipToys.Models.CartItem;
using Order = WingtipToys.Models.Order;
using OrderDetail = WingtipToys.Models.OrderDetail;

namespace WingtipToys.Tests.Models;

/// <summary>
/// Test-friendly version of ProductContext using EF Core for cross-platform compatibility
/// </summary>
public class TestProductContext : DbContext
{
    public TestProductContext(DbContextOptions<TestProductContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<CartItem> ShoppingCartItems { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OrderDetail> OrderDetails { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Product entity
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductID);
            entity.Property(e => e.ProductName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(10000).IsRequired();
            entity.Property(e => e.ImagePath).HasMaxLength(500);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Category)
                  .WithMany()
                  .HasForeignKey(e => e.CategoryID);
        });

        // Configure Category entity
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryID);
            entity.Property(e => e.CategoryName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        // Configure CartItem entity
        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.ItemId);
            entity.Property(e => e.ItemId).HasMaxLength(50);
            entity.Property(e => e.CartId).HasMaxLength(50);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId);
        });

        // Configure Order entity
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.Username).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Address).HasMaxLength(200);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.PostalCode).HasMaxLength(10);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PaymentTransactionId).HasMaxLength(100);
        });

        // Configure OrderDetail entity
        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId);
            entity.Property(e => e.Username).HasMaxLength(100);
            entity.Property(e => e.ProductName).HasMaxLength(100);
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
            entity.HasOne(e => e.Product)
                  .WithMany()
                  .HasForeignKey(e => e.ProductId);
        });

        // Seed data for testing
        SeedTestData(modelBuilder);
    }

    private static void SeedTestData(ModelBuilder modelBuilder)
    {
        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { CategoryID = 1, CategoryName = "Cars", Description = "Toy cars for all ages" },
            new Category { CategoryID = 2, CategoryName = "Planes", Description = "Model airplanes and jets" },
            new Category { CategoryID = 3, CategoryName = "Trucks", Description = "Heavy-duty toy trucks" },
            new Category { CategoryID = 4, CategoryName = "Boats", Description = "Boats and ships" }
        );

        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                ProductID = 1,
                ProductName = "Race Car",
                Description = "Fast racing car toy with realistic details",
                ImagePath = "~/Catalog/Images/carracer.png",
                UnitPrice = 15.99,
                CategoryID = 1
            },
            new Product
            {
                ProductID = 2,
                ProductName = "Fighter Jet",
                Description = "Military fighter jet model with moving parts",
                ImagePath = "~/Catalog/Images/planeace.png",
                UnitPrice = 24.99,
                CategoryID = 2
            },
            new Product
            {
                ProductID = 3,
                ProductName = "Fire Truck",
                Description = "Emergency fire truck with ladder and sirens",
                ImagePath = "~/Catalog/Images/truckfire.png",
                UnitPrice = 29.99,
                CategoryID = 3
            },
            new Product
            {
                ProductID = 4,
                ProductName = "Sailboat",
                Description = "Classic sailboat with authentic rigging",
                ImagePath = "~/Catalog/Images/boatsail.png",
                UnitPrice = 19.99,
                CategoryID = 4
            }
        );
    }
} 