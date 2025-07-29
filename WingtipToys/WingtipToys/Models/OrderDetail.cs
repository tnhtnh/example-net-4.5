using System.ComponentModel.DataAnnotations;

namespace WingtipToys.Models
{
  public class OrderDetail
  {
    public int OrderDetailId { get; set; }

    public int OrderId { get; set; }

    public string Username { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Product Product { get; set; }

  }
}