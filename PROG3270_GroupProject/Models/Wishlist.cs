namespace PROG3270_GroupProject.Models
{
    public class Wishlist
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<WishlistItem> Items { get; set; } = new();
    }

    public class WishlistItem
    {
        public int WishlistId { get; set; }
        public int ProductId { get; set; }
    }
}