using Microsoft.EntityFrameworkCore;
using PROG3270_GroupProject.Models;
using DbContext = Microsoft.EntityFrameworkCore.DbContext;
namespace PROG3270_GroupProject.Data
{
    public class ProjectContext : DbContext
    {
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
    }
}
