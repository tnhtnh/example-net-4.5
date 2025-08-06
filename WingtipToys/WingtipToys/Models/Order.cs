using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel;

namespace WingtipToys.Models
{
  public class Order
  {
    [Key]
    public int OrderId { get; set; }

    [Required]
    public DateTime OrderDate { get; set; }

    [Required]
    [StringLength(256)]
    public string Username { get; set; }

    [Required(ErrorMessage = "First Name is required")]
    [DisplayName("First Name")]
    [StringLength(160)]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required")]
    [DisplayName("Last Name")]
    [StringLength(160)]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Address is required")]
    [StringLength(70)]
    public string Address { get; set; }

    [Required(ErrorMessage = "City is required")]
    [StringLength(40)]
    public string City { get; set; }

    [Required(ErrorMessage = "State is required")]
    [StringLength(40)]
    public string State { get; set; }

    [Required(ErrorMessage = "Postal Code is required")]
    [DisplayName("Postal Code")]
    [StringLength(10)]
    public string PostalCode { get; set; }

    [Required(ErrorMessage = "Country is required")]
    [StringLength(40)]
    public string Country { get; set; }

    [StringLength(24)]
    public string Phone { get; set; }

    [Required(ErrorMessage = "Email Address is required")]
    [DisplayName("Email Address")]
    [RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
        ErrorMessage = "Email is not valid.")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [ScaffoldColumn(false)]
    [DataType(DataType.Currency)]
    [Range(0, double.MaxValue, ErrorMessage = "Total must be non-negative")]
    public decimal Total { get; set; }

    [ScaffoldColumn(false)]
    [StringLength(100)]
    public string PaymentTransactionId { get; set; }

    [ScaffoldColumn(false)]
    public bool HasBeenShipped { get; set; }

    public virtual List<OrderDetail> OrderDetails { get; set; }
  }
}