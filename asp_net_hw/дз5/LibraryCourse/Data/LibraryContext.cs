using LibraryCourse.Models;
using Microsoft.EntityFrameworkCore;
namespace LibraryCourse.Data;

public class LibraryContext(DbContextOptions<LibraryContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Reader> Readers => Set<Reader>();
    public DbSet<Loan> Loans => Set<Loan>();
    public DbSet<Librarian> Librarians => Set<Librarian>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reader>().HasIndex(r => r.Email).IsUnique();
        modelBuilder.Entity<Librarian>().HasIndex(u => u.Login).IsUnique();
        modelBuilder.Entity<Loan>().HasOne(l => l.Book).WithMany(b => b.Loans)
            .HasForeignKey(l => l.BookId).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Loan>().HasOne(l => l.Reader).WithMany(r => r.Loans)
            .HasForeignKey(l => l.ReaderId).OnDelete(DeleteBehavior.Restrict);
    }
}