namespace PROG3270_GroupProject.Models;

public class OrderDto
{
    public int CartId { get; set; }
    public int UserId { get; set; }
    public bool IsRegisteredUser { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Discount { get; set; }
    public decimal Total { get; set; }
    public DateTime OrderDate { get; set; }
    public List<CartProduct> Items { get; set; }
}