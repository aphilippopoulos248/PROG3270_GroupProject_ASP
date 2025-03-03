// Data/ProjectContext.cs
using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Models;

namespace PROG3270_GroupProject.Data
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartProduct>()
                .HasKey(cp => new { cp.CartId, cp.ProductId });

            modelBuilder.Entity<WishlistItem>()
                .HasKey(wi => new { wi.WishlistId, wi.ProductId });

            // Seed data for Members if needed
            modelBuilder.Entity<Member>().HasData(
                new Member
                {
                    MemberID = 1,
                    UserName = "JohnDoe12",
                    Email = "jdoe@example.com",
                    Password = "hello@1234"
                },
                new Member
                {
                    MemberID = 2,
                    UserName = "BobSmith34",
                    Email = "bsmith@example.com",
                    Password = "hello@1234"
                });
        }
    }
}