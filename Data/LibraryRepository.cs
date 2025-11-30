using Microsoft.EntityFrameworkCore;
using Capstone_Project_v0._1.Models;
using Capstone_Project_v0._1.Services;

namespace Capstone_Project_v0._1.Data;

//  LibraryRepository
//  This class provides methods to interact with the LibraryContext and the UI.
//  Handles all database queries and operations related to books and user-book relationships.
//  Handles Basic CRUD operations and custom queries.
public class LibraryRepository
{
    // The private database context (connection to the SQLite DB)
    private readonly LibraryContext _context;
    private readonly UserSessionState _session;

    public LibraryRepository(LibraryContext context, UserSessionState session)
    {
        _context = context;
        _session = session;
    }


    //  BASIC CRUD OPERATIONS (for Books)
    // Retrieves all Book records from the database asynchronously
    public async Task<List<Book>> GetAllBooksAsync() => await _context.Books.ToListAsync();

    // Finds and returns a single Book by its ID (or null if not found)
    public async Task<Book?> GetBookByIdAsync(int id) => await _context.Books.FindAsync(id);

    // Adds a new Book record to the database
    public async Task AddBookAsync(Book book) { _context.Books.Add(book); await _context.SaveChangesAsync(); }

    // Updates an existing Book record in the database
    public async Task UpdateBookAsync(Book book) { _context.Books.Update(book); await _context.SaveChangesAsync(); }

    // Deletes a Book record from the database by its ID
    public async Task DeleteBookAsync(int id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book is null) return;
        _context.Books.Remove(book);
        await _context.SaveChangesAsync();
    }


    public async Task<int> AddBookWithStatusAsync(string title, string authorName, string? isbn, int pageCount, DateTime dateAdded, Status status)
    {
        if (!_session.IsAuthenticated || !_session.UserId.HasValue)
            throw new InvalidOperationException("No user is logged in.");

        var userId = _session.UserId.Value;

        // Step 1: Create a new Book record
        var book = new Book
        {
            Title = title,
            AuthorName = authorName,
            ISBN = isbn ?? string.Empty,
            PageCount = pageCount,
            DateAdded = dateAdded,
        };
        _context.Books.Add(book);
        await _context.SaveChangesAsync(); // BookID generated

        // Step 2: Create a UserBook link (represents ownership or wishlist entry)
        var link = new UserBook
        {
            UserID = userId,
            BookID = book.BookID,
            Status = status,
            CurrentPage = 0,
            DateAdded = dateAdded
        };
        _context.UserBooks.Add(link);
        await _context.SaveChangesAsync();
        return book.BookID;
    }


    public async Task<List<UserBook>> GetByStatusAsync(Status status)
    {
        if (!_session.IsAuthenticated || !_session.UserId.HasValue)
            throw new InvalidOperationException("No user is logged in.");

        var userId = _session.UserId.Value;

        return await _context.UserBooks
            .Include(ub => ub.Book)
            .Where(ub => ub.Status == status && ub.UserID == userId)
            .OrderBy(ub => ub.Book!.Title)
            .ToListAsync();
    }


    // Shortcuts
    public Task<List<UserBook>> GetOwnedAsync() => GetByStatusAsync(Status.Owned);
    public Task<List<UserBook>> GetWishlistAsync() => GetByStatusAsync(Status.WishList);

    // Deletes *all* records from both tables (used for demo resets)
    public async Task ClearAllAsync()
    {
        // Delete from UserBooks first (depends on Books via FK)
        _context.UserBooks.RemoveRange(_context.UserBooks);
        // Then remove all Books
        _context.Books.RemoveRange(_context.Books);
        // Commit both deletions
        await _context.SaveChangesAsync();
    }

    // Removes a single owned record (UserBook) by its ID.
    // Optionally deletes the related Book if it is no longer linked to anyone.
    public async Task RemoveOwnedAsync(int ownedBookId, bool deleteBookIfOrphan)
    {
        // Look up the specific UserBook by primary key
        var userBook = await _context.UserBooks.FindAsync(ownedBookId);
        if (userBook != null)
        {
            _context.UserBooks.Remove(userBook); // Stage for deletion

            // If deleteBookIfOrphan = true, check if the Book is now unused
            if (deleteBookIfOrphan)
            {
                bool isOrphan = !_context.UserBooks.Any(ub => ub.BookID == userBook.BookID && ub.OwnedBookID != ownedBookId);
                // If no other links exist, delete the Book as well
                if (isOrphan)
                {
                    var book = await _context.Books.FindAsync(userBook.BookID);
                    if (book != null)
                    {
                        _context.Books.Remove(book);
                    }
                }
            }
            // Commit all deletions
            await _context.SaveChangesAsync();
        }
    }
}