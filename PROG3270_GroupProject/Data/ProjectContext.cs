using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Models;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;
namespace PROG3270_GroupProject.Data
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
