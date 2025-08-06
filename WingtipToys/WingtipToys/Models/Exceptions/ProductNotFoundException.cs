using System;

namespace WingtipToys.Models.Exceptions
{
    /// <summary>
    /// Exception thrown when a product is not found
    /// </summary>
    public class ProductNotFoundException : WingtipToysException
    {
        public int ProductId { get; }
        public string ProductName { get; }

        public ProductNotFoundException(int productId) 
            : base($"Product with ID {productId} was not found.")
        {
            ProductId = productId;
        }

        public ProductNotFoundException(string productName) 
            : base($"Product with name '{productName}' was not found.")
        {
            ProductName = productName;
        }

        public ProductNotFoundException(int productId, Exception innerException) 
            : base($"Product with ID {productId} was not found.", innerException)
        {
            ProductId = productId;
        }
    }
}