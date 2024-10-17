using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TIE_Decor.Entities;
using TIE_Decor.Models;

namespace TIE_Decor.DbContext;

public class AppDbContext : IdentityDbContext<User>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entityType.GetTableName();
            if (tableName != null && tableName.StartsWith("AspNet"))
            {
                entityType.SetTableName(tableName.Substring(6));
            }
        }

        modelBuilder.Entity<Cart>()
        .HasOne(p => p.Product)
        .WithMany()
        .HasForeignKey(p => p.ProductId);
    }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Consultation> Consultations { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Design> Designs { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<TIE_Decor.Entities.Blog> Blog { get; set; }
    public DbSet<PageViewTracking> PageViewTrackings { get; set; }
    public DbSet<ClickTracking> ClickTrackings { get; set; }
    public DbSet<DesignerSchedules> DesignerSchedules { get; set; }
    public DbSet<Favorite> Favorites { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Orderdetail> Orderdetails { get; set; }


}
