using System;

namespace WingtipToys.Models.Exceptions
{
    /// <summary>
    /// Exception thrown for order processing errors
    /// </summary>
    public class OrderException : WingtipToysException
    {
        public int? OrderId { get; }
        public string Username { get; }

        public OrderException(string message) : base(message) { }

        public OrderException(string message, int orderId) : base(message)
        {
            OrderId = orderId;
        }

        public OrderException(string message, string username) : base(message)
        {
            Username = username;
        }

        public OrderException(string message, Exception innerException) 
            : base(message, innerException) { }

        public OrderException(string message, int orderId, Exception innerException) 
            : base(message, innerException)
        {
            OrderId = orderId;
        }
    }
}