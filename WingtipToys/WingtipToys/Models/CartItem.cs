using System;
using System.ComponentModel.DataAnnotations;

namespace WingtipToys.Models
{
  public class CartItem
  {
    [Key]
    [StringLength(50)]
    public string ItemId { get; set; }

    [Required]
    [StringLength(50)]
    public string CartId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
    public int Quantity { get; set; }

    [Required]
    public DateTime DateCreated { get; set; }

    [Required]
    public int ProductId { get; set; }

    public virtual Product Product { get; set; }
  }
}