using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Capstone_Project_v0._1.Models;

namespace Capstone_Project_v0._1.Data;

//  LibraryContext class for Entity Framework Core
//  This class represents the bridge between the C# models and the SQLite database.
//  It defines tables, relationships, and configurations for the database schema.
public class LibraryContext : DbContext
{
    // Constructor: called when the context is created
    // It accepts DbContextOptions to configure the context
    // and passes them to the base DbContext class.
    public LibraryContext(DbContextOptions<LibraryContext> options) : base(options) { }
    // DbSet properties represent tables in the database
    // Each DbSet corresponds to a model class.
    public DbSet<Book> Books => Set<Book>();
    public DbSet<UserBook> UserBooks => Set<UserBook>();

    // OnModelCreating:
    // We define the table configs with primary keys, relationships, and constraints.
    protected override void OnModelCreating(ModelBuilder b)
    {
        // Books
        b.Entity<Book>(e =>
        {
            e.HasKey(x => x.BookID); // Primary Key
            e.Property(x => x.Title).IsRequired(); // Title is required
            e.Property(x => x.AuthorName).IsRequired(); // AuthorName is required
            e.Property(x => x.DateAdded); // DateAdded property
        });

        // User_Books
        b.Entity<UserBook>(e =>
        {
            e.HasKey(x => x.OwnedBookID); // Primary Key
            e.Property(x => x.Status).HasConversion<int>(); // Enum to int conversion
            e.HasOne(x => x.Book) // Relationship with Book
             .WithMany() // Many UserBooks can reference one Book
             .HasForeignKey(x => x.BookID) // Foreign Key
             .OnDelete(DeleteBehavior.Cascade); // Cascade delete
            e.Property(x => x.CurrentPage).HasDefaultValue(0); // Default value for CurrentPage
        });
    }
}