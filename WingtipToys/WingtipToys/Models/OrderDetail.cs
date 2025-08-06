using System.ComponentModel.DataAnnotations;

namespace WingtipToys.Models
{
  public class OrderDetail
  {
    [Key]
    public int OrderDetailId { get; set; }

    [Required]
    public int OrderId { get; set; }

    [Required]
    [StringLength(256)]
    public string Username { get; set; }

    [Required]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    [Required]
    [DataType(DataType.Currency)]
    [Range(0.01, double.MaxValue, ErrorMessage = "Unit price must be greater than 0")]
    public decimal? UnitPrice { get; set; }

    // Navigation properties
    public virtual Order Order { get; set; }
    public virtual Product Product { get; set; }
  }
}