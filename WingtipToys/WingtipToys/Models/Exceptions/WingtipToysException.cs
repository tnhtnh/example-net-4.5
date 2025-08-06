using System;

namespace WingtipToys.Models.Exceptions
{
    /// <summary>
    /// Base exception class for WingtipToys application
    /// </summary>
    public class WingtipToysException : Exception
    {
        public WingtipToysException() : base() { }
        
        public WingtipToysException(string message) : base(message) { }
        
        public WingtipToysException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}