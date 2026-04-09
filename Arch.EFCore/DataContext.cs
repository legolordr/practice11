using Microsoft.EntityFrameworkCore;

namespace Arch.EFCore;

public class DataContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
        optionsBuilder.LogTo(Console.WriteLine);
        optionsBuilder.EnableSensitiveDataLogging();
        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Note>().HasOne(n => n.User).WithMany(u => u.Notes).HasForeignKey(n => n.UserId)
            .IsRequired();
    }
    
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Note> Notes => Set<Note>();

    public DbSet<User> Users => Set<User>();
}