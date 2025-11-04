using Microsoft.EntityFrameworkCore;
using Capstone_Project_v0._1.Models;

namespace Capstone_Project_v0._1.Data;

public class LibraryRepository
{
    private readonly LibraryContext _context;
    public LibraryRepository(LibraryContext context) => _context = context;
    public async Task<List<Book>> GetAllBooksAsync() => await _context.Books.ToListAsync();
    public async Task<Book?> GetBookByIdAsync(int id) => await _context.Books.FindAsync(id);
    public async Task AddBookAsync(Book book) { _context.Books.Add(book); await _context.SaveChangesAsync(); }
    public async Task UpdateBookAsync(Book book) { _context.Books.Update(book); await _context.SaveChangesAsync(); }
    public async Task DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null) return;
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }


    // Add a new Book and link to default user with chosen status (Owned/Wishlist/etc.)
    public async Task<int> AddBookWithStatusAsync(string title, string authorName, string? isbn,
                                                  int pageCount, int publishYear, Status status)
    {
        var book = new Book
        {
            Title = title,
            AuthorName = authorName,
            ISBN = isbn ?? string.Empty,
            PageCount = pageCount,
            PublishYear = publishYear,
            DateAdded = DateTime.UtcNow
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync(); // BookID generated

        var link = new UserBook
        {
            UserID = 1,
            BookID = book.BookID,
            Status = status,
            CurrentPage = 0,
            StartDate = DateTime.UtcNow,
            DateAdded = DateTime.UtcNow
        };
        _context.UserBooks.Add(link);
        await _context.SaveChangesAsync();
        return book.BookID;
    }

    // Query list by status (Owned, Wishlist, Reading, Finished)
    public async Task<List<UserBook>> GetByStatusAsync(Status status)
    {
        return await _context.UserBooks
            .Include(ub => ub.Book)
            .Where(ub => ub.Status == status)
            .OrderBy(ub => ub.Book!.Title)
            .ToListAsync();
    }

    // Shortcuts
    public Task<List<UserBook>> GetOwnedAsync() => GetByStatusAsync(Status.Owned);
    public Task<List<UserBook>> GetWishlistAsync() => GetByStatusAsync(Status.WishList);

    // Demo reset
    public async Task ClearAllAsync()
    {
        // order matters due to FK
        _context.UserBooks.RemoveRange(_context.UserBooks);
        _context.Books.RemoveRange(_context.Books);
        await _context.SaveChangesAsync();
    }

    public async Task RemoveOwnedAsync(int ownedBookId, bool deleteBookIfOrphan)
    {
        // Example implementation, adjust to your data access logic
        var userBook = await _context.UserBooks.FindAsync(ownedBookId);
        if (userBook != null)
        {
            _context.UserBooks.Remove(userBook);

            if (deleteBookIfOrphan)
            {
                bool isOrphan = !_context.UserBooks.Any(ub => ub.BookID == userBook.BookID && ub.OwnedBookID != ownedBookId);
                if (isOrphan)
                {
                    var book = await _context.Books.FindAsync(userBook.BookID);
                    if (book != null)
                    {
                        _context.Books.Remove(book);
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}