using JWT.Models;
using Microsoft.EntityFrameworkCore;

namespace JWT.Context;

public class AppDbContext : DbContext
{
    protected AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }


    public DbSet<AppUser> Users { get; set; }
}