using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SportProductsWeb.Data
{
    public class ShopContext : IdentityDbContext<UserApplication>
    {
        public ShopContext(DbContextOptions<ShopContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Product>().Navigation(p => p.Category).AutoInclude();



            modelBuilder.Entity<Category>()
                .HasMany(navigationExpression: c => c.Products)
                .WithOne(navigationExpression: a => a.Category)
                .HasForeignKey(foreignKeyExpression: a => a.CategoryId);

            modelBuilder.Seed();

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}
