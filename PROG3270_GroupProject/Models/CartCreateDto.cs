namespace PROG3270_GroupProject.Models
{
    public class CartCreateDto
    {
        public int UserId { get; set; }
        public List<CartProductCreateDto> Products { get; set; } = new();
    }

    public class CartProductCreateDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}