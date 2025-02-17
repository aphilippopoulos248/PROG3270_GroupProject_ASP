// Models/Cart.cs
using System;
using System.Collections.Generic;

namespace PROG3270_GroupProject.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime Date { get; set; }
        public List<CartProduct> Products { get; set; } = new();
        
        public bool IsEmpty => Products == null || !Products.Any();
    }

    public class CartProduct
    {
        public int CartId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}