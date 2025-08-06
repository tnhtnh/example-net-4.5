using System;

namespace WingtipToys.Models.Exceptions
{
    /// <summary>
    /// Exception thrown for shopping cart related errors
    /// </summary>
    public class CartException : WingtipToysException
    {
        public string CartId { get; }
        public int? ProductId { get; }

        public CartException(string message) : base(message) { }

        public CartException(string message, string cartId) : base(message)
        {
            CartId = cartId;
        }

        public CartException(string message, string cartId, int productId) : base(message)
        {
            CartId = cartId;
            ProductId = productId;
        }

        public CartException(string message, Exception innerException) 
            : base(message, innerException) { }

        public CartException(string message, string cartId, Exception innerException) 
            : base(message, innerException)
        {
            CartId = cartId;
        }
    }
}