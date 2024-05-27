using Lemondo.Libary.API.Modules.Auth.Models;
using Lemondo.Libary.API.Modules.Authors.Models;
using Lemondo.Libary.API.Modules.Books.Models;
using Microsoft.EntityFrameworkCore;

namespace Lemondo.Libary.API.DataBase;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.UserName).IsUnique();
    }

}
