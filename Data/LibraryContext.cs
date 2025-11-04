using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Capstone_Project_v0._1.Models;

namespace Capstone_Project_v0._1.Data;

public class LibraryContext : DbContext
{
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<UserBook> UserBooks => Set<UserBook>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Books
        b.Entity<Book>(e =>
        {
            e.HasKey(x => x.BookID);
            e.Property(x => x.Title).IsRequired();
            e.Property(x => x.AuthorName).IsRequired();
            e.Property(x => x.DateAdded);
        });

        // User_Books
        b.Entity<UserBook>(e =>
        {
            e.HasKey(x => x.OwnedBookID);
            e.Property(x => x.Status).HasConversion<int>();
            e.HasOne(x => x.Book)
             .WithMany()
             .HasForeignKey(x => x.BookID)
             .OnDelete(DeleteBehavior.Cascade);
            e.Property(x => x.CurrentPage).HasDefaultValue(0);
        });
    }
}