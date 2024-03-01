using SessionMicroservice.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace SessionMicroservice.Services;

public class DataContext : DbContext
{
    public DbSet<Session> Sessions { get; set; }
    
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
    }
}